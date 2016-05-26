using System;
using System.Collections.Generic;

namespace Vault.Shared.Search
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
            var metadata = _documentMetadataProvider.GetMetadata();
            var searchDocument = new SearchDocument();

            foreach (var fieldDescriptor in metadata.Fields)
            {
                var rawValues = valueProvider.GetValues(fieldDescriptor.FieldName);

                //0: check if field have a multiple values. if so, convert raw values to list
                if (rawValues.Count > 1)
                {
                    var convertedValues = new List<object>(rawValues.Count);
                    foreach (var value in rawValues)
                    {
                        convertedValues.Add(fieldDescriptor.Converter.ConvertFromString(value));
                    }
                    searchDocument[fieldDescriptor.Name] = convertedValues;
                }
                //1: check the field have at least one value
                else if (rawValues.Count > 0)
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