using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vault.Activity.Utility;
using Vault.Shared;

namespace Vault.Activity.Sinks
{
    /// <summary>
    /// Base class for sinks that log events in batches. Batching is
    /// triggered asynchronously on a timer.
    /// </summary>
    /// <remarks>
    /// To avoid unbounded memory growth, events are discarded after attempting
    /// to send a batch, regardless of whether the batch succeeded or not. Implementers
    /// that want to change this behavior need to either implement from scratch, or
    /// embed retry logic in the batch emitting functions.
    /// </remarks>
    public abstract class PeriodicBatchingSink<T> : ISink<T>, IDisposable
    {
        readonly int _batchSizeLimit;
        readonly BoundedConcurrentQueue<T> _queue;
        readonly BatchedConnectionStatus _status;
        readonly Queue<T> _waitingBatch = new Queue<T>();
        readonly object _stateLock = new object();
        readonly ScheduledTimer _timer;
        readonly ILogger _logger;

        bool _unloading;
        bool _started;

        /// <summary>
        /// Construct a sink posting to the specified database.
        /// </summary>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        protected PeriodicBatchingSink(int batchSizeLimit, TimeSpan period, ILogger logger, IClock clock)
        {
            _batchSizeLimit = batchSizeLimit;
            _queue = new BoundedConcurrentQueue<T>();
            _status = new BatchedConnectionStatus(period);
            _logger = logger;

            _timer = new ScheduledTimer(() => OnTick(), logger, clock);
        }

        /// <summary>
        /// Construct a sink posting to the specified database.
        /// </summary>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="queueLimit">Maximum number of events in the queue.</param>
        protected PeriodicBatchingSink(
            int batchSizeLimit,
            TimeSpan period,
            int? queueLimit,
            ILogger logger,
            IClock clock)
            : this(batchSizeLimit, period, logger, clock)
        {
            if (queueLimit.HasValue)
                _queue = new BoundedConcurrentQueue<T>((int)queueLimit);
        }

        void CloseAndFlush()
        {
            lock (_stateLock)
            {
                if (!_started || _unloading)
                    return;

                _unloading = true;
            }

            _timer.Dispose();

            // This is the place where SynchronizationContext.Current is unknown and can be != null
            // so we prevent possible deadlocks here for sync-over-async downstream implementations 
            ResetSyncContextAndWait(OnTick);
        }

        void ResetSyncContextAndWait(Func<Task> taskFactory)
        {
            var prevContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            try
            {
                taskFactory().Wait();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevContext);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">If true, called because the object is being disposed; if false,
        /// the object is being disposed from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            CloseAndFlush();
        }

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="messages">The events to emit.</param>
        protected virtual Task EmitBatchAsync(IEnumerable<T> messages)
        {
            return Task.FromResult(true);
        }

        async Task<DateTime?> OnTick()
        {
            try
            {
                bool batchWasFull;
                do
                {
                    T next;
                    while (_waitingBatch.Count < _batchSizeLimit &&
                        _queue.TryDequeue(out next))
                    {
                        if (CanInclude(next))
                            _waitingBatch.Enqueue(next);
                    }

                    if (_waitingBatch.Count == 0)
                    {
                        await OnEmptyBatchAsync();
                        return null;
                    }

                    await EmitBatchAsync(_waitingBatch);

                    batchWasFull = _waitingBatch.Count >= _batchSizeLimit;
                    _waitingBatch.Clear();
                    _status.MarkSuccess();
                }
                while (batchWasFull); // Otherwise, allow the period to elapse
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, "Exception while emitting periodic batch from");
                _status.MarkFailure();
            }
            finally
            {
                if (_status.ShouldDropBatch)
                    _waitingBatch.Clear();

                if (_status.ShouldDropQueue)
                {
                    T evt;
                    while (_queue.TryDequeue(out evt)) { }
                }

                lock (_stateLock)
                {
                    if (!_unloading)
                        SetTimer(_status.NextInterval);
                }
            }

            return null;
        }

        protected void SetTimer(TimeSpan interval)
        {
            _timer.ScheduleNext(interval);
        }

        /// <summary>
        /// Emit the provided log event to the sink. If the sink is being disposed or
        /// the app domain unloaded, then the event is ignored.
        /// </summary>
        /// <param name="message">Log event to emit.</param>
        /// <exception cref="ArgumentNullException">The event is null.</exception>
        /// <remarks>
        /// The sink implements the contract that any events whose Emit() method has
        /// completed at the time of sink disposal will be flushed (or attempted to,
        /// depending on app domain state).
        /// </remarks>
        public void Emit(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (_unloading)
                return;

            if (!_started)
            {
                lock (_stateLock)
                {
                    if (_unloading) return;
                    if (!_started)
                    {
                        // Special handling to try to get the first event across as quickly
                        // as possible to show we're alive!
                        _queue.TryEnqueue(message);
                        _started = true;
                        SetTimer(TimeSpan.Zero);
                        return;
                    }
                }
            }

            _queue.TryEnqueue(message);
        }

        /// <summary>
        /// Determine whether a queued log event should be included in the batch. If
        /// an override returns false, the event will be dropped.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool CanInclude(T message)
        {
            return true;
        }

        /// <summary>
        /// Allows derived sinks to perform periodic work without requiring additional threads
        /// or timers (thus avoiding additional flush/shut-down complexity).
        /// </summary>
        protected virtual Task OnEmptyBatchAsync()
        {
            return Task.FromResult(true);
        }
    }
}
