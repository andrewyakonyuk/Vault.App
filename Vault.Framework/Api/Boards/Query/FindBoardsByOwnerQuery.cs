using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;
using Vault.Shared.Domain;
using Vault.Shared.NHibernate;

namespace Vault.Framework.Api.Boards.Query
{
    public class FindBoardsByOwnerQuery : LinqQueryBase<OwnerKey, List<Board>>
    {
        public FindBoardsByOwnerQuery(ILinqProvider linqProvider)
            : base(linqProvider)
        {
        }

        public override Task<List<Board>> AskAsync(OwnerKey criterion)
        {
            var boards = Query<Board>()
                .Where(t => t.OwnerId == criterion.OwnerId)
                .ToList();

            return Task.FromResult(boards);
        }
    }
}