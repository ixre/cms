/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/4
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Cms.DataTransfer;
using JR.Cms.ServiceContract;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using JR.Cms;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.Web;
using JR.DevFw.Web;

namespace JR.Cms.Web
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
                const string defaultRsp = "jr.Cms  WebApi  ver 1.0";
                string apiName = context.Request["name"];
                SiteDto site = Cms.Context.CurrentSite;
                String result = null;
                switch (apiName)
                {
                    case "rel_link":
                        result = WebApiProcess.GetRelatedlinks(
                              site,
                              context.Request["type"] ?? "archive",
                              int.Parse(context.Request["content_id"]));
                        break;

                    case "rel_link_json":
                        result = WebApiProcess.GetRelatedArchiveLinks(site,
                            context.Request["type"] ?? "archive",
                            int.Parse(context.Request["content_id"]));
                        break;
                }

                context.Response.Write(result ?? defaultRsp);
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

            public string thumbnail { get; set; }

            public string relatedIndent { get; set; }
        }
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
        IList<RelatedLinkDto> archives = new List<RelatedLinkDto>(ServiceCall.Instance.ContentService
            .GetRelatedLinks(site.SiteId, contentType, contentId));
        String host = WebCtx.Current.Host;
        String resDomain = Cms.Context.ResourceDomain;
        String defaultThumb = String.Concat(resDomain, "/" + CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto);

        ReValueRelateLinks(archives, host, defaultThumb, resDomain);

        return JsonSerializer.Serialize(archives);
    }

    private static void ReValueRelateLinks(IList<RelatedLinkDto> archives, string host, string defaultThumb, string resDomain)
    {
        for(int i=0;i<archives.Count;i++)
        {
            if (archives[i].Enabled)
            {
                archives[i].Url = archives[i].Url.Replace("#", host);
                if (archives[i].Thumbnail.Length == 0)
                {
                    archives[i].Thumbnail = defaultThumb;
                }
                else if (!archives[i].Thumbnail.StartsWith("http"))
                {
                    archives[i].Thumbnail = String.Concat(resDomain, "/", archives[i].Thumbnail);
                }
            }
            else
            {
                archives.Remove(archives[i]);
                i--;
            }
        }
    }
}
