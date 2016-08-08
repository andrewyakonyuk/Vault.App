using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using NEventStore;

namespace Vault.Shared.EventSourcing.NEventStore
{
    public class EventedUnitOfWorkFactory : IEventedUnitOfWorkFactory
    {
        private readonly IDetectConflicts _conflictDetector;
        private readonly IStoreEvents _eventStore;

        public EventedUnitOfWorkFactory(
           IStoreEvents eventStore,
           IDetectConflicts conflictDetector)
        {
            _eventStore = eventStore;
            _conflictDetector = conflictDetector;
        }

        public IEventedUnitOfWork Create()
        {
            return new EventedUnitOfWork(_eventStore, _conflictDetector);
        }
    }
}