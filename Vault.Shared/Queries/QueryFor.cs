using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Vault.Shared.Queries
{
    public class QueryFor<TResult> : IQueryFor<TResult>
    {
        private readonly IQueryFactory _factory;
        private readonly IList<IQueryInterceptor> _interceptors;

        public QueryFor(
            IQueryFactory factory,
            IEnumerable<IQueryInterceptor> interceptors)
        {
            _factory = factory;
            _interceptors = interceptors.OrderByDescending(t => t.Order).ToArray();
        }

        public Task<TResult> With<TCriterion>(TCriterion criterion)
            where TCriterion : ICriterion
        {
            var query = _factory.Create<TCriterion, TResult>();
            var askMethod = BuildMethod(query);
            return askMethod(criterion);
        }

        private AskQueryMethod<TCriterion, TResult> BuildMethod<TCriterion>(IQuery<TCriterion, TResult> query)
            where TCriterion : ICriterion
        {
            AskQueryMethod<TCriterion, TResult> result = query.AskAsync;

            for (int i = _interceptors.Count - 1; i >= 0; i--)
            {
                var item = _interceptors[i];

                var inner = result;
                result = (criterion) => item.AskAsync(criterion, inner);
            }
            return result;
        }
    }
}