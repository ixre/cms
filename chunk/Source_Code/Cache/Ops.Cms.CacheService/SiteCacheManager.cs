using Ops.Cms.Cache;
using Ops.Cms.Cache.CacheCompoment;
using Ops.Cms.DataTransfer;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ops.Cms.CacheService
{
    public static class SiteCacheManager 
    {
        private static SiteDto __defaultSite;
        /// <summary>
        /// 获取所有的站点
        /// </summary>
        /// <returns></returns>
        public static IList<SiteDto> GetAllSites()
        {
            return ServiceCall.Instance.SiteService.GetSites();
        }

        /// <summary>
        /// 获取站点
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static SiteDto GetSite(int siteId)
        {
            //foreach (SiteDto s in WeakRefCache.Sites)
            //{
            //    if (s.SiteId == siteID) return s;
            //}
            //return null;

            return ServiceCall.Instance.SiteService.GetSiteById(siteId);
        }

        public static SiteDto GetSingleOrDefaultSite(Uri uri)
        {  
            string siteCacheKey=null;
            string hostName = uri.Host;
            string appDirName = uri.Segments.Length==1?
                null:
                uri.Segments[1].Replace("/","");

            if(appDirName!=null){
                foreach(SiteDto site in WeakRefCache.Sites){
                    if(String.Compare(site.DirName,appDirName,true,CultureInfo.InvariantCulture)==0){
                        siteCacheKey=String.Concat(CacheSign.Site.ToString(), "_host_", hostName,"_",appDirName);
                        break;
                    }
                }
            }

            if(siteCacheKey==null){
                siteCacheKey=String.Concat(CacheSign.Site.ToString(), "_host_", hostName);
            }



            return CacheFactory.Sington.GetCachedResult<SiteDto>(siteCacheKey,
                 () =>
                 {
                     SiteDto site = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(uri);
                     SiteDto dto = GetSite(site.SiteId);
                     dto.RunType = site.RunType;
                     return site;
                 });
        }

        /// <summary>
        /// 默认站点
        /// </summary>
        public static SiteDto DefaultSite
        {
            get
            {
                if (__defaultSite.SiteId<=0)
                {
                    IList<SiteDto> sites = ServiceCall.Instance.SiteService.GetSites();
                    if (sites.Count == 0)
                    {
                        throw new ArgumentException("没有可用的站点!");
                    }
                    __defaultSite = sites[0];
                }
                return __defaultSite;
            }
        }

        //public static void BuiltAllSites()
        //{
        //    WeakRefCache.RebuiltSitesCache();
        //    if (WeakRefCache.Sites.Count == 0)
        //    {
        //        throw new ArgumentException("没有可用的站点!");
        //    }
        //}
    }
}
