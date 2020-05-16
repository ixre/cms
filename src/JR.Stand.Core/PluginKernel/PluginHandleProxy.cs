using System;
using System.Collections.Generic;
using System.Reflection;

namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件处理代理类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PluginHandleProxy<T> : IPluginHandleProxy<T>
    {
        private static IDictionary<string, PluginHandler<T>> reqHandlers;

        static PluginHandleProxy()
        {
            reqHandlers = new Dictionary<String, PluginHandler<T>>();
        }

        /// <summary>
        /// 扩展模块GET请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public virtual void HandleRequest(T context,
            string pluginWorkIndent,
            ref bool handleResult)
        {
            pluginWorkIndent = pluginWorkIndent.ToLower();

            //处理扩展
            if (reqHandlers.Keys.Contains(pluginWorkIndent))
            {
                reqHandlers[pluginWorkIndent](context, ref handleResult);
            }
        }


        /// <summary>
        /// 注册扩展处理程序
        /// </summary>
        /// <param name="extendName">扩展名称，而且也是访问地址的名称。如扩展名称为:ext,那么可使用/ext.sh访问该扩展插件</param>
        /// <param name="getReqHandler">委托PluginHandler<CmsContext,string>的实例</param>
        /// <param name="postReqHandler">委托PluginHandler<CmsContext,string>的实例</param>
        /// <returns></returns>
        public bool Register(IPlugin plugin,
            PluginHandler<T> reqHandler)
        {
            Type type = plugin.GetType();
            PluginPackAttribute attr = PluginUtil.GetAttribute(plugin);
            string indent = attr.WorkIndent;

            if (reqHandler == null || reqHandlers.Keys.Contains(indent))
            {
                return false;
            }

            if (reqHandler != null)
            {
                reqHandlers.Add(indent, reqHandler);
            }

            return true;
        }


        public object HandleRequestUse<THandleClass>(THandleClass t, T context, string action)
        {
            Type type = t.GetType();
            MethodInfo method = type.GetMethod(
                action,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (method != null)
            {
                return method.Invoke(t, new object[] { context });
            }
            throw new PluginException("无效请求:" + action);
        }

    }
}