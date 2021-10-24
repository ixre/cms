using System;

namespace JR.Stand.Core.PluginKernel.Web
{
    /// <summary>
    /// B/S插件宿主
    /// </summary>
    public interface IWebPluginHost<TContext> : IPluginHost
    {
        /// <summary>
        /// 注册扩展处理程序
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="getReqHandler">委托PluginHandler<HttpContext,string>的实例</param>
        /// <param name="postReqHandler">委托PluginHandler<HttpContext,string>的实例</param>
        /// <returns></returns>
        bool Register(IPlugin plugin, PluginHandler<TContext> getReqHandler,
            PluginHandler<TContext> postReqHandler);

        #region 调用插件

        /// <summary>
        /// 扩展模块GET请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="result"></param>
        /// <param name="context"></param>
        void HttpPluginRequest(TContext context, string pluginName, ref bool result);

        /// <summary>
        /// 扩展模块POST请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pluginName"></param>
        /// <param name="result"></param>
        void HttpPluginPost(TContext context, string pluginName, ref bool result);

        #endregion

        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <param name="requestPath"></param>
        /// <returns></returns>
        string GetRequestPath(string requestPath);

        /// <summary>
        /// 扩展插件Action请求，为插件提供Action功能
        /// </summary>
        /// <param name="t"></param>
        /// <param name="context"></param>
        /// <param name="requestPath"></param>
        /// <param name="isPostRequest"></param>
        object HandleRequestUse<T>(T t, TContext context,string requestPath, bool isPostRequest);


        /// <summary>
        /// 为自定义请求提供Action功能
        /// </summary>
        /// <param name="t"></param>
        /// <param name="context"></param>
        /// <param name="requestPath"></param>
        /// <param name="isPostRequest"></param>
        object HandleCustomRequestUse<T>(T t, TContext context, string requestPath,bool isPostRequest);
    }
}