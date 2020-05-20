using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using JR.Cms.Conf;
using JR.Cms.Web.Portal;
using JR.Stand.Abstracts.Web;
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
            if (Settings.SYS_FORCE_HTTPS || Settings.SYS_WWW_RD > 0)
            {
                var target = Utils.GetRdUrl(HttpHosting.Context.Request);
                if (target != null)
                {
                    throw new Exception(Settings.SYS_FORCE_HTTPS + "/" + target);
                }
                if (target != null)
                {
                    context.Response.AddHeader("Location", target);
                    context.Response.StatusCode = 301;
                    context.Response.End();
                    return false;
                }
            }
            return true;
        }

    }
}