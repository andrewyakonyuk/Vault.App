using System;
using System.Threading.Tasks;
using Vault.Shared.TransientFaultHandling;
using Vault.Activity.Client;
using Vault.Activity.Utility;
using Microsoft.Extensions.Logging;

namespace Vault.Activity.Services.Connectors
{
    public interface IPollingConnectionWorker
    {
        Task PullAsync();
    }

    public class PollingConnectionWorker :  IPollingConnectionWorker
    {
        readonly IConnectionPool<IPullConnectionProvider> _connectionPool;
        ILogger<PollingConnectionWorker> _logger;
        IClock _clock;
        readonly IActivityClient _activityClient;
        readonly string _providerName;
        readonly string _providerKey;
        readonly string _ownerId;
        DateTimeOffset? _lastFetchTimeUtc;

        public PollingConnectionWorker(
            string providerName,
            string providerKey,
            string ownerId,
            IConnectionPool<IPullConnectionProvider> connectionPool,
            IActivityClient activityClient,
            ILogger<PollingConnectionWorker> logger,
            IClock clock)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException(nameof(providerName));
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentNullException(nameof(ownerId));

            _providerName = providerName;
            _providerKey = providerKey ?? throw new ArgumentNullException(nameof(providerKey));
            _ownerId = ownerId;
            _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
            _activityClient = activityClient ?? throw new ArgumentNullException(nameof(activityClient));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PullAsync()
        {
            var connectionProvider = _connectionPool.GetByName(_providerName);
            if (connectionProvider == null)
                throw new NotSupportedException($"Provider '{_providerName}' is not support 'Pull' method");
            try
            {
                PullResult result = null;
                var batch = 0;
                var activityFeed = await _activityClient.GetStreamAsync(Buckets.Default, _ownerId);
                do
                {
                    result = await ExecuteBatchAsync(connectionProvider, batch, _lastFetchTimeUtc);

                    foreach (var activity in result)
                    {
                        await activityFeed.PushActivityAsync(activity);
                    }

                    _logger.LogInformation($"{_ownerId}: Finished pulling batch '{batch}' with {result.Count} results");

                    batch++;
                } while (!result.IsCancellationRequested);

                _lastFetchTimeUtc = _clock.OffsetUtcNow;
            }
            finally
            {
                _connectionPool.Release(connectionProvider);
            }
        }

        async Task<PullResult> ExecuteBatchAsync(IPullConnectionProvider connectionProvider, int batch, DateTimeOffset? lastFetchTimeUtc = null)
        {
            var userInfo = new UserInfo(_providerKey, _ownerId);
            var context = new PullConnectionContext(userInfo)
            {
                Batch = batch,
                LastFetchTimeUtc = lastFetchTimeUtc
            };

            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            var retryPolicy = new RetryPolicy<ConnectionErrorDetectionStrategy>(retryStrategy);

            return await retryPolicy.ExecuteAsync(() => connectionProvider.PullAsync(context));
        }
    }
}