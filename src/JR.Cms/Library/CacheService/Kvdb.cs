using JR.Cms.Infrastructure.KV;

namespace JR.Cms.Library.CacheService
{
    public static class Kvdb
    {
        /// <summary>
        /// 全局缓存
        /// </summary>
        public static MicroKvStorage Gca => Infrastructure.Kvdb._currentInstance;
    }
}