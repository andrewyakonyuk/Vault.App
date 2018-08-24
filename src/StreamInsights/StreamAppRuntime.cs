using Microsoft.Extensions.Logging;
using StreamInsights.Abstractions;
using StreamInsights.Infrastructure;
using StreamInsights.Infrastructure.Dataflow;
using StreamInsights.Persistance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace StreamInsights
{
    public class StreamAppRuntime : IDisposable
    {
        readonly CancellationTokenSource _environmentTokenSource;
        readonly IAppendOnlyActivityStore _appendOnlyStore;
        readonly ILogger<StreamAppRuntime> _logger;
        readonly NextStreamProcessor _nextProcessor;

        public StreamAppRuntime(string appId,
            IAppendOnlyActivityStore appendOnlyStore,
            IEnumerable<IStreamProcessor> streamProcessors,
            ILogger<StreamAppRuntime> logger,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentNullException(nameof(appId));
            if (streamProcessors == null)
                throw new ArgumentNullException(nameof(streamProcessors));

            AppId = appId;
            _appendOnlyStore = appendOnlyStore ?? throw new ArgumentNullException(nameof(appendOnlyStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environmentTokenSource = new CancellationTokenSource();

            _nextProcessor = BuildNextMethod(streamProcessors.ToList());
            if (_nextProcessor == null)
                return;

            var buffer = new BufferBlock<CommitedActivity>(new DataflowBlockOptions
            {
                BoundedCapacity = 1000,
                CancellationToken = _environmentTokenSource.Token
            });
            var partionedBlock = new PartitionedActionBlock<CommitedActivity>(
                entry => ProcessActivity(entry.Source, _environmentTokenSource.Token),
                GetPartitionKey,
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount * 10,
                    CancellationToken = _environmentTokenSource.Token
                });
            buffer.LinkTo(partionedBlock, new DataflowLinkOptions { PropagateCompletion = true });

            var subscription = new PollingSubscription<CommitedActivity>(_appendOnlyStore.ReadAsync,
                (activity, token) => buffer.SendAsync(activity, token),
                cancellationToken: _environmentTokenSource.Token);

            subscription.Run();
        }

        public string AppId { get; }

        async Task ProcessActivity(CommitedActivity activity, CancellationToken token)
        {
            var watch = ValueStopwatch.StartNew();
            try
            {
                await _nextProcessor(activity, token);
            }
            catch (Exception ex)
            {
                var exception = ex.Demystify();
                _logger.LogError(exception, "Could not to process the activity with checkout token: {0}", activity.CheckpointToken);
            }
            finally
            {
                var elapsedMs = watch.Stop();
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Processed activity '{0}' in {1}ms", activity.CheckpointToken, elapsedMs);
            }
        }

        static NextStreamProcessor BuildNextMethod(IReadOnlyList<IStreamProcessor> processors)
        {
            var queue = new Queue<NextStreamProcessor>();
            for (int i = processors.Count - 1; i >= 0; i--)
            {
                var isLast = i == processors.Count - 1;
                NextStreamProcessor next = (activity, token) => Task.CompletedTask;
                if (!isLast)
                {
                    next = queue.Dequeue();
                }

                var processor = processors[i];
                queue.Enqueue((activity, token) =>
                {
                    return processor.Process(activity, next, token);
                });
            }

            if (queue.Count > 0)
                return queue.Dequeue();

            return null;
        }

        static int GetPartitionKey(CommitedActivity activity)
        {
            var hashCode = activity.StreamId.GetHashCode() * 397 ^ activity.Bucket.GetHashCode();

            if (!string.IsNullOrEmpty(activity.Id))
            {
                hashCode ^= activity.Id.GetHashCode() * 42;
            }

            return hashCode;
        }

        public void Dispose()
        {
            _environmentTokenSource.Dispose();
        }
    }
}
