using Lucene.Net.Index;
using System;
using System.Collections.Concurrent;

namespace Vault.Shared.Search.Lucene
{
    /// <summary>
    /// The default implementation of <see cref="IIndexWriterAccessor"/> that has the responsibility to provide <see cref="IndexWriter"/> up to date
    /// and ready for use
    /// </summary>
    public class DefaultIndexWriterAccessor : IIndexWriterAccessor
    {
        readonly IIndexWriterInitializer _initializer;
        readonly ConcurrentDictionary<string, IndexWriterHolder> _indexWriters;

        public DefaultIndexWriterAccessor(IIndexWriterInitializer initializer)
        {
            if (initializer == null)
                throw new ArgumentNullException(nameof(initializer));

            _initializer = initializer;
            _indexWriters = new ConcurrentDictionary<string, IndexWriterHolder>(StringComparer.InvariantCultureIgnoreCase);
        }

        public IndexWriter GetWriter(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                throw new ArgumentException("Index name must not be null or empty", nameof(indexName));

            var holder = _indexWriters.GetOrAdd(indexName,
                (key) => new IndexWriterHolder(key, _initializer));

            return holder.Writer;
        }

        private class IndexWriterHolder
        {
            Lazy<LuceneIndexWriter> _indexWriter;
            readonly IIndexWriterInitializer _initializer;
            static object _locker = new object();
            readonly string _indexName;

            public IndexWriterHolder(string indexName, IIndexWriterInitializer initializer)
            {
                if (initializer == null)
                    throw new ArgumentNullException(nameof(initializer));

                _initializer = initializer;
                _indexName = indexName;
                _indexWriter = new Lazy<LuceneIndexWriter>(() => _initializer.Create(indexName), true);
            }

            public IndexWriter Writer
            {
                get
                {
                    if (!_indexWriter.Value.IsOpen())
                    {
                        lock (_locker)
                        {
                            if (!_indexWriter.Value.IsOpen())
                            {
                                _indexWriter = new Lazy<LuceneIndexWriter>(() => _initializer.Create(_indexName), true);
                            }
                        }
                    }
                    return _indexWriter.Value;
                }
            }
        }
    }
}