/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/4
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using J6.Cms.DataTransfer;
using J6.Cms.ServiceContract;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using J6.Cms.CacheService;

namespace J6.Cms.Web
{
    /// <summary>
    /// Description of PluginExtendRouteHandler.
    /// </summary>
    internal class WebApiRouteHandler : IRouteHandler
    {
        private class HttpHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
        {

            public HttpHandler(RequestContext context)
            {
                this.RequestContext = context;
            }


            private RequestContext RequestContext { get; set; }
            public bool IsReusable
            {
                get
                {
                    return true;
                }
            }

            public void ProcessRequest(HttpContext context)
            {
                string apiName = context.Request["name"];
                int siteId = Cms.Context.CurrentSite.SiteId;
                switch (apiName)
                {
                    case "rlink":
                        RLink(siteId, context);
                        break;

                    case "rlinked":
                        WebApiProcess.GetRelatedArchiveLinks(siteId, int.Parse(context.Request["contentId"]));
                        break;

                    default:
                        context.Response.Write("J6.Cms  WebApi  ver 0.1");
                        break;
                }

            }

            private void RLink(int siteId, HttpContext context)
            {
                context.Response.Write(WebApiProcess.GetRelatedlinks(
                    siteId,
                    context.Request["type"] ?? "archive",
                    int.Parse(context.Request["contentId"])));
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HttpHandler(requestContext);
        }

    }

    internal static class ApiTypes
    {
        public class RLink
        {
            public string name { get; set; }
            public string title { get; set; }
            public string url { get; set; }
        }
    }

    internal static class WebApiProcess
    {
        internal static string GetRelatedlinks(int siteId, string typeIndent, int contentId)
        {
            IContentServiceContract cs = ServiceCall.Instance.ContentService;
            IEnumerable<RelatedLinkDto> links = cs.GetOuterRelatedLinks(siteId, typeIndent, contentId);

            IList<ApiTypes.RLink> rlinks = new List<ApiTypes.RLink>();

            string url;
            if (links != null)
            {
                string appPath = Cms.Context.SiteAppPath;
                if (appPath == "/") appPath = "";
                foreach (RelatedLinkDto link in links)
                {
                    if (link.Enabled)
                    {
                            url = appPath + link.Url;
                        rlinks.Add(new ApiTypes.RLink
                        {
                            name = link.Title,
                            title = link.Title,
                            url = url
                        });
                    }
                }
            }
            return JsonSerializer.Serialize(rlinks);
        }

        internal static string GetRelatedArchiveLinks(int siteId, int contentId)
        {
            IEnumerable<ArchiveDto> archives  =  ServiceCall.Instance.ArchiveService.GetRelatedArchives(siteId, contentId);
           
            return JsonSerializer.Serialize(archives);
            
        }
    }
}
