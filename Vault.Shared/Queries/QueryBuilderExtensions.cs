using System;
using System.Threading.Tasks;

namespace Vault.Shared.Queries
{
    public static class QueryBuilderExtensions
    {
        public static async Task<int> Count<TCriterion>(this IQueryBuilder queryBuilder, TCriterion criterion)
            where TCriterion : class, ICriterion
        {
            if (queryBuilder == null)
                throw new ArgumentNullException("queryBuilder");

            return await queryBuilder
                .For<int>()
                .With(criterion);
        }
    }
}