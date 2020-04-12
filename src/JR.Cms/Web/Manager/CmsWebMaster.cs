using System;
using JR.Cms.Conf;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace JR.Cms.Web.Manager
{
    /// <summary>
    /// 
    /// </summary>
    public static class CmsWebMaster
    {
        private const string CurrentSiteSessionStr = "mgr_currentSite";

        private const string CookieNameKey = "cms_sid";

        public static SiteDto CurrentManageSite
        {
            get
            {
                var context = HttpHosting.Context;
                var siteJSON = context.Session.GetString(CurrentSiteSessionStr);
                if (!string.IsNullOrEmpty(siteJSON)) return JsonConvert.DeserializeObject<SiteDto>(siteJSON);

                var site = default(SiteDto);
                site = GetSiteFromCookie(context, site);

                var usr = UserState.Administrator.Current;
                if (usr == null) throw new Exception("access denied");
                if (site.SiteId == 0)
                {
                    // get site by host name
                    site = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(WebCtx.Current.Host, "");
                    // get the first site for user
                    if (!usr.IsMaster && usr.Roles.GetRole(site.SiteId) == null)
                    {
                        var sites = ServiceCall.Instance.UserService.GetUserRelationSites(usr.Id);
                        if (sites.Length == 0) throw new Exception("no permission");

                        site = sites[0];
                    }

                    SetCurrentManageSite(context, site);
                }

                SetCurrentSiteToSession(context, site);
                return site;
            }
        }

        private static void SetCurrentSiteToSession(ICompatibleHttpContext context, SiteDto site)
        {
            context.Session.SetObjectAsJson(CurrentSiteSessionStr, site);
        }

        private static SiteDto GetSiteFromCookie(ICompatibleHttpContext context, SiteDto site)
        {
            if (context.Request.TryGetCookie(CookieNameKey, out var strSiteId))
            {
                var siteId = int.Parse(strSiteId);
                if (siteId > 0) site = SiteCacheManager.GetSite(siteId);
            }

            return site;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public static void SetCurrentManageSite(ICompatibleHttpContext context, SiteDto value)
        {
            var opt = new HttpCookieOptions();
            opt.Expires = DateTime.Now.AddDays(2);
            opt.Path = "/" + Settings.SYS_ADMIN_TAG;
            context.Response.AppendCookie(CookieNameKey, value.SiteId.ToString(), opt);
            context.Session.Remove(CurrentSiteSessionStr);
        }
    }
}