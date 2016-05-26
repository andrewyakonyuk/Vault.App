using Lucene.Net.Documents;
using System;
using System.Collections.Generic;

namespace Vault.Shared.Search.Lucene
{
    public class LuceneDocumentValuesProvider : ISearchValuesProvider
    {
        private readonly Document _document;

        public LuceneDocumentValuesProvider(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _document = document;
        }

        public IReadOnlyList<string> GetValues(string key)
        {
            var values = _document.GetFieldables(key);
            var result = new List<string>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                var field = values[i];
                if (field.IsStored && !field.IsBinary)
                    result.Add(field.StringValue);
            }

            return result.AsReadOnly();
        }
    }
}