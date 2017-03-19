using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventStore;
using Orleans.Streams;
using Vault.Activity.Persistence;
using Orleans;

namespace Vault.Activity.Streams
{
    public class EventStoreQueryAdapter : IQueueAdapter
    {
        readonly IAppendOnlyStore _store;
        readonly ConcurrentDictionary<QueueId, EventStoreAdapterReceiver> _receivers;
        readonly IStreamQueueCheckpointer<string> _checkpointer;

        public EventStoreQueryAdapter(
            string providerName,
            IAppendOnlyStore store,
            IStreamQueueCheckpointer<string> checkpointer)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException(nameof(providerName));
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            if (checkpointer == null)
                throw new ArgumentNullException(nameof(checkpointer));

            Name = providerName;
            _store = store;
            _receivers = new ConcurrentDictionary<QueueId, EventStoreAdapterReceiver>();
            _checkpointer = checkpointer;
        }

        public StreamProviderDirection Direction => StreamProviderDirection.ReadOnly;

        public bool IsRewindable => true;

        public string Name { get; }

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return _receivers.GetOrAdd(queueId, MakeReceiver);
        }

        public Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            throw new NotSupportedException();
        }

        private EventStoreAdapterReceiver MakeReceiver(QueueId queueId)
        {
            return new EventStoreAdapterReceiver(queueId, _store, _checkpointer);
        }
    }
}