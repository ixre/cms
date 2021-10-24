using JR.Cms.Conf;
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
            app.Use(async (context, next) =>
            {
                if (Cms.IsStaticRequest(context.Request.Path)) await next();
                var redirect = false;
                if (Settings.SYS_FORCE_HTTPS || Settings.SYS_WWW_RD > 0)
                {
                    var target = Utils.GetRdUrl(HttpHosting.Context.Request);
                    if (target != null)
                    {
                        context.Response.StatusCode = 301;
                        context.Response.Headers.Add("Location", target);
                        redirect = true;
                    }
                }

                if (!redirect) await next();
            });
        }
    }
}