using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventStore;
using Orleans.Streams;

namespace Vault.Activity.Streams
{
    public class EventStoreQueryAdapter : IQueueAdapter
    {
        readonly IStoreEvents _store;
        readonly ConcurrentDictionary<QueueId, EventStoreAdapterReceiver> _receivers;
        readonly IStreamQueueCheckpointer<string> _checkpointer;

        public EventStoreQueryAdapter(
            string providerName,
            IStoreEvents store,
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

        public StreamProviderDirection Direction => StreamProviderDirection.ReadWrite;

        public bool IsRewindable => true;

        public string Name { get; }

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return _receivers.GetOrAdd(queueId, MakeReceiver);
        }

        public Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            var latestSnapshot = _store.Advanced.GetSnapshot(streamNamespace, streamGuid, int.MaxValue);

            using (var stream = latestSnapshot == null ?
                _store.OpenStream(streamNamespace, streamGuid) :
                _store.OpenStream(latestSnapshot, int.MaxValue))
            {
                foreach (var item in events)
                {
                    stream.Add(new EventMessage { Body = item, Headers = requestContext });
                }
                stream.CommitChanges(Guid.NewGuid());

                if (stream.CommitSequence % 1000 == 0)
                    _store.Advanced.AddSnapshot(new Snapshot(stream.BucketId, stream.StreamRevision, string.Empty));

                return Task.FromResult(true);
            }
        }

        private EventStoreAdapterReceiver MakeReceiver(QueueId queueId)
        {
            return new EventStoreAdapterReceiver(queueId, _store, _checkpointer);
        }
    }
}