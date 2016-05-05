using System;
using Vault.Shared.Domain;

namespace Vault.Shared.Lucene
{
    public static class IndexDocumentMetadataProviderExtensions
    {
        public static IndexDocumentMetadata GetMetadataForType(this IIndexDocumentMetadataProvider metadataProvider, IEntity entity)
        {
            var dictionary = ObjectDictionary.Create(entity);
            var documentType = dictionary["documentType"] as string;

            if (string.IsNullOrEmpty(documentType))
                throw new InvalidOperationException(string.Format("Unable to define the document type for '{0}' with type '{1}'", entity.Id, entity.GetType().FullName));

            return metadataProvider.GetMetadataForType(documentType);
        }
    }
}