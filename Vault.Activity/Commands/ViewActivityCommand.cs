using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Commands
{
    public abstract class ViewActivityCommand : ConsumeActivityCommand
    {
        protected ViewActivityCommand(
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
        }
    }

    [Serializable]
    public class ViewActivityCommand<TResource> : ViewActivityCommand, IHasResource<TResource>
       where TResource : ICanBeViewed
    {
        public ViewActivityCommand(
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