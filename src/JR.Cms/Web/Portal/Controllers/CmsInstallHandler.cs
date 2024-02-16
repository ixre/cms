using System.Threading.Tasks;
using JR.Cms.Web.Portal.Common;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.Web.Portal.Controllers
{
    /// <summary>
    /// Description of CmsInstallRouteHandler.
    /// </summary>
    public class CmsInstallHandler
    {
        private static readonly CmsInstallWizard Wiz = new CmsInstallWizard();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ProcessRequest(HttpContext context)
        {
            return Wiz.ProcessInstallRequest(HttpHosting.Context);
        }
    }
}