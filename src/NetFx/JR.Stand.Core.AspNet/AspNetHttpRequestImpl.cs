using System;
using System.Collections.Generic;
using System.Web;
using JR.Stand.Abstracts.Web;
using Microsoft.Extensions.Primitives;
using JR.Stand.Core.Framework.Extensions;
using JR.Stand.Core.Web;
using System.IO;
using JR.Stand.Core.Framework;
using System.Text;
using Newtonsoft.Json;

namespace JR.Stand.Core.AspNet
{
    internal class AspNetHttpRequestImpl : ICompatibleRequest
    {
        private HttpContext Context => HttpContext.Current;


        public string Method()
        {
            return this.Context.Request.HttpMethod;
        }

        public string GetHeader(string key)
        {
            return this.Context.Request.Headers[key];
        }

        public string GetHost()
        {
            var url = this.Context.Request.Url;
            var port = url.Port;
            var host = url.Host;
            return port == 80 || port == 443 ? host : $"{host}:{port}";
        }

        public string GetProto()
        {
            return this.GetScheme() == "https" || HttpUtils.IsProxyHttpsRequest(this) 
                ? "https" : "http";
        }


        public string GetApplicationPath()
        {
            return this.Context.Request.ApplicationPath;
        }


        private string GetScheme()
        {
            return this.Context.Request.Url.Scheme;
        }

        public string GetPath()
        {
            return this.Context.Request.Path;
        }

        public string GetQueryString()
        {
            return this.Context.Request.Url.Query;
        }

        public bool TryGetHeader(string key, out StringValues value)
        {
            var b = this.Context.Request.Headers[key];
            if (!String.IsNullOrEmpty(b))
            {
                value = b;
                return true;
            }

            return false;
        }

        public string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        public StringValues Query(string varKey)
        {
            return this.Context.Request.QueryString[varKey];
        }

        public StringValues Form(string key)
        {
            return this.Context.Request.Form[key];
        }

        public bool TryGetCookie(string key, out string o)
        {
            var ck = this.Context.Request.Cookies.Get(key);
            if (ck != null && ck.Expires > DateTime.Now)
            {
                o = ck.Value;
                return true;
            }

            o = "";
            return false;
        }

        public IEnumerable<string> CookiesKeys()
        {
            return this.Context.Request.Cookies.AllKeys;
        }

        public string GetEncodedUrl()
        {
            return this.Context.Request.Url.ToString();
        }

        public IEnumerable<string> FormKeys()
        {
            return this.Context.Request.Form.AllKeys;
        }

        public T ParseFormToEntity<T>()
        {
            return this.Context.Request.Form.ConvertToEntity<T>();
        }

        public string GetParameter(string key)
        {
            return this.Context.Request.Params[key];
        }

        public ICompatiblePostedFile File(string key)
        {
            var file = this.Context.Request.Files[key];
            return this.ParsePostedFile(file);
        }

        public ICompatiblePostedFile FileIndex(int i)
        {
            var file = this.Context.Request.Files[i];
            return this.ParsePostedFile(file);
        }

        public IDictionary<string, StringValues> Headers()
        {
            var headers = new Dictionary<String, StringValues>();
            var h = this.Context.Request.Headers;
            foreach (String key in h.Keys)
            {
                headers.Add(key, h.Get(key));
            }
            return headers;
        }

        private ICompatiblePostedFile ParsePostedFile(HttpPostedFile file)
        {
            if (file != null) return new AspNetPostedFileImpl(file);
            return null;
        }
        public T Bind<T>()
        {
            Stream stream = this.Context.Request.InputStream;
            stream.Position = 0; // 需要重新设置到流的开头
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            String body = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}