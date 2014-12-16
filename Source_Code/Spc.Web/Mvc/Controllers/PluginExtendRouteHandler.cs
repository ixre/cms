/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/4
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;

namespace Ops.Cms.Web
{
	/// <summary>
	/// Description of PluginExtendRouteHandler.
	/// </summary>
	internal class PluginExtendRouteHandler:IRouteHandler
	{
		private class HttpHandler:IHttpHandler,IRequiresSessionState
		{
			
			public HttpHandler(RequestContext context)
			{
				this.RequestContext=context;
			}
			
			
			private RequestContext RequestContext{get; set;}
			public bool IsReusable
			{
				get {
					return true;
				}
			}
			
			public void ProcessRequest(HttpContext context)
			{
				bool result=false;
				string extendName=(this.RequestContext.RouteData.Values["extend"]??"").ToString();
				//string path=(this.RequestContext.RouteData.Values["path"]??"").ToString();
				string httpMethod=context.Request.HttpMethod;

			    if (httpMethod == "POST")
			    {
			        Cms.Plugins.Extends.ExtendModulePost(context, extendName, ref result);
			    }
			    else
			    {
                    Cms.Plugins.Extends.ExtendModuleRequest(context, extendName, ref result);
			    }

				if(!result)
				{
					context.Response.Write("Access denied!");
				}
			}
		}
		
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new HttpHandler(requestContext);
		}
		
	}
}
