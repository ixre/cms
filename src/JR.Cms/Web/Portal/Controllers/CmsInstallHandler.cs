using System.Threading.Tasks;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.Web.Portal.Controllers
{
    /// <summary>
    /// Description of CmsInstallRouteHandler.
    /// </summary>
    public class CmsInstallHandler
    {
        private static readonly CmsInstallWiz wiz = new CmsInstallWiz();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ProcessRequest(HttpContext context)
        {
            return wiz.ProcessInstallRequest(HttpHosting.Context);
        }
    }
}