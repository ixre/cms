/*
 * Name     :   HttpMethodAttribute
 * Author   :   OPS newmin
 * Date     :   2010/10/26 10:05
 */

using System;
using System.Web;
using JR.Stand.Core.Framework.Web;

namespace JR.DevFw.Framework.Web.unused.Handler
{
    /// <summary>
    /// Post请求特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class GetAttribute : Attribute, IWebExecute
    {
        /// <summary>
        /// 允许刷新的时间戳,在时间内将停止请求
        /// </summary>
        public int AllowRefreshMillliSecond { get; set; }

        /// <summary>
        /// 刷新错误提示信息
        /// </summary>
        public string RefreshErrorMessage { get; set; }

        public void PreExecuting()
        {
            if (AllowRefreshMillliSecond > 0)
            {
                TimeSpan t = new TimeSpan(0, 0, 0, 0, AllowRefreshMillliSecond);
                object lastAccessDate = HttpContext.Current.Session["_lastaccessdatetime"];
                if (lastAccessDate == null) HttpContext.Current.Session["_lastaccessdatetime"] = DateTime.Now; //保存时间
                else
                {
                    bool isTimeout = DateTime.Now - (DateTime) lastAccessDate < t; //是否超过指定的再次请求时间
                    HttpContext.Current.Session["_lastaccessdatetime"] = DateTime.Now; //保存时间
                    if (isTimeout)
                    {
                        HttpContext.Current.Response.Write(String.IsNullOrEmpty(RefreshErrorMessage)
                            ? "Service unavailable!"
                            : RefreshErrorMessage);
                        HttpContext.Current.Response.End();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Post请求特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class PostAttribute : GetAttribute, IWebExecute
    {
        void IWebExecute.PreExecuting()
        {
            //POST请求
            if (HttpContext.Current.Request.HttpMethod != "POST")
            {
                HttpContext.Current.Response.Write("请求非法");
                HttpContext.Current.Response.End();
            }
            base.PreExecuting();
        }
    }
}