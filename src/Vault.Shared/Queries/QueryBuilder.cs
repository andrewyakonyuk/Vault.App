using System.Collections.Generic;

namespace Vault.Shared.Queries
{
    public sealed class QueryBuilder : IQueryBuilder
    {
        private readonly IQueryFactory _queryFactory;
        private readonly IEnumerable<IQueryInterceptor> _interceptors;

        public QueryBuilder(
            IQueryFactory queryFactory,
            IEnumerable<IQueryInterceptor> interceptors
            )
        {
            _queryFactory = queryFactory;
            _interceptors = interceptors;
        }

        public IQueryFor<TResult> For<TResult>()
        {
            return new QueryFor<TResult>(_queryFactory, _interceptors);
        }
    }
}