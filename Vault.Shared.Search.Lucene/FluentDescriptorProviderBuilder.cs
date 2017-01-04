using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Vault.Shared.Search.Lucene
{
    public class FluentDescriptorProviderBuilder
    {
        readonly Dictionary<string, IndexMetadataBuilder> _indexBuilders;

        public FluentDescriptorProviderBuilder()
        {
            _indexBuilders = new Dictionary<string, IndexMetadataBuilder>(StringComparer.InvariantCultureIgnoreCase);
        }

        public IndexMetadataBuilder Index(string indexName)
        {
            IndexMetadataBuilder indexBuilder;
            if (!_indexBuilders.TryGetValue(indexName, out indexBuilder))
            {
                indexBuilder = new IndexMetadataBuilder(this);
                _indexBuilders[indexName] = indexBuilder;
            }

            return indexBuilder;
        }

        public IIndexDocumentMetadataProvider BuildProvider()
        {
            return new DefaultIndexDocumentDescriptorProvider(_indexBuilders);
        }

        public class IndexMetadataBuilder
        {
            public List<DocumentFieldDescriptor> Descriptors { get; }
            readonly FluentDescriptorProviderBuilder _parentBuilder;

            public IndexMetadataBuilder(FluentDescriptorProviderBuilder parentBuilder)
            {
                Descriptors = new List<DocumentFieldDescriptor>();
                _parentBuilder = parentBuilder;
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

            public FluentDescriptorProviderBuilder BuildIndex()
            {
                return _parentBuilder;
            }
        }

        private class DefaultIndexDocumentDescriptorProvider : IIndexDocumentMetadataProvider
        {
            readonly Dictionary<string, IndexMetadataBuilder> _indexBuilders;

            public DefaultIndexDocumentDescriptorProvider(Dictionary<string, IndexMetadataBuilder> indexBuilders)
            {
                _indexBuilders = indexBuilders;
            }

            public IndexDocumentMetadata GetMetadata(string indexName)
            {
                IndexMetadataBuilder indexBuilder;
                if (!_indexBuilders.TryGetValue(indexName, out indexBuilder))
                    return new IndexDocumentMetadata(new List<DocumentFieldDescriptor>());

                return new IndexDocumentMetadata(indexBuilder.Descriptors);
            }
        }
    }
}