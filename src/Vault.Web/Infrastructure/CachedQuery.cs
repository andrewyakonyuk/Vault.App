using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Vault.Shared;
using Vault.Shared.Queries;

namespace Vault.WebApp.Infrastructure
{
    public sealed class CachedQuery<TCriterion, TResult> : IQuery<TCriterion, TResult>
        where TCriterion : ICriterion
    {
        readonly IQuery<TCriterion, TResult> _innerQuery;
        readonly IMemoryCache _memoryCache;

        public CachedQuery(
            IQuery<TCriterion, TResult> innerQuery,
            IMemoryCache memoryCache)
        {
            if (innerQuery == null)
                throw new ArgumentNullException(nameof(innerQuery));
            if (memoryCache == null)
                throw new ArgumentNullException(nameof(memoryCache));

            _innerQuery = innerQuery;
            _memoryCache = memoryCache;
        }

        public async Task<TResult> AskAsync(TCriterion criterion)
        {
            if (ReferenceEquals(criterion, null))
                throw new ArgumentNullException(nameof(criterion));

            var cacheKeyProvider = criterion as ICacheKeyProvider;
            if (cacheKeyProvider == null)
            {
                //if criterion does not implement ICacheKeyProvider interface, then continue without caching
                return await _innerQuery.AskAsync(criterion);
            }
            else {
                // the single criterion might be used for the different result types
                var cacheKey = string.Format(cacheKeyProvider.CacheKey, typeof(TResult).FullName);

                TResult result;
                if (_memoryCache.TryGetValue(cacheKey, out result))
                {
                    //todo: cache by reference but must be by value
                    //var cloneable = result as ICloneable;
                    //if (cloneable != null)
                    //{
                    //    result = (TResult)cloneable.Clone();
                    //}
                }
                else {
                    result = await _innerQuery.AskAsync(criterion);
                    _memoryCache.Set(cacheKey, result);
                }

                return result;
            }
        }
    }
}