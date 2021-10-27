using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using JR.Stand.Abstracts;
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
                if (cacheEnum.Key != null && cacheEnum.Key.ToString().Equals(key,StringComparison.OrdinalIgnoreCase))
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

        public void Reset()
        {
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                if (cacheEnum.Key != null)
                {
                    cache.Remove(cacheEnum.Key.ToString());
                }
            }
        }


        public List<string> GetCacheKeys()
        {
            var list = new List<string>();
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                if (cacheEnum.Key != null)
                {
                    list.Add(cacheEnum.Key.ToString());
                }
            }
            return list;
        }

        /// <summary>
        /// 搜索 匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<string> SearchKeys(string pattern)
        {
            var cacheKeys = GetCacheKeys();
            var l = cacheKeys.Where(k => Regex.IsMatch(k, pattern)).ToList();
            return l.AsReadOnly();
        }


        public int GetInt(string key)
        {
            return TypesConv.SafeParseInt(this.Get(key),-1);
        }

        public string GetString(string key)
        {
            return this.Get(key) as string;
        }
    }
}