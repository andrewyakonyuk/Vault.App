using System;
using System.Threading.Tasks;

namespace Vault.Shared.Queries
{
    public static class QueryForExtensions
    {
        public static Task<TResult> ById<TResult>(this IQueryFor<TResult> queryFor, int id)
        {
            if (queryFor == null)
                throw new ArgumentNullException(nameof(queryFor));

            return queryFor.With(new FindById(id));
        }

        public static Task<TResult> All<TResult>(this IQueryFor<TResult> queryFor)
        {
            if (queryFor == null)
                throw new ArgumentNullException(nameof(queryFor));

            return queryFor.With(new AllEntities());
        }

        public static Task<IPagedEnumerable<TResult>> Paging<TResult>(this IQueryFor<IPagedEnumerable<TResult>> queryFor, int offset, int count)
        {
            if (queryFor == null)
                throw new ArgumentNullException(nameof(queryFor));

            return queryFor.With(new Paging(offset, count));
        }
    }
}