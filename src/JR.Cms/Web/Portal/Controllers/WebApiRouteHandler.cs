/*
 * Created by SharpDevelop.
 * UserBll: newmin
 * Date: 2014/1/4
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Threading.Tasks;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.Web.Portal.Controllers
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