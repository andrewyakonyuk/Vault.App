using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.EventSourcing
{
    public interface IEventsRebuilder
    {
        Task RebuildAsync();
    }
}