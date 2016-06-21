using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.NEventStore
{
    public interface IEventsRebuilder
    {
        Task RebuildAsync();
    }
}