using System;
using System.Collections.Generic;

namespace Vault.Shared.EventSourcing.NEventStore
{
    public abstract class EntityBase : IEventProvider
    {
        readonly List<IEvent> _uncommittedEvents;
        private AggregateEventDispatcher _registeredRoutes;

        public EntityBase(AggregateRootBase aggregateRoot)
        {
            if (aggregateRoot == null)
                throw new ArgumentNullException(nameof(aggregateRoot));

            AggregateRoot = aggregateRoot;
            _uncommittedEvents = new List<IEvent>();
        }

        public Guid Id { get; protected set; }

        protected AggregateRootBase AggregateRoot { get; set; }

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
            RegisteredRoutes.Dispatch(@event);
            _uncommittedEvents.Add(@event);
        }

        void IEventProvider.ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        IReadOnlyList<IEvent> IEventProvider.GetUncommittedEvents()
        {
            return _uncommittedEvents.AsReadOnly();
        }

        void IEventProvider.LoadFromHistory(IReadOnlyList<IEvent> events)
        {
            if (events.Count == 0)
                return;

            foreach (var item in events)
            {
                RaiseEvent(item);
            }
        }
    }
}