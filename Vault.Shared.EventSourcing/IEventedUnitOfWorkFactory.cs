using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;

namespace Vault.Shared.EventSourcing
{
    public interface IEventedUnitOfWorkFactory
    {
        IEventedUnitOfWork Create();
    }
}