using System;
using System.Data;
using Vault.Shared.Domain;

namespace Vault.Shared.Search.Lucene
{
    public class LuceneUnitOfWorkFactory : IIndexUnitOfWorkFactory
    {
        readonly IIndexWriterAccessor _writerAccessor;
        readonly IIndexDocumentTransformer _documentTransformer;
        readonly IIndexDocumentMetadataProvider _metadataProvider;

        public LuceneUnitOfWorkFactory(
            IIndexWriterAccessor writerAccessor,
            IIndexDocumentTransformer documentTransformer,
            IIndexDocumentMetadataProvider metadataProvider)
        {
            if (writerAccessor == null)
                throw new ArgumentNullException(nameof(writerAccessor));
            if (documentTransformer == null)
                throw new ArgumentNullException(nameof(documentTransformer));
            if (metadataProvider == null)
                throw new ArgumentNullException(nameof(metadataProvider));

            _writerAccessor = writerAccessor;
            _documentTransformer = documentTransformer;
            _metadataProvider = metadataProvider;
        }

        public IIndexUnitOfWork Create(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                indexName = IndexNames.Default;
            var indexWriter = _writerAccessor.GetWriter(indexName);
            var metadata = _metadataProvider.GetMetadata(indexName);
            return new LuceneUnitOfWork(indexWriter, _documentTransformer, metadata);
        }
    }
}