using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vault.Activity;
using Vault.Shared;
using Vault.Shared.Activity;
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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IQueryBuilder _queryBuilder;
        private readonly IActivityClient _activityClient;

        public BoardsApi(
            IWorkContextAccessor workContextAccessor,
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizer authorizer,
            IActivityClient activityClient,
            IQueryBuilder queryBuilder)
        {
            _workContextAccessor = workContextAccessor;
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizer = authorizer;
            _activityClient = activityClient;
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

            var stream = await _activityClient.GetStreamAsync(Buckets.Timeline, board.OwnerId.ToString());
            var response = await stream.ReadEventsAsync(board.RawQuery, 0, count);

            board.Cards = CreateCards(PagedEnumerable.Create(response, count, count));

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

            var stream = await _activityClient.GetStreamAsync(Buckets.Timeline, board.OwnerId.ToString());
            var response = await stream.ReadEventsAsync(query, 0, count);

            board.Cards = CreateCards(PagedEnumerable.Create(response, count, count));

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

        private IPagedEnumerable<Card> CreateCards(IPagedEnumerable<CommitedActivityEvent> searchResults)
        {
            var result = new List<Card>(searchResults.Count);

            foreach (var item in searchResults)
            {
                if (item.Verb == ActivityVerbs.Read)
                {
                    var articleCard = new ArticleCard
                    {
                        Published = item.Published.UtcDateTime,
                        Name = item.Title,
                        Description = item.Content,
                        Body = item.Content,
                        Summary = item.Content,
                        Thumbnail = item.MetaBag.Thumbnail,
                        Url = item.Uri
                    };
                    result.Add(articleCard);
                }
            }

            return PagedEnumerable.Create(result, searchResults.Count, searchResults.TotalCount);
        }
    }
}