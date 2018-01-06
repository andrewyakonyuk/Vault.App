using System;
using System.Collections.Generic;

namespace Vault.Shared.Search
{
    [Serializable]
    public class SearchDocument : DynamicDictionary
    {
        public SearchDocument()
        {
        }

        public SearchDocument(IDictionary<string, object> properties)
            : base(properties)
        {
        }

        public string Id
        {
            get
            {
                object value;
                if (TryGetValue("Id", out value))
                    return (string)value;

                return default(string);
            }
            set
            {
                this["Id"] = value;
            }
        }

        public string OwnerId
        {
            get
            {
                object value;
                if (TryGetValue("OwnerId", out value))
                    return (string)value;

                return default(string);
            }
            set
            {
                this["OwnerId"] = value;
            }
        }

        public DateTime Published
        {
            get
            {
                object value;
                if (TryGetValue("Published", out value))
                    return (DateTime)value;

                return default(DateTime);
            }
            set
            {
                this["Published"] = value;
            }
        }
    }
}