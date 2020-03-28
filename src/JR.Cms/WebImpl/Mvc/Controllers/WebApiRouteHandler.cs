/*
 * Created by SharpDevelop.
 * UserBll: newmin
 * Date: 2014/1/4
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceContract;
using JR.Cms.ServiceDto;
using JR.Cms.WebImpl.Json;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.WebImpl.Mvc.Controllers
{
    /// <summary>
    /// Description of PluginExtendRouteHandler.
    /// </summary>
    public class WebApiRouteHandler : Controller
    {
        public Task ProcessRequest(HttpContext context)
        {
          return  CmsWebApiResponse.ProcessRequest(HttpHosting.Context);
        }

       
    }

}