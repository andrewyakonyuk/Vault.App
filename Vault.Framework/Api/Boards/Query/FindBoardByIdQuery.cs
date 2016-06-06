using System;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;
using Vault.Shared.Domain;
using Vault.Shared.NHibernate;

namespace Vault.Framework.Api.Boards.Query
{
    public class FindBoardByIdQuery : LinqQueryBase<ContentKey, Board>
    {
        public FindBoardByIdQuery(ILinqProvider linqProvider)
            : base(linqProvider)
        {
        }

        public override Task<Board> AskAsync(ContentKey criterion)
        {
            if (criterion == null)
                throw new ArgumentNullException(nameof(criterion));

            var board = Query<Board>().SingleOrDefault(t =>
                 t.Id == criterion.EntityId
                 && t.OwnerId == criterion.OwnerId);

            return Task.FromResult(board);
        }
    }
}