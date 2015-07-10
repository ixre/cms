using J6.Cms.Infrastructure.KV;

namespace J6.Cms.CacheService
{
    public static class Kvdb
    {
        /// <summary>
        /// 全局缓存
        /// </summary>
        public static LevelDb GCA
        {
            get { return Infrastructure.Kvdb._currentInstance;}
        }
    }
}
