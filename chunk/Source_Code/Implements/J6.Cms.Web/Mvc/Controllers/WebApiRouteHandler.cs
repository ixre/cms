/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/4
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using J6.Cms.DataTransfer;
using J6.Cms.ServiceContract;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using J6.Cms.CacheService;
using J6.DevFw.Web;

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
                SiteDto site = Cms.Context.CurrentSite;
                switch (apiName)
                {
                    case "rel_link":
                        RLink(site, context);
                        break;

                    case "rel_link_json":
                        WebApiProcess.GetRelatedArchiveLinks(site,
                            context.Request["type"] ?? "archive",
                            int.Parse(context.Request["content_id"]));
                        break;

                    default:
                        context.Response.Write("J6.Cms  WebApi  ver 1.0");
                        break;
                }

            }

            private void RLink(SiteDto site, HttpContext context)
            {
                context.Response.Write(WebApiProcess.GetRelatedlinks(
                    site,
                    context.Request["type"] ?? "archive",
                    int.Parse(context.Request["content_id"])));
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
        internal static string GetRelatedlinks(SiteDto site, string contentType, int contentId)
        {
            IContentServiceContract cs = ServiceCall.Instance.ContentService;
            IEnumerable<RelatedLinkDto> links = cs.GetRelatedLinks(site.SiteId, contentType, contentId);

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

        internal static string GetRelatedArchiveLinks(SiteDto site, string contentType, int contentId)
        {
            RelatedLinkDto[] archives = ServiceCall.Instance.ContentService
                .GetRelatedLinks(site.SiteId, contentType, contentId).ToArray();
            String preUrl = site.FullDomain.Replace("#", WebCtx.Current.Host);
            foreach (var relatedLinkDto in archives)
            {
                relatedLinkDto.Url = String.Concat(preUrl, relatedLinkDto.Url);
            }
            return JsonSerializer.Serialize(archives);
        }
    }
}
