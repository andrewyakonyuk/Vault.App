using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Commands
{
    public abstract class ReadActivityCommand : ConsumeActivityCommand
    {
        protected ReadActivityCommand(
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
        }
    }

    public class ReadActivityCommand<TResource> : ReadActivityCommand, IHasResource<TResource>
        where TResource : ICanBeRead
    {
        public ReadActivityCommand(
            TResource resource,
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
            Resource = resource;
        }

        public TResource Resource { get; protected set; }
    }
}