using Vault.Shared;
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;

namespace Vault.Activity.Indexes
{
    public class IndexStore : ISearchProvider, IIndexUnitOfWorkFactory
    {
        readonly IIndexWriterAccessor _writerAccessor;
        readonly string _indexName;
        readonly IndexDocumentMetadata _metadata;
        readonly IIndexDocumentTransformer _documentTransformer;
        readonly ISearchResultTransformer _resultTransformer;

        public IndexStore(
            string indexName,
            IndexDocumentMetadata metadata,
            IIndexWriterAccessor writerAccessor,
            IIndexDocumentTransformer documentTransformer,
            ISearchResultTransformer resultTransformer)
        {
            _indexName = indexName;
            _metadata = metadata;
            _writerAccessor = writerAccessor;
            _documentTransformer = documentTransformer;
            _resultTransformer = resultTransformer;
        }

        public IPagedEnumerable<SearchDocument> Search(SearchRequest request)
        {
            var indexWriter = _writerAccessor.GetWriter(_indexName);
            var searchProvider = new LuceneSearchProvider(indexWriter, _resultTransformer, _metadata);
            return searchProvider.Search(request);
        }

        public IIndexUnitOfWork CreateUnitOfWork()
        {
            var indexWriter = _writerAccessor.GetWriter(_indexName).Value;
            return new LuceneUnitOfWork(indexWriter, _documentTransformer, _metadata);
        }
    }
}
