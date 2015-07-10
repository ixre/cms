
using System;
using System.Collections.Generic;
using J6.Cms.Cache;
using J6.Cms.Infrastructure;
using J6.Cms.DataTransfer;

namespace J6.Cms.CacheService
{
    /// <summary>
    /// 弱引用缓存
    /// </summary>
    internal class WeakRefCache
    {
        public static WatchBehavior OnSiteCacheBuilding;

        #region Site

        //栏目的弱引用,保证释放资源时回收
        private static WeakReference site_ref;
        private static IList<SiteDto> sites = new List<SiteDto>();



        /// <summary>
        /// 站点缓存
        /// </summary>
        public static IList<SiteDto> Sites
        {
            get
            {
                const string cacheKey = "Site_global_sites";
                if (site_ref == null || CacheFactory.Sington.Get(cacheKey) == null)
                {
                    RebuiltSitesCache();
                    CacheFactory.Sington.Insert(cacheKey, String.Empty);
                }
                return site_ref.Target as IList<SiteDto>;
            }
        }

        /// <summary>
        /// 重建站点缓存
        /// </summary>
        public static void RebuiltSitesCache()
        {

            //释放资源
            site_ref = null;


            //重新赋值
            if (sites != null)
            {
                for (int i = 0; i < sites.Count; i++)
                {
                    sites.Remove(sites[i]);
                }
            }

            sites = ServiceCall.Instance.SiteService.GetSites();

            //指定弱引用
            site_ref = new WeakReference(sites);

            if (OnSiteCacheBuilding != null)
            {
                OnSiteCacheBuilding();
            }
        }


        #endregion

    }
}
