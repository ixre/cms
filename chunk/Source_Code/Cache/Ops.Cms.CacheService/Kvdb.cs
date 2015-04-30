using AtNet.Cms.Infrastructure.KV;

namespace AtNet.Cms.CacheService
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
