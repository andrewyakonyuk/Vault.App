using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Events
{
    /// <summary>
    /// The act of consuming written content.
    /// </summary>
    public abstract class ReadActivityEvent : ConsumedActivityEvent
    {
        protected ReadActivityEvent()
        {
        }

        protected ReadActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }

    /// <summary>
    /// The act of consuming written content.
    /// </summary>
    public class ReadActivityEvent<TResource> : ReadActivityEvent, IHasResource<TResource>
        where TResource : ICanBeRead
    {
        protected ReadActivityEvent()
        {
        }

        public ReadActivityEvent(Guid id,
            TResource resource,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
            Resource = resource;
        }

        public TResource Resource { get; set; }
    }
}