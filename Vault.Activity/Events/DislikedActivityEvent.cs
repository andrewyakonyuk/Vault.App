using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Events
{
    public abstract class DislikedActivityEvent : ReactedActivityEvent
    {
        protected DislikedActivityEvent()
        {
        }

        protected DislikedActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }

    [Serializable]
    public class DislikedActivityEvent<TResource> : DislikedActivityEvent, IHasResource<TResource>
        where TResource : ICanBeLiked
    {
        protected DislikedActivityEvent()
        {
        }

        public DislikedActivityEvent(Guid id,
            TResource resource,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
            Resource = resource;
        }

        public TResource Resource { get; protected set; }
    }
}