using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Events
{
    /// <summary>
    /// The act of consuming static visual content.
    /// </summary>
    public abstract class ViewedActivityEvent : ConsumedActivityEvent
    {
        protected ViewedActivityEvent()
        {
        }

        public ViewedActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }

    /// <summary>
    /// The act of consuming static visual content.
    /// </summary>
    [Serializable]
    public class ViewedActivityEvent<TResource> : ViewedActivityEvent, IHasResource<TResource>
        where TResource : ICanBeViewed
    {
        protected ViewedActivityEvent()
        {
        }

        public ViewedActivityEvent(
            Guid id,
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