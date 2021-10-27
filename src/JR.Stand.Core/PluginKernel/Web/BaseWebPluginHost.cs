using System;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.PluginKernel.Web
{
    /// <summary>
    /// B/S插件宿主
    /// </summary>
    [PluginHost("B/S插件宿主", "使用{module}.sh/{action}访问自定义扩展")]
    public abstract class BaseWebPluginHost<TContext> : BasePluginHost, IWebPluginHost<TContext>
    {
        protected WebPluginHandleProxy<TContext> WebHandler;

        /// <summary>
        /// 
        /// </summary>
        protected BaseWebPluginHost()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_webHandler"></param>
        protected BaseWebPluginHost(WebPluginHandleProxy<TContext> _webHandler)
        {
            this.WebHandler = _webHandler;
        }

        /// <summary>
        /// 注册扩展处理程序
        /// </summary>
        /// <param name="plugin">扩展名称，而且也是访问地址的名称。如扩展名称为:ext,那么可使用/ext.sh访问该扩展插件</param>
        /// <param name="getReqHandler">委托PluginHandler<CmsContext,string>的实例</param>
        /// <param name="postReqHandler">委托PluginHandler<CmsContext,string>的实例</param>
        /// <returns></returns>
        public bool Register(IPlugin plugin, PluginHandler<TContext> getReqHandler,
            PluginHandler<TContext> postReqHandler)
        {
            return this.WebHandler.Register(plugin, getReqHandler, postReqHandler);
        }


        /// <summary>
        /// 扩展模块GET请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public virtual void HttpPluginRequest(TContext context, string extendName, ref bool result)
        {
            extendName = extendName.ToLower();
            this.WebHandler.HandleGetRequest(context, extendName, ref result);
        }


        /// <summary>
        /// 扩展模块POST请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public virtual void HttpPluginPost(TContext context, string extendName, ref bool result)
        {
            extendName = extendName.ToLower();
            this.WebHandler.HandlePostRequest(context, extendName, ref result);
        }

        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <returns></returns>
        public string GetRequestPath(string fullPath)
        {
            Match match = Regex.Match(fullPath, "/(.+?)/([^\\?]+)");
            return match == Match.Empty ? "" : match.Groups[2].Value;
        }

        /// <summary>
        /// 扩展插件Action请求，为插件提供Action功能
        /// </summary>
        /// <param name="t"></param>
        /// <param name="context"></param>
        /// <param name="requestPath"></param>
        /// <param name="isPostRequest"></param>
        public virtual object HandleRequestUse<T>(T t, TContext context,string requestPath, bool isPostRequest)
        {
            string path = this.GetRequestPath(requestPath);
            return this.HandleCustomRequestUse(t, context, path, isPostRequest);
        }


        public object HandleCustomRequestUse<T>(T t, TContext context, string path, bool isPostRequest)
        {
            string action = null;
            if (path.Length != 0)
            {
                action = path.IndexOf('/') == -1 ? path : path.Substring(0, path.IndexOf('/'));
            }
            action = String.Concat(action ?? "default", isPostRequest ? "_post" : "");
            return this.WebHandler.HandleRequestUse(t, context, action);
            // try
            // {
            //     return this.WebHandler.HandleRequestUse<T>(
            //         t
            //         , context
            //         , action);
            // }
            // catch (PluginException exc)
            // {
            //     Logger.Println("[ Request][ Error]" + exc.Message + ";url :" + context.Request.RawUrl);
            //     throw exc.InnerException ?? exc;
            // }
            // catch (Exception exc)
            // {
            //     Logger.PrintException(context.Request.RawUrl, exc);
            //     throw;
            // }
        }
    }
}