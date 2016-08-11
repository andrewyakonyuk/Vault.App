using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Commands
{
    public abstract class ListenActivityCommand : ConsumeActivityCommand
    {
        protected ListenActivityCommand(
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
        }
    }

    public class ListenActivityCommand<TResource> : ListenActivityCommand, IHasResource<TResource>
      where TResource : ICanBeListened
    {
        public ListenActivityCommand(
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