using System;

namespace Vault.Activity.Services.Connectors
{
    [Serializable]
    public class ConnectionKey : IEquatable<ConnectionKey>, ICloneable
    {
        protected ConnectionKey()
        {
        }

        public ConnectionKey(
            Guid ownerId,
            string providerName,
            string providerKey)
        {
            OwnerId = ownerId;
            ProviderName = providerName;
            ProviderKey = providerKey;
        }

        public Guid OwnerId { get; }
        public string ProviderName { get; }
        public string ProviderKey { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConnectionKey);
        }

        public virtual bool Equals(ConnectionKey other)
        {
            if (other == null)
                return false;

            return string.Equals(ProviderName, other.ProviderName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ProviderKey, other.ProviderKey, StringComparison.InvariantCulture)
                && OwnerId == other.OwnerId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = ProviderName.ToLowerInvariant().GetHashCode() * 15;
                result += ProviderKey.GetHashCode() * 15;
                result += OwnerId.GetHashCode() * 15;
                return result;
            }
        }

        public object Clone()
        {
            return new ConnectionKey(OwnerId, ProviderName, ProviderKey);
        }

        public override string ToString()
        {
            return string.Join("/", OwnerId.ToString("N"), ProviderName, ProviderKey);
        }
    }
}