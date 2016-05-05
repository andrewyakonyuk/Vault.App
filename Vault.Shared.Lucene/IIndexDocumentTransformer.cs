using Lucene.Net.Documents;
using System.Collections.Generic;
using Vault.Shared.Domain;

namespace Vault.Shared.Lucene
{
    public interface IIndexDocumentTransformer
    {
        /// <summary>
        /// Transform an entity into lucene document
        /// </summary>
        /// <param name="entity">An antity</param>
        /// <returns>Lucene document</returns>
        Document Transform(IEntity entity);
    }
}