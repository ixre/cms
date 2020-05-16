using System;
using System.Collections.Generic;
using System.Reflection;

namespace JR.Stand.Core.PluginKernel.Web
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebPluginHandleProxy<TContext> : PluginHandleProxy<TContext>
    {
        private static readonly IDictionary<string, PluginHandler<TContext>> postHandlers;

        static WebPluginHandleProxy()
        {
            postHandlers = new Dictionary<string, PluginHandler<TContext>>();
        }

        public virtual void HandleGetRequest(TContext context,
            string pluginWorkIndent,
            ref bool handleResult)
        {
            this.HandleRequest(context, pluginWorkIndent, ref handleResult);
        }

        public virtual void HandlePostRequest(TContext context,
            string pluginWorkIndent,
            ref bool handleResult)
        {
            pluginWorkIndent = pluginWorkIndent.ToLower();

            //处理扩展
            if (postHandlers.Keys.Contains(pluginWorkIndent))
            {
                postHandlers[pluginWorkIndent](context, ref handleResult);
            }
        }


        /// <summary>
        /// 注册处理事件
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="reqHandler"></param>
        /// <param name="postReqHandler"></param>
        /// <returns></returns>
        public bool Register(IPlugin plugin, PluginHandler<TContext> getHandler, PluginHandler<TContext> postHandler)
        {
            Type type = plugin.GetType();
            PluginPackAttribute attr = PluginUtil.GetAttribute(plugin);
            string indent = attr.WorkIndent;

            if (postHandler == null || postHandlers.Keys.Contains(indent))
            {
                return false;
            }
            postHandlers.Add(indent, postHandler);

            return base.Register(plugin, getHandler);
        }


        //
        // 此段代码仅供参考
        //
        // private bool HandleRequestUse2<HandleClass>(HandleClass t, T context, bool isPostRequest, string requestPath)
        // {
        //     string action = null;
        //
        //     if (requestPath.Length != 0)
        //     {
        //         action = requestPath.IndexOf('/') == -1
        //             ? requestPath
        //             : requestPath.Substring(0, requestPath.IndexOf('/'));
        //     }
        //
        //     if (action == null) action = "default";
        //
        //     //交由C#处理
        //     Type type = t.GetType();
        //     MethodInfo method = type.GetMethod(
        //         String.Concat(action, isPostRequest ? "_post" : ""),
        //         BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        //     if (method != null)
        //     {
        //         try
        //         {
        //             if (method.ReturnType == typeof (String))
        //             {
        //                 httpContext.Response.Write(
        //                     method.Invoke(t, new object[] {context}) as String
        //                     );
        //             }
        //             else
        //             {
        //                 method.Invoke(t, new object[] {context});
        //             }
        //         }
        //         catch (Exception exc)
        //         {
        //             Logger.PrintException(httpContext.Request.RawUrl, exc);
        //         }
        //
        //         return true;
        //     }
        //     return false;
        // }
    }
}