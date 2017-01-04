using Lucene.Net.Documents;
using System.Collections.Generic;
using Vault.Shared.Domain;

namespace Vault.Shared.Search.Lucene
{
    public interface IIndexDocumentTransformer
    {
        /// <summary>
        /// Transform an entity into lucene document
        /// </summary>
        /// <param name="entity">A search document</param>
        /// <returns>Lucene document</returns>
        Document Transform(SearchDocument entity, IndexDocumentMetadata metadata);
    }
}