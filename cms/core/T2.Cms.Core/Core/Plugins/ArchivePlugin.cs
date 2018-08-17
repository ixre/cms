using System;
using System.Web.Routing;
using JR.DevFw.PluginKernel;

namespace T2.Cms.Core.Plugins
{
    /// <summary>
    /// CMS插件
    /// </summary>
    [PluginHost("文档插件","")]
    public class ArchivePlugin : BasePluginHost
    {
        public ArchivePlugin()
        {
        }

        /// <summary>
        /// 当映射路由时发生
        /// </summary>
        private event PluginHandler<RouteCollection> OnRouteMapping;

        /// <summary>
        /// 映射路由
        /// </summary>
        /// <param name="routes"></param>
        //private void MapRoutes(RouteCollection routes)
        //{
        //    bool result=false;
        //    if (this.OnRouteMapping != null)
        //    {
        //        this.OnRouteMapping(routes,ref result);
        //    }
        //}

        /// <summary>
        /// 文档页Get请求时发生
        /// </summary>
        public event PluginHandler<CmsContext, String> OnArchivePageRequest;

        /// <summary>
        /// 文档页Post请求时发生
        /// </summary>
        public event PluginHandler<CmsContext, String> OnArchivePagePost;


        /// <summary>
        /// 栏目请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public void Request(CmsContext controller, string allhtml, ref bool result)
        {
            if (this.OnArchivePageRequest != null)
            {
                this.OnArchivePageRequest(controller, allhtml, ref result);
            }
        }


        /// <summary>
        /// 栏目请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public void PostRequest(CmsContext controller, string allhtml, ref bool result)
        {
            if (this.OnArchivePagePost != null)
            {
                this.OnArchivePagePost(controller, allhtml, ref result);
            }
        }
    }
}
