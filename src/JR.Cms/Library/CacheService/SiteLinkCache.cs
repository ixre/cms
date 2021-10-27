namespace JR.Cms.Library.CacheService
{
    public static class SiteLinkCache
    {
        public static void ClearForSite(int siteId)
        {
            Kvdb.Gca.Delete("link:navigator:" + siteId.ToString());
            Kvdb.Gca.Delete("link:flink:" + siteId.ToString());
        }

        public static string GetNavigatorBySiteId(int siteId)
        {
            return Kvdb.Gca.Get("link:navigator:" + siteId.ToString());
        }

        public static void SetNavigatorForSite(int siteId, string cache)
        {
            Kvdb.Gca.Put("link:navigator:" + siteId.ToString(), cache);
        }


        public static void SetFLinkForSite(int siteId, string cache)
        {
            Kvdb.Gca.Put("link:flink:" + siteId.ToString(), cache);
        }

        public static string GetFLinkBySiteId(int siteId)
        {
            return Kvdb.Gca.Get("link:flink:" + siteId.ToString());
        }
    }
}