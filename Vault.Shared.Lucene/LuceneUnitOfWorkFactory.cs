using System;
using System.Data;
using Vault.Shared.Domain;

namespace Vault.Shared.Lucene
{
    public class LuceneUnitOfWorkFactory : IUnitOfWorkFactory
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

        public IUnitOfWork Create()
        {
            return Create(IsolationLevel.Unspecified);
        }

        public IUnitOfWork Create(IsolationLevel isolationLevel)
        {
            return new LuceneUnitOfWork(_writerAccessor.Writer, _documentTransformer, _metadataProvider);
        }
    }
}