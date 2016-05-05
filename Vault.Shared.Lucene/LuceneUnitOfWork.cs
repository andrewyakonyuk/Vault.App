using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Shared.Domain;

namespace Vault.Shared.Lucene
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
                    _indexWriter.Optimize();
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
            }
        }

        public void Delete(IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var metadata = _metadataProvider.GetMetadataForType(entity);
            var document = ObjectDictionary.Create(entity);
            var termsToDelete = metadata.Keys
                .Select(t => new Term(t.FieldName, t.Converter.ConvertToString(document[t.Name])))
                .ToArray();

            try
            {
                _indexWriter.DeleteDocuments(termsToDelete);
                _uncommitedCount++;
            }
            catch (OutOfMemoryException)
            {
                _indexWriter.Dispose();
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