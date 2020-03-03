using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Framework.Web
{
    public  class HttpContextAdapter
    {
        private static IHttpContextAccessor _accessor;
        private static readonly HttpContextAdapter Instance = new HttpContextAdapter();

        public  HttpContext Raw => _accessor.HttpContext;

        private HttpContext Context => _accessor.HttpContext;

        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 获取当前上下文 
        /// </summary>
        public static HttpContextAdapter Current => HttpContextAdapter.Instance;

        /// <summary>
        /// 获取上下文项目
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetItem(string key)
        {
            var b = this.Context.Items.TryGetValue(key, out var r);
            return b ? r : null;
        }

        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <returns></returns>
        public string RequestPath()
        {
            return _accessor.HttpContext.Request.Path;
        }

        /// <summary>
        /// 设置上下文项目
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveItem(string key, object value)
        {
            this.Context.Items[key] = value;
        }

        public IFormCollection Form()
        {
            return this.Context.Request.Form;
        }
    }
}