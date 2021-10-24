/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Stand.Abstracts;
using JR.Stand.Core.Utils;

namespace JR.Stand.Core.Web.Cache.Component
{
	 /// <summary>
    /// 缓存基础类
    /// </summary>
    public abstract class CacheBase : ICache
    {

        private T GetCacheResult<T>(string cacheKey, BuiltCacheResultHandler<T> func, bool autoCache)
        {
            T data = default(T);

            object _data = this.Get(cacheKey);
            if (_data == null)
            {
                data = func();

                if (autoCache) this.Insert(cacheKey, data);
            }
            else
            {
                data = (T)_data;
            }
            return data;
        }


        //
        // public virtual void Insert(string key, object value, DateTime absoluteExpireTime)
        // {
        //     HttpRuntime.Cache.Insert(key, value,null,absoluteExpireTime,TimeSpan.Zero);
        // }
        //
        // public virtual void Insert(string key, object value)
        // {
        //     HttpRuntime.Cache.Insert(key, value);
        // }
        //
        // public void Insert(string key, object value, string filename)
        // {
        //     HttpRuntime.Cache.Insert(key, value, new CacheDependency(filename)
        //         , System.Web.Caching.Cache.NoAbsoluteExpiration
        //         , TimeSpan.Zero);
        // }
        //
        // public virtual void Clear(string keySign)
        // {
        //     if (keySign != null)
        //     {
        //         foreach (DictionaryEntry dict in HttpRuntime.Cache)
        //         {
        //             if (dict.Key.ToString().Contains(keySign))
        //                 HttpRuntime.Cache.Remove(dict.Key.ToString());
        //         }
        //     }
        // }
        //
        // public virtual object Get(string key)
        // {
        //     return HttpRuntime.Cache[key];
        // }

        public virtual string Rebuilt()
        {
            return "";
        }

        public abstract IMemoryCacheWrapper RawCache();


        /// <summary>
        /// 获取缓存结果
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetCachedResult<T>(string cacheKey, BuiltCacheResultHandler<T> func)
        {
            return GetCacheResult<T>(cacheKey, func,true);
        }



        public T GetResult<T>(string cacheKey, BuiltCacheResultHandler<T> func)
        {
            return GetCacheResult<T>(cacheKey, func, false);
        }


        public abstract void Insert(string key, object value, DateTime absoluteExpireTime);

        public abstract void Insert(string key, object value, string filename);

        public abstract void Insert(string key, object value);

        public abstract void Clear(string keySign);

        public abstract object Get(string cacheKey);
        public abstract void Reset(FwHandler handler);

    }

}
