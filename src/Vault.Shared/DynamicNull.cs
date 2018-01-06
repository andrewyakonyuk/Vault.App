using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Shared
{
    public class DynamicNull : DynamicObject, IEnumerable
    {
        public static readonly DynamicNull Null = new DynamicNull();

        private DynamicNull() { }

        public IEnumerator GetEnumerator()
        {
            return (new List<DynamicNull>()).GetEnumerator();
        }

        public int Count => 0;

        public bool Any() => false;

        public override string ToString()
        {
            return string.Empty;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this;
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = this;
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = this;
            return true;
        }

        public bool IsNull()
        {
            return true;
        }

        public bool HasValue()
        {
            return false;
        }

        public T ToObject<T>()
            where T : class, new()
        {
            return new T();
        }

        public T Value<T>()
            where T : class, new()
        {
            return new T();
        }

        public static implicit operator bool(DynamicNull n)
        {
            return false;
        }

        public static implicit operator DateTimeOffset(DynamicNull n)
        {
            return DateTimeOffset.MinValue;
        }

        public static implicit operator DateTime(DynamicNull n)
        {
            return DateTime.MinValue;
        }

        public static implicit operator int(DynamicNull n)
        {
            return 0;
        }

        public static implicit operator long(DynamicNull n)
        {
            return 0L;
        }

        public static implicit operator decimal(DynamicNull n)
        {
            return 0M;
        }

        public static implicit operator short(DynamicNull n)
        {
            return 0;
        }

        public static implicit operator string(DynamicNull n)
        {
            return string.Empty;
        }

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
