using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;
using Vault.Shared.Search;
using Vault.Shared.Search.Criteria;
using Vault.Shared.Search.Parsing;

namespace Vault.Activity.Services.Search
{
    public interface ISearchGrain : IGrainWithIntegerKey
    {
        Task<IPagedEnumerable<SearchDocument>> Search(SearchRequest request);

        Task<SearchQueryTokenStream> ParseSearchQuery(string query);
    }

    public class SearchGrain : Grain, ISearchGrain
    {
        readonly ISearchProvider _searchProvider;
        private readonly ISearchQueryParser _searchQueryParser;

        readonly static IDictionary<string, string> DefaultFieldsMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "type", "documentType" },
            { "published", "published" },
            { "startDate", "startdate" },
            { "endDate", "enddate" },
            { "title", "name" },
            { "description", "description" },
            { "desc", "description" },
            { "duration", "duration" },
            { "artist", "byartist" },
            { "album", "inalbum" },
            { "keywords" , "keywords" }
        };

        public SearchGrain(
            ISearchProvider searchProvider,
            ISearchQueryParser searchQueryParser)
        {
            _searchProvider = searchProvider;
            _searchQueryParser = searchQueryParser;
        }

        public Task<IPagedEnumerable<SearchDocument>> Search(SearchRequest request)
        {
            var searchResult = _searchProvider.Search(request);

            return Task.FromResult(searchResult);
        }

        public Task<SearchQueryTokenStream> ParseSearchQuery(string query)
        {
            var tokenStream = _searchQueryParser.Parse(query);

            return Task.FromResult(tokenStream);
        }
    }
}
