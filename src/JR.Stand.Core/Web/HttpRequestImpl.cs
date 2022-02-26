using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Extensions.Http;
using JR.Stand.Core.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using JsonSerializer = JR.Stand.Core.Framework.JsonSerializer;

namespace JR.Stand.Core.Web
{
    public class HttpRequestImpl : ICompatibleRequest
    {
        private readonly IHttpContextAccessor _accessor;

        public HttpRequestImpl(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
        }

        private HttpContext Context()
        {
            return this._accessor.HttpContext;
        }

        public string Method()
        {
            return this._accessor.HttpContext.Request.Method;
        }

        public string GetHeader(string key)
        {
           return this.Context().Request.Headers[key];
        }

        public string GetHost()
        {
            var port = this.Context().Request.Host.Port ?? 80;
            var host =  this.Context().Request.Host.Host;
            return port == 80 ? host : $"{host}:{port}";
        }

        public string GetProto()
        {
            if (this.Context().Request.Scheme == "https") return "https";
            return HttpUtils.IsProxyHttpsRequest(this) ? "https" : "http";
        }

        public string GetApplicationPath()
        {
            return  this.Context().Request.PathBase.Value;
        }
        
        public string GetPath()
        {
            return this.Context().Request.Path.Value;
        }

        public string GetQueryString()
        {
            return this.Context().Request.QueryString.Value;
        }

        public bool TryGetHeader(string key, out StringValues value)
        {
            return this.Context().Request.Headers.TryGetValue(key, out value);
        }

        public string UrlEncode(string url)
        {
            return WebUtility.UrlEncode(url);
        }

        public string UrlDecode(string url)
        {
            return WebUtility.UrlDecode(url);
        }


        public StringValues Query(string varKey)
        {
            return this.Context().Request.Query[varKey];
        }

        public StringValues Form(string varKey)
        {
            return this.Context().Request.Form[varKey];
        }

        public bool TryGetCookie(string key, out string o)
        {
            return this.Context().Request.Cookies.TryGetValue(key, out o);
        }

        public IEnumerable<string> CookiesKeys()
        {
            return this.Context().Request.Cookies.Keys;
        }

        public string GetEncodedUrl()
        {
            return this.Context().Request.GetEncodedUrl();
        }

        public IEnumerable<string> FormKeys()
        {
            return this.Context().Request.Form.Keys;
        }

        public T ParseFormToEntity<T>()
        {
            return this.Context().Request.Form.ConvertToEntity<T>();
        }

        public string GetParameter(string key)
        {
            return this.Context().Request.GetParameter(key);
        }

        public ICompatiblePostedFile File(string key)
        {
            var file = this.Context().Request.Form.Files[key];
            return this.ParsePostedFile(file);
        }

        public ICompatiblePostedFile FileIndex(int i)
        {
            var file = this.Context().Request.Form.Files[i];
            return this.ParsePostedFile(file);
        }

        public IDictionary<string, StringValues> Headers()
        {
            return this.Context().Request.Headers;
        }

        public T Bind<T>()
        {
            StreamReader sr = new StreamReader(this.Context().Request.Body);
            String body = sr.ReadToEnd();
            return JsonSerializer.DeserializeObject<T>(body);
        }


        private ICompatiblePostedFile ParsePostedFile(IFormFile file)
        {
            if (file != null) return new PostedFileImpl(file);
            return null;
        }
    }
}