using NEventStore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;
using Vault.Shared.Events;

namespace Vault.Shared.NEventStore
{
    public interface IEventedUnitOfWork : IDisposable
    {
        void Commit();

        IEventedStream GetStream(string streamId);

        void Save(IEventedStream @event);
    }

    public interface IEventedStream : IEnumerable<IEvent>
    {
        string StreamId { get; }

        void Add(IEvent @event);

        IEventedStream AsUncommited();
    }

    public sealed class EventedStream : IEventedStream
    {
        private List<IEvent> _events;
        private List<IEvent> _uncommitedEvents;

        public EventedStream(string streamId, IEnumerable<IEvent> events)
        {
            StreamId = streamId;
            _events = new List<IEvent>(events);
            _uncommitedEvents = new List<IEvent>();
        }

        public string StreamId { get; private set; }

        public void Add(IEvent @event)
        {
            _uncommitedEvents.Add(@event);
        }

        public IEventedStream AsUncommited()
        {
            return new EventedStream(StreamId, _uncommitedEvents);
        }

        public IEnumerator<IEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator();
        }
    }
}