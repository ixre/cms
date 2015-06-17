using AtNet.Cms.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AtNet.Cms.CacheService;
using AtNet.Cms.Domain.Interface.Models;
using AtNet.Cms.Utility;

namespace AtNet.Cms.Web.WebManager
{
    public static class CmsWebMaster
    {
        internal const string currentSiteSessionStr = "mgr_currentSite";

        internal const string cookieNameKey = "cms_sid";

        public static SiteDto CurrentManageSite
        {
            get
            {

                HttpContext context = HttpContext.Current;
                object sessSite = context.Session[currentSiteSessionStr];

                if (sessSite != null)
                {
                    return (SiteDto)sessSite;
                }

                SiteDto site = default(SiteDto);

                UserDto usr = UserState.Administrator.Current;

                if (usr != null && usr.SiteId > 0)
                {
                    site = SiteCacheManager.GetSite(usr.SiteId);
                }
                else
                {
                    //超级管理员获取站点
                    HttpCookie cookie = context.Request.Cookies[cookieNameKey];
                    if (cookie != null)
                    {
                        int siteID = 0;
                        int.TryParse(cookie.Value, out siteID);
                        if (siteID > 0)
                        {
                            site = SiteCacheManager.GetSite(siteID);
                        }
                    }
                }

                if (site.SiteId <=0)
                {
                    site = SiteCacheManager.DefaultSite;
                }

                context.Session[currentSiteSessionStr] = site;

                return site;
            }
        }
    }
}
