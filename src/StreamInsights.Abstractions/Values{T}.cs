using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamInsights.Abstractions
{
    /// <summary>
    /// A single or list of values.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <seealso cref="T:StreamInsights.IValue" />
    public struct Values<T> : IValue, IEquatable<Values<T>>
    {
        private readonly T _item;
        private readonly List<T> _list;
        private readonly bool _hasItem;

        public static readonly Values<T> Empty = new Values<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Values{T}"/> struct.
        /// </summary>
        /// <param name="item">The single item value.</param>
        public Values(T item)
        {
            _item = item;
            _list = null;
            _hasItem = !EqualityComparer<T>.Default.Equals(_item, default(T));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Values{T}"/> struct.
        /// </summary>
        /// <param name="list">The list of values.</param>
        public Values(List<T> list)
        {
            _item = default(T);
            _list = list;
            _hasItem = !EqualityComparer<T>.Default.Equals(_item, default(T));
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a single or list of values.
        /// </summary>
        /// <value><c>true</c> if this instance has at least one value; otherwise, <c>false</c>.</value>
        public bool HasValue => (_list != null && _list.Count > 0) || _hasItem;

        /// <summary>
        /// Gets the single item value.
        /// </summary>
        /// <value>The single item value.</value>
        public T Item => _item;

        /// <summary>
        /// Gets the list of values.
        /// </summary>
        /// <value>The list of values.</value>
        public List<T> List => _list;

        /// <summary>
        /// Gets the non-null object representing the instance.
        /// </summary>
        public object Value
        {
            get
            {
                if (_list != null)
                {
                    return _list;
                }
                else if (_hasItem)
                {
                    return _item;
                }

                return null;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="Values{T}"/>.
        /// </summary>
        /// <param name="item">The single item value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Values<T>(T item)
        {
            if (EqualityComparer<T>.Default.Equals(item, default(T)))
                return Empty;

            return new Values<T>(item);
        }

        public static explicit operator T(Values<T> values) => values._item;

        /// <summary>
        /// Performs an implicit conversion from <see cref="List{T}"/> to <see cref="Values{T}"/>.
        /// </summary>
        /// <param name="list">The list of values.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Values<T>(List<T> list)
        {
            if (list == null)
                return Empty;

            return new Values<T>(list);
        }

        public static implicit operator Values<T>(T[] array) => new Values<T>(new List<T>(array));

        public static explicit operator List<T>(Values<T> values)
        {
            if (values._hasItem)
                return new List<T>(1) { values._item };
            return new List<T>(values._list);
        }

        public override string ToString()
        {
            if (HasValue)
                return Value.ToString();

            return "<empty>";
        }

        public bool Any()
        {
            return HasValue;
        }

        public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            // fast check
            if (!HasValue)
                return Enumerable.Empty<TResult>();

            if (_hasItem)
                return GetSingleEnumerable(selector(_item));

            return _list.Select(selector);
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            // fast check
            if (!HasValue)
                return Enumerable.Empty<T>();

            if (_hasItem && predicate(_item))
                return GetSingleEnumerable(_item);

            if (_list != null && _list.Count > 0)
                return _list.Where(predicate);

            return Enumerable.Empty<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            // fast check
            if (!HasValue)
                return EmptyEnumerator<T>.Default;

            if (_hasItem)
                return GetSingleEnumerator(_item);

            return _list.GetEnumerator();
        }

        public static bool operator ==(Values<T> left, Values<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Values<T> left, Values<T> right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Values<T> other)
        {
            if (other.HasValue && HasValue)
            {
                return other.Value.Equals(Value);
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is Values<T> values)
            {
                return Equals(values);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (!HasValue)
                return 0;

            return Value.GetHashCode() ^ 12 + 7;
        }

        static IEnumerator<TResult> GetSingleEnumerator<TResult>(TResult item)
        {
            yield return item;
        }

        static IEnumerable<TResult> GetSingleEnumerable<TResult>(TResult item)
        {
            yield return item;
        }

        class EmptyEnumerator<TValue> : IEnumerator<TValue>
        {
            public static readonly IEnumerator<TValue> Default = new EmptyEnumerator<TValue>();

            private EmptyEnumerator() { }

            public TValue Current => default(TValue);

            object IEnumerator.Current => null;

            public void Dispose()
            {
               
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
                
            }
        }
    }
}
