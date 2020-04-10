using System;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Web;

namespace JR.Cms.WebImpl.Mvc
{
    /// <summary>
    /// Description of PluginExtendRouteHandler.
    /// </summary>
    internal class AspNetWebApiHandler : IRouteHandler
    {
        private class HttpHandler : IHttpHandler, IRequiresSessionState
        {
            public HttpHandler(RequestContext context)
            {
                this.RequestContext = context;
            }


            private RequestContext RequestContext { get; set; }

            public bool IsReusable
            {
                get { return true; }
            }

            public void ProcessRequest(HttpContext context)
            {
                CmsWebApiResponse.ProcessRequest(HttpHosting.Context);
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HttpHandler(requestContext);
        }
    }
}