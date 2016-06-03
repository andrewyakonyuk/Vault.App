using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    public interface IConnectionService
    {
        Task PullAsync(string providerName, string providerKey);
    }
}
