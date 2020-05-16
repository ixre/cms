using System;
using JR.Cms.Conf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;

namespace JR.Cms.Web.Portal
{
    /// <summary>
    /// Cms Middleware
    /// </summary>
    public static class Middleware
    {
        internal static void UseCmsMiddleware(this IApplicationBuilder app)
        {
            // 如果没有安装,则跳转到安装地址
            app.Use(async (context, next) =>
            {
                if (Cms.IsInstalled() || context.Request.Path == "/install")
                {
                    await next();
                    return;
                }
                context.Response.Redirect("/install");
            });
            // 自动跳转到www开头的域名
            app.Use(async (context, next) =>
            {
                var redirect = false;
                if (Settings.SYS_AUTOWWW)
                {
                    var url = context.Request.GetEncodedUrl() ??
                              throw new ArgumentNullException("context.Request.GetEncodedUrl()");
                    var host = context.Request.Host.Host;
                    if (host.Split('.').Length == 2)
                    {
                        context.Response.Redirect(url.Replace(host, "www." + host));
                        redirect = true;
                    }
                }

                if (!redirect) await next();
            });
        }
    }
}