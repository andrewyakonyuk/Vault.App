using Microsoft.Extensions.Logging;
using StreamInsights.Abstractions;
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
        readonly NextStreamHandler _nextHandler;

        public StreamAppRuntime(string appId,
            IAppendOnlyActivityStore appendOnlyStore,
            IEnumerable<IStreamHandler> streamHandlers,
            ILogger<StreamAppRuntime> logger,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentNullException(nameof(appId));
            if (streamHandlers == null)
                throw new ArgumentNullException(nameof(streamHandlers));

            AppId = appId;
            _appendOnlyStore = appendOnlyStore ?? throw new ArgumentNullException(nameof(appendOnlyStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environmentTokenSource = new CancellationTokenSource();

            _nextHandler = BuildNextMethod(streamHandlers.ToList());
            if (_nextHandler == null)
                return;

            var partionedOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = _environmentTokenSource.Token
            };
            var partionedBlock = new PartitionedActionBlock<CommitedActivity>(
                entry => HandleNotification(entry.Source, _environmentTokenSource.Token), GetPartitionKey, partionedOptions);

            var subscription = new PollingSubscription<CommitedActivity>(_appendOnlyStore.ReadAsync,
                (activity, token) => partionedBlock.SendAsync(activity, token),
                cancellationToken: _environmentTokenSource.Token);

            subscription.Run();
        }

        public string AppId { get; }

        async Task HandleNotification(CommitedActivity commited, CancellationToken token)
        {
            try
            {
                await _nextHandler(commited, token);
            }
            catch (Exception ex)
            {
                var exception = ex.Demystify();
                _logger.LogError(exception, exception.ToString());
            }
        }

        static NextStreamHandler BuildNextMethod(IReadOnlyList<IStreamHandler> handlers)
        {
            var queue = new Queue<NextStreamHandler>();
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                var isLast = i == handlers.Count - 1;
                NextStreamHandler next = (activity, token) => Task.CompletedTask;
                if (!isLast)
                {
                    next = queue.Dequeue();
                }

                var handler = handlers[i];
                queue.Enqueue((activity, token) =>
                {
                    return handler.Handle(activity, token, next);
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
