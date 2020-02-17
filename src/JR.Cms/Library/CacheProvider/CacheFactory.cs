
using JR.Cms.Library.CacheProvider.CacheCompoment;

namespace JR.Cms.Library.CacheProvider
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
        public static ICmsCache Singleton
        {
            get
            {
                return cacheInstance ??= new CmsCache(new CmsDependCache());
               // return cacheInstance ?? (cacheInstance = new CmsCache(new LevelDbCacheProvider()));
            }
        }
    }
}
