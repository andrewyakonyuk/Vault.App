using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Vault.Activity.Client;
using Vault.Activity.Utility;
using Microsoft.Extensions.Logging;

namespace Vault.Activity.Services.Connectors
{
    public interface ICatchConnectionWorker
    {
        Task CatchAsync(HttpResponse response);
    }
    
    public class CatchConnectionState
    {
        public Guid OwnerId { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderName { get; set; }
    }

    public class CatchConnectionWorker :  ICatchConnectionWorker
    {
        readonly IConnectionPool<ICatchConnectionProvider> _connectionPool;
        ILogger<PollingConnectionWorker> _logger;
        IClock _clock;
        readonly IActivityClient _activityClient;
        readonly string _providerName;
        readonly string _providerKey;
        readonly string _ownerId;

        public CatchConnectionWorker(
            string providerName,
            string providerKey,
            string ownerId,
            IConnectionPool<ICatchConnectionProvider> connectionPool,
            IActivityClient activityClient,
            ILogger<PollingConnectionWorker> logger,
            IClock clock)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException(nameof(providerName));

            _providerName = providerName;
            _providerKey = providerKey ?? throw new ArgumentNullException(nameof(providerKey));
            _ownerId = ownerId;
            _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
            _activityClient = activityClient ?? throw new ArgumentNullException(nameof(activityClient));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CatchAsync(HttpResponse response)
        {
            var connectionProvider = _connectionPool.GetByName(_providerName);
            if (connectionProvider == null)
                throw new NotSupportedException($"Provider '{_providerName}' is not support 'Catch' method");

            try
            {
                var activityFeed = await _activityClient.GetStreamAsync(Buckets.Default, _ownerId);
                var userInfo = new UserInfo(_providerKey, _ownerId);
                var context = new CatchConnectionContext(userInfo, response);

                var result = await connectionProvider.CatchAsync(context);

                foreach (var activity in result)
                {
                    await activityFeed.PushActivityAsync(activity);
                }

                _logger.LogInformation($"{_ownerId}: Finished '{_providerName}'s catch hook with {result.Count} results");
            }
            finally
            {
                _connectionPool.Release(connectionProvider);
            }
        }
    }
}