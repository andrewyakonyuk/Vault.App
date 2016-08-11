using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Commands
{
    public abstract class WatchActivityCommand : ConsumeActivityCommand
    {
        protected WatchActivityCommand(
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
        }
    }

    public class WatchActivityCommand<TResource> : WatchActivityCommand, IHasResource<TResource>
      where TResource : ICanBeWatched
    {
        public WatchActivityCommand(
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