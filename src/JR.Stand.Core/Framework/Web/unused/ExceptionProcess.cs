/*
 * file :   exceptionProcess
 * author:  OPS newmin
 * Date :   2010/09/20
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using JR.Stand.Core.Framework.Web;

namespace JR.DevFw.Framework.Web.unused
{
    public static class ExceptionProcess
    {
        /// <summary>
        /// 记录错误,记录的文件路径为~/Logs/error.log
        /// </summary>
        /// <param name="func"></param>
        /// <param name="context">当前请求上下文</param>
        /// <param name="e">触发的异常</param>
        public static void TraceError(HttpContext context, Exception e)
        {
            StringBuilder sb = new StringBuilder(3000);
            sb.Append("[Error] Hapend at ").Append(String.Format("{0:yyyy:MM:dd HH:mm:ss}", DateTime.Now))
                .Append("\tip:").Append(context.Request.UserHostAddress).Append(" \tpath:")
                .Append(context.Request.Url.PathAndQuery).Append("\r\n").Append('-', 100).Append("\r\n")
                .Append("[message]:").Append(e.Message).Append(" [type]:").Append(e.GetType().ToString())
                .Append("\r\n").Append("[stack]:").Append(e.StackTrace).Append("\r\n");
            using (StreamWriter sw = new StreamWriter(context.Server.MapPath("~/") + "/logs/error.log", true))
            {
                sw.WriteLine(sb.ToString());
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// 处理远程提交的错误
        /// </summary>
        /// <param name="func"></param>
        /// <param name="context">当前请求上下文</param>
        /// <param name="e">触发的异常</param>
        /// <param name="errorUri">错误地址</param>
        /// <param name="ip">ip地址</param>
        public static void PostError(HttpContext context, string errorUri, string ip, string type, string msg,
            string trace)
        {
            StringBuilder sb = new StringBuilder(3000);
            sb.Append("[Error] Hapend at ").Append(String.Format("{0:yyyy:MM:dd HH:mm:ss}", DateTime.Now))
                .Append("\tip:").Append(ip).Append(" \tpath:")
                .Append(errorUri).Append("\r\n").Append('-', 100).Append("\r\n")
                .Append("[message]:").Append(msg).Append(" [type]:").Append(type)
                .Append("\r\n").Append("[stack]:").Append(trace).Append("\r\n");

            using (StreamWriter sw = new StreamWriter(context.Server.MapPath("~/") + "/logs/error.log", true))
            {
                sw.WriteLine(sb.ToString());
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// 提交错误到远程地址
        /// </summary>
        /// <param name="context">当前请求上下文</param>
        /// <param name="remoteUri">接收错误的远程uri</param>
        /// <param name="errorUri">错误地址</param>
        /// <param name="ip">用户IP</param>
        /// <param name="type">错误异常类型</param>
        /// <param name="msg">错误异常消息</param>
        /// <param name="trace">错误异常堆栈</param>
        public static void PostRemoteError(HttpContext context, string remoteUri, Exception e)
        {
            string reportUri = remoteUri, param = null;
            if (string.IsNullOrEmpty(reportUri)) return;
            if (reportUri.IndexOf("?") != -1)
            {
                param = reportUri.Substring(reportUri.IndexOf("?") + 1) + "&";
                reportUri = reportUri.Substring(0, reportUri.IndexOf("?"));
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(context.Request.Url.ToString().Replace(",", "__"))
                .Append(",").Append(context.Request.UserHostAddress).Append(",").Append(e.GetType().ToString())
                .Append(",").Append(e.Message).Append(",").Append(e.StackTrace.Replace(",", "__"));

            param += "error=" + HttpUtility.HtmlEncode(sb.ToString());

            // context.Response.Write("提交前的参数<br />" + param);

            WebRequest wr = WebRequest.Create(reportUri);
            wr.Method = "POST";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.ContentLength = param.Length;
            using (Stream stream = wr.GetRequestStream())
            {
                stream.Write(Encoding.UTF8.GetBytes(param), 0, param.Length);
                wr.GetResponse();
                //StreamReader sr = new StreamReader(wr.GetResponse().GetResponseStream());
                //context.Response.Write("接收到的参数:"+sr.ReadToEnd());
                //context.Response.End();
            }
        }
    }
}