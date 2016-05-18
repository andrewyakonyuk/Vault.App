using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System;
using System.ComponentModel;
using Vault.Shared.Lucene;

namespace Vault.Framework.Search.Criteria
{
    public class LuceneFilterBuilder : Filter, ISearchFilterBuilder
    {
        private readonly BooleanQuery _filterQuery;
        readonly IndexDocumentMetadata _documentMetadata;

        public LuceneFilterBuilder(IndexDocumentMetadata documentMetadata)
        {
            _filterQuery = new BooleanQuery();
            _documentMetadata = documentMetadata;
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
                _filterQuery.Add(new TermRangeQuery(RewriteFieldName(fieldName), ConvertValue(fieldName, lowValue), ConvertValue(fieldName, highValue), !strict, !strict), Occur.SHOULD);
                return this;
            }
        }

        public ISearchFilterBuilder AddContains(string fieldName, object value, bool not)
        {
            _filterQuery.Add(new WildcardQuery(new Term(RewriteFieldName(fieldName), "*" + ConvertValue(fieldName, value) + "*")), not ? Occur.MUST_NOT : Occur.MUST);
            return this;
        }

        public ISearchFilterBuilder AddEqual(string fieldName, object value, bool not)
        {
            var clause = new BooleanClause(new TermQuery(new Term(RewriteFieldName(fieldName), ConvertValue(fieldName, value))), not ? Occur.MUST_NOT : Occur.MUST);
            _filterQuery.Add(clause);
            return this;
        }

        public ISearchFilterBuilder AddGreater(string fieldName, object value, bool strict)
        {
            _filterQuery.Add(new TermRangeQuery(RewriteFieldName(fieldName), ConvertValue(fieldName, value), (string)null, !strict, false), Occur.SHOULD);
            return this;
        }

        public ISearchFilterBuilder AddLess(string fieldName, object value, bool strict)
        {
            _filterQuery.Add(new TermRangeQuery(RewriteFieldName(fieldName), (string)null, ConvertValue(fieldName, value), !strict, false), Occur.SHOULD);
            return this;
        }

        string ConvertValue(string name, object value)
        {
            if (value == null)
                return string.Empty;

            DocumentFieldDescriptor descriptor;
            TypeConverter converter = new StringConverter();
            if (_documentMetadata.TryGetDescriptor(name, out descriptor))
            {
                converter = descriptor.Converter;
            }
            else if (!string.Equals(name, IndexDocumentMetadata.KeywordsFieldName))
                throw new InvalidOperationException($"No descriptor specified for '{name}'");

            var convertedValue = converter.ConvertToString(value);
            convertedValue = convertedValue.ToLower();
            return QueryParser.Escape(convertedValue);
        }

        string RewriteFieldName(string fieldName)
        {
            fieldName = _documentMetadata.RewriteToFieldName(fieldName);
            return QueryParser.Escape(fieldName);
        }

        public override DocIdSet GetDocIdSet(IndexReader reader)
        {
            var wrappedQuery = new QueryWrapperFilter(_filterQuery);
            var innerFilter = FilterManager.Instance.GetFilter(wrappedQuery);
            return innerFilter.GetDocIdSet(reader);
        }
    }
}