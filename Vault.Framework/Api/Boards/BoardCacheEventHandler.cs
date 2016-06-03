using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Vault.Shared;
using Vault.Shared.Events;
using System.Threading.Tasks;

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

        public async Task HandleAsync(EntityDeleted<Board> @event)
        {
            var cacheKey = string.Format(OwnerKey.FormatCacheKey(@event.Entity.OwnerId), typeof(List<Board>).FullName);
            _memoryCache.Remove(cacheKey);
        }

        public async Task HandleAsync(EntityCreated<Board> @event)
        {
            var cacheKey = string.Format(OwnerKey.FormatCacheKey(@event.Entity.OwnerId), typeof(List<Board>).FullName);
            _memoryCache.Remove(cacheKey);
        }

        public async Task HandleAsync(EntityUpdated<Board> @event)
        {
            var cacheKey = string.Format(ContentKey.FormatCacheKey(@event.Entity.Id, @event.Entity.OwnerId), typeof(Board).FullName);
            _memoryCache.Remove(cacheKey);
        }
    }
}