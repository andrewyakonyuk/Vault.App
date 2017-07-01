using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace Vault.Shared.Activity
{
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public class DynamicJsonObject : DynamicObject
    {
        readonly JObject _container;

        public DynamicJsonObject()
        {
            _container = new JObject();
        }

        public DynamicJsonObject(JObject other)
        {
            _container = new JObject(other);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            var comparision = binder.IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
            if (_container.TryGetValue(binder.Name, comparision, out JToken value))
                result = value.ToObject(binder.ReturnType);

            if (ReferenceEquals(result, null))
                result = DynamicNull.Null;

            if (result is JToken jToken && jToken.IsNullOrEmpty())
                result = DynamicNull.Null;

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _container[binder.Name] = value == null
                ? JValue.CreateNull()
                : JToken.FromObject(value);

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _container.Properties().Select(t => t.Name);
        }

        public override string ToString()
        {
            return _container.ToString();
        }
    }
}
