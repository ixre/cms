using JR.Cms.Conf;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.WebImpl.Mvc.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CmsPkgController : Controller
    {
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
    }
}