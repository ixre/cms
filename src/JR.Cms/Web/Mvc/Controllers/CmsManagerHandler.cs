using System.Threading.Tasks;
using JR.Cms.WebImpl.WebManager;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.WebImpl.Mvc.Controllers
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