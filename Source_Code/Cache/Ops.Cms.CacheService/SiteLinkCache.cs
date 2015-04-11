using System;

namespace AtNet.Cms.CacheService
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
            return  Kvdb.GCA.Get(siteId.ToString() + "@navigator");
        }

        public static void SetNavigatorForSite(int siteId, string cache)
        {
            Kvdb.GCA.Put(siteId.ToString() + "@navigator",cache);
        }


        public static void SetFLinkForSite(int siteId, string cache)
        {
            Kvdb.GCA.Put(siteId.ToString() + "@flink", cache);
        }

        public static string GetFLinkBySiteId(int siteId)
        {
           return Kvdb.GCA.Get(siteId.ToString() + "@flink");
        }
    }
}
