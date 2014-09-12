using System.Web;
using Com.PluginKernel;
using Com.PluginKernel.Web;
using System.Text.RegularExpressions;
using System;

namespace Ops.Cms.Core.Plugins
{

    /// <summary>
    /// 插件应用
    /// </summary>
    public interface IExtendApp : IExtendPluginHost
    {
    }

    /// <summary>
    /// CMS插件
    /// </summary>
    [PluginHost("扩展模块插件", "使用{module}.sh/{action}访问自定义扩展")]
    internal class ExtendsPlugin : BaseExtendPluginHost,IExtendApp
    {
        private ExtendsPlugin()
        {

        }

        internal ExtendsPlugin(PluginWebHandleProxy<HttpContext> _webHandler)
            : base(_webHandler)
        {

        }


        /// <summary>
        /// 扩展模块GET请求
        /// </summary>
        private event PluginHandler<CmsContext, string, string> OnExtendModuleRequest;

        /// <summary>
        /// 扩展模块POST请求
        /// </summary>
        private event PluginHandler<CmsContext, string, string> OnExtendModulePost;




        /// <summary>
        /// 扩展模块GET请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public override void ExtendModuleRequest(HttpContext context, string extendName, string path, ref bool result)
        {
            extendName = extendName.ToLower();

            if (this.OnExtendModuleRequest != null)
            {
                this.OnExtendModuleRequest(Cms.Context, extendName, path, ref result);
            }

            //处理扩展
            this.WebHandler.HandleGetRequest(context, extendName, ref result);
        }


        /// <summary>
        /// 扩展模块POST请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public override void ExtendModulePost(HttpContext context, string extendName, string path, ref bool result)
        {
            extendName = extendName.ToLower();

            if (this.OnExtendModulePost != null)
            {
                this.OnExtendModulePost(Cms.Context, extendName, path, ref result);
            }

            //处理扩展
            this.WebHandler.HandlePostRequest(context, extendName, ref result);
        }
    }
}
