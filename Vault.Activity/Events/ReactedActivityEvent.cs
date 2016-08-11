using System;

namespace Vault.Activity.Events
{
    public abstract class ReactedActivityEvent : ActivityEventBase
    {
        protected ReactedActivityEvent()
        {
        }

        protected ReactedActivityEvent(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(id, itemKey, published)
        {
        }
    }
}