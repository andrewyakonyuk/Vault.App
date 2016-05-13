using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System;

namespace Vault.Framework.Search.Criteria
{
    public class LuceneFilterBuilder : ISearchFilterBuilder
    {
        private readonly BooleanQuery _filterQuery;

        public LuceneFilterBuilder()
        {
            _filterQuery = new BooleanQuery();
        }

        public ISearchFilterBuilder AddBetween(string fieldName, object lowValue, object highValue, bool strict)
        {
            if (lowValue == null)
            {
                return AddLess(fieldName, highValue, strict);
            }
            else if (highValue == null)
            {
                return AddGreater(fieldName, lowValue, strict);
            }
            else
            {
                _filterQuery.Add(new TermRangeQuery(QueryParser.Escape(fieldName), QueryParser.Escape(Convert.ToString(lowValue)), QueryParser.Escape(Convert.ToString(highValue)), !strict, !strict), Occur.SHOULD);
                return this;
            }
        }

        public ISearchFilterBuilder AddContains(string fieldName, object value, bool not)
        {
            _filterQuery.Add(new WildcardQuery(new Term(QueryParser.Escape(fieldName), "*" + QueryParser.Escape(Convert.ToString(value)) + "*")), not ? Occur.MUST_NOT : Occur.MUST);
            return this;
        }

        public ISearchFilterBuilder AddEqual(string fieldName, object value, bool not)
        {
            var clause = new BooleanClause(new TermQuery(new Term(QueryParser.Escape(fieldName), QueryParser.Escape(Convert.ToString(value)))), not ? Occur.MUST_NOT : Occur.MUST);
            _filterQuery.Add(clause);
            return this;
        }

        public ISearchFilterBuilder AddGreater(string fieldName, object value, bool strict)
        {
            _filterQuery.Add(new TermRangeQuery(QueryParser.Escape(fieldName), QueryParser.Escape(Convert.ToString(value)), (string)null, !strict, false), Occur.SHOULD);
            return this;
        }

        public ISearchFilterBuilder AddLess(string fieldName, object value, bool strict)
        {
            _filterQuery.Add(new TermRangeQuery(QueryParser.Escape(fieldName), (string)null, QueryParser.Escape(Convert.ToString(value)), !strict, false), Occur.SHOULD);
            return this;
        }

        public object Build()
        {
            var wrappedQuery = new QueryWrapperFilter(_filterQuery);
            return FilterManager.Instance.GetFilter(wrappedQuery);
        }
    }
}