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

namespace JR.DevFw.Web.Plugin
{
	/// <summary>
	/// Description of PluginExtendRouteHandler.
	/// </summary>
	public class PluginRouteHandler:IRouteHandler
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
				string extendName=(this.RequestContext.RouteData.Values["plugin"]??"").ToString();
				//string path=(this.RequestContext.RouteData.Values["path"]??"").ToString();
				string httpMethod=context.Request.HttpMethod;

			    if (httpMethod == "POST")
			    {
			       WebCtx.Current.Plugin.HttpPluginPost(context, extendName, ref result);
			    }
			    else
			    {
                    WebCtx.Current.Plugin.HttpPluginRequest(context, extendName, ref result);
			    }

				if(!result)
				{
				    context.Response.StatusCode = 404;
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
