using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Search.Lucene
{
    /// <summary>
    /// Provide a factory to create a new instance of <see cref="LuceneIndexWriter"/>
    /// </summary>
    public interface IIndexWriterInitializer
    {
        LuceneIndexWriter Create();
    }
}