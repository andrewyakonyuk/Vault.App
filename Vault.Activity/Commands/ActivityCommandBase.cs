using System;
using Vault.Shared.Commands;

namespace Vault.Activity.Commands
{
    public abstract class ActivityCommandBase : ICommandContext
    {
        protected ActivityCommandBase(
            ResourceKey itemKey,
            DateTimeOffset published)
        {
            if (itemKey == null)
                throw new ArgumentNullException(nameof(itemKey));

            ItemKey = itemKey;
            Published = published;
        }

        public virtual DateTimeOffset Published { get; protected set; }
        public virtual ResourceKey ItemKey { get; protected set; }
    }
}