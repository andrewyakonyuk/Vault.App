using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vault.Framework.Search.Parsing
{
    public class SearchQueryTokenStream : IEnumerable<SearchQueryToken>
    {
        public static SearchQueryTokenStream Empty = new SearchQueryTokenStream();

        public string RawQuery { get; protected set; }

        private readonly IEnumerable<SearchQueryToken> _tokens;

        public SearchQueryTokenStream(string query, IEnumerable<SearchQueryToken> tokens)
        {
            RawQuery = query;
            _tokens = tokens;
        }

        protected SearchQueryTokenStream()
            : this(string.Empty, Enumerable.Empty<SearchQueryToken>())
        {
        }

        public IEnumerator<SearchQueryToken> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class SearchQueryToken
    {
        public string FieldName { get; private set; }
        public string Type { get; private set; }
        public string RawValue { get; private set; }

        public SearchQueryToken(string fieldName, string type, string rawValue)
        {
            FieldName = fieldName;
            Type = type;
            RawValue = rawValue;
        }
    }
}