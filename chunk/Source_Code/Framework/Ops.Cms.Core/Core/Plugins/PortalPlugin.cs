using System.Web.Routing;
using AtNet.DevFw.PluginKernel;

namespace AtNet.Cms.Core.Plugins
{
    /// <summary>
    /// CMS插件
    /// </summary>
    [PluginHost("CMS入口插件", "")]
    public class PortalPlugin : BasePluginHost
    {
        public PortalPlugin()
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
        private void MapRoutes(RouteCollection routes)
        {
            bool result=false;
            if (this.OnRouteMapping != null)
            {
                this.OnRouteMapping(routes,ref result);
            }
        }


        public event PluginHandler<CmsContext> OnPortalRequest;


        public event PluginHandler<CmsContext,string,string> OnSearchPageRequest;


        public event PluginHandler<CmsContext,string> OnTagPageRequest;
        


        /// <summary>
        /// 入口请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public void PortalRequest(CmsContext context, ref bool result)
        {
            if (this.OnPortalRequest != null)
            {
                this.OnPortalRequest(context, ref result);
            }
        }


        /// <summary>
        /// 入口请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public void SearchRequest(CmsContext context,string cate,string word, ref bool result)
        {
            if (this.OnSearchPageRequest != null)
            {
                this.OnSearchPageRequest(context,cate,word, ref result);
            }
        }

        /// <summary>
        /// 入口请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public void TagRequest(CmsContext context, string tag, ref bool result)
        {
            if (this.OnTagPageRequest != null)
            {
                this.OnTagPageRequest(context,tag, ref result);
            }
        }
        
    }
}
