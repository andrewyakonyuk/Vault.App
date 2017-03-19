using System;
using System.Threading.Tasks;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;
using Orleans.Providers;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;
using NEventStore.Serialization;
using Vault.Activity.Persistence;

namespace Vault.Activity.Streams
{
    public class EventStoreAdapterFactory : IQueueAdapterFactory
    {
        Logger _logger;
        IServiceProvider _serviceProvider;
        IProviderConfiguration _providerConfig;
        string _providerName;
        IAppendOnlyStore _store;
        int _cacheSize;
        IQueueAdapterCache _adapterCache;
        IStreamQueueMapper _streamQueueMapper;
        IStreamQueueCheckpointer<string> _checkpointer;

        internal const int CacheSizeDefaultValue = 4096;
        public const int NumQueuesDefaultValue = 8; // keep as power of 2.

        protected Func<QueueId, Task<IStreamFailureHandler>> StreamFailureHandlerFactory { private get; set; }

        public Task<IQueueAdapter> CreateAdapter()
        {
            var adapter = new EventStoreQueryAdapter(_providerName, _store, _checkpointer);
            return Task.FromResult<IQueueAdapter>(adapter);
        }

        public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
        {
            return StreamFailureHandlerFactory(queueId);
        }

        public IQueueAdapterCache GetQueueAdapterCache() => _adapterCache;

        public IStreamQueueMapper GetStreamQueueMapper() => _streamQueueMapper;

        public void Init(IProviderConfiguration config, string providerName, Logger logger, IServiceProvider serviceProvider)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException(nameof(providerName));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _providerConfig = config;
            _providerName = providerName;
            _logger = logger;
            _serviceProvider = serviceProvider;

            _store = new SqlAppendOnlyStore(new PostgreSqlConnectionFactory(config.GetProperty("DataConnectionString", null)), new Persistence.JsonSerializer());

            _cacheSize = SimpleQueueAdapterCache.ParseSize(config, CacheSizeDefaultValue);
            _adapterCache = new SimpleQueueAdapterCache(_cacheSize, logger);

            if (StreamFailureHandlerFactory == null)
            {
                StreamFailureHandlerFactory =
                    qid => Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler(false));
            }

            _streamQueueMapper = new HashRingBasedStreamQueueMapper(NumQueuesDefaultValue, providerName);
            _checkpointer = new InMemoryCheckpointer();
        }
    }
}