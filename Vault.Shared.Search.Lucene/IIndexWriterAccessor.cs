using Lucene.Net.Index;
using System;

namespace Vault.Shared.Search.Lucene
{
    /// <summary>
    /// Use for share the single IndexWriter per index across threads to fit the near realtime search
    /// </summary>
    public interface IIndexWriterAccessor
    {
        IndexWriter GetWriter(string indexName);
    }
}