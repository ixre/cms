/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2013/12/10
 * 时间: 13:57
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using T2.Cms.Web.Mvc;
using T2.Cms.Web.Task;
using JR.DevFw.Web;

namespace T2.Cms
{
    /// <summary>
    /// Description of HttpApplicaiton.
    /// </summary>
    public partial class Application : HttpApplication
    {
        static Application()
        {
            //解决依赖
            __ResolveAppDomain.Resolve();
        }

        protected virtual void RegisterRoutes(RouteCollection routes)
        {

            // ------------------------------------------------
            //  注册首页路由，可以自由修改首页位置
            //  routes.MapRoute("HomeIndex", ""
            //  , new { controller = "Cms", action = "Index" });
            // ------------------------------------------------


            // ------------------------------------------------
            //     自定义路由放在这里
            //  -----------------------------------------------

        }

        protected virtual void Application_Start()
        {
            try
            {
                Cms.OnInit += CmsEventRegister.Init;
                Cms.Init(BootFlag.Normal,null);

                //注册路由;
                RouteCollection routes = RouteTable.Routes;
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
                //在加载cms后避免覆盖 /cms.do
                Routes.RegisterCmsMajorRoutes(routes);
                // 加载插件
                WebCtx.Current.Plugin.Connect();
                //注册自定义路由
                RegisterRoutes(routes);
                //注册CMS路由
                Routes.RegisterCmsRoutes(routes, null);
                //RouteDebug.RouteDebugger.RewriteRoutesForTesting(routes);

                //加载自定义插件
                //Cms.Plugins.Extends.LoadFromAssembly(typeof(sp.datapicker.CollectionExtend).Assembly);


                //注册定时任务
                CmsTask.Init();
            }
            catch (Exception exc)
            {
                if (exc.InnerException != null)
                {
                    exc = exc.InnerException;
                }
                try
                {
                    HttpResponse rsp = HttpContext.Current.Response;
                    rsp.Write(@"<html><head></head>
                        <body style=""text-align:center""><h1 style="""">" + exc.Message
                        + "</h1><hr /><span>JR..Cms v" + Cms.Version
                        + "</span>\r\n<span style=\"display:none\">"
                        + exc.StackTrace + "</span>"
                        + "</body></html>");
                    rsp.End();
                    Server.ClearError();
                }
                catch (Exception ex)
                {
                    throw exc;
                }
                finally
                {
                    HttpRuntime.UnloadAppDomain();
                }
                return;
            }

        }

        protected virtual void Application_Error(object o, EventArgs e)
        {
            return;
            Exception exc = Server.GetLastError();
            if (exc.InnerException != null)
            {
                exc = exc.InnerException;
            }

            //PageUtility.RenderException(Server.GetLastError(),true);
            //Server.ClearError();

            #region 记录并发送错误到邮箱

            if (true)
            {
                new System.Threading.Thread(() =>
                {

                    try
                    {
                        string mailContent = "";
                        string mailSubject = "";


                        HttpRequest req = HttpContext.Current.Request;

                        StringBuilder sb = new StringBuilder();

                        sb.Append("---------------------------------------------------------------------\r\n")
                            .Append("[错误]：IP:").Append(req.UserHostAddress)
                            .Append("\t时间：").Append(DateTime.Now.ToString())
                            .Append("\r\n[信息]：").Append(exc.Message)
                            .Append("\r\n[路径]：").Append(req.Url.PathAndQuery)
                            .Append("  -> 来源：").Append(req.Headers["referer"] ?? "无")
                            .Append("\r\n[堆栈]：").Append(exc.StackTrace)
                            .Append("\r\n\r\n");

                        mailContent = sb.ToString();

                        mailSubject = String.Format("[{0}]异常:{2}", Cms.Context.SiteDomain, exc.Message);


                        //发送邮件
                        SmtpClient smtp = new SmtpClient("smtp.126.com", 25);
                        smtp.Credentials = new NetworkCredential("q12176@126.com", "123000");

                        MailMessage msg = new MailMessage("q12176@126.com", "q12177@126.com");

                        msg.Subject = mailSubject;
                        msg.IsBodyHtml = true;
                        msg.Body = mailContent;

                        //msg.Priority= MailPriority.High;

                        smtp.Send(msg);

                    }
                    catch (Exception ex)
                    {

                    }
                }).Start();
            }

            #endregion
        }

        protected virtual void Application_BeginRequest(object o, EventArgs e)
        {
            //mono 下编码可能会有问题
            if (Cms.RunAtMono)
            {
                Response.Charset = "utf-8";
            }
        }
    }
}