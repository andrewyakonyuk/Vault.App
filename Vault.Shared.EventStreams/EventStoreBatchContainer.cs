using System;
using System.Collections.Generic;
using System.Linq;
using NEventStore;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;

namespace Vault.Shared.EventStreams
{
    [Serializable]
    public class EventStoreBatchContainer : IBatchContainer
    {
        public StreamSequenceToken SequenceToken { get; }

        public Guid StreamGuid { get; }

        public string StreamNamespace { get; }

        ICollection<EventMessage> Events { get; }

        int CommitSequence { get; }

        public EventStoreBatchContainer(ICommit commit)
        {
            if (commit == null)
                throw new ArgumentNullException(nameof(commit));

            StreamGuid = Guid.Parse(commit.StreamId);
            StreamNamespace = commit.BucketId;
            SequenceToken = new EventSequenceToken(commit.CommitSequence);
            Events = commit.Events;
            CommitSequence = commit.CommitSequence;
        }

        public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
        {
            return Events.Where(t => t.Body is T).Select((t, i) =>
                 Tuple.Create<T, StreamSequenceToken>((T)t.Body,
                     new EventSequenceToken(CommitSequence, i)));
        }

        public bool ImportRequestContext()
        {
            return false;
        }

        public bool ShouldDeliver(IStreamIdentity stream, object filterData, StreamFilterPredicate shouldReceiveFunc)
        {
            if (shouldReceiveFunc == null)
                return true;

            foreach (var item in Events)
            {
                if (shouldReceiveFunc(stream, filterData, item.Body))
                    return true; // There is something in this batch that the consumer is intereted in, so we should send it.
            }
            return false; // Consumer is not interested in any of these events, so don't send.
        }
    }
}