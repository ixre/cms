using System;
using System.Collections.Generic;
using JR.Cms.Library.CacheProvider;
using JR.Cms.Library.CacheProvider.CacheCompoment;
using JR.Cms.ServiceDto;

namespace JR.Cms.Library.CacheService
{
    public static class SiteCacheManager
    {
        private static SiteDto _defaultSite;

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
            return ServiceCall.Instance.SiteService.GetSiteById(siteId);
        }

        public static SiteDto GetSingleOrDefaultSite(string host, string appPath)
        {
            SiteDto dto = default;
            var siteCacheKey = string.Concat(CacheSign.Site.ToString(), "_host_", host, "_" + appPath);
            var siteId = CmsCacheFactory.Singleton.GetCachedResult(siteCacheKey, () =>
            {
                dto = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(host, appPath);
                return dto.SiteId;
            }, DateTime.Now.AddHours(24));

            if (dto.SiteId == 0) dto = ServiceCall.Instance.SiteService.GetSiteById(siteId);
            return dto;
        }

        /// <summary>
        /// 默认站点(暂管理员使用)
        /// </summary>
        public static SiteDto DefaultSite
        {
            get
            {
                if (_defaultSite.SiteId <= 0)
                {
                    var sites = ServiceCall.Instance.SiteService.GetSites();
                    if (sites.Count == 0) throw new ArgumentException("没有可用的站点!");

                    //如果找不到站点，则获取默认第一个站点
                    if (_defaultSite.SiteId <= 0) _defaultSite = sites[0];
                }

                return _defaultSite;
            }
        }
    }
}