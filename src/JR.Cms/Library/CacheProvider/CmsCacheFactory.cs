using JR.Cms.Library.CacheProvider.CacheCompoment;
using JR.Stand.Core.Utils;

namespace JR.Cms.Library.CacheProvider
{
    /// <summary>
    /// 缓存工厂
    /// </summary>
    public static class CmsCacheFactory
    {
        private static ICmsCache cacheInstance;

        /// <summary>
        /// 获取缓存实例
        /// </summary>
        public static ICmsCache Singleton => cacheInstance;


        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cache">缓存包装器</param>
        public static void Configure(IMemoryCacheWrapper cache)
        {
            cacheInstance = new CmsDependCache(cache);
        }
    }
}