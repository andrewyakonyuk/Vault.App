using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vault.Shared.Domain;
using Vault.Shared.Queries;

namespace Vault.Shared.Connectors
{
    public class DefaultConnectionService : IConnectionService
    {
        readonly IDictionary<string, IPullConnectionProvider> _pullProviders;
        readonly CancellationTokenSource _tokenSource;
        readonly IQueryBuilder _queryBuilder;
        readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DefaultConnectionService(
            IEnumerable<IPullConnectionProvider> pullProviders,
            IQueryBuilder queryBuilder,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _pullProviders = pullProviders.ToDictionary(t => t.Name);
            _queryBuilder = queryBuilder;
            _unitOfWorkFactory = unitOfWorkFactory;
            _tokenSource = new CancellationTokenSource();
        }

        public async Task PullAsync(string providerName, string providerKey)
        {
            IPullConnectionProvider provider;
            if (!_pullProviders.TryGetValue(providerName, out provider))
                throw new InvalidOperationException();

            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);
            var state = await _queryBuilder.For<PullConnectionState>().With(new ConnectionStateKey(providerName, providerKey));

            state = state ?? new PullConnectionState
            {
                IsLastSucceded = true,
                Iteration = 0,
                ProviderKey = providerKey,
                ProviderName = providerName
            };


            var context = new PullConnectionContext(state, linkedTokenSource.Token);

            try
            {


                while (true)
                {
                    var result = await provider.PullAsync(context);
                    if (result.IsCancellationRequested)
                    {
                        state.IsLastSucceded = true;
                        state.Iteration = 0;
                        SaveConnectionState(state);
                        break;
                    }

                    context.Iteration++;
                }
            }
            catch (PullConnectionException)
            {
                state.IsLastSucceded = false;
                SaveConnectionState(state);
            }
        }

        void SaveConnectionState(PullConnectionState state)
        {
            using (var unitOfWork = _unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                state.LastRunTime = DateTime.UtcNow;
                unitOfWork.Save(state);
                unitOfWork.Commit();
            }
        }
    }
}
