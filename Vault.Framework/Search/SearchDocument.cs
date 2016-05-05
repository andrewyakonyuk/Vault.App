using System;
using System.Collections.Generic;
using Vault.Shared.Domain;

namespace Vault.Framework.Search
{
    public class SearchDocument : DynamicDictionary, IEntity, IContent
    {
        public SearchDocument()
        {
        }

        public SearchDocument(IDictionary<string, object> properties)
            : base(properties)
        {
        }

        public int Id
        {
            get
            {
                object value;
                if (TryGetValue("Id", out value))
                    return (int)value;

                return default(int);
            }
            set
            {
                this["Id"] = value;
            }
        }

        public int OwnerId
        {
            get
            {
                object value;
                if (TryGetValue("OwnerId", out value))
                    return (int)value;

                return default(int);
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