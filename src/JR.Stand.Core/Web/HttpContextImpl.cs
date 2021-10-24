using JR.Stand.Abstracts;
using JR.Stand.Abstracts.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Web
{

    public  class HttpContextImpl:ICompatibleHttpContext
    {
        private readonly IHttpContextAccessor _accessor;

        internal HttpContextImpl(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
            this.Request = new HttpRequestImpl(accessor);
            this.Response = new HttpResponseImpl(accessor);
            this.Session = new HttpSessionImpl(accessor);
            this.Hosting = new HttpHostingImpl(accessor);
        }

        private HttpContext Context => _accessor.HttpContext;


        public IServeHosting Hosting { get; }

        public object RawContext()
        {
            return this._accessor.HttpContext;
        }

        public ICompatibleRequest Request { get; }
        public ICompatibleResponse Response { get; }
        public ICompatibleSession Session { get; }

        /// <summary>
        /// 获取上下文项目
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetItem<T>(string key, out T value)
        {
            if (this.Context.Items.TryGetValue(key, out var r))
            {
                value = (T) r;
                return true;
            }
            value = default;
            return false;
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

        public string RemoteAddress()
        {
            return this._accessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}