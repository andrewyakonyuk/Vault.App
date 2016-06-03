using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vault.Shared.Connectors
{
    public class DefaultConnectionService : IConnectionService
    {
        readonly IDictionary<string, IPullConnectionProvider> _pullProviders;
        readonly CancellationTokenSource _tokenSource;

        public DefaultConnectionService(IEnumerable<IPullConnectionProvider> pullProviders)
        {
            _pullProviders = pullProviders.ToDictionary(t => t.Name);
            _tokenSource = new CancellationTokenSource();
        }

        public async Task PullAsync(string providerName, string providerKey)
        {
            IPullConnectionProvider provider;
            if (!_pullProviders.TryGetValue(providerName, out provider))
                throw new InvalidOperationException();

            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);
            var context = new PullConnectionContext(new UserInfo(providerKey), linkedTokenSource.Token);

            while (true)
            {
                var result = await provider.PullAsync(context);
                if (result.IsCancellationRequested)
                    break;

                context.Iteration++;
            }
        }
    }
}
