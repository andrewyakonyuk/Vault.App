using System.Threading.Tasks;

namespace Vault.Shared.Queries
{
    public class QueryFor<TResult> : IQueryFor<TResult>
    {
        private readonly IQueryFactory _factory;

        public QueryFor(IQueryFactory factory)
        {
            _factory = factory;
        }

        public Task<TResult> With<TCriterion>(TCriterion criterion)
            where TCriterion : ICriterion
        {
            return _factory.Create<TCriterion, TResult>().AskAsync(criterion);
        }
    }
}