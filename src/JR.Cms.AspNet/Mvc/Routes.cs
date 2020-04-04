/*
 * Copyright(C) 2010-2013 OPS.CC
 * 
 * File Name	: Routes
 * Author	: Newmin (new.min@msn.com)
 * Create	: 2013/04/04 15:59:54
 * Description	:
 *
 */


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using JR.Cms.AspNet.Mvc;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Enum;

namespace JR.Cms.WebImpl.Mvc
{
    /// <summary>
    /// CMS路由设置
    /// </summary>
    public static class Routes
    {
        private static void RegisterCmsMajorRoutes(RouteCollection routes)
        {
            string defaultControllerPrefix = CmsVariables.DEFAULT_CONTROLLER_NAME; //使用别名访问cms系统action
            routes.MapRoute(
                "major_cms_controller", defaultControllerPrefix + "/{action}",
                new {controller = "CmsPkg", action = "Help"}
            );
            routes.MapRoute(
                "major_sub_path_controller", "{site}/" + defaultControllerPrefix + "/{action}",
                new {controller = "CmsPkg", action = "Help"}
            );

            //安装路由
            routes.Add("install_route", new Route("install/cms", new CmsInstallHandler()));
            //管理后台
            routes.Add("administrator_route", new Route(Settings.SYS_ADMIN_TAG, new AspNetCmsManagerHandler()));
            //WebAPI接口
            //routes.Add("web_api", new Route("web_api/{*path}", new WebApiRouteHandler()));
            routes.Add("web_api_router", new Route("web_api", new AspNetWebApiHandler()));
            routes.Add("web_api_sub_site_router", new Route("{site}/web_api", new AspNetWebApiHandler()));
        }

        /// <summary>
        /// 注册路由 
        /// </summary>
        /// <param name="routes">路由集合</param>
        /// <param name="cmsHandleType"></param>
        private static void RegisterInstalledCmsRoutes(RouteCollection routes, Type cmsHandleType)
        {
            //路由前缀，前缀+虚拟路径
            //string routePrefix = (String.IsNullOrEmpty(prefix) ? "" : prefix + "/")
            //    + (String.IsNullOrEmpty(Settings.SYS_VIRTHPATH) ? String.Empty:Settings.SYS_VIRTHPATH + "/");

            // string urlPrefix = "/" + routePrefix;
            string urlPrefix = String.Empty, routePrefix = String.Empty;

            //Cms 控制器名称,如果继承默认的Handler,则使用默认的Handler
            string cmsControllerName =
                Regex.Replace(cmsHandleType.Name, "controller$", String.Empty, RegexOptions.IgnoreCase);

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
            routes.IgnoreRoute("{staticDir}/{*pathInfo}",
                new {staticDir = "^(uploads|resources|content|static|plugins|libs|scripts|images|style|themes)$"});

            //templates路由处理(忽略静态文件)
            routes.IgnoreRoute("templates/{*pathInfo}",
                new {pathInfo = "^(.+?)\\.(jpg|jpeg|css|js|json|xml|gif|png|bmp)$"});
            routes.MapRoute("tpl_catchall", "templates/{*catchall}",
                new {controller = cmsControllerName, action = "Disallow"});


            //兼容以前插件
            //  IRouteHandler pluginHandler = new PluginRouteHandler();
            // routes.Add("plugin_sh_pl", new Route("{plugin}.sh/{*path}", pluginHandler));
            // routes.Add("plugin_sh_aspx", new Route("{plugin}.sh.aspx/{*path}", pluginHandler));

            // if (FwCtx.Mono())
            // {
            //     routes.Add("plugin_mono_sh_pl", new Route("{plugin}.sh", pluginHandler));
            //     routes.Add("plugin_mono_sh_aspx", new Route("{plugin}.sh.aspx", pluginHandler));
            // }


            //支付
            //routes.Add(new Route(routePrefix + "netpay", new CmsNetpayHandler()));


            //搜索档案
            routes.MapRoute(
                dict[UrlRulePageKeys.Search][0] + "_site", "{site}/" + dict[UrlRulePageKeys.Search][1],
                new {controller = cmsControllerName, action = "Search", p = 1}
            );


            //搜索档案
            routes.MapRoute(
                dict[UrlRulePageKeys.Search][0], dict[UrlRulePageKeys.Search][1],
                new {controller = cmsControllerName, action = "Search", p = 1}
            );

            //标签档案
            routes.MapRoute(
                dict[UrlRulePageKeys.Tag][0], dict[UrlRulePageKeys.Tag][1],
                new {controller = cmsControllerName, action = "Tag", p = 1}
            );

            //栏目档案列表
            routes.MapRoute(
                dict[UrlRulePageKeys.Category][0], dict[UrlRulePageKeys.Category][1],
                new {controller = cmsControllerName, action = "Category", page = 1}, new
                {
                    all_cate = "^(?!" +
                               CmsVariables.DEFAULT_CONTROLLER_NAME + ")((.+?)/(list_\\d+\\.html)?|([^/]+/)*[^\\.]+)$"
                }
            );

            #endregion

            //显示档案
            routes.MapRoute(
                dict[UrlRulePageKeys.Archive][0], dict[UrlRulePageKeys.Archive][1],
                new {controller = cmsControllerName, action = "Archive"}, new {all_html = "^(.+?).html$"}
            );

            //默认路由
            routes.MapRoute(
                "Default", // Route name
                routePrefix + "{controller}/{action}/{id}", // URL with parameters
                new {controller = cmsControllerName, action = "Index", id = UrlParameter.Optional} // Parameter defaults
            );


            routes.MapRoute("all_path", "{*path}", new {controller = cmsControllerName, action = "NotFound"});


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

        public static void MapRoutes(RouteCollection routes)
        {
            routes.Clear();
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //在加载cms后避免覆盖 /cms.do
            RegisterCmsMajorRoutes(routes);
            //注册CMS路由
            RegisterInstalledCmsRoutes(routes, typeof(AspNetPortalController));
        }
    }
}