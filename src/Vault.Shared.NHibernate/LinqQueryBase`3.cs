using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;
using Vault.Shared.Queries;

namespace Vault.Shared.NHibernate
{
    public abstract class LinqQueryBase<TEntity, TCriterion, TResult> : IQuery<TCriterion, TResult>
        where TCriterion : ICriterion
        where TEntity : class, IEntity, new()
    {
        private readonly ILinqProvider _linq;

        protected LinqQueryBase(ILinqProvider linq)
        {
            _linq = linq;
        }

        public virtual IQueryable<TEntity> Query
        {
            get { return _linq.Query<TEntity>(); }
        }

        public abstract Task<TResult> AskAsync(TCriterion criterion);
    }
}