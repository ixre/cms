using System;
using System.Web;
using JR.Cms.Conf;

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
                if (path == "/favicon.ico")return true;
                if (path.StartsWith("/install/cms")) return true;
                if (path.IndexOf(".", StringComparison.Ordinal) != -1) return true;
                context.Response.Redirect("/install/cms",true);
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
            if (Settings.SYS_WWW_RD)
            {
                var url = context.Request.Url ?? throw new ArgumentNullException("context.Request.GetEncodedUrl()");
                var host = context.Request.Url.Host;
                if (host.Split('.').Length == 2)
                {
                    context.Response.Redirect(url.ToString().Replace(host, "www." + host));
                    context.Response.End();
                    return false;
                }
            }

            return true;
        }
    }
}