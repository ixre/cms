
using Ops.Cms.Cache.CacheCompoment;

namespace Ops.Cms.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class CacheFactory
    {
        private static ICmsCache _cacheInstance;

        /// <summary>
        /// 
        /// </summary>
        public static ICmsCache Sington
        {
            get
            {
                return _cacheInstance ?? (_cacheInstance = new CmsCache(new CmsDependCache()));
            }
        }
    }
}
