using System;
using JR.Cms.Conf;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Builder;

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
                if (Cms.IsStaticRequest(context.Request.Path)) await next(); // 如果静态资源直接输出
                if (Cms.IsInstalled() || context.Request.Path == "/install")
                {
                    await next();
                    return;
                }

                context.Response.Redirect("/install");
            });
            // 自动跳转到www开头的域名
            String boardPath = "/admin";
            if (!String.IsNullOrEmpty(Settings.SYS_ADMIN_TAG))
            {
                boardPath = "/" + Settings.SYS_ADMIN_TAG;
            }

            app.Use(async (context, next) =>
            {
                String path = context.Request.Path;
                if (Cms.IsStaticRequest(context.Request.Path) || path.StartsWith(boardPath))
                {
                    await next();
                    return;
                }

                SiteDto site = Cms.Context.CurrentSite;
                var target = Utils.GetSiteRedirectUrl(HttpHosting.Context.Request, site);
                if (target != null)
                {
                    context.Response.StatusCode = 301;
                    context.Response.Headers.Add("Location", target);
                }
                else
                {
                    await next();
                }
            });
        }
    }
}