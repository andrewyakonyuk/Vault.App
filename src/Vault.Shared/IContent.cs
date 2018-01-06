using System;
using Vault.Shared.Queries;

namespace Vault.Shared
{
    public interface IContent : IHasId
    {
        int OwnerId { get; set; }

        DateTime Published { get; set; }
    }

    public class ContentKey : ICriterion, ICacheKeyProvider
    {
        readonly int _entityId;
        readonly int _ownerId;

        public ContentKey(int entityId, int ownerId)
        {
            _entityId = entityId;
            _ownerId = ownerId;
        }

        public int EntityId { get { return _entityId; } }
        public int OwnerId { get { return _ownerId; } }

        string ICacheKeyProvider.CacheKey
        {
            get
            {
                return FormatCacheKey(_entityId, _ownerId);
            }
        }

        public static string FormatCacheKey(int entityId, int ownerId)
        {
            return string.Join(":", "ContentKey:{0}", entityId, ownerId);
        }
    }

    public class OwnerKey : ICriterion, ICacheKeyProvider
    {
        readonly int _ownerId;

        public OwnerKey(int ownerId)
        {
            _ownerId = ownerId;
        }

        public int OwnerId { get { return _ownerId; } }

        string ICacheKeyProvider.CacheKey
        {
            get
            {
                return FormatCacheKey(_ownerId);
            }
        }

        public static string FormatCacheKey(int ownerId)
        {
            return string.Join(":", "OwnerKey:{0}", ownerId);
        }
    }
}