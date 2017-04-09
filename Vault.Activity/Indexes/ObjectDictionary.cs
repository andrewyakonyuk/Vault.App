using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Vault.Activity.Indexes
{
    internal class ObjectDictionary : Dictionary<string, object>
    {
        public ObjectDictionary(object entity)
            : this(Create(entity))
        {
        }

        private ObjectDictionary(IDictionary<string, object> dictionary)
            : base(dictionary, StringComparer.OrdinalIgnoreCase)
        {
        }

        static IDictionary<string, object> WrapToDictionary(object entity)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in entity.GetType()
                .GetProperties().Where(t => t.CanRead))
            {
                result[property.Name] = property.GetValue(entity);
            }

            return result;
        }
        

        public static IDictionary<string, object> Create(object entity)
        {
            var dictionary = entity as IDictionary<string, object>;
            if (dictionary != null)
                return dictionary;

            return WrapToDictionary(entity);
        }
    }
}
