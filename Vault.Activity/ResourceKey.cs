using System;
using System.Diagnostics;
using Vault.Shared.Domain;
using Vault.Shared.Queries;

namespace Vault.Activity
{
    [DebuggerDisplay("{StringToDisplay()}")]
    public class ResourceKey : ICriterion, IEntityComponent, IEquatable<ResourceKey>
    {
        protected ResourceKey()
        {
        }

        public ResourceKey(string resourceId, string serviceName, int ownerId)
        {
            if (string.IsNullOrEmpty(resourceId))
                throw new ArgumentException("Must be not null or empty", nameof(resourceId));
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentException("Must be not null or empty", nameof(serviceName));
            if (ownerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(ownerId));

            ServiceName = serviceName;
            ResourceId = resourceId;
            OwnerId = ownerId;
        }

        public virtual string ServiceName { get; protected set; }
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

            return ServiceName == other.ServiceName
                && ResourceId == other.ResourceId
                && OwnerId == other.OwnerId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = ResourceId.GetHashCode() * 12;
                hashcode = hashcode + ServiceName.GetHashCode() * 12;
                hashcode = hashcode + OwnerId.GetHashCode() * 12;
                return hashcode;
            }
        }

        private string StringToDisplay()
        {
            return string.Join("/", OwnerId, ServiceName, ResourceId);
        }
    }
}