using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Events
{
    public abstract class LikedActivityEvent : ReactedActivityEvent
    {
        protected LikedActivityEvent()
        {
        }

        protected LikedActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }

    public class LikedActivityEvent<TResource> : LikedActivityEvent, IHasResource<TResource>
        where TResource : ICanBeLiked
    {
        protected LikedActivityEvent()
        {
        }

        public LikedActivityEvent(Guid id,
            TResource resource,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
            Resource = resource;
        }

        public LikedActivityEvent(Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : this(id, default(TResource), itemKey, published)
        {
        }

        public TResource Resource { get; protected set; }
    }
}