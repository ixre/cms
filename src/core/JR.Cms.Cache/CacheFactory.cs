
using JR.Cms.Cache.CacheCompoment;

namespace JR.Cms.Cache
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
