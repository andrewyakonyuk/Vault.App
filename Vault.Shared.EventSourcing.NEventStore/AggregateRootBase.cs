using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;

namespace Vault.Shared.EventSourcing.NEventStore
{
    public abstract class AggregateRootBase : IAggregate, IEventProvider, IRegisterChildEntities, IEquatable<IAggregate>
    {
        private readonly List<IEventProvider> _childEventProviders;
        private readonly List<IEvent> _uncommittedEvents;
        private AggregateEventDispatcher _registeredRoutes;

        protected AggregateRootBase()
        {
            _childEventProviders = new List<IEventProvider>();
            _uncommittedEvents = new List<IEvent>();
        }

        public Guid Id { get; protected set; }

        public int Version { get; internal set; }

        public virtual bool Equals(IAggregate other)
        {
            return other != null && other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IAggregate);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public virtual IMemento GetSnapshot()
        {
            return null;
        }

        protected AggregateEventDispatcher RegisteredRoutes
        {
            get
            {
                AggregateEventDispatcher router;
                if ((router = _registeredRoutes) == null)
                {
                    router = (_registeredRoutes = new AggregateEventDispatcher(true, this));
                }
                return router;
            }
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("AggregateRootBase must have an event router to function");
                }
                _registeredRoutes = value;
            }
        }

        protected void RaiseEvent(IEvent @event)
        {
            ((IAggregate)this).ApplyEvent(@event);
            _uncommittedEvents.Add(@event);
        }

        void IAggregate.ApplyEvent(object @event)
        {
            RegisteredRoutes.Dispatch((IEvent)@event);
            Version++;
        }

        void IAggregate.ClearUncommittedEvents()
        {
            ((IEventProvider)this).ClearUncommittedEvents();
        }

        ICollection IAggregate.GetUncommittedEvents()
        {
            return ((IEventProvider)this).GetUncommittedEvents().ToArray();
        }

        void IEventProvider.ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
            _childEventProviders.ForEach(p => p.ClearUncommittedEvents());
        }

        IReadOnlyList<IEvent> IEventProvider.GetUncommittedEvents()
        {
            return _uncommittedEvents.Concat(GetChildEventsAndUpdateEventVersion()).ToArray();
        }

        void IEventProvider.LoadFromHistory(IReadOnlyList<IEvent> events)
        {
            if (events.Count == 0)
                return;

            foreach (var item in events)
            {
                ((IAggregate)this).ApplyEvent(item);
            }
        }

        void IRegisterChildEntities.RegisterChildEventProvider(IEventProvider eventProvider)
        {
            _childEventProviders.Add(eventProvider);
        }

        IEnumerable<IEvent> GetChildEventsAndUpdateEventVersion()
        {
            return _childEventProviders.SelectMany(entity => entity.GetUncommittedEvents());
        }
    }
}