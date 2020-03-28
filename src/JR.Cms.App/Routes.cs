using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.WebImpl.Mvc.Controllers;
using JR.Stand.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace JR.Cms.App
{
    /// <summary>
    /// CMS路由设置
    /// </summary>
    public static class Routes
    {
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
            var defaults = new {controller = "Software", action = "Version"};
            endpoints.MapControllerRoute("cms_controller", defaultControllerPrefix + "/{action}", defaults);
            endpoints.MapControllerRoute("cms_sub_path_controller",
                "{site}/" + defaultControllerPrefix + "/{action}", defaults);
            // 安装路由
            endpoints.Map("install", new CmsInstallHandler().ProcessRequest);
            // 管理后台
            endpoints.Map(Settings.SYS_ADMIN_TAG, new CmsManagerHandler().ProcessRequest);
            // WebAPI接口
            endpoints.Map("web_api", new WebApiRouteHandler().ProcessRequest);
            endpoints.Map("{site}/web_api", new WebApiRouteHandler().ProcessRequest);
            // 支付
            //routes.Add(new Route(routePrefix + "netpay", new CmsNetpayHandler()));
            
        }

        /// <summary>
        /// 注册路由 
        /// </summary>
        /// <param name="routes">路由集合</param>
        /// <param name="cmsHandleType"></param>
        private static void RegisterInstalledCmsRoutes(IEndpointRouteBuilder endpoints, Type portalType)
        {
            var portal = new PortalController();
            //路由前缀，前缀+虚拟路径
            //string routePrefix = (String.IsNullOrEmpty(prefix) ? "" : prefix + "/")
            //    + (String.IsNullOrEmpty(Settings.SYS_VIRTHPATH) ? String.Empty:Settings.SYS_VIRTHPATH + "/");

            // string urlPrefix = "/" + routePrefix;
            var urlPrefix = String.Empty;
            var routePrefix = String.Empty;

            //Cms 控制器名称,如果继承默认的Handler,则使用默认的Handler
            var cmsControllerName =
                Regex.Replace(portalType.Name, "controller$", String.Empty, RegexOptions.IgnoreCase);

            //MVC路由规则词典
            IDictionary<UrlRulePageKeys, string[]> dict = new Dictionary<UrlRulePageKeys, string[]>();

            dict.Add(UrlRulePageKeys.Common, new[] {"cms_common", routePrefix + "{0}", urlPrefix + "{0}"});

            dict.Add(UrlRulePageKeys.Search,
                new[] {"cms_search", routePrefix + "search", urlPrefix + "search?w={0}&c={1}"});
            dict.Add(UrlRulePageKeys.SearchPager, new[] {null, null, urlPrefix + "search?w={0}&c={1}&p={2}"});

            dict.Add(UrlRulePageKeys.Tag, new[] {"cms_tag", routePrefix + "tag", urlPrefix + "tag?t={0}"});
            dict.Add(UrlRulePageKeys.TagPager, new[] {null, null, urlPrefix + "tag?t={0}&p={1}"});

            dict.Add(UrlRulePageKeys.Category, new[] {"cms_category", routePrefix + "{*all_cate}", urlPrefix + "{0}/"});
            dict.Add(UrlRulePageKeys.CategoryPager, new[] {null, null, urlPrefix + "{0}/list_{1}.html"});

            dict.Add(UrlRulePageKeys.Archive,
                new[] {"cms_archive", routePrefix + "{*all_html}", urlPrefix + "{0}/{1}.html"});
            dict.Add(UrlRulePageKeys.SinglePage, new[] {null, null, urlPrefix + "{0}.html"});

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

            //标签档案
            endpoints.MapControllerRoute(dict[UrlRulePageKeys.Tag][0], 
                dict[UrlRulePageKeys.Tag][1],
                new { controller = cmsControllerName, action = "Tag", p = 1 }
            );
            
            //栏目档案列表
            endpoints.MapControllerRoute(dict[UrlRulePageKeys.Category][0], 
                dict[UrlRulePageKeys.Category][1],
                new { controller = cmsControllerName, action = "Category", page = 1 }, 
                new { all_cate = "^(?!" +CmsVariables.DEFAULT_CONTROLLER_NAME+ ")((.+?)/(p\\d+\\.html)?|([^/]+/)*[^\\.]+)$" }
            );

*/
    
            //栏目档案列表
            endpoints.MapGet("{*cate:regex(^([^/]+/)*[^\\.]+$)}", portal.Category);
            endpoints.MapGet("{*cate:regex(^(.+)/list_\\d+.html$)}", portal.Category);

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
            foreach (KeyValuePair<UrlRulePageKeys, string[]> p in dict)
            {
                urlDict.Add(p.Key, p.Value[2]);
            }

            //设置地址
            TemplateUrlRule.SetUrl(UrlRuleType.Mvc, urlDict);

            //使用MVC
            TemplateUrlRule.SetRule(UrlRuleType.Mvc);
            
        }
    }
}