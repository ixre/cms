using System;

namespace T2.Cms.CacheService
{
    public static class SiteLinkCache
    {
        public static void ClearForSite(int siteId)
        {
            SetNavigatorForSite(siteId, String.Empty);
            SetFLinkForSite(siteId, String.Empty);
        }

        public static string GetNavigatorBySiteId(int siteId)
        {
            return  Kvdb.Gca.Get(siteId.ToString() + "@navigator");
        }

        public static void SetNavigatorForSite(int siteId, string cache)
        {
            Kvdb.Gca.Put(siteId.ToString() + "@navigator",cache);
        }


        public static void SetFLinkForSite(int siteId, string cache)
        {
            Kvdb.Gca.Put(siteId.ToString() + "@flink", cache);
        }

        public static string GetFLinkBySiteId(int siteId)
        {
           return Kvdb.Gca.Get(siteId.ToString() + "@flink");
        }
    }
}
