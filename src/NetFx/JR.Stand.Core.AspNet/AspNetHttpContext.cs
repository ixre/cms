using System.Web;
using JR.Stand.Abstracts;
using JR.Stand.Abstracts.Web;

namespace JR.Stand.Core.AspNet
{
    public  class AspNetHttpContext:ICompatibleHttpContext
    {
        internal AspNetHttpContext()
        {
            this.Request = new AspNetHttpRequestImpl();
            this.Response = new AspNetHttpResponseImpl();
            this.Session = new AspNetSessionImpl();
            this.Hosting = new AspNetHostingImpl();
        }
    
    public ICompatibleRequest Request { get; }
    public ICompatibleResponse Response { get; }
    public ICompatibleSession Session { get; }


    public IServeHosting Hosting { get; }

    public object RawContext()
        {
            return HttpContext.Current;
        }

        public bool TryGetItem<T>(string key, out T value)
        {
            var v = HttpContext.Current.Items[key];
            if (v != null)
            {
                value = (T) v;
            }
            else
            {
                value = default;
            }

            return v != null;
        }

        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <returns></returns>
        public string RequestPath()
        {
            return HttpContext.Current.Request.Path;
        }

        /// <summary>
        /// 设置上下文项目
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveItem(string key, object value)
        {
            HttpContext.Current.Items[key] = value;
        }

        public string RemoteAddress()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }
    }
}