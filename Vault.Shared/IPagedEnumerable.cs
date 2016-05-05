using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vault.Shared
{
    public interface IPagedEnumerable<T> : IEnumerable<T>
    {
        int TotalCount { get; }
    }

    [Serializable]
    [DataContract]
    public sealed class PagedEnumerable<T> : PagedEnumerable, IPagedEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;

        public static PagedEnumerable<T> Empty = new PagedEnumerable<T>(new T[0], 0);

        public PagedEnumerable(IEnumerable<T> enumerable, int totalCount)
        {
            _inner = enumerable;
            TotalCount = totalCount;
        }

        [DataMember]
        public int TotalCount { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }
    }

    public abstract class PagedEnumerable
    {
        public static IPagedEnumerable<T> Create<T>(IEnumerable<T> enumerable, int totalCount)
        {
            return new PagedEnumerable<T>(enumerable, totalCount);
        }   
    }
}
