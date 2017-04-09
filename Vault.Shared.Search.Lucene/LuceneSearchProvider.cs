using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using Vault.Shared.Search.Lucene.Criteria;
using Version = Lucene.Net.Util.Version;

namespace Vault.Shared.Search.Lucene
{
    using global::Lucene.Net.Analysis;
    using SortField = global::Lucene.Net.Search.SortField;

    public class LuceneSearchProvider : ISearchProvider
    {
        private readonly ISearchResultTransformer _defaultResultTransformer;
        private readonly IndexDocumentMetadata _metadata;
        private readonly IIndexWriterHolder _indexWriter;

        public LuceneSearchProvider(
            IIndexWriterHolder indexWriter,
            ISearchResultTransformer resultTransformer,
            IndexDocumentMetadata metadata)
        {
            if (indexWriter == null)
                throw new ArgumentNullException(nameof(indexWriter));
            if (resultTransformer == null)
                throw new ArgumentNullException(nameof(resultTransformer));
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            _indexWriter = indexWriter;
            _defaultResultTransformer = resultTransformer;
            _metadata = metadata;
        }

        public IPagedEnumerable<SearchDocument> Search(SearchRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            
            var indexWriter = _indexWriter.Value;
            var numHits = request.Offset + request.Count;
            var sort = CreateSort(request, _metadata);
            var query = CreateQuery(request, indexWriter.Analyzer, _metadata);
            var collector = TopFieldCollector.Create(sort, Math.Max(numHits, 1), false, false, false, false);
            var filter = CreateFilter(request, _metadata);

            using (var indexReader = indexWriter.GetReader())
            using (var indexSearcher = new IndexSearcher(indexReader))
            {
                // the search() method returns a Hits object re-executes the search internally when you need more than 100 hits.
                // so, we need to use the search method that takes a HitCollector instead
                indexSearcher.Search(query, filter, collector);
                var topDocs = collector.TopDocs();
                if (numHits <= 0)
                    return PagedEnumerable.Create(new SearchDocument[0], 0, topDocs.TotalHits);

                var scoreDocs = topDocs.ScoreDocs;
                var requestedCount = scoreDocs.Length - request.Offset;
                if (requestedCount <= 0)
                    return PagedEnumerable.Create(new SearchDocument[0], 0, topDocs.TotalHits);

                var result = new List<SearchDocument>(requestedCount);
                var resultTransformer = request.ResultTransformer ?? _defaultResultTransformer;

                for (var i = request.Offset; i < scoreDocs.Length; i++)
                {
                    var doc = indexSearcher.Doc(scoreDocs[i].Doc);
                    var valuesProvider = new LuceneDocumentValuesProvider(doc);
                    var document = resultTransformer.Transform(valuesProvider, _metadata);
                    result.Add(document);
                }

                return PagedEnumerable.Create(result, result.Count, topDocs.TotalHits);
            }
        }

        Query CreateQuery(SearchRequest request, Analyzer analyzer, IndexDocumentMetadata metadata)
        {
            if (request.Criteria.Count == 0)
                return new MatchAllDocsQuery();

            var builder = new LuceneSearchCriteriaBuilder(metadata, analyzer, Version.LUCENE_30, false);

            for (var i = 0; i < request.Criteria.Count; i++)
            {
                request.Criteria[i].Apply(builder);
            }

            return builder.Query;
        }

        Filter CreateFilter(SearchRequest request, IndexDocumentMetadata metadata)
        {
            //todo: map owner
            var query = new TermQuery(new Term(metadata.RewriteFieldName("StreamId"), request.OwnerId.ToString(CultureInfo.InvariantCulture)));
            return FilterManager.Instance.GetFilter(new QueryWrapperFilter(query));
        }

        Sort CreateSort(SearchRequest request, IndexDocumentMetadata metadata)
        {
            if (request.SortBy == null || request.SortBy.Count == 0)
                return new Sort();

            var sortFields = new List<SortField>(request.SortBy.Count);
            foreach (var item in request.SortBy)
            {
                var fieldName = metadata.RewriteFieldName(item.FieldName);
                var sortField = new SortField(fieldName, SortField.STRING, !item.Ascending);
                sortFields.Add(sortField);
            }
            return new Sort(sortFields.ToArray());
        }
    }
}