using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventStore;

namespace Vault.Shared.EventSourcing.NEventStore
{
    public interface IEventStoreInitializer
    {
        IStoreEvents Create();
    }
}