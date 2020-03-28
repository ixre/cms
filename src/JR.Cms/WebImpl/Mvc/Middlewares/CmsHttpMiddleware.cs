using Microsoft.AspNetCore.Http;

namespace JR.Cms.WebImpl.Mvc.Middlewares
{
    public class CmsHttpMiddleware
    {
        private readonly RequestDelegate _next;

        public CmsHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public System.Threading.Tasks.Task Invoke(HttpContext ctx)
        {
            return _next(ctx);
        }
    }
}