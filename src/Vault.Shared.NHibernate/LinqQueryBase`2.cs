using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;
using Vault.Shared.Queries;

namespace Vault.Shared.NHibernate
{
    public abstract class LinqQueryBase<TCriterion, TResult> : IQuery<TCriterion, TResult>
        where TCriterion : ICriterion
    {
        private readonly ILinqProvider _linq;

        protected LinqQueryBase(ILinqProvider linq)
        {
            _linq = linq;
        }

        public abstract Task<TResult> AskAsync(TCriterion criterion);

        protected virtual IQueryable<TEntity> Query<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return _linq.Query<TEntity>();
        }
    }
}