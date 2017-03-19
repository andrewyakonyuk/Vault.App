using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventStore;
using Orleans.Streams;
using Vault.Activity.Persistence;

namespace Vault.Activity.Streams
{
    public class EventStoreAdapterReceiver : IQueueAdapterReceiver
    {
        readonly QueueId _queueId;
        readonly IStreamQueueCheckpointer<string> _checkpointer;
        long _offset;
        readonly IAppendOnlyStore _store;

        public EventStoreAdapterReceiver(
            QueueId queueId,
            IAppendOnlyStore store,
            IStreamQueueCheckpointer<string> checkpointer)
        {
            if (queueId == null)
                throw new ArgumentNullException(nameof(queueId));
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            if (checkpointer == null)
                throw new ArgumentNullException(nameof(checkpointer));

            _queueId = queueId;
            _store = store;
            _checkpointer = checkpointer;
        }

        public async Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
        {
            var records = await _store.ReadRecordsAsync(_offset, maxCount);
            if (records.Count == 0)
                return new List<IBatchContainer>();

            var result = new List<IBatchContainer>(records.Count);
            foreach (var item in records)
            {
                var batchContainer = new EventStoreBatchContainer(item);
                result.Add(batchContainer);

                _offset = item.CheckpointToken;
            }

            _checkpointer.Update(_offset.ToString(), DateTime.UtcNow);

            return result;
        }

        public async Task Initialize(TimeSpan timeout)
        {
            if (!long.TryParse((await _checkpointer.Load()), out _offset))
                _offset = 0;
        }

        public Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
        {
            return Task.FromResult(true);
        }

        public Task Shutdown(TimeSpan timeout)
        {
            return Task.FromResult(true);
        }
    }
}