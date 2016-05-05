using Lucene.Net.Index;
using System;

namespace Vault.Shared.Lucene
{
    /// <summary>
    /// The default implementation of <see cref="IIndexWriterAccessor"/> that has the responsibility to provide <see cref="IndexWriter"/> up to date
    /// and ready for use
    /// </summary>
    public class DefaultIndexWriterAccessor : IIndexWriterAccessor
    {
        Lazy<LuceneIndexWriter> _indexWriter;
        readonly IIndexWriterInitializer _initializer;

        public DefaultIndexWriterAccessor(IIndexWriterInitializer initializer)
        {
            if (initializer == null)
                throw new ArgumentNullException(nameof(initializer));

            _initializer = initializer;
            _indexWriter = new Lazy<LuceneIndexWriter>(_initializer.Create, true);
        }

        public IndexWriter Writer
        {
            get
            {
                //ensure the index writer still valid
                if (!_indexWriter.Value.IsOpen())
                {
                    lock (_indexWriter)
                    {
                        if (!_indexWriter.Value.IsOpen())
                        {
                            _indexWriter = new Lazy<LuceneIndexWriter>(_initializer.Create, true);
                        }
                    }
                }
                return _indexWriter.Value;
            }
        }
    }
}