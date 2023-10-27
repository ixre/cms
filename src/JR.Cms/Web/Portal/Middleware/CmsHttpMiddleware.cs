using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.Web.Portal.Middlewares
{
    /// <summary>
    /// CMS中间件
    /// </summary>
    public class CmsHttpMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public CmsHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext ctx)
        {
            return _next(ctx);
        }
    }
}