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

        public IIndexWriterHolder GetWriter(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                throw new ArgumentException("Index name must not be null or empty", nameof(indexName));

            return _indexWriters.GetOrAdd(indexName,
                 (key) => new IndexWriterHolder(key, _initializer));
        }
    }
}