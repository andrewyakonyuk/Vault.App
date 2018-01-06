using Lucene.Net.Index;
using System;

namespace Vault.Shared.Search.Lucene
{
    public class IndexWriterHolder : IIndexWriterHolder
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

        public IndexWriter Value
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
