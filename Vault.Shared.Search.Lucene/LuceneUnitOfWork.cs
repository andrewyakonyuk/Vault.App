using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Vault.Shared.Domain;

namespace Vault.Shared.Search.Lucene
{
    public class LuceneUnitOfWork : IIndexUnitOfWork
    {
        IndexWriter _indexWriter;
        IIndexDocumentTransformer _documentTransformer;
        IndexDocumentMetadata _metadata;

        bool _isCommited = false;
        bool _isDisposed = false;
        int _uncommitedCount = 0;

        public LuceneUnitOfWork(
            IndexWriter indexWriter,
            IIndexDocumentTransformer documentTransformer,
            IndexDocumentMetadata metadata)
        {
            if (indexWriter == null)
                throw new ArgumentNullException(nameof(indexWriter));
            if (documentTransformer == null)
                throw new ArgumentNullException(nameof(documentTransformer));
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            _indexWriter = indexWriter;
            _documentTransformer = documentTransformer;
            _metadata = metadata;
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

        public void Save(SearchDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Delete(document);
            var indexDocument = _documentTransformer.Transform(document, _metadata);
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

        public void Delete(SearchDocument document)
        {
            if (!_metadata.Keys.Any())
                throw new InvalidOperationException("Any document keys are not specified");

            var conditionForDelete = new BooleanQuery();
            foreach (var descriptor in _metadata.Keys)
            {
                var convertedValue = descriptor.Converter.ConvertToString(document[descriptor.Name]);
                var term = new Term(descriptor.FieldName, convertedValue);
                conditionForDelete.Add(new TermQuery(term), Occur.MUST);
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
                    _metadata = null;
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