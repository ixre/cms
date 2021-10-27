using System.Threading.Tasks;
using JR.Cms.Web.Manager;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.Web.Portal.Controllers
{
    /// <summary>
    /// CMS管理
    /// </summary>
    public class CmsManagerHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ProcessRequest(HttpContext context)
        {
            // 解决中文乱码
            context.Response.ContentType = "text/html;charset=utf-8";
            return Logic.Request(HttpHosting.Context);
        }
    }
}