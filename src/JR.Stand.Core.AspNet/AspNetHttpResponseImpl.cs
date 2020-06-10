using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JR.Stand.Abstracts.Safety;
using JR.Stand.Abstracts.Web;
using Microsoft.AspNetCore.Http;
using HttpContext = System.Web.HttpContext;

namespace JR.Stand.Core.AspNet
{
    internal class AspNetHttpResponseImpl : ICompatibleResponse
    {

        private static HttpContext Context=>HttpContext.Current;


        public Task WriteAsync(string content)
        {
            Context.Response.Write(content);
            return SafetyTask.CompletedTask;
        }

        public void AddHeader(string key, string value)
        {
            try
            {
                
                // 如果在经典模式下,提示： 此操作要求使用 IIS 集成管线模式。 
                Context.Response.Headers.Add(key, value);
            }
            catch(PlatformNotSupportedException ex)
            {
                //IIS经典模式
                Context.Response.AppendHeader(key,value);
            }
        }

        public void StatusCode(int status)
        {
            Context.Response.StatusCode = status;
        }

        public void AppendCookie(string key, string value, HttpCookieOptions opt)
        {
            HttpCookie cookie = new HttpCookie(key,value);
            CopyCookieOptions( cookie,opt);
            Context.Response.AppendCookie(cookie);
        }

        private static void CopyCookieOptions(HttpCookie cookie,HttpCookieOptions opt)
        {
            if (opt != null)
            {
                if (opt.Expires != null)
                {
                    cookie.Expires = DateTime.Now.Add(opt.Expires.Value.Offset);
                }

                if (opt.Path != null)
                {
                    cookie.Path = opt.Path;
                }

                if (opt.MaxAge != null)
                {
                    cookie.Expires = DateTime.Now.Add(opt.MaxAge.Value);
                }

                if (opt.Domain != null)
                {
                    cookie.Domain = opt.Domain;
                }

                cookie.HttpOnly = opt.HttpOnly;
            }
        }

        public void DeleteCookie(string key, HttpCookieOptions opt)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Expires = DateTime.Now.AddSeconds(-1*10E9);
            CopyCookieOptions(cookie,opt);
        }

        public void Redirect(string url, bool permanent)
        {
            if (permanent)
            {
                Context.Response.RedirectPermanent(url,true);
            }
            else
            {
                Context.Response.Redirect(url, true);
            }
        }

        public void AppendHeader(string key, string value)
        {
            Context.Response.AppendHeader(key,value);
        }

        public void ContentType(string contentType)
        {
            Context.Response.ContentType = contentType;
        }

        public void Write(byte[] bytes, int offset, int count)
        {
            Context.Response.BinaryWrite(bytes);
        }

        public Task WriteAsync(byte[] bytes)
        {
            Context.Response.BinaryWrite(bytes);
            return SafetyTask.CompletedTask;
        }
    }
}