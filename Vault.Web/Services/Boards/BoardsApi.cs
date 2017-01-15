using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vault.Activity;
using Vault.Shared;
using Vault.Shared.Domain;
using Vault.Shared.Queries;
using Vault.Shared.Search;
using Vault.Shared.Search.Criteria;
using Vault.Shared.Search.Parsing;
using Vault.WebHost.Services.Security;

namespace Vault.WebHost.Services.Boards
{
    public class BoardsApi : IBoardsApi
    {
        private readonly IAuthorizer _authorizer;
        private readonly ISearchQueryParser _searchQueryParser;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IQueryBuilder _queryBuilder;

        readonly static IDictionary<string, string> DefaultFieldsMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "type", "documentType" },
            { "published", "published" },
            { "startDate", "startdate" },
            { "endDate", "enddate" },
            { "title", "name" },
            { "description", "description" },
            { "desc", "description" },
            { "duration", "duration" },
            { "artist", "byartist" },
            { "album", "inalbum" },
            { "keywords" , "keywords" }
        };

        public BoardsApi(
            IWorkContextAccessor workContextAccessor,
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizer authorizer,
            ISearchQueryParser searchQueryParser,
            IQueryBuilder queryBuilder)
        {
            _workContextAccessor = workContextAccessor;
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizer = authorizer;
            _searchQueryParser = searchQueryParser;
            _queryBuilder = queryBuilder;
        }

        public Task<Board> CreateBoardAsync(string name, string query)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!_authorizer.Authorize(Permissions.CreateBoard))
                throw new SecurityException(string.Format("The user '{0}' does not has permission to create a new board", _workContextAccessor.WorkContext.User.UserName));

            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                var board = new Board
                {
                    OwnerId = _workContextAccessor.WorkContext.User.Id,
                    Published = DateTime.UtcNow,
                    RawQuery = query,
                    Name = name
                };

                unitOfWork.Save(board);
                unitOfWork.Commit();

                return Task.FromResult(board);
            }
        }

        public async Task<bool> DeleteBoardAsync(int boardId)
        {
            if (!_authorizer.Authorize(Permissions.UpdateBoard))
                throw new SecurityException(string.Format("The user '{0}' doest not has permission to update a board '{1}'", _workContextAccessor.WorkContext.User.UserName, boardId));

            var board = await _queryBuilder.For<Board>()
                .With(new ContentKey(boardId, _workContextAccessor.WorkContext.Owner.Id));

            if (board == null)
                return false;

            using (var unitOfWork = _unitOfWorkFactory.Create())
            {
                unitOfWork.Delete(board);
                unitOfWork.Commit();
                return true;
            }
        }

        public async Task<Board> GetBoardAsync(int boardId, int offset, int count)
        {
            var board = await _queryBuilder.For<Board>()
                .With(new ContentKey(boardId, _workContextAccessor.WorkContext.Owner.Id));

            if (board == null || !_authorizer.Authorize(Permissions.ViewBoard, board))
                return null;

            var request = new SearchRequest
            {
                Criteria = ParseSearchQuery(board.RawQuery),
                OwnerId = "a314130a91c244e3949bbe4c60bf1752", //todo: hardcode board.OwnerId,
                Offset = offset,
                Count = count,
                SortBy = new[] { new SortField("Published", false) }
            };
            var searchResults = await _queryBuilder.For<IPagedEnumerable<SearchDocument>>().With(request);

            board.Cards = CreateCards(searchResults);

            return board;
        }

        public async Task<Board> GetBoardByQueryAsync(string query, int offset, int count)
        {
            var board = new Board
            {
                Published = DateTime.UtcNow,
                OwnerId = _workContextAccessor.WorkContext.Owner.Id,
                Name = string.Empty,
                RawQuery = query
            };

            if (!_authorizer.Authorize(Permissions.ViewBoard, board))
                return null;

            var request = new SearchRequest
            {
                Offset = offset,
                Count = count,
                OwnerId = "a314130a91c244e3949bbe4c60bf1752", //todo: hardcode board.OwnerId,
                Criteria = ParseSearchQuery(query),
                SortBy = new[] { new SortField("Published", false) }
            };

            var searchResults = await _queryBuilder.For<IPagedEnumerable<SearchDocument>>().With(request);

            board.Cards = CreateCards(searchResults);

            return board;
        }

        public async Task<IEnumerable<Board>> GetBoardsAsync()
        {
            var boards = await _queryBuilder.For<List<Board>>()
                .With(new OwnerKey(_workContextAccessor.WorkContext.Owner.Id));

            for (int i = boards.Count - 1; i >= 0; i--)
            {
                var board = boards[i];
                if (!_authorizer.Authorize(Permissions.ViewBoard, board))
                    boards.RemoveAt(i);
            }

            return boards.AsEnumerable();
        }

        public async Task<bool> UpdateBoardAsync(int boardId, string name, string query)
        {
            if (!_authorizer.Authorize(Permissions.UpdateBoard))
                throw new SecurityException(string.Format("The user '{0}' doest not has permission to update a board '{1}'", _workContextAccessor.WorkContext.User.UserName, boardId));

            var board = await _queryBuilder.For<Board>()
                .With(new ContentKey(boardId, _workContextAccessor.WorkContext.Owner.Id));

            if (board == null)
                return false;

            var hasChanges = false;
            if (!string.Equals(query, board.RawQuery, StringComparison.InvariantCultureIgnoreCase))
            {
                board.RawQuery = query;
                hasChanges = true;
            }

            if (!string.Equals(name, board.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                board.Name = name;
                hasChanges = true;
            }

            if (hasChanges)
            {
                using (var unitOfWork = _unitOfWorkFactory.Create())
                {
                    unitOfWork.Save(board);
                    unitOfWork.Commit();
                }
            }

            return hasChanges;
        }

        private IPagedEnumerable<Card> CreateCards(IPagedEnumerable<SearchDocument> searchResults)
        {
            var result = new List<Card>(searchResults.Count);

            foreach (dynamic item in searchResults)
            {
                if (item.DocumentType == "Event")
                {
                    var eventCard = new EventCard
                    {
                        Id = item.Id,
                        OwnerId = item.OwnerId,
                        Published = item.Published,
                        Name = item.Name,
                        Description = item.Description,
                        Duration = item.Duration,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate
                    };
                    result.Add(eventCard);
                }
                else if (item.DocumentType == "Place")
                {
                    var placeCard = new PlaceCard
                    {
                        Id = item.Id,
                        OwnerId = item.OwnerId,
                        Published = item.Published,
                        Name = item.Name,
                        Description = item.Description,
                        Elevation = item.Elevation,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude
                    };

                    var placeImageUrl = new StringBuilder("//maps.googleapis.com/maps/api/staticmap?");
                    placeImageUrl.Append("zoom=15");
                    placeImageUrl.Append("&size=600x200");
                    placeImageUrl.Append("&maptype=roadmap");
                    placeImageUrl.AppendFormat("&center={0},{1}", placeCard.Latitude, placeCard.Longitude);
                    placeImageUrl.AppendFormat("&markers=color:red%7Clabel:C%7C{0},{1}", placeCard.Latitude, placeCard.Longitude);
                    placeCard.Thumbnail = placeImageUrl.ToString();

                    result.Add(placeCard);
                }
                else if (item.DocumentType == "Article" || item.Verb == ActivityVerbs.Read)
                {
                    var articleCard = new ArticleCard
                    {
                       // Id = item.Id,
                       // OwnerId = item.OwnerId,
                        Published = item.Published,
                        Name = item.Title,
                        Description = item.Content,
                        Body = item.Content,
                        Summary = item.Content,
                        Thumbnail = item.Thumbnail,
                        Url = item.Url
                    };
                    result.Add(articleCard);
                }
                else if (item.DocumentType == "Audio")
                {
                    var audioCard = new AudioCard
                    {
                        Id = item.Id,
                        OwnerId = item.OwnerId,
                        Published = item.Published,
                        Name = item.Name,
                        Description = item.Description,
                        ByArtist = item.ByArtist,
                        Duration = item.Duration,
                        InAlbum = item.InAlbum
                    };
                    result.Add(audioCard);
                }
            }

            return PagedEnumerable.Create(result, searchResults.Count, searchResults.TotalCount);
        }

        private IList<ISearchCriteria> ParseSearchQuery(string query)
        {
            return _searchQueryParser.Parse(query)
                .RewriteWith(DefaultFieldsMap)
                .AsCriteria()
                .ToList();
        }
    }
}