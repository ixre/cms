/*
 * file : HttpModuleBase
 * author:OPS newmin
 * createdate:2010/09/20
 */

using System;
using System.Web;

namespace JR.DevFw.Framework.Web.unused.HttpModule
{
    public abstract class HttpModuleBase : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            //Bind Events
            context.BeginRequest += BeginRequest;
            context.Error += ProcessError;
            context.EndRequest += EndRequest;
        }

        /// <summary>
        /// 开始处理请求时发生
        /// </summary>
        /// <param name="sender">HttpApplication</param>
        /// <param name="e"></param>
        public abstract void BeginRequest(object sender, EventArgs e);

        /// <summary>
        /// 当发生错误的时候处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void ProcessError(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 当请求结束时候发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void EndRequest(object sender, EventArgs e)
        {
        }
    }
}