using System;
using System.Collections.Concurrent;
using Vault.Shared.Search;
using Vault.Shared.Search.Lucene;

namespace Vault.Activity.Indexes
{
    public interface IIndexStoreAccessor
    {
        IndexStore NewIndexStore<TDocument>(AbstractIndexCreationTask<TDocument> creationTask);
        IndexStore GetIndexStore(string indexName);
    }

    public class DefaultIndexStoreAccessor : IIndexStoreAccessor
    {
        readonly IIndexWriterAccessor _writerAccessor;
        readonly ConcurrentDictionary<string, IndexStore> _indexSets;
        readonly IIndexDocumentTransformer _documentTransformer;
        readonly ISearchResultTransformer _resultTransformer;

        public DefaultIndexStoreAccessor(
            IIndexWriterAccessor writerAccessor,
            IIndexDocumentTransformer documentTransformer,
            ISearchResultTransformer resultTransformer)
        {
            _writerAccessor = writerAccessor;
            _documentTransformer = documentTransformer;
            _resultTransformer = resultTransformer;
            _indexSets = new ConcurrentDictionary<string, IndexStore>(StringComparer.InvariantCultureIgnoreCase);
        }

        public IndexStore NewIndexStore<TDocument>(AbstractIndexCreationTask<TDocument> creationTask)
        {
            return _indexSets.GetOrAdd(creationTask.IndexName, _ =>
            {
                var indexName = creationTask.IndexName;
                var metadata = creationTask.GetIndexMetadata();

                return new IndexStore(indexName, metadata, _writerAccessor, _documentTransformer, _resultTransformer);
            });
        }

        public IndexStore GetIndexStore(string indexName)
        {
            IndexStore indexSet = null;
            if (_indexSets.TryGetValue(indexName, out indexSet))
                return indexSet;

            return null;
        }
    }
}
