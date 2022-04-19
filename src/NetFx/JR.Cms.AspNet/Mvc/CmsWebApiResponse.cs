using System.Web.Mvc;
using System;
using System.Collections.Generic;

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
        public String TestGet(String formId,String formSubject,[FromBody] Dictionary<String,String> form)
        {
            return id;
        }

    }
}