using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using JR.Stand.Core.Utils;

namespace JR.Stand.Core.AspNet
{
    public class AspNetCacheWrapper:IMemoryCacheWrapper
    {
        public bool Exists(string key)
        {
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                if (cacheEnum.Key.ToString().Equals(key,StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiresAbsolute)
        {
            HttpRuntime.Cache.Insert(key, value, null,
                DateTime.Now.Add(expiresAbsolute),expiresSliding);
            return true;
        }

        public bool Set(string key, object value, TimeSpan expiresAbsolute)
        {
            HttpRuntime.Cache.Insert(key,value,null,
                DateTime.Now.Add(expiresAbsolute),TimeSpan.Zero);
            return true;
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public T Get<T>(string key) where T : class
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
           return HttpRuntime.Cache[key] as T;
        }

        public object Get(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return  HttpRuntime.Cache[key];
        }

        public void RemoveCacheAll()
        {
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cache.Remove(cacheEnum.Key.ToString());
            }
        }


        public List<string> GetCacheKeys()
        {
            var list = new List<string>();
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                list.Add(cacheEnum.Key.ToString());
            }
            return list;
        }
    }
}