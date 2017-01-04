namespace Vault.Shared.Search
{
    public interface IIndexDocumentMetadataProvider
    {
        /// <summary>
        /// Provide the metadata for the specific index
        /// </summary>
        /// <returns></returns>
        IndexDocumentMetadata GetMetadata(string indexName);
    }
}