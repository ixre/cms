
using T2.Cms.Cache.CacheCompoment;

namespace T2.Cms.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class CacheFactory
    {
        private static ICmsCache cacheInstance;

        /// <summary>
        /// 
        /// </summary>
        public static ICmsCache Sington
        {
            get
            {
                return cacheInstance ?? (cacheInstance = new CmsCache(new CmsDependCache()));
               // return cacheInstance ?? (cacheInstance = new CmsCache(new LevelDbCacheProvider()));
            }
        }
    }
}
