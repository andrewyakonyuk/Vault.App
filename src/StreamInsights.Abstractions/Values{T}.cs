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
    public sealed class Values<T> : Values, IValues, IEquatable<Values<T>>, IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        private readonly T[] _array;

        public static readonly Values<T> Empty = new Values<T>();

        private Values()
        {
            _array = Array.Empty<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Values{T}"/> class.
        /// </summary>
        /// <param name="item">The single item value.</param>
        public Values(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _array = new[] { item };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Values{T}"/> class.
        /// </summary>
        /// <param name="list">The list of values.</param>
        public Values(List<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _array = new T[list.Count];
            list.CopyTo(_array);
        }

        public Values(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            _array = new T[array.Length];
            array.CopyTo(_array, 0);
        }

        public Values(Values<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            _array = new T[other.Count];
            other._array.CopyTo(_array, 0);
        }

        public int Count => _array.Length;

        object IValues.this[int index] => _array[index];

        public T this[int index] => _array[index];

        public T First()
        {
            if (!Any())
                throw new InvalidOperationException();

            return _array[0];
        }

        public T FirstOfDefault()
        {
            if (Any())
                return _array[0];

            return default(T);
        }

        public bool Any()
        {
            return _array.Length > 0;
        }

        public bool None()
        {
            return _array.Length == 0;
        }

        public T[] ToArray()
        {
            var newArray = new T[Count];
            _array.CopyTo(newArray, 0);

            return newArray;
        }

        public List<T> ToList()
        {
            return new List<T>(_array);
        }

        public Values<T> ToValues()
        {
            if (IsNullOrEmpty(this))
                return Empty;

            return new Values<T>(this);
        }

        /// <summary>
        /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="Values{T}"/>.
        /// </summary>
        /// <param name="item">The single item value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Values<T>(T item) => Values.New(item);

        public static explicit operator T(Values<T> values)
        {
            if (IsNullOrEmpty(values))
                return default(T);

            return values._array[0];
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="List{T}"/> to <see cref="Values{T}"/>.
        /// </summary>
        /// <param name="list">The list of values.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Values<T>(List<T> list) => Values.New(list);

        public static explicit operator List<T>(Values<T> values)
        {
            if (IsNullOrEmpty(values))
                return new List<T>();

            return values.ToList();
        }

        public static implicit operator Values<T>(T[] array) => Values.New(array);

        public static explicit operator T[] (Values<T> values)
        {
            if (IsNullOrEmpty(values))
                return Array.Empty<T>();

            return values.ToArray();
        }

        public static bool operator ==(Values<T> left, Values<T> right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (ReferenceEquals(left, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Values<T> left, Values<T> right)
        {
            if (ReferenceEquals(left, right))
                return false;

            if (ReferenceEquals(left, null))
                return true;

            return !left.Equals(right);
        }

        public bool Equals(Values<T> other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Count == 0 && other.Count == 0)
                return true;

            if (Count != other.Count)
                return false;

            var hashSet = new HashSet<T>(other, EqualityComparer<T>.Default);

            for (int i = 0; i < _array.Length; i++)
            {
                var item = _array[i];
                if (!hashSet.Remove(item))
                    return false;

                if (hashSet.Count == 0)
                    break;
            }

            return hashSet.Count == 0;
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
            if (_array.Length == 0)
                return 0;

            return _array.GetHashCode() ^ 12 + 7;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_array.Length == 0)
                return EmptyEnumerator<T>.Default;

            return ((IEnumerable<T>)_array).GetEnumerator();
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
