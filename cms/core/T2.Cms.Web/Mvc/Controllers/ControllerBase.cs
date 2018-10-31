//
// 文件名称: CmsController.cs
// 作者：  newmin
// 创建时间：2012-10-01
// 修改说明：
//  2013-03-28  09:55   newmin  [!]:301重定向
//

using System.Web.Routing;
using T2.Cms.Conf;
using T2.Cms.Core;
using T2.Cms.DB;

namespace T2.Cms.Web.Mvc
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using T2.Cms.DataTransfer;
    using T2.Cms;
    using System.Diagnostics;
    using System.Collections.Generic;

    
    public delegate void CmsPageHandler(CmsContext controller, ref bool result);
    public delegate void CmsPageHandler<in T>(CmsContext controller, T t, ref bool result);
    public delegate void CmsPageHandler<in T, in T1>(CmsContext controller, T t, T1 t1, ref bool result);
    public delegate void CmsPageHandler<in T, in T1, in T2>(CmsContext controller,T t,T1 t1,T2 t2,ref bool result);


    
    public abstract class ControllerBase :Controller
    {
        protected CmsContext OutputContext;
        private TimeSpan _startTime;
        //private static readonly bool _showDebugInformation;
        private  bool _showDebugInformation;

        protected override void Initialize(RequestContext requestContext)
        {
            if (Cms.IsStaticRequest(requestContext.HttpContext.ApplicationInstance.Context))return;
            //if (!Cms.Installed) HttpRuntime.UnloadAppDomain();

            this.OutputContext = Cms.Context;
            this.OutputContext.Source = this;


            // ==========================================//

            _startTime = new TimeSpan(DateTime.Now.Ticks);
            _showDebugInformation = Settings.Opti_Debug;
            //如果自动301
            if (Settings.SYS_AUTOWWW)
            {
                const string mainDomainPattern = "^([^\\.]+)\\.([^\\.]+)$";
                HttpContextBase c = requestContext.HttpContext;
                string url = c.Request.Url.ToString();
                string protrol = url.Remove(url.IndexOf("://"));
                string host = c.Request.Url.Host; // c.Request.ServerVariables["server_name"];
                string appPath = c.Request.ApplicationPath;


                if (Regex.IsMatch(host, mainDomainPattern))
                {
                    Match match = Regex.Match(host, mainDomainPattern);

                    //检查是否存在于忽略的301列表中
                    //if (Array.Exists(ignoreActions, a => String.Compare(a, requestContext.RouteData.Values["action"].ToString(), true) == 0))
                    //{
                    //    goto initialize;
                    // }
                    string redirectUrl = String.Format("{0}://www.{1}{2}",
                        protrol,
                        host,
                        c.Request.RawUrl
                        );

                    c.Response.AppendHeader("Location", redirectUrl);
                    c.Response.Status = "301 Moved Permanently";

                    /*
                    try
                    {
                        //MONO或IIS集成模式
                        c.Response.Headers.Add("Location", redirectUrl);
                    }
                    catch(PlatformNotSupportedException ex)
                    {
                        //IIS经典模式
                        c.Response.AppendHeader("Location", redirectUrl);
                    }*/

                    c.Response.End();
                    return;
                }
            }
            //初始化
            initialize:
            base.Initialize(requestContext);
        }

       

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //检测网站状态
            if (!this.OutputContext.CheckSiteState())
            {
                filterContext.HttpContext.Response.End();
                throw new Exception("站点被停用！");
            }

            base.OnActionExecuting(filterContext);
            return;

            //显示站点信息
            if (filterContext.RequestContext.HttpContext.Request.Path != "/admin")
            {
                SiteDto site = Cms.Context.CurrentSite;
                filterContext.HttpContext.Response.Write(site.SiteId + "<br />" + site.Domain + "<br />" + site.Name + "<br />");
            }
        }

         protected override void OnResultExecuted(ResultExecutedContext filterContext)
         {
             if (_showDebugInformation)
             {
                 HttpResponseBase rsp = filterContext.HttpContext.Response;
                 rsp.Write("<div style=\"border-top:solid 2px #ff6600;background:#ffff00;font-size:12px;padding:10px 20px;color:#000\">");
                 rsp.Write("============= 页面执行信息 =============<br />");
                 rsp.Write(String.Format("<br />缓存数：{0} - 内存占用：{1}M",
                     HttpRuntime.Cache.Count.ToString(),
                     ((Double)Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024).ToString("N2")));

                 rsp.Write(String.Format("<br />程序执行时间：{0:mm}分{0:ss}秒{0:fff}毫秒.", DateTime.Now - _startTime));

                 rsp.Write("<br /><br /><span style=\"font-size:80%;color:#ff0000\">注：网站运行时，请在后台系统设置-》关闭调试模式！</font></div>");
             }
             base.OnResultExecuted(filterContext);
         }

        public void NotFound()
        {
            DefaultWebOuput.RenderNotFound(this.OutputContext);
        }

        /// <summary>
        /// 访问限制
        /// </summary>
        public void Disallow()
        {
            this.OutputContext.Response.Write("Access denied!");
        }
    }
}
