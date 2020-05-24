using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Core.Interface;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Safety;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Template;

namespace JR.Cms.Web.Portal.Comm
{
    /// <summary>
    /// 内容门户控制器
    /// </summary>
    public class PortalControllerHandler
    {
        private static readonly object locker = new object();

        /// <summary>
        /// 获取真实的请求地址
        /// </summary>
        /// <param name="path"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private string SubPath(String path, string appPath)
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
        /// <returns></returns>
        private bool CheckSiteUrl(ICompatibleHttpContext ctx, SiteDto site)
        {
            //　跳转到自定义地址
            if (!String.IsNullOrEmpty(site.Location))
            {
                ctx.Response.Redirect(site.Location, false); //302
                return false;
            }

            // 跳转到站点首页
            var path = ctx.Request.GetPath();
            if (!String.IsNullOrEmpty(site.AppPath) && path == "/")
            {
                ctx.Response.Redirect("/" + site.AppPath, false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 首页
        /// </summary>
        public Task Index(ICompatibleHttpContext context)
        {
            // 如果启用了静态文件缓存
            if (Settings.PERM_INDEX_CACHE_SECOND > 0)
            {
                var task = this.CheckStaticIndex(context, Settings.PERM_INDEX_CACHE_SECOND);
                if (task != null) return task;
            }

            // bool eventResult = false;
            // if (OnIndexRequest != null)
            // {
            //     OnIndexRequest(base.OutputContext, ref eventResult);
            // }
            var ctx = Cms.Context;
            SiteDto site = ctx.CurrentSite;
            // 站点站点路径
            if (!this.CheckSiteUrl(context, site)) return SafetyTask.CompletedTask;

            //检测网站状态及其缓存
            if (ctx.CheckSiteState() && ctx.CheckAndSetClientCache())
            {
                DefaultWebOutput.RenderIndex(ctx);
            }

            return SafetyTask.CompletedTask;


            //如果返回false,则执行默认输出
            // if (!eventResult)
            // {
            //     DefaultWebOutput.RenderIndex(base.OutputContext);
            // }
        }

        private Task CheckStaticIndex(ICompatibleHttpContext context, int seconds)
        {
            const string cacheKey = "site:index:cache";
            const string cacheUnixKey = "site:index:last-create";
            String html;
            // 如果非首页访问, 则使用动态的站点首页
            var req = context.Request;
            var path = context.Request.GetPath();
            var appPath = context.Request.GetApplicationPath();
            if (path.Length - 1 > appPath.Length) return null;
            // 缓存失效
            var last = Cms.Cache.GetInt(cacheUnixKey);
            var unix = TimeUtils.Unix(DateTime.Now);
            if (last < unix - seconds)
            {
#if DEBUG
                Console.WriteLine("[ cms][ Info]: update index page cache..");
#endif
                html = GenerateCache(cacheKey);
                Cms.Cache.Insert(cacheUnixKey, unix);
            }
            else
            {
                html = Cms.Cache.Get(cacheKey) as String;
                if (String.IsNullOrEmpty(html))
                {
                    html = GenerateCache(cacheKey);
                    Cms.Cache.Insert(cacheUnixKey, unix);
                }
            }

            context.Response.ContentType("text/html;charset=utf-8");
            return context.Response.WriteAsync(html);
        }

        private static string GenerateCache(string cacheKey)
        {
            String html;
            lock (locker)
            {
                ICmsPageGenerator cmsPage = new PageGeneratorObject(Cms.Context);
                html = cmsPage.GetIndex();
                Cms.Cache.Insert(cacheKey, html);
                TemplateUtils.SaveFile(html, Cms.PhysicPath + "index.html", Encoding.UTF8);
            }

            return html;
        }

        /// <summary>
        /// 检查位于根目录和root下的静态文件是否存在
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private Task CheckStaticFile(ICompatibleHttpContext context, string path)
        {
            if (path.LastIndexOf("/", StringComparison.Ordinal) == 0)
            {
                if (!File.Exists(Cms.PhysicPath + path))
                {
                    path = "root" + path;
                    if (!File.Exists(Cms.PhysicPath + path))
                    {
                        return null;
                    }
                }

                // 输出静态文件
                var bytes = File.ReadAllBytes(Cms.PhysicPath + path);
                context.Response.WriteAsync(bytes);
                return SafetyTask.CompletedTask;
            }

            return null;
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
                // 如果为"/news/",跳转到"/news"
                var pLen = path.Length;
                if (path[pLen-1] == '/')
                {
                    context.Response.StatusCode(301);
                    context.Response.AddHeader("Location",path.Substring(0, pLen-1));
                    return SafetyTask.CompletedTask;
                }

                // 验证是否为当前站点的首页
                if (path == sitePath)return this.Index(context);
                
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


                DefaultWebOutput.RenderCategory(ctx, catPath, page);
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
                //     DefaultWebOutput.RenderCategory(ctx, catPath, page);
                // }
            }

            return SafetyTask.CompletedTask;
        }
        
        /// <summary>
        /// 文档页
        /// </summary>
        /// <returns></returns>
        public Task Archive(ICompatibleHttpContext context)
        {
            context.Response.ContentType("text/html;charset=utf-8");
            var path = context.Request.GetPath();
            var task = this.CheckStaticFile(context, path);
            if (task != null) return task;
            CmsContext ctx = Cms.Context;
            //检测网站状态及其缓存
            if (ctx.CheckSiteState() && ctx.CheckAndSetClientCache())
            {
                context.Response.ContentType("text/html;charset=utf-8");
                String archivePath = this.SubPath(path, ctx.SiteAppPath);
                archivePath = archivePath.Substring(0, archivePath.LastIndexOf(".", StringComparison.Ordinal));
                DefaultWebOutput.RenderArchive(ctx, archivePath);
            }

            return SafetyTask.CompletedTask;

            /*
            bool eventResult = false;
            if (OnArchiveRequest != null)
            {
                OnArchiveRequest(ctx, archivePath, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOutput.RenderArchive(ctx, archivePath);
            }
            */
        }

    }
}