using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Vault.Shared.Events;

namespace Vault.Framework.Api.Boards
{
    public class BoardCacheEventHandler :
        IHandle<EntityUpdated<Board>>,
        IHandle<EntityCreated<Board>>,
        IHandle<EntityDeleted<Board>>
    {
        readonly IMemoryCache _memoryCache;

        public BoardCacheEventHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Handle(EntityDeleted<Board> @event)
        {
            var cacheKey = string.Format(OwnerKey.FormatCacheKey(@event.Entity.OwnerId), typeof(List<Board>).FullName);
            _memoryCache.Remove(cacheKey);
        }

        public void Handle(EntityCreated<Board> @event)
        {
            var cacheKey = string.Format(OwnerKey.FormatCacheKey(@event.Entity.OwnerId), typeof(List<Board>).FullName);
            _memoryCache.Remove(cacheKey);
        }

        public void Handle(EntityUpdated<Board> @event)
        {
            var cacheKey = string.Format(ContentKey.FormatCacheKey(@event.Entity.Id, @event.Entity.OwnerId), typeof(Board).FullName);
            _memoryCache.Remove(cacheKey);
        }
    }
}