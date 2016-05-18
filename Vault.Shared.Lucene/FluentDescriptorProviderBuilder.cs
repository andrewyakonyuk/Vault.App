using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Vault.Shared.Lucene
{
    public class FluentDescriptorProviderBuilder
    {
        readonly List<DocumentFieldDescriptor> _fieldDescriptors;

        public FluentDescriptorProviderBuilder()
        {
            _fieldDescriptors = new List<DocumentFieldDescriptor>();
        }

        public FluentDescriptorProviderBuilder Field(
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

            _fieldDescriptors.Add(descriptor);

            return this;
        }

        public IIndexDocumentMetadataProvider Build()
        {
            return new DefaultIndexDocumentDescriptorProvider(_fieldDescriptors);
        }

        private class DefaultIndexDocumentDescriptorProvider : IIndexDocumentMetadataProvider
        {
            readonly List<DocumentFieldDescriptor> _descriptors;

            public DefaultIndexDocumentDescriptorProvider(List<DocumentFieldDescriptor> descriptors)
            {
                _descriptors = descriptors;
            }

            public IndexDocumentMetadata GetMetadata()
            {
                return new IndexDocumentMetadata(_descriptors);
            }
        }
    }
}