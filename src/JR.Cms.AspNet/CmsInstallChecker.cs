using System;
using System.Web;

namespace JR.Cms.AspNet
{
    public static class CmsInstallChecker
    {
        // 如果没有安装,则跳转到安装地址
        public static bool Check(HttpApplication context)
        {
            if (!Cms.IsInstalled())
            {
                var path = context.Request.Path;
                if (path == "/favicon.ico")return true;
                if (path.StartsWith("/install/cms")) return true;
                if (path.IndexOf(".") != -1) return true;
                context.Response.Redirect("/install/cms");
                return false;
            }
            return true;
        }
    }
}