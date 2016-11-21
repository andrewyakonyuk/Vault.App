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
            return Events.Select((t, i) =>
                Tuple.Create<T, StreamSequenceToken>((T)t.Body,
                    new EventSequenceToken(CommitSequence, i)));
        }

        public bool ImportRequestContext()
        {
            //if (_commit.Headers != null)
            //{
            //    RequestContext.Import(new Dictionary<string, object>(_commit.Headers));
            //    return true;
            //}
            return false;
        }

        public bool ShouldDeliver(IStreamIdentity stream, object filterData, StreamFilterPredicate shouldReceiveFunc)
        {
            return true;

            //if(shouldReceiveFunc == null)
            //return true;

            //return shouldReceiveFunc(stream, filterData,)
        }
    }
}