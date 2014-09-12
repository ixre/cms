/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/20
 * 时间: 19:45
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

using System.Web.Routing;
using Spc;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Ops.Cms;
using Ops.Cms.Web;
using Com.PluginKernel;
using System.Globalization;

namespace Ops.Cms
{
    /// <summary>
    /// Description of CmsPluginExtensions.
    /// </summary>
    public static class CmsPluginExtensions
    {

        /// <summary>
        /// 注册插件路由
        /// </summary>
        /// <param name="aliasName">别名</param>
        /// <param name="extendName">扩展名</param>
        public static void MapExtendPluginRoute(this CmsPluginContext table, string aliasName, string extendName)
        {
            RouteValueDictionary routeValue = new RouteValueDictionary();
            routeValue.Add("extend", extendName);

            RouteTable.Routes.Add("extend_sh_" + aliasName,
                                  new Route(aliasName + "/{*path}",
                                  routeValue,
                                  new PluginExtendRouteHandler()));

            if (Cms.RunAtMono)
            {
                RouteTable.Routes.Add("mono_extend_sh_" + aliasName,
                                  new Route(aliasName,
                                  routeValue,
                                  new PluginExtendRouteHandler()));
            }
        }

        /// <summary>
        /// 注册插件路由
        /// </summary>
        /// <param name="extendName">扩展名</param>
        public static void MapExtendPluginRoute(this CmsPluginContext table, IPlugin plugin)
        {
            string aliasName;
            string extendName;

            PluginPackAttribute attr = PluginUtil.GetAttribute(plugin);
            extendName = attr.WorkIndent;
            aliasName = attr.Settings["override.url.indent"];

            //throw new Exception(extendName + "/" + aliasName);
            if (aliasName == ""
                || String.Compare(aliasName,
                extendName, true,
                CultureInfo.InvariantCulture) == 0)
                return;
            MapExtendPluginRoute(table, aliasName, extendName);
        }
    }
}
