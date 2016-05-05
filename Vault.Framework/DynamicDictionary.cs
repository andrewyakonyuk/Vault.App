using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Vault.Framework
{
    public class DynamicDictionary : DynamicObject, IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _properties;

        public DynamicDictionary()
        {
            _properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public DynamicDictionary(IDictionary<string, object> properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            _properties = new Dictionary<string, object>(properties, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_properties.TryGetValue(binder.Name, out result))
                return true;

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public object this[string key]
        {
            get
            {
                return _properties[key];
            }

            set
            {
                _properties[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return _properties.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _properties.IsReadOnly;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _properties.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return _properties.Values;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _properties.Add(item);
        }

        public void Add(string key, object value)
        {
            _properties.Add(key, value);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _properties.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _properties.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _properties.Remove(item);
        }

        public bool Remove(string key)
        {
            return _properties.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _properties.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }
    }
}