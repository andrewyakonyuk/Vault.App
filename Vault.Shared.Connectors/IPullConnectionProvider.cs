using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    public interface IPullConnectionProvider
    {
        string Name { get; }

        Task<PullConnectionResult> PullAsync(PullConnectionContext context);
    }
}
