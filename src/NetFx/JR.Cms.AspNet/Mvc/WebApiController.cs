using System.Web.Mvc;
using System;
using System.Collections.Generic;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Abstracts;
using JR.Stand.Core.Web;
using JR.Cms.ServiceDto;

namespace JR.Cms.Web.Mvc
{

    /// <summary>
    /// WebAPI
    /// </summary>
    public class WebApiController : Controller
    {
        [Route("cms/webapi/test")]
        [HttpGet]
        public String TestGet(String id)
        {
            return id;
        }

        [Route("cms/webapi/form/{formId}/{formSubject}")]
        [HttpPost]
        public Result PostForm(String formId,String formSubject)
        {
            Dictionary<String, String> forms = WebCtx.HttpCtx.Request.Bind<Dictionary<String,String>>();
            return WebApiHandler.PostForm(formId, formSubject, forms);
        }

        /// <summary>
        /// 查询文档关联的URL
        /// </summary>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        [Route("cms/webapi/relate/{archiveId}")]
        [HttpGet]
        public IList<RelatedLinkDto> QueryArchiveRelateLink(long archiveId)
        {
            SiteDto site = Cms.Context.CurrentSite;
            return WebApiHandler.GetRelateArchiveLinks(site, "archive", Convert.ToInt32(archiveId));
        }
    }
}