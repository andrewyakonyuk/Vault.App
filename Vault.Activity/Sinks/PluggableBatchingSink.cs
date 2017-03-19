using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Activity.Utility;
using Vault.Shared;

namespace Vault.Activity.Sinks
{
    public sealed class PluggableBatchingSink<T> : PeriodicBatchingSink<T>
    {
        readonly IPeriodicBatchingAdapter<T> _adapter;

        public PluggableBatchingSink(IPeriodicBatchingAdapter<T> adapter, ILogger logger, IClock clock)
            : base(adapter.BatchSizeLimit, adapter.Period, adapter.QueueLimit, logger, clock)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            _adapter = adapter;
        }

        protected override async Task EmitBatchAsync(IEnumerable<T> messages)
        {
            await _adapter.EmitBatchAsync(messages);
        }

        protected override async Task OnEmptyBatchAsync()
        {
            await _adapter.OnEmptyBatchAsync();
        }

        protected override bool CanInclude(T message)
        {
            return _adapter.CanInclude(message);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _adapter.Dispose();
        }
    }

    public interface IPeriodicBatchingAdapter<in T> : IDisposable
    {
        int BatchSizeLimit { get; }
        TimeSpan Period { get; }
        int? QueueLimit { get; }

        Task EmitBatchAsync(IEnumerable<T> messages);
        Task OnEmptyBatchAsync();
        bool CanInclude(T message);
    }
}
