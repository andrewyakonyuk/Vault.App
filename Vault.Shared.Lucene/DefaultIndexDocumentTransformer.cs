using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Vault.Shared.Domain;

namespace Vault.Shared.Lucene
{
    public class DefaultIndexDocumentTransformer : IIndexDocumentTransformer
    {
        readonly IIndexDocumentMetadataProvider _documentDescriptorProvider;

        const string KeywordsFieldName = "keywords";

        public DefaultIndexDocumentTransformer(IIndexDocumentMetadataProvider documentDescriptorProvider)
        {
            if (documentDescriptorProvider == null)
                throw new ArgumentNullException(nameof(documentDescriptorProvider));

            _documentDescriptorProvider = documentDescriptorProvider;
        }

        public Document Transform(IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var dictionary = ObjectDictionary.Create(entity);
            var metadata = _documentDescriptorProvider.GetMetadataForType(entity);
            return Transform(dictionary, metadata);
        }

        Document Transform(IDictionary<string, object> entity, IndexDocumentMetadata metadata)
        {
            var document = new Document();

            var separator = new[] { Environment.NewLine };

            for (int i = 0; i < metadata.Fields.Count; i++)
            {
                var fieldDescriptor = metadata.Fields[i];

                object value;
                if (entity.TryGetValue(fieldDescriptor.Name, out value))
                {
                    var store = fieldDescriptor.IsStored ? Field.Store.YES : Field.Store.NO;
                    var index = FieldExtensions.ToIndex(fieldDescriptor.IsIndexed, fieldDescriptor.IsAnalysed, fieldDescriptor.OmitNorms);
                    //todo: return list of string instead simple string
                    var values = fieldDescriptor.Converter.ConvertToString(value)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < values.Length; j++)
                    {
                        var rawValue = values[j];
                        var field = new Field(fieldDescriptor.FieldName, rawValue, store, index);
                        document.Add(field);
                    }

                    if (fieldDescriptor.IsKeyword)
                    {
                        for (int j = 0; j < values.Length; j++)
                        {
                            var item = values[j];
                            var field = new Field(KeywordsFieldName, item, Field.Store.YES, Field.Index.ANALYZED);
                            document.Add(field);
                        }
                    }

                    if (fieldDescriptor.IsSorted)
                    {
                        //todo: store field as sorted
                    }
                }
            }

            return document;
        }
    }
}