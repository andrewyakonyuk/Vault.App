using System;
using System.Collections.Generic;

namespace Vault.Shared.EventSourcing
{
    public interface IEventProvider
    {
        Guid Id { get; }

        void LoadFromHistory(IReadOnlyList<IEvent> events);

        void ClearUncommittedEvents();

        IReadOnlyList<IEvent> GetUncommittedEvents();
    }
}