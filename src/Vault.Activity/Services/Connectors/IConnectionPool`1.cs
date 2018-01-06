using System;
using System.Collections.Generic;
using System.Linq;

namespace Vault.Activity.Services.Connectors
{
    public interface IConnectionPool<TConnection>
        where TConnection : IConnectionProvider
    {
        TConnection GetByName(string providerName);

        void Release(TConnection connection);
    }

    public class DefaultConnectionPool<TConnection> : IConnectionPool<TConnection>
        where TConnection : class, IConnectionProvider
    {
        readonly IDictionary<string, TConnection> _providers;

        public DefaultConnectionPool(IEnumerable<TConnection> providers)
        {
            _providers = providers.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
        }

        public TConnection GetByName(string providerName)
        {
            TConnection result;
            return _providers.TryGetValue(providerName, out result) ? result : null;
        }

        public void Release(TConnection provider)
        {
            //todo: (provider as IDisposable)?.Dispose();
        }
    }
}