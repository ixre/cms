using System.Threading.Tasks;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheService;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.Web.Portal.Controllers
{
    /// <summary>
    /// 内容门户控制器
    /// </summary>
    public class PortalController
    {
        private static readonly PortalControllerHandler portal = new PortalControllerHandler();


        /// <summary>
        /// 首页
        /// </summary>
        public Task Index(HttpContext context)
        {
           Error err = ServiceCall.Instance.ContentService.SaveTag(new SiteWord
            {
                Id = 0,
                Word = "软件",
                Url = "#",
                Title = ""
            });
           var list = ServiceCall.Instance.ContentService.GetTags();
            return portal.Index(HttpHosting.Context);
        }
        
        /// <summary>
        /// 文档页
        /// </summary>
        /// <returns></returns>
        public Task Archive(HttpContext context)
        {           
            return portal.Archive(HttpHosting.Context);
        }

        /// <summary>
        /// 栏目页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Category(HttpContext context)
        {
            return portal.Category(HttpHosting.Context);
        }
    }
}