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

        public void AppendCookie(string key, string value, CookieOptions opt)
        {
            this._accessor.HttpContext.Response.Cookies.Append(key,value,opt);
        }

        public void DeleteCookie(string key, CookieOptions opt)
        {
            this._accessor.HttpContext.Response.Cookies.Delete(key,opt);
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

        public void WriteAsync(byte[] bytes)
        {
            this._accessor.HttpContext.Response.Body.Write(bytes,0,bytes.Length);
        }
    }
}