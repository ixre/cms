using System.Threading.Tasks;
using JR.Cms.Web.Portal.Comm;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.Web.Portal.Controllers
{
    /// <summary>
    ///     内容门户控制器
    /// </summary>
    public class PortalController
    {
        private static readonly PortalControllerHandler portal = new PortalControllerHandler();


        /// <summary>
        ///     首页
        /// </summary>
        public Task Index(HttpContext context)
        {
            return portal.Index(HttpHosting.Context);
        }

        /// <summary>
        ///     文档页
        /// </summary>
        /// <returns></returns>
        public Task Archive(HttpContext context)
        {
            return portal.Archive(HttpHosting.Context);
        }

        /// <summary>
        ///     栏目页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Category(HttpContext context)
        {
            return portal.Category(HttpHosting.Context);
        }

        /// <summary>
        /// 自定义错误页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task Error(HttpContext context)
        {
            ICompatibleHttpContext ctx = HttpHosting.Context;
            int statusCode = 500;
            if (context.Request.Path.Value.EndsWith("404"))
            {
                statusCode = 404;
            }
            return portal.Error(ctx,statusCode);
        }
    }
}