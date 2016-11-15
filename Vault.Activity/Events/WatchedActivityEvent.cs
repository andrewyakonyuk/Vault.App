using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Events
{
    /// <summary>
    /// The act of consuming dynamic/moving visual content.
    /// </summary>
    public abstract class WatchedActivityEvent : ConsumedActivityEvent
    {
        protected WatchedActivityEvent()
        {
        }

        public WatchedActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }

    /// <summary>
    /// The act of consuming dynamic/moving visual content.
    /// </summary>
    [Serializable]
    public class WatchedActivityEvent<TResource> : WatchedActivityEvent, IHasResource<TResource>
        where TResource : ICanBeWatched
    {
        protected WatchedActivityEvent()
        {
        }

        public WatchedActivityEvent(
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