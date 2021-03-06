﻿using System.Collections.Generic;
using System.Linq;

namespace Vault.Shared.Search.Parsing
{
    public class RewrittenSearchQueryTokenStream : SearchQueryTokenStream
    {
        public RewrittenSearchQueryTokenStream(SearchQueryTokenStream tokenStream, IDictionary<string, string> fieldsMap)
            : base(tokenStream.RawQuery, RewriteTokenStream(tokenStream, fieldsMap))
        {
        }

        private static IEnumerable<SearchQueryToken> RewriteTokenStream(SearchQueryTokenStream tokenStream, IDictionary<string, string> fieldsMap)
        {
            foreach (var item in tokenStream.AsEnumerable())
            {
                string fieldName;
                if (fieldsMap.TryGetValue(item.FieldName, out fieldName))
                {
                    yield return new SearchQueryToken(fieldName, item.Type, item.RawValue);
                }
            }
        }
    }
}