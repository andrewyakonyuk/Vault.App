using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using Vault.Shared.Domain;

namespace Vault.Shared.Search.Lucene
{
    public class DefaultIndexDocumentTransformer : IIndexDocumentTransformer
    {
        public Document Transform(SearchDocument document, IndexDocumentMetadata metadata)
        {
            var luceneDocument = new Document();

            var separator = new[] { Environment.NewLine };

            for (int i = 0; i < metadata.Fields.Count; i++)
            {
                var fieldDescriptor = metadata.Fields[i];

                object value;
                if (document.TryGetValue(fieldDescriptor.Name, out value))
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
                        luceneDocument.Add(field);

                        if (fieldDescriptor.IsKeyword)
                        {
                            var keywordsField = new Field(IndexDocumentMetadata.KeywordsFieldName, rawValue, Field.Store.YES, Field.Index.ANALYZED);
                            luceneDocument.Add(keywordsField);
                        }
                    }
                }
            }

            return luceneDocument;
        }
    }
}