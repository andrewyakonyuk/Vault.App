using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Vault.Shared.Lucene
{
    public class FluentDescriptorProviderBuilder
    {
        List<IndexDocumentDescriptorBuilder> _documentDescriptors;

        public FluentDescriptorProviderBuilder()
        {
            _documentDescriptors = new List<IndexDocumentDescriptorBuilder>();
        }

        public IIndexDocumentMetadataProvider Build()
        {
            var hash = _documentDescriptors.ToDictionary(t => t.Name, t => t.List());
            return new DefaultIndexDocumentDescriptorProvider(hash);
        }

        public FluentDescriptorProviderBuilder Document(string documentType, Action<IndexDocumentDescriptorBuilder> builder)
        {
            var documentBuilder = new IndexDocumentDescriptorBuilder(documentType);
            builder(documentBuilder);
            _documentDescriptors.Add(documentBuilder);
            return this;
        }

        public class IndexDocumentDescriptorBuilder
        {
            readonly DocumentFieldDescriptor[] _defaultDescriptors = new[] {
                new DocumentFieldDescriptor("Id", "_id") {
                    IsAnalysed = false,
                    IsIndexed = true,
                    IsStored = true,
                    Converter = new Int32Converter(),
                    IsKey = true
                },
                new DocumentFieldDescriptor("OwnerId", "_ownerId") {
                    IsAnalysed = false,
                    IsIndexed = true,
                    IsStored = true,
                    Converter = new Int32Converter()
                },
                new DocumentFieldDescriptor("Published", "_published") {
                    IsAnalysed = false,
                    IsIndexed = true,
                    IsStored = true,
                    IsSorted = true,
                    Converter = new Converters.LuceneDateTimeConverter()
                },
                new DocumentFieldDescriptor("DocumentType", "_documentType") {
                    IsStored = true,
                    IsIndexed = true,
                    IsAnalysed = true,
                    OmitNorms = true,
                    IsKey = true
                }
            };

            readonly List<DocumentFieldDescriptor> _fieldDescriptors;

            public IndexDocumentDescriptorBuilder(string name)
            {
                _fieldDescriptors = new List<DocumentFieldDescriptor>();

                Name = name;
            }

            internal string Name { get; private set; }

            public IndexDocumentDescriptorBuilder Field(
                string name,
                string fieldName = null,
                bool isStored = true,
                bool isSorted = false,
                bool isKeyword = false,
                bool isIndexed = true,
                bool isAnalysed = true,
                bool omitNorms = true,
                bool isKey = false,
                TypeConverter converter = null)
            {
                fieldName = fieldName ?? name.ToLowerInvariant();
                converter = converter ?? DocumentFieldDescriptor.DefaultConverter;
                var descriptor = new DocumentFieldDescriptor(name, fieldName)
                {
                    IsSorted = isSorted,
                    IsKeyword = isKeyword,
                    IsAnalysed = isAnalysed,
                    IsIndexed = isIndexed,
                    IsStored = isStored,
                    OmitNorms = omitNorms,
                    Converter = converter
                };

                _fieldDescriptors.Add(descriptor);

                return this;
            }

            internal List<DocumentFieldDescriptor> List()
            {
                return _fieldDescriptors
                    .Concat(_defaultDescriptors)
                    .Distinct()
                    .ToList();
            }
        }

        private class DefaultIndexDocumentDescriptorProvider : IIndexDocumentMetadataProvider
        {
            readonly IDictionary<string, List<DocumentFieldDescriptor>> _descriptorsHash;

            public DefaultIndexDocumentDescriptorProvider(IDictionary<string, List<DocumentFieldDescriptor>> descriptorsHash)
            {
                _descriptorsHash = descriptorsHash;
            }

            public IndexDocumentMetadata GetMetadataForType(string documentType)
            {
                if (_descriptorsHash.ContainsKey(documentType))
                    return new IndexDocumentMetadata
                    {
                        DocumentType = documentType,
                        Fields = _descriptorsHash[documentType]
                    };

                return new IndexDocumentMetadata
                {
                    DocumentType = documentType,
                    Fields = new List<DocumentFieldDescriptor>()
                };
            }
        }
    }
}