using NEventStore;
using NEventStore.Dispatcher;
using System;
using Vault.Shared.Events;

namespace Vault.Shared.NEventStore
{
    public class InMemoryDispatcher : IDispatchCommits
    {
        readonly IEventHandlerFactory _eventHandlerFactory;
        private bool _disposed;

        public InMemoryDispatcher(IEventHandlerFactory eventHandlerFactory)
        {
            if (eventHandlerFactory == null)
                throw new ArgumentNullException(nameof(eventHandlerFactory));

            _eventHandlerFactory = eventHandlerFactory;
        }

        public void Publish<TEvent>(TEvent message)
            where TEvent : IEvent
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var handlers = _eventHandlerFactory.GetHandlers<TEvent>(message);
            foreach (dynamic handler in handlers)
            {
                handler.Handle(message);
            }
        }

        public void Dispatch(ICommit commit)
        {
            if (commit == null)
                throw new ArgumentNullException(nameof(commit));

            foreach (var @event in commit.Events)
            {
                Publish((dynamic)@event.Body);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //noop atm
                }
                _disposed = true;
            }
        }
    }
}