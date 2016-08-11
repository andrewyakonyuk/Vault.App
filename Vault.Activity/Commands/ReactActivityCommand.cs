using System;

namespace Vault.Activity.Commands
{
    public abstract class ReactActivityCommand : ActivityCommandBase
    {
        protected ReactActivityCommand(
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(itemKey, published)
        {
        }
    }
}