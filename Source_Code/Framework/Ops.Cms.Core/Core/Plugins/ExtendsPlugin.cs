using System.Web;
using AtNet.DevFw.PluginKernel;
using AtNet.DevFw.PluginKernel.Web;

namespace AtNet.Cms.Core.Plugins
{

   

    /// <summary>
    /// CMS插件
    /// </summary>
    [PluginHost("扩展模块插件", "使用{module}.sh/{action}访问自定义扩展")]
    internal class ExtendsPlugin : BaseExtendPluginHost,IExtendApp
    {
        private ExtendsPlugin()
        {

        }

        internal ExtendsPlugin(PluginWebHandleProxy<HttpContext> webHandler)
            : base(webHandler)
        {

        }


        /// <summary>
        /// 扩展模块GET请求
        /// </summary>
        private event PluginHandler<CmsContext, string> OnExtendModuleRequest;

        /// <summary>
        /// 扩展模块POST请求
        /// </summary>
        private event PluginHandler<CmsContext, string> OnExtendModulePost;


        /// <summary>
        /// 扩展模块GET请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="extendName"></param>
        /// <param name="path"></param>
        /// <param name="result"></param>
        public override void ExtendModuleRequest(HttpContext context, string extendName,ref bool result)
        {
            extendName = extendName.ToLower();

            if (this.OnExtendModuleRequest != null)
            {
                this.OnExtendModuleRequest(Cms.Context, extendName, ref result);
            }

            //处理扩展
            this.WebHandler.HandleGetRequest(context, extendName, ref result);
        }


        /// <summary>
        /// 扩展模块POST请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="extendName"></param>
        /// <param name="path"></param>
        /// <param name="result"></param>
        public override void ExtendModulePost(HttpContext context, string extendName,ref bool result)
        {
            extendName = extendName.ToLower();

            if (this.OnExtendModulePost != null)
            {
                this.OnExtendModulePost(Cms.Context, extendName, ref result);
            }

            //处理扩展
            this.WebHandler.HandlePostRequest(context, extendName, ref result);
        }
    }
}
