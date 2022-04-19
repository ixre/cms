using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Util;
using JR.Stand.Abstracts;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.Web.Portal.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CmsWebApiResponse
    {
        public static Task ProcessRequest(ICompatibleHttpContext context)
        {
            const string defaultRsp = "jr.Cms  WebApi  ver 1.0";
            var apiName = context.Request.GetParameter("name");
            var site = Cms.Context.CurrentSite;
            string result = null;
            switch (apiName)
            {
                case "rel_link":
                    result = WebApiProcess.GetRelatedlinks(
                        site, context.Request.GetParameter("type") ?? "archive",
                        int.Parse(context.Request.GetParameter("content_id")));
                    break;

                case "rel_link_json":
                    result = WebApiProcess.GetRelatedArchiveLinks(site,
                        context.Request.GetParameter("type") ?? "archive",
                        int.Parse(context.Request.GetParameter("content_id")));
                    break;
            }

            return context.Response.WriteAsync(result ?? defaultRsp);
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

    internal static class WebApiProcess
    {
        internal static string GetRelatedlinks(SiteDto site, string contentType, int contentId)
        {
            var cs = LocalService.Instance.ContentService;
            var links = cs.GetRelatedLinks(site.SiteId, contentType, contentId);

            IList<ApiTypes.RLink> rlinks = new List<ApiTypes.RLink>();

            string url;
            if (links != null)
            {
                var appPath = Cms.Context.SiteAppPath;
                if (appPath == "/") appPath = "";
                foreach (var link in links)
                    if (link.Enabled)
                    {
                        url = appPath + link.Url + ".html";
                        rlinks.Add(new ApiTypes.RLink
                        {
                            name = link.Title,
                            title = link.Title,
                            url = url
                        });
                    }
            }

            return JsonSerializer.Serialize(rlinks);
        }

        internal static string GetRelatedArchiveLinks(SiteDto site, string contentType, int contentId)
        {
            IList<RelatedLinkDto> archives = new List<RelatedLinkDto>(LocalService.Instance.ContentService
                .GetRelatedLinks(site.SiteId, contentType, contentId));
            var host = WebCtx.Current.Host;
            var resDomain = Cms.Context.ResourceDomain;
            var defaultThumb = string.Concat(resDomain, "/" + CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto);

            ReValueRelateLinks(archives, host, defaultThumb, resDomain);

            return JsonSerializer.Serialize(archives);
        }

        private static void ReValueRelateLinks(IList<RelatedLinkDto> archives, string host, string defaultThumb,
            string resDomain)
        {
            for (var i = 0; i < archives.Count; i++)
                if (archives[i].Enabled)
                {
                    archives[i].Url = archives[i].Url.Replace("#", host);
                    if (!archives[i].Url.EndsWith(".html")) archives[i].Url += ".html";
                    if (archives[i].Thumbnail.Length == 0)
                        archives[i].Thumbnail = defaultThumb;
                    else if (!archives[i].Thumbnail.StartsWith("http"))
                        archives[i].Thumbnail = string.Concat(resDomain, "/", archives[i].Thumbnail);
                }
                else
                {
                    archives.Remove(archives[i]);
                    i--;
                }
        }
    }

    /// <summary>
    /// WebAPI
    /// </summary>
    [Route("/cms/webapi")]
    public class WebApiController:Controller
    {
        [HttpGet("/test")]
        public String TestGet(String id)
        {
            return id;
        }
        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="formId">表单编号</param>
        /// <param name="formSubject">表单主题</param>
        /// <param name="forms">表单数据</param>
        /// <returns></returns>
        [HttpPost("form/{formId}/{formSubject}")]
        public Result PostForm(string formId, string formSubject, [FromBody] Dictionary<String, String> forms)
        {
            String fromAddress = "support@fze.net";
            SiteDto site = Cms.Context.CurrentSite;
            String toAddress = site.ProEmail.Trim().Replace("#", "@");
            String subject = site.Name + "-" + formSubject;
            if (toAddress.Length == 0)
            {
                toAddress = fromAddress;
            }

            String ip = WebCtx.HttpCtx.RemoteAddress();
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='border:solid 1px #DDDD;border-radius:8px'>");
            sb.Append("<div style='background:#34B334;color:#FFF;font-size:16px;font-weight:500;padding:15px;border-radius:8px 8px 0 0'>")
                .Append(site.Name+formSubject).Append("(#"+formId+")").Append("</div>");
            sb.Append("<div style='padding:30px;'>");
            sb.Append("<div style='color:#00aa00; font-weight:400;'>")
                .Append("用户:&nbsp;").Append(ip)
                .Append("&nbsp;&nbsp;&nbsp;&nbsp;提交时间:&nbsp;").Append($"{DateTime.Now:yyyy:MM:dd HH:mm}")
                .Append("</div>");
            sb.Append("<ul style='margin:30px 0 0 0;padding:0'>");
            foreach (var p in forms)
            {
                sb.Append("<li style='list-style:none'><label>")
                    .Append(p.Key).Append(":</label>&nbsp;&nbsp;<span>")
                    .Append(p.Value)
                    .Append("</span></li>");
            }

            sb.Append("</ul>");
            sb.Append("<br /><br /><br />")
                .Append("<div style='color:#999;font-size:12px;'>")
                .Append("本邮件为系统<a style='color:#999;' href='https://fze.net/cms' target='_blank'>JR.Cms v")
                .Append(CmsVariables.VERSION)
                .Append("</a>自动发布,请您定期查看邮箱,以免错过重要信息！</div>");
                sb.Append("</div></div>");
            SendMailClient client = new SendMailClient(
                Settings.SMTP_USERNAME, Settings.SMTP_PASSWORD, Settings.SMTP_HOST, Settings.SMTP_PORT, Settings.SMTP_SSL);
            new Thread(() =>
            {
                client.Send(fromAddress,
                    toAddress,
                    "站点机器人",
                    subject,
                    sb.ToString(), true);
            }).Start();
            return Result.Success("");
        }
    }
}