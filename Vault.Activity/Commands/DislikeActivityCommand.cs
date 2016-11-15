using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Commands
{
    public abstract class DislikeActivityCommand : ReactActivityCommand
    {
        protected DislikeActivityCommand(
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(itemKey, published)
        {
        }
    }

    [Serializable]
    public class DislikeActivityCommand<TResource> : DislikeActivityCommand, IHasResource<TResource>
        where TResource : ICanBeLiked
    {
        public DislikeActivityCommand(
            TResource resource,
            ResourceKey itemKey,
            DateTimeOffset published)
            : base(itemKey, published)
        {
            Resource = resource;
        }

        public DislikeActivityCommand(ResourceKey itemKey,
            DateTimeOffset published)
            : this(default(TResource), itemKey, published)
        {
        }

        public TResource Resource { get; protected set; }
    }
}