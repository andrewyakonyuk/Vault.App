using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using Vault.Framework.Search.Criteria;
using Vault.Shared;
using Vault.Shared.Lucene;

namespace Vault.Framework.Search
{
    public class LuceneSearchProvider : ISearchProvider
    {
        private readonly IIndexWriterAccessor _writerAccessor;
        private readonly ISearchResultTransformer _defaultResultTransformer;

        public LuceneSearchProvider(
            IIndexWriterAccessor writerAccessor,
            ISearchResultTransformer resultTransformer)
        {
            if (writerAccessor == null)
                throw new ArgumentNullException(nameof(writerAccessor));
            if (resultTransformer == null)
                throw new ArgumentNullException(nameof(resultTransformer));

            _writerAccessor = writerAccessor;
            _defaultResultTransformer = resultTransformer;
        }

        public IPagedEnumerable<SearchDocument> Search(SearchRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var numHits = request.Offset + request.Count;
            var query = new MatchAllDocsQuery();
            var sort = new Sort();
            var collector = TopFieldCollector.Create(sort, Math.Max(numHits, 1), false, false, false, false);
            var filter = CreateFilter(request);

            using (var indexReader = _writerAccessor.Writer.GetReader())
            using (var indexSearcher = new IndexSearcher(indexReader))
            {
                // the search() method returns a Hits object re-executes the search internally when you need more than 100 hits.
                // so, we need to use the search method that takes a HitCollector instead
                indexSearcher.Search(query, filter, collector);
                var topDocs = collector.TopDocs();
                if (numHits <= 0)
                    return PagedEnumerable.Create(new SearchDocument[0], topDocs.TotalHits);

                var scoreDocs = topDocs.ScoreDocs;
                var requestedCount = scoreDocs.Length - request.Offset;
                if (requestedCount <= 0)
                    return PagedEnumerable.Create(new SearchDocument[0], topDocs.TotalHits);

                var result = new List<SearchDocument>(requestedCount);
                var resultTransformer = request.ResultTransformer ?? _defaultResultTransformer;

                for (var i = request.Offset; i < scoreDocs.Length; i++)
                {
                    var doc = indexSearcher.Doc(scoreDocs[i].Doc);
                    var valuesProvider = new LuceneDocumentValuesProvider(doc);
                    var document = resultTransformer.Transform(valuesProvider);
                    result.Add(document);
                }

                return PagedEnumerable.Create(result, topDocs.TotalHits);
            }
        }

        protected virtual Filter CreateFilter(SearchRequest request)
        {
            ISearchFilterBuilder criteriaBuilder = new LuceneFilterBuilder();
            criteriaBuilder.AddEqual("_ownerId", request.OwnerId, false);
            for (var i = 0; i < request.Criteria.Count; i++)
            {
                request.Criteria[i].Apply(criteriaBuilder);
            }
            return (Filter)criteriaBuilder.Build();
        }
    }
}