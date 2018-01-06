using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System;
using System.ComponentModel;
using System.Linq;
using Vault.Shared.Search.Criteria;
using Version = Lucene.Net.Util.Version;

namespace Vault.Shared.Search.Lucene.Criteria
{
    public class LuceneSearchCriteriaBuilder : ISearchCriteriaBuilder
    {
        readonly bool _allowLeadingWildcard;
        readonly Analyzer _analyzer;
        readonly IndexDocumentMetadata _documentMetadata;
        private readonly Occur _occurance = Occur.MUST;
        private readonly BooleanQuery _query;
        readonly MultiFieldQueryParserWrapper _queryParser;
        readonly Version _version;

        public LuceneSearchCriteriaBuilder(
            IndexDocumentMetadata documentMetadata,
            Analyzer analyzer,
            Version version,
            bool allowLeadingWildcards)
        {
            _query = new BooleanQuery();
            _query.Add(new MatchAllDocsQuery(), Occur.SHOULD);
            _documentMetadata = documentMetadata;
            _analyzer = analyzer;
            _version = version;
            _allowLeadingWildcard = allowLeadingWildcards;

            var fields = _documentMetadata.Fields.Select(t => t.FieldName).ToArray();
            _queryParser = new MultiFieldQueryParserWrapper(_version, fields, _analyzer);
            _queryParser.AllowLeadingWildcard = allowLeadingWildcards;
        }

        protected internal Query Query { get { return _query; } }

        public IBooleanOperation Boolean()
        {
            return new LuceneBooleanOperation(this);
        }

        public IBooleanOperation Field(string fieldName, ISearchValue value)
        {
            return Field(fieldName, value, _occurance);
        }

        protected internal IBooleanOperation Field(string fieldName, ISearchValue value, Occur occurance)
        {
            var queryToAdd = GetFieldInternalQuery(fieldName, value);
            if (queryToAdd != null)
            {
                _query.Add(queryToAdd, occurance);
            }

            return new LuceneBooleanOperation(this);
        }

        public IBooleanOperation Grouped(ISearchCriteriaBuilder group)
        {
            return Grouped(group, _occurance);
        }

        protected internal IBooleanOperation Grouped(ISearchCriteriaBuilder group, Occur occurance)
        {
            _query.Add(((LuceneSearchCriteriaBuilder)group).Query, occurance);
            return new LuceneBooleanOperation(this);
        }

        public IBooleanOperation Range(string fieldName, object lower, object upper, bool includeLower, bool includeUpper)
        {
            fieldName = RewriteFieldName(fieldName);
            return Range(fieldName, lower, upper, includeLower, includeUpper, _occurance);
        }

        protected internal IBooleanOperation Range(string fieldName, object lower, object upper, bool includeLower, bool includeUpper, Occur occurance)
        {
            return Range(fieldName, (dynamic)lower, (dynamic)upper, includeLower, includeUpper, occurance);
        }

        protected IBooleanOperation Range(string fieldName, DateTime? lower, DateTime? upper, bool includeLower, bool includeUpper, Occur occurance)
        {
            var resolution = DateTools.Resolution.MILLISECOND;
            string lowerString = null;
            string upperString = null;
            if (lower.HasValue)
                lowerString = DateTools.DateToString(lower.Value, resolution);
            if (upper.HasValue)
                upperString = DateTools.DateToString(upper.Value, resolution);
            return Range(fieldName, lowerString, upperString, includeLower, includeUpper, occurance);
        }

        protected IBooleanOperation Range(string fieldName, int? lower, int? upper, bool includeLower, bool includeUpper, Occur occurance)
        {
            _query.Add(NumericRangeQuery.NewIntRange(fieldName, lower, upper, includeLower, includeUpper), occurance);
            return new LuceneBooleanOperation(this);
        }

        protected IBooleanOperation Range(string fieldName, long? lower, long? upper, bool includeLower, bool includeUpper, Occur occurance)
        {
            _query.Add(NumericRangeQuery.NewLongRange(fieldName, lower, upper, includeLower, includeUpper), occurance);
            return new LuceneBooleanOperation(this);
        }

        protected IBooleanOperation Range(string fieldName, double? lower, double? upper, bool includeLower, bool includeUpper, Occur occurance)
        {
            _query.Add(NumericRangeQuery.NewDoubleRange(fieldName, lower, upper, includeLower, includeUpper), occurance);
            return new LuceneBooleanOperation(this);
        }

        protected IBooleanOperation Range(string fieldName, float? lower, float? upper, bool includeLower, bool includeUpper, Occur occurance)
        {
            _query.Add(NumericRangeQuery.NewFloatRange(fieldName, lower, upper, includeLower, includeUpper), occurance);
            return new LuceneBooleanOperation(this);
        }

        protected IBooleanOperation Range(string fieldName, string lower, string upper, bool includeLower, bool includeUpper, Occur occurance)
        {
            _query.Add(new TermRangeQuery(fieldName, lower, upper, includeLower, includeUpper), occurance);
            return new LuceneBooleanOperation(this);
        }

        protected virtual Query GetFieldInternalQuery(string fieldName, ISearchValue value)
        {
            var rewrittenFieldName = RewriteFieldName(fieldName);
            var stringValue = ConvertValue(fieldName, value.Value);

            switch (value.Examineness)
            {
                case Examineness.Fuzzy:
                    return _queryParser.CreateFuzzyQuery(rewrittenFieldName, stringValue, value.Level);

                case Examineness.SimpleWildcard:
                case Examineness.ComplexWildcard:
                    return _queryParser.CreateWildcardQuery(rewrittenFieldName, stringValue);

                case Examineness.Escaped:
                    string escapedQuery = rewrittenFieldName + ":" + stringValue;
                    return ParseRawQuery(escapedQuery);

                case Examineness.Boosted:
                    var boostedQuery = _queryParser.CreateFieldQuery(rewrittenFieldName, stringValue);
                    boostedQuery.Boost = value.Level;
                    return boostedQuery;

                case Examineness.Proximity:
                    string proximityQuery = string.Concat(rewrittenFieldName, ":\"", stringValue, "\"~", Convert.ToInt32(value.Level).ToString());
                    return _queryParser.Parse(proximityQuery);

                case Examineness.Explicit:
                    return _queryParser.CreateFieldQuery(rewrittenFieldName, stringValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.Examineness));
            }
        }

        protected Query ParseRawQuery(string rawQuery)
        {
            QueryParser queryParser = new QueryParser(this._version, string.Empty, _analyzer);
            return queryParser.Parse(rawQuery);
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
            return convertedValue;
        }

        string RewriteFieldName(string fieldName)
        {
            fieldName = _documentMetadata.RewriteFieldName(fieldName);
            return QueryParser.Escape(fieldName);
        }

        private class MultiFieldQueryParserWrapper : MultiFieldQueryParser
        {
            public MultiFieldQueryParserWrapper(Version version, string[] fields, Analyzer analyzer)
                : base(version, fields, analyzer)
            {
            }

            public virtual Query CreateFieldQuery(string field, string queryText)
            {
                return GetFieldQuery(field, queryText);
            }

            public virtual Query CreateFieldQuery(string field, string queryText, int slop)
            {
                return GetFieldQuery(field, queryText, slop);
            }

            public virtual Query CreateFuzzyQuery(string field, string termStr, float minSimilarity)
            {
                return GetFuzzyQuery(field, termStr, minSimilarity);
            }

            public virtual Query CreateWildcardQuery(string field, string termStr)
            {
                return base.GetWildcardQuery(field, termStr);
            }
        }
    }
}