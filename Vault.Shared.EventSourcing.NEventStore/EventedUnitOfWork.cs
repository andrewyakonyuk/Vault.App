using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using NEventStore;
using NEventStore.Persistence;
using Vault.Shared.EventSourcing;

namespace Vault.Shared.EventSourcing.NEventStore
{
    public class EventedUnitOfWork : IEventedUnitOfWork
    {
        private readonly IDetectConflicts _conflictDetector;
        private readonly IStoreEvents _eventStore;
        private readonly IDictionary<string, IEventStream> _streams;
        const string DefaultBucketId = "default";

        public EventedUnitOfWork(
            IStoreEvents eventStore,
            IDetectConflicts conflictDetector)
        {
            _eventStore = eventStore;
            _conflictDetector = conflictDetector;
            _streams = new Dictionary<string, IEventStream>();
        }

        public void Commit()
        {
            foreach (var stream in _streams.Values)
            {
                Commit(Guid.NewGuid(), stream);
            }
            _streams.Clear();
        }

        protected virtual void Commit(Guid commitId, IEventStream stream)
        {
            while (true)
            {
                int commitEventCount = stream.CommittedEvents.Count;
                try
                {
                    stream.CommitChanges(commitId);
                }
                catch (DuplicateCommitException)
                {
                    stream.ClearChanges();
                }
                catch (ConcurrencyException e)
                {
                    bool conflict = ThrowOnConflict(stream, commitEventCount);
                    stream.ClearChanges();
                    if (conflict)
                    {
                        throw new ConflictingCommandException(e.Message, e);
                    }
                    continue;
                }
                catch (StorageException ex)
                {
                    throw new PersistenceException(ex.Message, ex);
                }
                break;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            lock (_streams)
            {
                foreach (KeyValuePair<string, IEventStream> stream in _streams)
                {
                    stream.Value.Dispose();
                }
                _streams.Clear();
            }
        }

        public void Save(IEventedStream entity)
        {
            var headers = new Dictionary<string, object>();
            PrepareStream(DefaultBucketId, entity, headers);
        }

        private IEventStream PrepareStream(string bucketId, IEventedStream entity, Dictionary<string, object> headers)
        {
            string streamId = bucketId + "+" + entity.StreamId;
            IEventStream stream;
            if (!_streams.TryGetValue(streamId, out stream))
            {
                stream = (_streams[streamId] = _eventStore.CreateStream(bucketId, entity.StreamId));
            }

            foreach (KeyValuePair<string, object> item in headers)
            {
                stream.UncommittedHeaders[item.Key] = item.Value;
            }

            foreach (var item in entity.AsUncommited())
            {
                var message = new EventMessage
                {
                    Body = item
                };
                stream.Add(message);
            }

            return stream;
        }

        private bool ThrowOnConflict(IEventStream stream, int skip)
        {
            IEnumerable<object> committed = from x in stream.CommittedEvents.Skip(skip)
                                            select x.Body;
            IEnumerable<object> uncommitted = from x in stream.UncommittedEvents
                                              select x.Body;
            return _conflictDetector.ConflictsWith(uncommitted, committed);
        }

        public IEventedStream GetStream(string id)
        {
            var stream = OpenStream(DefaultBucketId, id, int.MaxValue);
            var events = new List<IEvent>();
            foreach (var item in stream.CommittedEvents)
            {
                events.Add((IEvent)item.Body);
            }

            return new EventedStream(id, events);
        }

        IEventStream OpenStream(string bucketId, string id, int version)
        {
            string streamId = bucketId + "+" + id;
            IEventStream stream;
            if (_streams.TryGetValue(streamId, out stream))
            {
                return stream;
            }
            stream = _eventStore.OpenStream(streamId, maxRevision: version);
            return _streams[streamId] = stream;
        }
    }
}