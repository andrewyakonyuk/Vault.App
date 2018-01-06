using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Shared.Search.Criteria;

namespace Vault.Shared.Search.Parsing
{
    public static class SearchQueryTokenStreamExtensions
    {
        public static IEnumerable<ISearchCriteria> AsCriteria(this SearchQueryTokenStream tokenStream)
        {
            foreach (var token in tokenStream.AsEnumerable())
            {
                switch (token.Type.ToLowerInvariant())
                {
                    case ">":
                        yield return new GreaterCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue
                        };
                        break;

                    case ">=":
                        yield return new GreaterCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue,
                            Strict = true
                        };
                        break;

                    case "<":
                        yield return new LessCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue
                        };
                        break;

                    case "<=":
                        yield return new LessCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue,
                            Strict = true
                        };
                        break;

                    case "=":
                        yield return new EqualCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue
                        };
                        break;

                    case "!=":
                        yield return new EqualCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue,
                            Not = true
                        };
                        break;

                    case "in":
                        yield return new ContainsCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue
                        };
                        break;

                    case "nin":
                        yield return new ContainsCriteria
                        {
                            FieldName = token.FieldName,
                            Value = token.RawValue,
                            Not = true
                        };
                        break;

                    default:
                        throw new IndexOutOfRangeException(string.Format("Operator '{0}' is not supported in the search queries", token.Type));
                }
            }
        }

        public static SearchQueryTokenStream RewriteWith(this SearchQueryTokenStream tokenStream, IDictionary<string, string> fieldsMap)
        {
            if (tokenStream == null)
                return SearchQueryTokenStream.Empty;

            return new RewrittenSearchQueryTokenStream(tokenStream, fieldsMap);
        }
    }
}