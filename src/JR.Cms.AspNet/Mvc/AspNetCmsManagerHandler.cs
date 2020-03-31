using System.Web;
using System.Web.Routing;
using JR.Cms.AspNet;
using JR.Cms.AspNet.Mvc;
using JR.Cms.WebImpl.WebManager;
using JR.Stand.Core.Web;

namespace JR.Cms.WebImpl.Mvc
{
    /// <summary>
    /// Description of CmsManagerRouteHandler.
    /// </summary>
    internal class AspNetCmsManagerHandler :IRouteHandler
    {
        private class HttpHandler:IHttpHandler,System.Web.SessionState.IRequiresSessionState
        {
            public HttpHandler(HttpContextBase context)
            {
            }
			
            public bool IsReusable => true;

            public void ProcessRequest(HttpContext context)
            {
                if (!Cms.IsInstalled())
                {
                    context.Response.Redirect("/install/cms",true);
                }
                // 解决中文乱码
                context.Response.ContentType = "text/html;charset=utf-8";
                Logic.Request(HttpHosting.Context);
            }
        }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HttpHandler(requestContext.HttpContext);
        }
    }
}