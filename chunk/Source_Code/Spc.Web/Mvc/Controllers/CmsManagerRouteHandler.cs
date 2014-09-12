using Ops.Cms.WebManager;
using System.Web;
using System.Web.Routing;

namespace Ops.Cms.Web
{
	/// <summary>
	/// Description of CmsManagerRouteHandler.
	/// </summary>
	internal class CmsManagerRouteHandler :IRouteHandler
	{
		private class HttpHandler:IHttpHandler,System.Web.SessionState.IRequiresSessionState
		{
			public HttpHandler(HttpContextBase context)
			{
			}
			
			public bool IsReusable 
			{
				get 
				{
					return true;
				}
			}
			
			public void ProcessRequest(HttpContext context)
			{
                Logic.Request(context);
			}
		}
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new HttpHandler(requestContext.HttpContext);
		}
	}
}