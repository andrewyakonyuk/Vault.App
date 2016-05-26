namespace Vault.Shared.Search
{
    public interface IIndexDocumentMetadataProvider
    {
        /// <summary>
        /// Provide the metadata for the all search fields
        /// </summary>
        /// <returns></returns>
        IndexDocumentMetadata GetMetadata();
    }
}