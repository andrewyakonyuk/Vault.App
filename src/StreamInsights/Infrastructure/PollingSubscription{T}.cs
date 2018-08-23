using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Linq;
using StreamInsights.Abstractions;

namespace StreamInsights.Infrastructure
{
    public delegate Task<IReadOnlyCollection<T>> PollingDataAccessor< T>(long checkpoint, int maxCount, CancellationToken token)
        where T : ICommittable;

    public delegate Task PollingResultCallback<in T>(T item, CancellationToken cancellationToken);

    public sealed class PollingSubscription<T> : IDisposable
        where T: ICommittable
    {
        readonly CancellationTokenSource _pollingCancellationSource;
        readonly Task _pollingTask;

        public PollingSubscription(
            PollingDataAccessor<T> accessor, 
            PollingResultCallback<T> callback, 
            long initialCheckpoint = 0L,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            PollingDataAccessor<T> cachedAccessor = accessor;
            PollingResultCallback<T> cachedCallback = callback;
            var checkpoint = initialCheckpoint;
            _pollingCancellationSource = new CancellationTokenSource();
            var pollingCancellationToken = _pollingCancellationSource.Token;
            var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(pollingCancellationToken, cancellationToken);

            var buffer = new BufferBlock<T>(new DataflowBlockOptions
            {
                BoundedCapacity = 100,
                CancellationToken = linkedCancellation.Token
            });
            var processor = new ActionBlock<T>(item => cachedCallback(item, linkedCancellation.Token), new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 100,
                CancellationToken = linkedCancellation.Token
            });
            buffer.LinkTo(processor, new DataflowLinkOptions { PropagateCompletion = true });

            _pollingTask = new Task(async () =>
             {
                 TimeSpan interval = TimeSpan.Zero;
                 TimeSpan waitAfterSuccessInterval = TimeSpan.Zero;
                 TimeSpan waitAfterErrorInterval = TimeSpan.FromSeconds(10);
                 var waitAfterEmptyResultIntervals = new Throttler(new [] { 1000d, 5000d, 10000d });
                 int requestedCount = 100;

                 while (!pollingCancellationToken.IsCancellationRequested)
                 {
                     if (interval > TimeSpan.Zero)
                         await Task.Delay(interval, pollingCancellationToken);

                     try
                     {
                         var result = await cachedAccessor.Invoke(checkpoint, requestedCount, pollingCancellationToken);
                         if (result.Count == 0)
                             interval = waitAfterEmptyResultIntervals.NextInterval();
                         else
                         {
                             waitAfterEmptyResultIntervals.Reset();
                             interval = result.Count < requestedCount ? waitAfterEmptyResultIntervals.NextInterval() : waitAfterSuccessInterval;

                             foreach (var item in result)
                             {
                                 // the events order is matters. That is why we cannot simply use Task.WaitAll
                                 await buffer.SendAsync(item, cancellationToken);
                                 checkpoint = item.CheckpointToken;
                             }
                         }

                         // Occasionally check the cancellation state.
                         if (linkedCancellation.IsCancellationRequested)
                         {
                             buffer.Complete();
                             break;
                         }
                     }
                     catch (Exception caught)
                     {
                         //check whenever exception is transient
                         if (caught is TimeoutException
                              || caught is OperationCanceledException)
                         {
                             // Log the exception and try one more
                             interval = waitAfterErrorInterval;
                         }
                         else throw; //todo: log bug
                     }
                 }
                 buffer.Complete();
             }, linkedCancellation.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);
        }

        public void Run()
        {
            _pollingTask.Start();
        }

        public void Cancel()
        {
            _pollingCancellationSource.Cancel();
        }

        public void Dispose()
        {
            _pollingCancellationSource.Cancel();
            using (_pollingCancellationSource)
            using (_pollingTask) { }
        }

        private class Throttler
        {
            private readonly TimeSpan[] _slots;
            private int _position = -1;

            public Throttler(double[] slots)
            {
                _slots = slots.Select(TimeSpan.FromMilliseconds).ToArray();
            }

            public TimeSpan NextInterval()
            {
                _position += _slots.Length - 1 > _position ? 1 : 0;
                return _slots[_position];
            }

            public void Reset()
            {
                _position = -1;
            }
        }
    }
}
