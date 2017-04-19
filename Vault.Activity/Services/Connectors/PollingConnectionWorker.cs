using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using Vault.Shared.TransientFaultHandling;
using System.Linq;
using Vault.Activity.Client;
using Vault.Activity.Utility;

namespace Vault.Activity.Services.Connectors
{
    public interface IPollingConnectionWorker : IGrainWithGuidKey
    {
        Task<bool> TryConnectAsync(ConnectionKey key);

        Task DisconnectAsync();

        Task PullAsync();
    }
    
    public class PollingConnectionState
    {
        public Guid OwnerId { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderName { get; set; }
        public DateTimeOffset? LastFetchDateUtc { get; set; }
    }

    public class PollingConnectionWorker : Grain<PollingConnectionState>, IPollingConnectionWorker, IRemindable
    {
        readonly IConnectionPool<IPullConnectionProvider> _connectionPool;
        Logger _logger;
        IClock _clock;
        readonly IActivityClient _activityClient;

        public PollingConnectionWorker(
            IConnectionPool<IPullConnectionProvider> connectionPool,
            IActivityClient activityClient,
            IClock clock)
        {
            if (connectionPool == null)
                throw new ArgumentNullException(nameof(connectionPool));
            if (activityClient == null)
                throw new ArgumentNullException(nameof(activityClient));
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

            _connectionPool = connectionPool;
            _activityClient = activityClient;
            _clock = clock;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            _logger = GetLogger();

            _logger.Verbose($"{this.GetPrimaryKey()}: Created activation of {nameof(PollingConnectionWorker)} grain");
        }

        public async Task<bool> TryConnectAsync(ConnectionKey connectionKey)
        {
            var connectionProvider = _connectionPool.GetByName(connectionKey.ProviderName);
            if (connectionProvider != null)
            {
                State.ProviderKey = connectionKey.ProviderKey;
                State.ProviderName = connectionKey.ProviderName;
                State.OwnerId = connectionKey.OwnerId;

                await RegisterOrUpdateReminder("Polling", new TimeSpan(0, 0, 5), new TimeSpan(0, 1, 0));

                await WriteStateAsync();

                _logger.Verbose($"{this.GetPrimaryKey()}: Connected a new '{connectionKey.ProviderName}'s login for '{connectionKey.OwnerId}' user");

                return true;
            }
            return false;
        }

        public async Task PullAsync()
        {
            if (string.IsNullOrEmpty(State.ProviderKey))
                throw new InvalidOperationException("Connection must be configured before any usage");

            var connectionProvider = _connectionPool.GetByName(State.ProviderName);
            if (connectionProvider == null)
                throw new NotSupportedException($"Provider '{State.ProviderName}' is not support 'Pull' method");
            try
            {
                PullResult result = null;
                var batch = 0;
                var activityFeed = await _activityClient.GetStreamAsync(Buckets.Default, State.OwnerId);
                do
                {
                    result = await ExecuteBatchAsync(connectionProvider, batch, State.LastFetchDateUtc);

                    foreach (var activity in result)
                    {
                        await activityFeed.PushActivityAsync(activity);
                    }

                    _logger.Verbose($"{this.GetPrimaryKey()}: Finished pulling batch '{batch}' with {result.Count} results");

                    batch++;
                } while (!result.IsCancellationRequested);

                State.LastFetchDateUtc = _clock.OffsetUtcNow;
                await WriteStateAsync();
            }
            finally
            {
                _connectionPool.Release(connectionProvider);
            }
        }

        async Task<PullResult> ExecuteBatchAsync(IPullConnectionProvider connectionProvider, int batch, DateTimeOffset? lastFetchTimeUtc = null)
        {
            var userInfo = new UserInfo(State.ProviderKey, State.OwnerId);
            var context = new PullConnectionContext(userInfo)
            {
                Batch = batch,
                LastFetchTimeUtc = lastFetchTimeUtc
            };

            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            var retryPolicy = new RetryPolicy<ConnectionErrorDetectionStrategy>(retryStrategy);

            return await retryPolicy.ExecuteAsync(() => connectionProvider.PullAsync(context));
        }

        public async Task DisconnectAsync()
        {
            var reminder = await GetReminder("Polling");
            if (reminder != null)
                await UnregisterReminder(reminder);

            _logger.Verbose($"{this.GetPrimaryKey()}: Disconnected '{State.ProviderName}'s login for '{State.OwnerId}' user");
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            await PullAsync();
        }
    }
}