using System.Web.Mvc;
using System;
using System.Collections.Generic;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Abstracts;
using JR.Stand.Core.Web;

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
        [HttpPOST]
        public Result PostForm(String formId,String formSubject)
        {
            Dictionary<String, String> forms = WebCtx.HttpCtx.Request.Bind < Dictionary<String, String>();
            WebApiHandler.PostForm(formId, formSubject, forms);
        }
    }
}