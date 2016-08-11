using System;

namespace Vault.Activity.Commands
{
    public abstract class ConsumeActivityCommand : ActivityCommandBase
    {
        protected ConsumeActivityCommand(
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
        }
    }
}