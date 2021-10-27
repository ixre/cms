using System.Threading.Tasks;
using JR.Stand.Abstracts.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Web
{
    public class HttpResponseImpl : ICompatibleResponse
    {
        private readonly IHttpContextAccessor _accessor;

        public HttpResponseImpl(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
        }

        public Task WriteAsync(string str)
        {
           return this._accessor.HttpContext.Response.WriteAsync(str);
        }

        public void AddHeader(string key, string value)
        {
            this._accessor.HttpContext.Response.Headers.Add(key,value);
        }

        public void StatusCode(int status)
        {
            this._accessor.HttpContext.Response.StatusCode = status;
        }

        public void AppendCookie(string key, string value, HttpCookieOptions opt)
        {
            this._accessor.HttpContext.Response.Cookies.Append(key,value,this.ParseOptions(opt));
        }

        public void DeleteCookie(string key, HttpCookieOptions opt)
        {
            this._accessor.HttpContext.Response.Cookies.Delete(key,this.ParseOptions(opt));
        }

        private CookieOptions ParseOptions(HttpCookieOptions opt)
        {
            return new CookieOptions
            {
                Domain = opt.Domain,
                Expires = opt.Expires,
                HttpOnly = opt.HttpOnly,
                IsEssential = opt.IsEssential,
                MaxAge = opt.MaxAge,
                Path = opt.Path,
                Secure = opt.Secure,
            };
        }

        public void Redirect(string url, bool permanent)
        {
            this._accessor.HttpContext.Response.Redirect(url,permanent);
        }

        public void AppendHeader(string key, string value)
        {
            this._accessor.HttpContext.Response.Headers.Append(key,value);
        }

        public void ContentType(string contentType)
        {
            this._accessor.HttpContext.Response.ContentType = contentType;
        }

        public void Write(byte[] bytes, int offset, int count)
        {
            this._accessor.HttpContext.Response.Body.Write(bytes,offset,count);
        }

        public Task WriteAsync(byte[] bytes)
        {
            return this._accessor.HttpContext.Response.Body.WriteAsync(bytes,0,bytes.Length);
        }
    }
}