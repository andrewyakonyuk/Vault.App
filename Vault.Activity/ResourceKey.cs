using System;
using System.Diagnostics;
using Vault.Shared.Domain;
using Vault.Shared.Queries;

namespace Vault.Activity
{
    public class ResourceKey : ICriterion, IEntityComponent, IEquatable<ResourceKey>
    {
        protected ResourceKey()
        {
        }

        public ResourceKey(
            string resourceId,
            string resourceType,
            string serviceName,
            int ownerId)
        {
            if (string.IsNullOrEmpty(resourceId))
                throw new ArgumentException("Must be not null or empty", nameof(resourceId));
            if (string.IsNullOrEmpty(resourceType))
                throw new ArgumentNullException("Must be not null or empty", nameof(resourceType));
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentException("Must be not null or empty", nameof(serviceName));
            if (ownerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(ownerId));

            ServiceName = serviceName;
            ResourceId = resourceId;
            ResourceType = resourceType;
            OwnerId = ownerId;
        }

        public virtual string ServiceName { get; protected set; }
        public virtual string ResourceType { get; protected set; }
        public string ResourceId { get; protected set; }
        public int OwnerId { get; protected set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ResourceKey);
        }

        public virtual bool Equals(ResourceKey other)
        {
            if (other == null)
                return false;

            return string.Equals(ServiceName, other.ServiceName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ResourceType, ResourceType, StringComparison.OrdinalIgnoreCase)
                && ResourceId == other.ResourceId
                && OwnerId == other.OwnerId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = ResourceId.GetHashCode() * 12;
                hashcode = hashcode + ResourceType.ToLowerInvariant().GetHashCode() * 12;
                hashcode = hashcode + ServiceName.ToLowerInvariant().GetHashCode() * 12;
                hashcode = hashcode + OwnerId.GetHashCode() * 12;
                return hashcode;
            }
        }

        public override string ToString()
        {
            return string.Join("/", OwnerId, ResourceType, ServiceName, ResourceId);
        }

        public static explicit operator string(ResourceKey key)
        {
            return key.ToString();
        }
    }
}