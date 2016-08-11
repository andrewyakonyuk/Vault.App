using System;
using Vault.Shared.EventSourcing;

namespace Vault.Activity.Events
{
    public abstract class ActivityEventBase : IEvent
    {
        protected ActivityEventBase()
        {
        }

        protected ActivityEventBase(
            Guid id,
            ResourceKey itemKey,
            DateTimeOffset published)
        {
            if (itemKey == null)
                throw new ArgumentNullException(nameof(itemKey));

            Id = id;
            ItemKey = itemKey;
            Published = published;
        }

        public virtual Guid Id { get; protected set; }

        public virtual ResourceKey ItemKey { get; protected set; }

        public virtual DateTimeOffset Published { get; protected set; }
    }
}