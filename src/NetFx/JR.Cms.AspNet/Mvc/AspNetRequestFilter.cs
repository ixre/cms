using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace JR.Cms.AspNet.Mvc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AspNetRequestFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (Cms.IsStaticRequest(filterContext.HttpContext.ApplicationInstance.Context.Request.Path)) return;
            // 检测Cms是否安装, 如果未安装进入安装界面
            var context = filterContext.HttpContext;
            if (!AspNetCmsChecker.Check(context)) context.Response.End();
            // 自动跳转到www开头的域名
            if (!AspNetCmsChecker.Check301Redirect(context)) context.Response.End();
            base.OnResultExecuting(filterContext);
        }

        //protected CmsContext OutputContext;

        private TimeSpan _startTime;

        //private static readonly bool _showDebugInformation;
        private bool _showDebugInformation = false;

        protected void Initialize(RequestContext requestContext)
        {
            // this.OutputContext = Cms.Context;
            // this.OutputContext.Source = this;

            //
            // // ==========================================//
            //
            // _startTime = new TimeSpan(DateTime.Now.Ticks);
            // _showDebugInformation = Settings.OPTI_DEBUG_MODE;
            // //如果自动301
            // if (Settings.SYS_WWW_RD)
            // {
            //     const string mainDomainPattern = "^([^\\.]+)\\.([^\\.]+)$";
            //     HttpContextBase c = requestContext.HttpContext;
            //     string url = c.Request.Url.ToString();
            //     string protrol = url.Remove(url.IndexOf("://"));
            //     string host = c.Request.Url.Host; // c.Request.ServerVariables["server_name"];
            //     string appPath = c.Request.ApplicationPath;
            //
            //
            //     if (Regex.IsMatch(host, mainDomainPattern))
            //     {
            //         Match match = Regex.Match(host, mainDomainPattern);
            //
            //         //检查是否存在于忽略的301列表中
            //         //if (Array.Exists(ignoreActions, a => String.Compare(a, requestContext.RouteData.Values["action"].ToString(), true) == 0))
            //         //{
            //         //    goto initialize;
            //         // }
            //         string redirectUrl = String.Format("{0}://www.{1}{2}",
            //             protrol,
            //             host,
            //             c.Request.RawUrl
            //         );
            //
            //         c.Response.AppendHeader("Location", redirectUrl);
            //         c.Response.Status = "301 Moved Permanently";
            //
            //         /*
            //         try
            //         {
            //             //MONO或IIS集成模式
            //             c.Response.Headers.Add("Location", redirectUrl);
            //         }
            //         catch(PlatformNotSupportedException ex)
            //         {
            //             //IIS经典模式
            //             c.Response.AppendHeader("Location", redirectUrl);
            //         }*/
            //
            //         c.Response.End();
            //         return;
            //     }
            // }
            //
            // //初始化
            // initialize:
            //base.Initialize(requestContext);
        }
    }
}