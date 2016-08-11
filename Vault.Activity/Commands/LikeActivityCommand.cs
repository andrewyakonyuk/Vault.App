using System;
using Vault.Activity.Resources;

namespace Vault.Activity.Commands
{
    public abstract class LikeActivityCommand : ReactActivityCommand
    {
        protected LikeActivityCommand(
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
        }
    }

    public class LikeActivityCommand<TResource> : LikeActivityCommand, IHasResource<TResource>
        where TResource : ICanBeLiked
    {
        public LikeActivityCommand(
            TResource resource,
            ResourceKey key,
            DateTimeOffset published)
            : base(key, published)
        {
            Resource = resource;
        }

        public LikeActivityCommand(ResourceKey key,
            DateTimeOffset published)
            : this(default(TResource), key, published)
        {
        }

        public TResource Resource { get; protected set; }
    }
}