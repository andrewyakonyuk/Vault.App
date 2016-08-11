using System;

namespace Vault.Activity.Events
{
    /// <summary>
    /// The act of consuming audio content.
    /// </summary>
    public abstract class ListenedActivityEvent : ConsumedActivityEvent
    {
        protected ListenedActivityEvent()
        {
        }

        protected ListenedActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }
}