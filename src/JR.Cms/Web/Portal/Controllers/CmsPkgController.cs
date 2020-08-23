using JR.Cms.Conf;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.Web.Portal.Controllers
{
    /// <summary>
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

        public void Change()
        {
            proxy.Change(HttpHosting.Context);
        }
    }
}