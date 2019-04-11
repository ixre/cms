using System;
using JR.Cms.DataTransfer;
using System.Web;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.Utility;
using JR.DevFw.Web;

namespace JR.Cms.Web.WebManager
{
    public static class CmsWebMaster
    {
        internal const string CurrentSiteSessionStr = "mgr_currentSite";

        internal const string CookieNameKey = "cms_sid";

        public static SiteDto CurrentManageSite
        {
            get
            {
                HttpContext context = HttpContext.Current;
                object sessSite = context.Session[CurrentSiteSessionStr];
                if (sessSite != null)
                {
                    return (SiteDto)sessSite;
                }

                SiteDto site = default(SiteDto);
               site = GetSiteFromCookie(context, site);

                UserDto usr = UserState.Administrator.Current;
                if(usr == null) throw new Exception("access denied");
                if (site.SiteId == 0)
                {
                    Uri uri = context.Request.Url;
                    string appPath = uri.Segments.Length == 1 ? null : uri.Segments[1].Replace("/", "");
                    // get site by host name
                    site = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(WebCtx.Current.Host,appPath);

                    // get the first site for user
                    if (!usr.IsMaster && usr.Roles.GetRole(site.SiteId) == null)
                    {
                       SiteDto[] sites = ServiceCall.Instance.UserService.GetUserRelationSites(usr.Id);
                        if (sites.Length == 0)
                        {
                            throw  new Exception("no permission");
                        }
                        site = sites[0];
                    }

                    SetCurrentManageSite(context, site);
                }
               
                SetCurrentSiteToSession(context, site);
                return site;
            }
        }

        private static void SetCurrentSiteToSession(HttpContext context, SiteDto site)
        {
            context.Session[CurrentSiteSessionStr] = site;
        }

        private static SiteDto GetSiteFromCookie(HttpContext context, SiteDto site)
        {
            HttpCookie cookie = context.Request.Cookies[CookieNameKey];
            if (cookie != null)
            {
                int siteId = 0;
                int.TryParse(cookie.Value, out siteId);
                if (siteId > 0)
                {
                    try
                    {
                        site = SiteCacheManager.GetSite(siteId);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            return site;
        }

        public static void SetCurrentManageSite(HttpContext context, SiteDto value)
        {
            HttpCookie cookie = context.Request.Cookies.Get(CmsWebMaster.CookieNameKey);
            if (cookie != null)
            {
                if (value.SiteId <= 0)
                {
                    cookie.Expires = cookie.Expires.AddYears(-2);
                }
                else
                {
                    cookie.Values.Clear();
                    cookie.Value = value.SiteId.ToString();
                    cookie.Expires = DateTime.Now.AddDays(2);
                    cookie.Path = "/" + Settings.SYS_ADMIN_TAG;
                }
                context.Response.Cookies.Set(cookie);
            }
            else
            {
                cookie = new HttpCookie(CmsWebMaster.CookieNameKey, value.SiteId.ToString())
                {
                    Expires = DateTime.Now.AddDays(2),
                    Path = "/" + Settings.SYS_ADMIN_TAG
                };
                context.Response.Cookies.Add(cookie);
            }

            context.Session[CmsWebMaster.CurrentSiteSessionStr] = null;
        }
    }
}
