using System;
using System.Threading.Tasks;
using JR.Cms.Core;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.WebImpl.Mvc.Controllers
{
    /// <summary>
    /// 内容门户控制器
    /// </summary>
    public class PortalControllerHandler
    {
        /// <summary>
        /// 获取真实的请求地址
        /// </summary>
        /// <param name="path"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private string SubPath(String path,string appPath)
        {
            int len = appPath.Length;
            if (len > 2)
            {
                if (path.Length < len) return "";
                return path.Substring(len);
            }
            return path;
        }

        /// <summary>
        /// 检查站点首页路径
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="site"></param>
        /// <param name="siteDto"></param>
        /// <returns></returns>
        private bool CheckSiteUrl(ICompatibleHttpContext ctx, SiteDto site)
        {
            //　跳转到自定义地址
            if (!String.IsNullOrEmpty(site.Location))
            {
                ctx.Response.Redirect(site.Location,false);  //302
                return false;
            }
            // 跳转到站点首页
            var path = ctx.Request.GetPath();
            if (!String.IsNullOrEmpty(site.AppPath) &&  path == "/")
            {
                ctx.Response.Redirect("/" + site.AppPath,false);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 首页
        /// </summary>
        public Task Index(ICompatibleHttpContext context)
        {
            // bool eventResult = false;
            // if (OnIndexRequest != null)
            // {
            //     OnIndexRequest(base.OutputContext, ref eventResult);
            // }
            var ctx = Cms.Context;
            SiteDto site = ctx.CurrentSite;
            // 站点站点路径
            if (!this.CheckSiteUrl(context,site)) return Task.CompletedTask;
            //检测网站状态及其缓存
            if (ctx.CheckSiteState() && ctx.CheckAndSetClientCache())
            {          
                DefaultWebOuput.RenderIndex(ctx);
            }
            return Task.CompletedTask;

            //如果返回false,则执行默认输出
            // if (!eventResult)
            // {
            //     DefaultWebOuput.RenderIndex(base.OutputContext);
            // }
        }
        
        /// <summary>
        /// 文档页
        /// </summary>
        /// <returns></returns>
        public Task Archive(ICompatibleHttpContext context)
        {           
            context.Response.ContentType("text/html;charset=utf-8");
            CmsContext ctx = Cms.Context;
            //检测网站状态及其缓存
            if (ctx.CheckSiteState() && ctx.CheckAndSetClientCache())
            {
                context.Response.ContentType("text/html;charset=utf-8");
                var path = context.Request.GetPath();
                String archivePath = this.SubPath(path, ctx.SiteAppPath);
                archivePath = archivePath.Substring(0, archivePath.LastIndexOf(".", StringComparison.Ordinal));
                DefaultWebOuput.RenderArchive(ctx, archivePath);
            }
            return Task.CompletedTask;
            /*
            bool eventResult = false;
            if (OnArchiveRequest != null)
            {
                OnArchiveRequest(ctx, archivePath, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderArchive(ctx, archivePath);
            }
            */
        }

        /// <summary>
        /// 栏目页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Category(ICompatibleHttpContext context)
        {
            context.Response.ContentType("text/html;charset=utf-8");
            CmsContext ctx = Cms.Context;
            //检测网站状态及其缓存
            if (ctx.CheckSiteState() && ctx.CheckAndSetClientCache())
            {
                var path = context.Request.GetPath();
                var sitePath = ctx.SiteAppPath;
                // 验证是否为当前站点的首页
                if (path ==sitePath )
                {
                    return this.Index(context);
                }
                // 如果为"/site/",跳转到"/site"
                if (path == sitePath+"/")
                {
                    context.Response.Redirect(path.Substring(0, path.Length - 1), false);
                    return Task.CompletedTask;
                }
                String catPath = this.SubPath(path, sitePath);
                int page = 1;
                //获取页码和tag
                if (catPath.EndsWith(".html"))
                {
                    var ls = catPath.LastIndexOf("/", StringComparison.Ordinal);
                    var len = catPath.Length;
                    var begin = ls + 1 + "list_".Length;
                    var ps = catPath.Substring(begin, len - begin - 5);
                    int.TryParse(ps, out page);
                    catPath = catPath.Substring(0, ls);
                }
         
           
                DefaultWebOuput.RenderCategory(ctx, catPath, page);
                // //执行
                // bool eventResult = false;
                // if (OnCategoryRequest != null)
                // {
                //     OnCategoryRequest(ctx, catPath, page, ref eventResult);
                // }
                //
                // //如果返回false,则执行默认输出
                // if (!eventResult)
                // {
                //     DefaultWebOuput.RenderCategory(ctx, catPath, page);
                // }
            }
            return Task.CompletedTask;

        }
    }
}