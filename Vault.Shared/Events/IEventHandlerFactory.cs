using System.Collections.Generic;

namespace Vault.Shared.Events
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IHandle> GetHandlers<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}