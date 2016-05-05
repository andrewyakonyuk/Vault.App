using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Framework.Search
{
    public interface ISearchQueryParser
    {
        SearchQueryParserResult Parse(string query);
    }

    public class SearchQueryParserResult
    {
        public static SearchQueryParserResult Empty = new SearchQueryParserResult(string.Empty);

        public string RawQuery { get; private set; }

        public IEnumerable<SearchQueryGroup> Groups { get; private set; }

        public SearchQueryParserResult(string query, IEnumerable<SearchQueryGroup> groups)
        {
            RawQuery = query;
            Groups = groups;
        }

        public SearchQueryParserResult(string query)
            : this(query, new List<SearchQueryGroup>())
        {
        }
    }

    public class SearchQueryGroup
    {
        public string Field { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public SearchQueryGroup(string field, string type, string value)
        {
            Field = field;
            Type = type;
            Value = value;
        }

        public SearchQueryGroup()
        {
        }
    }
}