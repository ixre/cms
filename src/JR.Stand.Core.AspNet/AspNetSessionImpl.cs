using System.Web;
using JR.Stand.Abstracts.Web;
using Newtonsoft.Json;
using System.Linq;

namespace JR.Stand.Core.AspNet
{
    internal class AspNetSessionImpl : ICompatibleSession
    {
        private static HttpContext Context => HttpContext.Current;


        public void SetInt32(string key, in int value)
        {
            Context.Session[key] = value;
        }

        public int GetInt32(string key)
        {
            var context = HttpContext.Current.Session;
            if (this.CheckSessionKey(key))
            {
                var o = Context.Session[key];
                if (o != null) return (int) o;
            }

            return 0;
        }

        private bool CheckSessionKey(string key)
        {
            foreach (string k in Context.Session.Keys)
            {
                if (k == key) return true;
            }

            return false;
        }

        public void SetString(string key, string value)
        {
            Context.Session[key] = value;
        }

        public string GetString(string key)
        {
            if (this.CheckSessionKey(key))
            {
                var o = Context.Session[key];
                if (o != null) return (string) o;
            }

            return "";
        }

        public T GetObjectFromJson<T>(string key)
        {
            if (this.CheckSessionKey(key))
            {
                var value = this.GetString(key);
                return value == null ? default : JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public void SetObjectAsJson(string key, object value)
        {
            Context.Session[key]= JsonConvert.SerializeObject(value);   
        }

        public void Remove(string key)
        {
            Context.Session.Remove(key);
        }
    }
}