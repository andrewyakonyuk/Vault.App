using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using System.Linq;
using Vault.Activity.Client;
using Vault.Activity.Utility;

namespace Vault.Activity.Services.Connectors
{
    public interface ICatchConnectionWorker : IGrainWithGuidKey
    {
        Task CatchAsync(HttpResponse response);

        Task<bool> TryConnectAsync(ConnectionKey key);

        Task DisconnectAsync();
    }
    
    public class CatchConnectionState
    {
        public Guid OwnerId { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderName { get; set; }
    }

    public class CatchConnectionWorker : Grain<CatchConnectionState>, ICatchConnectionWorker
    {
        readonly IConnectionPool<ICatchConnectionProvider> _connectionPool;
        readonly IActivityClient _activityClient;
        Logger _logger;
        IClock _clock;

        public CatchConnectionWorker(
            IConnectionPool<ICatchConnectionProvider> connectionPool,
            IActivityClient activityClient,
            IClock clock)
        {
            if (connectionPool == null)
                throw new ArgumentNullException(nameof(connectionPool));
            if (activityClient == null)
                throw new ArgumentNullException(nameof(activityClient));

            _connectionPool = connectionPool;
            _activityClient = activityClient;
            _clock = clock;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            _logger = GetLogger();

            _logger.Verbose($"{this.GetPrimaryKey()}: Created activation of {nameof(CatchConnectionWorker)} grain");
        }

        public async Task<bool> TryConnectAsync(ConnectionKey connectionKey)
        {
            var connectionProvider = _connectionPool.GetByName(connectionKey.ProviderName);
            if (connectionProvider != null)
            {
                State.ProviderKey = connectionKey.ProviderKey;
                State.ProviderName = connectionKey.ProviderName;
                State.OwnerId = connectionKey.OwnerId;

                var userInfo = new UserInfo(State.ProviderKey, State.OwnerId);
                var context = new SubscribeConnectionContext(userInfo, this.GetPrimaryKey());
                await connectionProvider.SubscribeAsync(context);

                await WriteStateAsync();

                _logger.Verbose($"{this.GetPrimaryKey()}: Connected a new '{connectionKey.ProviderName}'s login for '{connectionKey.OwnerId}' user");
                return true;
            }
            else
            {
                _logger.Verbose($"{this.GetPrimaryKey()}: Tried to connect a new '{connectionKey.ProviderName}'s login for '{connectionKey.OwnerId}' user for non supported provider");
                return false;
            }
        }

        public async Task CatchAsync(HttpResponse response)
        {
            if (string.IsNullOrEmpty(State.ProviderKey))
                throw new InvalidOperationException("Connection must be configured before any usage");

            var connectionProvider = _connectionPool.GetByName(State.ProviderName);
            if (connectionProvider == null)
                throw new NotSupportedException($"Provider '{State.ProviderName}' is not support 'Catch' method");

            try
            {
                var activityFeed = await _activityClient.GetStreamAsync(Buckets.Default, State.OwnerId);
                var userInfo = new UserInfo(State.ProviderKey, State.OwnerId);
                var context = new CatchConnectionContext(userInfo, response);

                var result = await connectionProvider.CatchAsync(context);

                foreach (var activity in result)
                {
                    await activityFeed.PushActivityAsync(activity);
                }

                _logger.Verbose($"{this.GetPrimaryKey()}: Finished '{State.ProviderName}'s catch hook with {result.Count} results");
            }
            finally
            {
                _connectionPool.Release(connectionProvider);
            }
        }

        public async Task DisconnectAsync()
        {
            var connectionProvider = _connectionPool.GetByName(State.ProviderName);
            if (connectionProvider != null)
            {
                var userInfo = new UserInfo(State.ProviderKey, State.OwnerId);
                var context = new SubscribeConnectionContext(userInfo, this.GetPrimaryKey());
                await connectionProvider.UnsubscribeAsync(context);

                _logger.Verbose($"{this.GetPrimaryKey()}: Disconnected '{State.ProviderName}'s login for '{State.OwnerId}' user");
            }
        }
    }
}