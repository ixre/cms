
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
        private static WeakReference _siteRef;
        private static IList<SiteDto> _sites = new List<SiteDto>();



        /// <summary>
        /// 站点缓存
        /// </summary>
        public static IList<SiteDto> Sites
        {
            get
            {
                const string cacheKey = "Site_global_sites";
                if (_siteRef == null || CacheFactory.Sington.Get(cacheKey) == null)
                {
                    RebuiltSitesCache();
                    CacheFactory.Sington.Insert(cacheKey, String.Empty);
                }
                return _siteRef.Target as IList<SiteDto>;
            }
        }

        /// <summary>
        /// 重建站点缓存
        /// </summary>
        public static void RebuiltSitesCache()
        {

            //释放资源
            _siteRef = null;


            //重新赋值
            if (_sites != null)
            {
                for (int i = 0; i < _sites.Count; i++)
                {
                    _sites.Remove(_sites[i]);
                }
            }

            _sites = ServiceCall.Instance.SiteService.GetSites();

            //指定弱引用
            _siteRef = new WeakReference(_sites);

            if (OnSiteCacheBuilding != null)
            {
                OnSiteCacheBuilding();
            }
        }


        #endregion

    }
}
