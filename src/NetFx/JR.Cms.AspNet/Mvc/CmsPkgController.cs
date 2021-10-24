using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JR.Cms.Conf;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Core.Web;

namespace JR.Cms.AspNet.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public class CmsPkgController : Controller
    {
        private static readonly CmsPkgProxyController proxy = new CmsPkgProxyController();
        // public System.Threading.Tasks.Task Version(HttpContext context)
        // {
        //     return context.Response.WriteAsync("hallo every one");
        //     //return this._next(context);
        // }

        public ActionResult Version()
        {
            return Content("JR-Cms " + CmsVariables.VERSION);
        }
        
        
        [Route("software/errors/{statusCode}")]
        public void CustomError(int statusCode)
        {
            this.HttpContext.Response.StatusCode = statusCode;
            this.HttpContext.Response.ContentType = "text/html;charset=utf-8";
            Cms.Context.RenderNotfound(statusCode == 500 ? "您访问的页面出错了" : "page not found", null);
        }

        public void Change()
        {
             proxy.Change(HttpHosting.Context);
        }
    }
}