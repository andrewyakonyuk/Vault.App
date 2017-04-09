using System.Collections.Generic;
using System.ComponentModel;

namespace Vault.Shared.Search.Lucene
{
    public class IndexMetadataBuilder
    {
        public List<DocumentFieldDescriptor> Descriptors { get; }

        public IndexMetadataBuilder()
        {
            Descriptors = new List<DocumentFieldDescriptor>();
        }

        public IndexMetadataBuilder Field(
                string name,
                string fieldName = null,
                bool isStored = true,
                bool isKeyword = false,
                bool isIndexed = true,
                bool isAnalysed = false,
                bool omitNorms = true,
                bool isKey = false,
                TypeConverter converter = null)
        {
            fieldName = fieldName ?? name.ToLowerInvariant();
            converter = converter ?? DocumentFieldDescriptor.DefaultConverter;
            var descriptor = new DocumentFieldDescriptor(name, fieldName)
            {
                IsKeyword = isKeyword,
                IsAnalysed = isAnalysed,
                IsIndexed = isIndexed,
                IsStored = isStored,
                OmitNorms = omitNorms,
                Converter = converter,
                IsKey = isKey
            };

            Descriptors.Add(descriptor);

            return this;
        }

        public IndexDocumentMetadata NewMetadata()
        {
            return new IndexDocumentMetadata(Descriptors);
        }
    }
}