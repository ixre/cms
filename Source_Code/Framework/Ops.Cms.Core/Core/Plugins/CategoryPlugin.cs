using System.Web.Routing;
using Com.PluginKernel;

namespace Ops.Cms.Core.Plugins
{
    /// <summary>
    /// CMS插件
    /// </summary>
    [PluginHost("栏目插件", "")]
    public class CategoryPlugin : BasePluginHost
    {
        public CategoryPlugin()
        {
        }

        /// <summary>
        /// 当映射路由时发生
        /// </summary>
        private event PluginHandler<RouteCollection> OnRouteMapping;

        /// <summary>
        /// 栏目页请求时发生
        /// </summary>
        public event PluginHandler<CmsContext, string,int> OnCategoryPageRequest;


        /// <summary>
        /// 栏目请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public void Request(CmsContext controller, string tag,int page,ref bool result)
        {
            if (this.OnCategoryPageRequest != null)
            {
                this.OnCategoryPageRequest(controller, tag,page, ref result);
            }
        }
    }
}
