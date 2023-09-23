using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Portal.Comm;
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
    [Obsolete]
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
                case "archive/info":
                    result = WebApiProcess.GetArchiveInfo(site,
                        context.Request.GetParameter("path"));
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

    [Obsolete]
    internal static class WebApiProcess
    {
        internal static string GetRelatedlinks(SiteDto site, string contentType, int contentId)
        {
            var cs = LocalService.Instance.ContentService;
            var links = cs.GetRelateLinks(site.SiteId, contentType, contentId);

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
                .GetRelateLinks(site.SiteId, contentType, contentId));
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

/// <summary>
/// 获取文档详情, /{site_id}/web_api?name=archive/info&path=brand/welcome
/// </summary>
/// <param name="site"></param>
/// <param name="path"></param>
/// <returns></returns>
        internal static string GetArchiveInfo(SiteDto site, String path)
        {
            ArchiveDto archiveDto = LocalService.Instance.ArchiveService.GetArchiveByPath(site.SiteId, path);
            archiveDto.TemplatePath = "";
            archiveDto.Flag = -1;
            archiveDto.Id = 0;
            archiveDto.IsSelfTemplate = false;
            return Newtonsoft.Json.JsonConvert.SerializeObject(archiveDto);
        }
    }

    /// <summary>
    /// WebAPI(该写法仅支持.NET core及.NET5及其以上)
    /// </summary>
    [Route("/cms/webapi")]
    public class WebApiController : Controller
    {
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
            return WebApiHandler.PostForm(formId, formSubject, forms);
        }

        /// <summary>
        /// 查询文档关联的URL
        /// </summary>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        [HttpGet("relate/{archiveId}")]
        public IList<RelatedLinkDto> QueryArchiveRelateLink(long archiveId)
        {
            SiteDto site = Cms.Context.CurrentSite;
            return WebApiHandler.GetRelateArchiveLinks(site, "archive", Convert.ToInt32(archiveId));
        }

        /// <summary>
        /// 获取文档信息
        /// </summary>
        /// <param name="id">文档编号</param>
        /// <returns></returns>
        [HttpGet("archive/{id}")]
        public ArchiveDto QueryArchive([FromQuery] int id)
        {
            SiteDto site = Cms.Context.CurrentSite;
            ArchiveDto archiveDto = LocalService.Instance.ArchiveService.GetArchiveById(site.SiteId, id);
            archiveDto.TemplatePath = "";
            archiveDto.Flag = -1;
            archiveDto.Id = 0;
            archiveDto.IsSelfTemplate = false;
            return archiveDto;
        }
    }
}