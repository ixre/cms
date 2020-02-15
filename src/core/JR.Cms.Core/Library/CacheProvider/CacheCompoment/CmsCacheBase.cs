/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using JR.Cms.Infrastructure;

namespace JR.Cms.Library.CacheProvider.CacheCompoment
{
	 /// <summary>
    /// 缓存基础类
    /// </summary>
    public class CmsCacheBase : ICmsCache
    {

        private T GetCacheResult<T>(string cacheKey, BuiltCacheResultHandler<T> func, bool autoCache, DateTime expiresTime)
        {
            T data = default(T);

            object _data = this.Get(cacheKey);
            if (_data == null)
            {
                data = func();

                if (autoCache) this.Insert(cacheKey, data,expiresTime);
            }
            else
            {
                data = (T)_data;
            }
            return data;
        }



        public virtual void Insert(string key, object value, DateTime absoluteExpireTime)
        {
        }

        public virtual void Insert(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value);
        }

        public void Insert(string key, object value, string filename)
        {
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(filename)
                , System.Web.Caching.Cache.NoAbsoluteExpiration
                , TimeSpan.Zero);
        }

        public virtual void Clear(string keySign)
        {
            if (keySign != null)
            {
                foreach (DictionaryEntry dict in HttpRuntime.Cache)
                {
                    String key = dict.Key.ToString();
                    if (key.StartsWith(keySign))  HttpRuntime.Cache.Remove(key);
                }
            }
        }

        public virtual object Get(string key)
        {
            return HttpRuntime.Cache[key];
        }

        public virtual string Rebuilt()
        {
            return "";
        }


	     /// <summary>
	     /// 获取缓存结果
	     /// </summary>
	     /// <param name="cacheKey"></param>
	     /// <param name="func"></param>
	     /// <param name="expiresTime"></param>
	     /// <returns></returns>
	     public T GetCachedResult<T>(string cacheKey, BuiltCacheResultHandler<T> func, DateTime expiresTime)
        {
            return GetCacheResult<T>(cacheKey, func,true,expiresTime);
        }

      


        public virtual void Reset(CmsHandler handler)
        {
        }
    }

}
