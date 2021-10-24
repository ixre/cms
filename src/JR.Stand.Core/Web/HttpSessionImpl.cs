using JR.Stand.Abstracts.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Web
{
    internal class HttpSessionImpl : ICompatibleSession
    {
        private readonly IHttpContextAccessor _accessor;

        public HttpSessionImpl(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
        }

        public void SetInt32(string key, in int value)
        {
            this._accessor.HttpContext.Session.SetInt32(key, value);
        }

        public int GetInt32(string key)
        {
            return this._accessor.HttpContext.Session.GetInt32(key)??0;
        }


        public void SetString(string key, string value)
        {
            this._accessor.HttpContext.Session.SetString(key,value);
        }

        public string GetString(string key)
        {
            return this._accessor.HttpContext.Session.GetString(key);
        }

        public T GetObjectFromJson<T>(string key)
        {
            return this._accessor.HttpContext.Session.GetObjectFromJson<T>(key);
        }

        public void SetObjectAsJson(string key, object o)
        {
            this._accessor.HttpContext.Session.SetObjectAsJson(key,o);
        }

        public void Remove(string key)
        {
            this._accessor.HttpContext.Session.Remove(key);
        }
    }
}