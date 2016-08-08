using System.Collections.Generic;

namespace Vault.Shared.EventSourcing
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IHandle> GetHandlers<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}