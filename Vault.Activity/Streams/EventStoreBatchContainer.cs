using System;
using System.Collections.Generic;
using System.Linq;
using NEventStore;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Concurrency;

namespace Vault.Activity.Streams
{
    [Serializable]
    public class EventStoreBatchContainer : IBatchContainer
    {
        public StreamSequenceToken SequenceToken { get; }

        public Guid StreamGuid { get; }

        public string StreamNamespace { get; }

        int CommitSequence { get; }

        readonly CommitedActivityEvent _activity;

        public EventStoreBatchContainer()
        {

        }

        public EventStoreBatchContainer(CommitedActivityEvent activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            _activity = activity;
            StreamGuid = activity.StreamId;
            StreamNamespace = activity.Bucket;
            SequenceToken = new EventSequenceToken(activity.CheckpointToken);
            CommitSequence = (int)activity.CheckpointToken;
        }

        public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
        {
            yield return new Tuple<T, StreamSequenceToken>((T)(object)_activity, new EventSequenceToken(CommitSequence));
        }

        public bool ImportRequestContext()
        {
            return false;
        }

        public bool ShouldDeliver(IStreamIdentity stream, object filterData, StreamFilterPredicate shouldReceiveFunc)
        {
            if (shouldReceiveFunc == null)
                return true;

            if (shouldReceiveFunc(stream, filterData, _activity))
                return true; // There is something in this batch that the consumer is intereted in, so we should send it.

            return false; // Consumer is not interested in any of these events, so don't send.
        }
    }
}