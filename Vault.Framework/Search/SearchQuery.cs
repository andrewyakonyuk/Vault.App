using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;
using Vault.Shared.Queries;

namespace Vault.Framework.Search
{
    public class SearchQuery : IQuery<SearchRequest, IPagedEnumerable<SearchDocument>>
    {
        readonly ISearchProvider _searchProvider;

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