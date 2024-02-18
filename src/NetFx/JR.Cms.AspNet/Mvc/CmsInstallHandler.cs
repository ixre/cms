using System.Web;
using System.Web.Routing;
using JR.Cms.Web.Portal.Common;
using JR.Stand.Core.Web;

namespace JR.Cms.WebImpl.Mvc
{
    internal class CmsInstallHandler : IRouteHandler
    {
        private class HttpHandler : IHttpHandler
        {
            private readonly CmsInstallWizard wiz = new CmsInstallWizard();
            public bool IsReusable => true;

            public void ProcessRequest(HttpContext context)
            {
                this.wiz.ProcessInstallRequest(HttpHosting.Context);
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HttpHandler();
        }
    }
}