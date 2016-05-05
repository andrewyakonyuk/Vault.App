namespace Vault.Shared.Lucene
{
    public interface IIndexDocumentMetadataProvider
    {
        /// <summary>
        /// Provide the metadata for the specific document type.
        /// </summary>
        /// <param name="documentType">Document type</param>
        /// <returns></returns>
        IndexDocumentMetadata GetMetadataForType(string documentType);
    }
}