using StreamInsights.Abstractions;
using StreamInsights.Persistance;
using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace StreamInsights
{
    public class DefaultActivityClient : IActivityClient, IDisposable
    {
        readonly IClock _clock;
        readonly IAppendOnlyActivityStore _appendOnlyStore;
        readonly CancellationTokenSource _tokenSource;
        readonly ITargetBlock<UncommitedActivity> _target;
        readonly Timer _timer;

        public DefaultActivityClient(
            IAppendOnlyActivityStore appendOnlyStore,
            IClock clock)
        {
            _appendOnlyStore = appendOnlyStore ?? throw new ArgumentNullException(nameof(appendOnlyStore));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _tokenSource = new CancellationTokenSource();

            var buffer = new BufferBlock<UncommitedActivity>(new DataflowBlockOptions
            {
                BoundedCapacity = 200,
                CancellationToken = _tokenSource.Token
            });
            var batcher = new BatchBlock<UncommitedActivity>(25, new GroupingDataflowBlockOptions
            {
                BoundedCapacity = 100,
                Greedy = true,
                CancellationToken = _tokenSource.Token
            });
            var appendToStore = new ActionBlock<UncommitedActivity[]>(async batch =>
            {
                await _appendOnlyStore.AppendAsync(batch, _tokenSource.Token);
            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = _tokenSource.Token
            });
            buffer.LinkTo(batcher, new DataflowLinkOptions { PropagateCompletion = true });
            batcher.LinkTo(appendToStore, new DataflowLinkOptions { PropagateCompletion = true });

            _timer = new Timer(_ => batcher.TriggerBatch(), null, 10000, 5000);
            _target = buffer;
        }

        public IActivityStream GetStream(string bucket, string streamId)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(streamId))
                throw new ArgumentNullException(nameof(streamId));
            
            return new ActivityStream(bucket, streamId, _target, _appendOnlyStore, _clock);
        }

        public void Dispose()
        {
            try
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }
            finally
            {
                _timer.Dispose();
            }
        }
    }
    
}
