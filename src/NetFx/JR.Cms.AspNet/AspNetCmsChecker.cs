using System;
using System.Web;
using JR.Cms.Conf;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Portal;
using JR.Stand.Core.Web;

namespace JR.Cms.AspNet
{
    /// <summary>
    /// 
    /// </summary>
    public static class AspNetCmsChecker
    {
        // 如果没有安装,则跳转到安装地址
        public static bool Check(HttpContextBase context)
        {
            if (!Cms.IsInstalled())
            {
                var path = context.Request.Path;
                if (path == "/favicon.ico") return true;
                if (path.StartsWith("/install/cms")) return true;
                if (path.IndexOf(".", StringComparison.Ordinal) != -1) return true;
                context.Response.Redirect("/install/cms", true);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 自动跳转到www开头的域名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Check301Redirect(HttpContextBase context)
        {
            // 自动跳转到www开头的域名
            String boardPath = "/admin";
            if (!String.IsNullOrEmpty(Settings.SYS_ADMIN_TAG))
            {
                boardPath = "/" + Settings.SYS_ADMIN_TAG;
            }
            String path = context.Request.Path;
            if (Cms.IsStaticRequest(context.Request.Path) || path.StartsWith(boardPath))
            {
                return true;
            }

            SiteDto site = Cms.Context.CurrentSite;
            var target = Utils.GetSiteRedirectUrl(HttpHosting.Context.Request,site);
            if (target != null)
            {
                context.Response.AddHeader("Location", target);
                context.Response.StatusCode = 301;
                context.Response.End();
                return false;
            }
            return true;
        }

    }
}