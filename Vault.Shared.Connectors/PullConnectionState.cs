using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Domain;
using Vault.Shared.Queries;

namespace Vault.Shared.Connectors
{
    public class PullConnectionState : IEntity
    {
        public virtual int Id { get; set; }

        public virtual int Iteration { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual DateTime? LastRunTime { get; set; }

        public virtual bool IsLastSucceded { get; set; }

        public virtual string ProviderName { get; set; }
    }

    public class ConnectionStateKey : ICriterion
    {
        readonly string _providerKey;
        readonly string _providerName;

        public ConnectionStateKey(string providerName, string providerKey)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException(nameof(providerName));
            if (string.IsNullOrEmpty(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            _providerKey = providerKey;
            _providerName = providerName;
        }

        public string ProviderKey { get { return _providerKey; } }

        public string ProviderName { get { return _providerName; } }
    }
}
