/*
 * file : HttpReportModule
 * author:OPS newmin
 * createdate:2010/09/20
 */

using System;
using System.Web;
using JR.Stand.Core.Framework.Web;

namespace JR.DevFw.Framework.Web.unused.HttpModule
{
    public class HttpReportModule : HttpModuleBase
    {
        public override void ProcessError(object sender, EventArgs e)
        {
            HttpContext context = (sender as HttpApplication).Context;
            // 记录错误:文件保存在/Logs/error.log
            // 以下情况会将错误日志保存在当前应用程序目录下
            // 1.errorReportUri为空
            // 2.web.config中未配置errorReportUri节点
            // 3.errorReportUri键值的domain与当前域一致且当前域不为虚拟目录
            // 其他情况将提交到errorReportUri

            if (ConfigurationDictionary.RecordError)
            {
                Exception ex = context.Server.GetLastError().InnerException;
                string remoteuri = ConfigurationDictionary.ReportErrorUri ?? "";
                string domain = context.Request.Url.Host;

                if (String.IsNullOrEmpty(remoteuri) ||
                    (remoteuri.IndexOf(domain) != -1 && HttpContext.Current.Request.ApplicationPath == "/"))
                    ExceptionProcess.TraceError(HttpContext.Current, ex); //当前程序域处理错误
                else
                    ExceptionProcess.PostRemoteError(HttpContext.Current, remoteuri, ex); //不同域或当前域的虚拟目录处理错误
            }
        }

        public override void BeginRequest(object sender, EventArgs e)
        {
        }
    }
}