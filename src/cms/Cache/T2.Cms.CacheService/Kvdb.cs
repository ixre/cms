using T2.Cms.Infrastructure.KV;

namespace T2.Cms.CacheService
{
    public static class Kvdb
    {
        /// <summary>
        /// 全局缓存
        /// </summary>
        public static LevelDb Gca
        {
            get { return Infrastructure.Kvdb._currentInstance;}
        }
    }
}
