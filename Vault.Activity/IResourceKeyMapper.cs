using System;
using System.Collections.Generic;

namespace Vault.Activity
{
    public interface IResourceKeyMapper
    {
        void Link(ResourceKey key, Guid modelId);

        bool TryResolveId(ResourceKey key, out Guid modelId);
    }

    public class InMemoryResourceKeyMapper : IResourceKeyMapper
    {
        readonly Dictionary<ResourceKey, Guid> _map = new Dictionary<ResourceKey, Guid>();

        public void Link(ResourceKey key, Guid modelId)
        {
            _map.Add(key, modelId);
        }

        public bool TryResolveId(ResourceKey key, out Guid modelId)
        {
            return _map.TryGetValue(key, out modelId);
        }
    }
}