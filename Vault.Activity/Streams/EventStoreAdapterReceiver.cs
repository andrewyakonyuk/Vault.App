using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventStore;
using Orleans.Streams;

namespace Vault.Activity.Streams
{
    public class EventStoreAdapterReceiver : IQueueAdapterReceiver
    {
        readonly IStoreEvents _store;
        readonly QueueId _queueId;
        readonly IStreamQueueCheckpointer<string> _checkpointer;
        string _offset;

        public EventStoreAdapterReceiver(
            QueueId queueId,
            IStoreEvents store,
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

        public Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
        {
            var commits = _store.Advanced.GetFrom(_offset).ToArray();
            if (commits.Length == 0)
                return Task.FromResult<IList<IBatchContainer>>(new List<IBatchContainer>());

            var result = new List<IBatchContainer>(commits.Length);
            foreach (var item in commits)
            {
                var batchContainer = new EventStoreBatchContainer(item);
                result.Add(batchContainer);

                _offset = item.CheckpointToken;
            }

            _checkpointer.Update(_offset, DateTime.UtcNow);

            return Task.FromResult<IList<IBatchContainer>>(result);
        }

        public async Task Initialize(TimeSpan timeout)
        {
            _offset = await _checkpointer.Load();
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