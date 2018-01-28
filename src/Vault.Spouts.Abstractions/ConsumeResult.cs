using System;
using System.Collections.Generic;

namespace Vault.Spouts.Abstractions
{
    public class ConsumeResult<T>
    {
        readonly List<T> _items;

        public ConsumeResult(IEnumerable<T> items, int iteration)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = new List<T>(items);
            Iteration = iteration;
        }

        public ConsumeResult()
        {
            _items = new List<T>();
        }

        public readonly static ConsumeResult<T> Empty = new ConsumeResult<T> { IsCancellationRequested = true };

        public bool IsCancellationRequested { get; set; }

        public int Iteration { get; }

        public int Count { get { return _items.Count; } }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
