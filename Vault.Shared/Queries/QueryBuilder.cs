namespace Vault.Shared.Queries
{
    public sealed class QueryBuilder : IQueryBuilder
    {
        private readonly IQueryFactory _queryFactory;

        public QueryBuilder(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public IQueryFor<TResult> For<TResult>()
        {
            return new QueryFor<TResult>(_queryFactory);
        }
    }
}