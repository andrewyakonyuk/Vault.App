using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Shared.Domain;

namespace Vault.Shared.Search.Lucene
{
    public class LuceneUnitOfWork : IUnitOfWork
    {
        IndexWriter _indexWriter;
        IIndexDocumentTransformer _documentTransformer;
        IIndexDocumentMetadataProvider _metadataProvider;
        bool _isCommited = false;
        bool _isDisposed = false;
        int _uncommitedCount = 0;

        public LuceneUnitOfWork(
            IndexWriter indexWriter,
            IIndexDocumentTransformer documentTransformer,
            IIndexDocumentMetadataProvider metadataProvider)
        {
            if (indexWriter == null)
                throw new ArgumentNullException(nameof(indexWriter));
            if (documentTransformer == null)
                throw new ArgumentNullException(nameof(documentTransformer));
            if (metadataProvider == null)
                throw new ArgumentNullException(nameof(metadataProvider));

            _indexWriter = indexWriter;
            _documentTransformer = documentTransformer;
            _metadataProvider = metadataProvider;
        }

        public void Commit()
        {
            if (_uncommitedCount > 0)
            {
                try
                {
                    _indexWriter.PrepareCommit();
                    _indexWriter.Commit(new Dictionary<string, string> { { "ETag", DateTime.UtcNow.Ticks.ToString() } });
                }
                catch (OutOfMemoryException)
                {
                    _indexWriter.Dispose();
                    throw;
                }
                finally
                {
                    _isCommited = true;
                    _uncommitedCount = 0;
                }
            }
        }

        public void Save(IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Delete(entity);
            var indexDocument = _documentTransformer.Transform(entity);
            try
            {
                _indexWriter.AddDocument(indexDocument);
                _uncommitedCount++;
            }
            catch (OutOfMemoryException)
            {
                _indexWriter.Dispose();
                throw;
            }
        }

        public void Delete(IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var metadata = _metadataProvider.GetMetadata();
            var document = ObjectDictionary.Create(entity);

            if (!metadata.Keys.Any())
                throw new InvalidOperationException("Any document keys are not specified");

            var conditionForDelete = new BooleanQuery();
            foreach (var descriptor in metadata.Keys)
            {
                var convertedValue = descriptor.Converter.ConvertToString(document[descriptor.Name]).ToLower();
                var term = new Term(descriptor.FieldName, convertedValue);
                conditionForDelete.Add(new TermQuery(term), Occur.MUST);
                break;
            }

            try
            {
                _indexWriter.DeleteDocuments(conditionForDelete);
                _uncommitedCount++;
            }
            catch (OutOfMemoryException)
            {
                _indexWriter.Dispose();
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (!_isCommited && _uncommitedCount > 0)
                    {
                        _indexWriter.Rollback();
                        _uncommitedCount = 0;
                    }

                    _documentTransformer = null;
                    _metadataProvider = null;
                    _indexWriter = null;
                }

                _isDisposed = true;
            }
        }

        ~LuceneUnitOfWork()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}