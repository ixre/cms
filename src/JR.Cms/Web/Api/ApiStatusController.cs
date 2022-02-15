using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.Web.Api
{
    /// <summary>
    /// 开放API状态控制器
    /// </summary>
    [Route("/openapi")]
    [ApiController]
    public class ApiStatusController : Controller
    {
        /// <summary>
        /// 查询API状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IDictionary<String,Object> Status()
        {
            IDictionary<String, Object> dict = new Dictionary<String, Object>();
            dict.Add("status","ok");
            dict.Add("version",Cms.Version);
            return dict;
        }
    }
}