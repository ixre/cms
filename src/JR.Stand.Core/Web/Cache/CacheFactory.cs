using JR.Stand.Abstracts;
using JR.Stand.Core.Utils;
using JR.Stand.Core.Web.Cache.Component;

namespace JR.Stand.Core.Web.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class CacheFactory
    {
        private static ICache _cacheInstance;

        /// <summary>
        /// 
        /// </summary>
        public static ICache Sington
        {
            get
            {
                return _cacheInstance ?? (_cacheInstance = new BasicCache(new DependCache(null)));
               // return cacheInstance ?? (cacheInstance = new CmsCache(new LevelDbCacheProvider()));
            }
        }

        public static void Configure(IMemoryCacheWrapper  cache)
        {
            _cacheInstance = new BasicCache(new DependCache(cache));
        }
    }
}
