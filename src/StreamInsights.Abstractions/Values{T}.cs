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
    public class Values<T> : Values, IValues, IEnumerable<T>, IEquatable<Values<T>>, IReadOnlyCollection<T>
    {
        private readonly T[] _array;

        public static readonly Values<T> Empty = new Values<T>();

        private Values()
        {
            _array = Array.Empty<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Values{T}"/> struct.
        /// </summary>
        /// <param name="item">The single item value.</param>
        public Values(T item)
        {
            _array = new T[] { item };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Values{T}"/> struct.
        /// </summary>
        /// <param name="list">The list of values.</param>
        public Values(List<T> list)
        {
            _array = new T[list.Count];
            list.CopyTo(_array);
        }

        public Values(T[] array)
        {
            _array = new T[array.Length];
            array.CopyTo(_array, 0);
        }

        public int Count => _array.Length;

        int IValues.Count { get; }

        object IValues.this[int index] { get { return _array[index]; } }

        public T this[int index]
        {
            get { return _array[index]; }
        }

        public bool Any()
        {
            return _array.Length > 0;
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

            // values.FirstOrDefault();
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
            if (Values.IsNullOrEmpty(values))
                return new List<T>();

            return values.ToList();
        }

        public static implicit operator Values<T>(T[] array) => Values.New(array);

        public static explicit operator T[] (Values<T> values)
        {
            if (Values.IsNullOrEmpty(values))
                return Array.Empty<T>();

            return values.ToArray();
        }

        public static bool operator ==(Values<T> left, Values<T> right)
        {
            if (left == null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Values<T> left, Values<T> right)
        {
            if (left == null)
                return true;

            return !left.Equals(right);
        }

        public bool Equals(Values<T> other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (_array == other._array)
                return true;

            if (_array.Length == 0 && other._array.Length == 0)
                return true;

            var comparer = EqualityComparer<T>.Default;
            var hashSet = new HashSet<T>(other, comparer);

            foreach (var item in _array)
            {
                hashSet.Remove(item);
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
            return _array.GetHashCode() ^ 12 + 7;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }
    }
}
