using System;

namespace Vault.Activity.Events
{
    /// <summary>
    /// The act of ingesting information/resources/food.
    /// </summary>
    public abstract class ConsumedActivityEvent : ActivityEventBase
    {
        protected ConsumedActivityEvent()
        {
        }

        protected ConsumedActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }
}