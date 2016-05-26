using System;
using System.Threading.Tasks;
using Vault.Shared.Queries;

namespace Vault.Shared.Search
{
    public class SearchQuery : IQuery<SearchRequest, IPagedEnumerable<SearchDocument>>
    {
        private readonly ISearchProvider _searchProvider;

        public SearchQuery(ISearchProvider searchProvider)
        {
            if (searchProvider == null)
                throw new ArgumentNullException(nameof(searchProvider));

            _searchProvider = searchProvider;
        }

        public Task<IPagedEnumerable<SearchDocument>> AskAsync(SearchRequest criterion)
        {
            if (criterion == null)
                throw new ArgumentNullException(nameof(criterion));

            var searchResult = _searchProvider.Search(criterion);

            return Task.FromResult(searchResult);
        }
    }
}