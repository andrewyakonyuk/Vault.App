using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vault.Framework.Search;
using Vault.Framework.Search.Criteria;
using Vault.Framework.Search.Parsing;
using Vault.Framework.Security;
using Vault.Shared;
using Vault.Shared.Domain;
using Vault.Shared.Events;
using Vault.Shared.Queries;

namespace Vault.Framework.Api.Boards
{
    public class BoardsApi : IBoardsApi
    {
        private readonly IAuthorizer _authorizer;
        private readonly IEventPublisher _eventPublisher;
        private readonly IReportUnitOfWorkFactory _reportUnitOfWorkFactory;
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchQueryParser _searchQueryParser;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IQueryBuilder _queryBuilder;

        readonly static IDictionary<string, string> DefaultFieldsMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "type", "_documentType" },
            { "published", "_published" },
            { "startDate", "startDate" },
            { "endDate", "endDate" },
            { "title", "name" },
            { "description", "description" },
            { "desc", "description" },
            { "duration", "duration" },
            { "artist", "byArtist" },
            { "album", "inAlbum" },
            { "keywords" , "keywords" }
        };

        public BoardsApi(
            IWorkContextAccessor workContextAccessor,
            IUnitOfWorkFactory unitOfWorkFactory,
            ISearchProvider searchProvider,
            IAuthorizer authorizer,
            IEventPublisher eventPublisher,
            ISearchQueryParser searchQueryParser,
            IReportUnitOfWorkFactory reportUnitOfWorkFactory,
            IQueryBuilder queryBuilder)
        {
            _workContextAccessor = workContextAccessor;
            _unitOfWorkFactory = unitOfWorkFactory;
            _searchProvider = searchProvider;
            _authorizer = authorizer;
            _eventPublisher = eventPublisher;
            _searchQueryParser = searchQueryParser;
            _reportUnitOfWorkFactory = reportUnitOfWorkFactory;
            _queryBuilder = queryBuilder;
        }

        public Task<Board> CreateBoardAsync(string name, string query)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!_authorizer.Authorize(Permissions.CreateBoard))
                throw new SecurityException(string.Format("The user '{0}' does not has permission to create a new board", _workContextAccessor.WorkContext.User.UserName));

            //  Hook();

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

                _eventPublisher.EntityCreated(board);

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
                await _eventPublisher.EntityDeleted(board);
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
                OwnerId = board.OwnerId,
                Offset = offset,
                Count = count
            };
            var searchResults = _searchProvider.Search(request);

            board.Cards = CreateCards(searchResults);

            return board;
        }

        public Task<Board> GetBoardByQueryAsync(string query, int offset, int count)
        {
            var request = new SearchRequest
            {
                Offset = offset,
                Count = count,
                OwnerId = _workContextAccessor.WorkContext.Owner.Id,
                Criteria = ParseSearchQuery(query)
            };

            var searchResults = _searchProvider.Search(request);

            var board = new Board
            {
                Published = DateTime.UtcNow,
                OwnerId = request.OwnerId,
                Name = string.Empty,
                RawQuery = query,
                Cards = CreateCards(searchResults)
            };

            return Task.FromResult(board);
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
                    await _eventPublisher.EntityUpdated(board);
                }
            }

            return hasChanges;
        }

        private IPagedEnumerable<Card> CreateCards(IPagedEnumerable<SearchDocument> searchResults)
        {
            var result = new List<Card>(searchResults.Count());

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
                else if (item.DocumentType == "Article")
                {
                    var articleCard = new ArticleCard
                    {
                        Id = item.Id,
                        OwnerId = item.OwnerId,
                        Published = item.Published,
                        Name = item.Name,
                        Description = item.Description,
                        Body = item.Body,
                        Summary = item.Summary
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

            return PagedEnumerable.Create(result, searchResults.TotalCount);
        }

        private void Hook()
        {
            var random = new Random();

            var types = new[] { "Event", "Place", "Article", "Audio" };
            var mapPoints = new[] {
                new {
                    Latitude = 40.714728, Longitude = -73.998672
                },
                 new {
                    Latitude = 49.24195, Longitude = 8.5491213
                },
                  new {
                    Latitude = 50.4496346, Longitude = 30.5231952
                },
                   new {
                    Latitude = 50.2481061, Longitude = 28.6802412
                },
            };

            using (var unitOfWork = _reportUnitOfWorkFactory.Create())
            {
                for (int i = _workContextAccessor.WorkContext.Owner.Id * 1001; i < _workContextAccessor.WorkContext.Owner.Id * 1001 + 10000; i++)
                {
                    var actualType = types[Math.Min((int)(random.Next(0, 40) / 10), 3)];

                    dynamic searchDocument = new SearchDocument();

                    searchDocument.Id = i;
                    searchDocument.OwnerId = _workContextAccessor.WorkContext.Owner.Id;
                    searchDocument.Published = DateTime.UtcNow;
                    searchDocument.DocumentType = actualType;

                    if (actualType == "Event")
                    {
                        searchDocument.Name = "Event" + i;
                        searchDocument.Description = "EventDescription" + i;
                        searchDocument.Duration = new TimeSpan(1, 30, 0);
                        searchDocument.StartDate = DateTime.UtcNow;
                        searchDocument.EndDate = DateTime.UtcNow.AddHours(1.5);
                    }
                    else if (actualType == "Place")
                    {
                        searchDocument.Name = "Place" + i;
                        searchDocument.Description = "Place description" + i;
                        searchDocument.Elevation = random.NextDouble();
                        var point = mapPoints[Math.Min((int)(random.Next(0, 40) / 10), 3)];
                        searchDocument.Latitude = point.Latitude;
                        searchDocument.Longitude = point.Longitude;
                    }
                    else if (actualType == "Audio")
                    {
                        searchDocument.Name = "Audio" + i;
                        searchDocument.Description = "Audio description" + i;
                        searchDocument.ByArtist = "By artist" + i;
                        searchDocument.InAlbum = "In album" + i;
                        searchDocument.Duration = new TimeSpan(0, 3, 33);
                    }
                    else if (actualType == "Article")
                    {
                        searchDocument.Name = "Article" + i;
                        searchDocument.Description = "Article description" + i;
                        searchDocument.Body = "Article body" + i;
                        searchDocument.Summary = "Article summary" + i;
                    }

                    unitOfWork.Save(searchDocument);
                }

                unitOfWork.Commit();
            }
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