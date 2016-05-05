using System;
using System.Collections.Generic;
using System.Linq;
using Vault.Shared.Lucene;

namespace Vault.Framework.Search
{
    public class DefaultSearchResultTransformer : ISearchResultTransformer
    {
        private readonly IIndexDocumentMetadataProvider _documentMetadataProvider;

        public DefaultSearchResultTransformer(IIndexDocumentMetadataProvider provider)
        {
            _documentMetadataProvider = provider;
        }

        public SearchDocument Transform(ISearchValuesProvider valueProvider)
        {
            var documentType = valueProvider.GetValues("_documentType").FirstOrDefault();
            if (string.IsNullOrEmpty(documentType))
                throw new InvalidOperationException(string.Format("There are no any types specified for document '{0}'", valueProvider.GetValues("_id").FirstOrDefault()));

            var metadata = _documentMetadataProvider.GetMetadataForType(documentType);
            var searchDocument = new SearchDocument();

            foreach (var fieldDescriptor in metadata.Fields)
            {
                var rawValues = valueProvider.GetValues(fieldDescriptor.FieldName).ToArray();

                //0: check if field have a multiple values. if so, convert raw values to list
                if (rawValues.Length > 1)
                {
                    var convertedValues = new List<object>(rawValues.Length);
                    foreach (var value in rawValues)
                    {
                        convertedValues.Add(fieldDescriptor.Converter.ConvertFromString(value));
                    }
                    searchDocument[fieldDescriptor.Name] = convertedValues;
                }
                //1: check the field have at least one value
                else if (rawValues.Length > 0)
                {
                    var convertedValue = fieldDescriptor.Converter.ConvertFromString(rawValues[0]);
                    switch (fieldDescriptor.Name.ToLowerInvariant())
                    {
                        case "id":
                            searchDocument.Id = (int)convertedValue;
                            break;

                        case "ownerid":
                            searchDocument.OwnerId = (int)convertedValue;
                            break;

                        case "published":
                            searchDocument.Published = (DateTime)convertedValue;
                            break;

                        default:
                            searchDocument[fieldDescriptor.Name] = convertedValue;
                            break;
                    }
                }
                //2: nothing to do in case no values for field
            }

            return searchDocument;
        }
    }
}