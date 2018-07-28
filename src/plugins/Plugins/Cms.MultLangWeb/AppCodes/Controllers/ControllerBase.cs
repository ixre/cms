//
// 文件名称: CmsController.cs
// 作者：  newmin
// 创建时间：2012-10-01
// 修改说明：
//  2013-03-28  09:55   newmin  [!]:301重定向
//

namespace CMS
{
    using System;
    using System.Web.Mvc;
    using J6.Cms;
    using System.Web;
    using System.Text.RegularExpressions;

    public abstract class ControllerBase : Controller
    {
        /// <summary>
        /// 不自动301定向忽略的action列表
        /// </summary>
       //private static readonly string[] ignoreActions = new string[] { "Admin", "VerifyImg" };

       protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {

           //如果自动301
           if (Settings.SYS_AUTOWWW)
           {
               const string mainDomainPattern = "^([^\\.]+)\\.([^\\.]+)$";
               HttpContextBase c = requestContext.HttpContext;
               string url = c.Request.Url.ToString();
               string protrol = url.Remove(url.IndexOf("://"));
               string host = c.Request.Url.Host;    // c.Request.ServerVariables["server_name"];
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
        protected string RenderTemplate(string templateID, object data)
        {
            return PageUtility.Require(templateID, data);
        }
        protected string Render404()
        {
            Response.StatusCode = 404;
            try
            {
                return PageUtility.Require(String.Format("/{0}/notfound", Settings.TPL_Name), null);
            }
            catch
            {
                return "not found!";
            }
        }
    }
}
