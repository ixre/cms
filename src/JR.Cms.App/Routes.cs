using System;
using System.Collections.Generic;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Web.Portal.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace JR.Cms.App
{
    /// <summary>
    /// CMS路由设置
    /// </summary>
    public static class Routes
    {
        private static readonly CmsPkgController cmsPKG = new CmsPkgController();
        public static void UseCmsRoutes(this IApplicationBuilder app)
        {
            // https://aregcode.com/blog/2019/dotnetcore-understanding-aspnet-endpoint-routing/
            app.UseEndpoints(endpoints =>
            {
                MapCmsRoutes(endpoints);
                RegisterInstalledCmsRoutes(endpoints, typeof(PortalController));
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }

        /// <summary>
        ///  映射CMS内置的路由
        /// </summary>
        /// <param name="endpoints"></param>
        private static void MapCmsRoutes(IEndpointRouteBuilder endpoints)
        {
            string defaultControllerPrefix = CmsVariables.DEFAULT_CONTROLLER_NAME; //使用别名访问cms系统action
            // endpoints.Map(defaultControllerPrefix+"/version", cmsProcessor.Version);
            var defaults = new { controller = "CmsPkg", action = "Version" };
            endpoints.MapControllerRoute("cms_controller", defaultControllerPrefix + "/{action}", defaults);
            endpoints.MapControllerRoute("cms_sub_path_controller",
                "{site}/" + defaultControllerPrefix + "/{action}", defaults);
            // 安装路由
            endpoints.Map("install", new CmsInstallHandler().ProcessRequest);
            // 管理后台
            endpoints.Map(Settings.SYS_ADMIN_TAG, new CmsManagerHandler().ProcessRequest);
            // WebAPI接口
            endpoints.Map("webapi", new WebApiRouteHandler().ProcessRequest);
            endpoints.Map("{site}/webapi", new WebApiRouteHandler().ProcessRequest);
            // 支付
            //routes.Add(new Route(routePrefix + "netpay", new CmsNetpayHandler()));

        }

        /// <summary>
        /// 注册路由 
        /// </summary>
        private static void RegisterInstalledCmsRoutes(IEndpointRouteBuilder endpoints, Type portalType)
        {
            var portal = new PortalController();
            //路由前缀，前缀+虚拟路径
            //string routePrefix = (String.IsNullOrEmpty(prefix) ? "" : prefix + "/")
            //    + (String.IsNullOrEmpty(Settings.SYS_VIRTHPATH) ? String.Empty:Settings.SYS_VIRTHPATH + "/");

            // string urlPrefix = "/" + routePrefix;
            var urlPrefix = String.Empty;

            //MVC路由规则词典
            IDictionary<UrlRulePageKeys, string> dict = new Dictionary<UrlRulePageKeys, string>();

            dict.Add(UrlRulePageKeys.Common, urlPrefix + "{0}");

            dict.Add(UrlRulePageKeys.Search, urlPrefix + "search?keyword={0}&cate={1}");
            dict.Add(UrlRulePageKeys.SearchPager, urlPrefix + "search?keyword={0}&cate={1}&page={2}");

            dict.Add(UrlRulePageKeys.Tag, urlPrefix + "tag?word={0}");
            dict.Add(UrlRulePageKeys.TagPager, urlPrefix + "tag?word={0}&page={1}");

            dict.Add(UrlRulePageKeys.Category, urlPrefix + "{0}");
            dict.Add(UrlRulePageKeys.CategoryPager, urlPrefix + "{0}/list_{1}.html");

            dict.Add(UrlRulePageKeys.Archive, urlPrefix + "{0}.html");
            dict.Add(UrlRulePageKeys.SinglePage, urlPrefix + "{0}.html");

            //注册插件路由
            //Cms.Plugins.Extends.MapRoutes(routes);

            //Cms.Plugins.MapRoutes(routes);


            #region 系统路由

            //忽略静态目录
            //routes.IgnoreRoute("{staticDir}/{*pathInfo}", new { staticDir = "^(uploads|resources|content|static|plugins|libs|scripts|images|style|themes)$" });

            //templates路由处理(忽略静态文件)
            //routes.IgnoreRoute("templates/{*pathInfo}", new { pathInfo = "^(.+?)\\.(jpg|jpeg|css|js|json|xml|gif|png|bmp)$" });
            //routes.MapRoute("tpl_catchall", "templates/{*catchall}", new { controller = cmsControllerName, action = "Disallow" });

            #endregion


            /*
            //搜索档案
            endpoints.MapControllerRoute(dict[UrlRulePageKeys.Search][0] + "_site",
                "{site}/" + dict[UrlRulePageKeys.Search][1],
                new { controller = cmsControllerName, action = "Search", p = 1 }
            );


            //搜索档案
            endpoints.MapControllerRoute(dict[UrlRulePageKeys.Search][0], 
            dict[UrlRulePageKeys.Search][1],
                new { controller = cmsControllerName, action = "Search", p = 1 }
            );

          
            
*/

            //栏目档案列表
            endpoints.MapGet("{*cate:regex(^([^/]+/)*[^\\.]+$)}", portal.Category);
            endpoints.MapGet("{*cate:regex(^(.+)/list_\\d+.html$)}", portal.Category);
            // 搜索页面
            endpoints.MapGet("/search.html", portal.Search);
            endpoints.MapGet("{site}/search.html", portal.Search);
              //标签档案
            endpoints.MapGet("/tag", portal.Tag);
            endpoints.MapGet("/{site}/tag", portal.Tag);
            // 显示档案,不包含"/list_\d.html"
            endpoints.MapGet("{*archive:regex(^((?!list_\\d+).)+.html$)}", portal.Archive);
            // 首页
            endpoints.MapGet("/", portal.Index);

            //默认路由
            // endpoints.MapControllerRoute("Default",                                                                                             
            //     routePrefix + "{controller}/{action}/{id*}",                                                       
            //     new { controller = cmsControllerName, action = "Index"}       
            // );


            //endpoints.MapControllerRoute("allpath", "{*path}", new { controller = cmsControllerName, action = "NotFound" });


            IDictionary<UrlRulePageKeys, string> urlDict = new Dictionary<UrlRulePageKeys, string>();
            foreach (KeyValuePair<UrlRulePageKeys, string> p in dict)
            {
                urlDict.Add(p.Key, p.Value);
            }

            //设置地址
            TemplateUrlRule.SetUrl(UrlRuleType.Mvc, urlDict);

            //使用MVC
            TemplateUrlRule.SetRule(UrlRuleType.Mvc);

        }
    }
}