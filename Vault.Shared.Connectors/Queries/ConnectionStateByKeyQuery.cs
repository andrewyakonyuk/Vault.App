using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;
using Vault.Shared.Queries;

namespace Vault.Shared.Connectors.Queries
{
    public class ConnectionStateByKeyQuery : IQuery<ConnectionStateKey, PullConnectionState>
    {
        readonly ILinqProvider _linqProvider;

        public ConnectionStateByKeyQuery(ILinqProvider linqProvider)
        {
            _linqProvider = linqProvider;
        }

        public Task<PullConnectionState> AskAsync(ConnectionStateKey criterion)
        {
            var result = _linqProvider.Query<PullConnectionState>()
                .FirstOrDefault(t => t.ProviderKey == criterion.ProviderKey && t.ProviderName == criterion.ProviderName);

            return Task.FromResult(result);
        }
    }
}
