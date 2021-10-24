/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Cms.Infrastructure;

namespace JR.Cms.Library.CacheProvider.CacheComponent
{
    /// <summary>
    /// 缓存基础类
    /// </summary>
    public abstract class CmsCacheBase : ICmsCache
    {
        private T GetCacheResult<T>(string cacheKey, BuiltCacheResultHandler<T> func, bool autoCache,
            DateTime expiresTime)
        {
            var data = default(T);

            var _data = Get(cacheKey);
            if (_data == null)
            {
                data = func();

                if (autoCache) Insert(cacheKey, data, expiresTime);
            }
            else
            {
                data = (T) _data;
            }

            return data;
        }


        public abstract void Insert(string key, object value, DateTime absoluteExpireTime);

        public abstract void Insert(string key, object value);

        public abstract void Insert(string key, object value, string filename);

        public abstract void Clear(string keySign);

        public abstract object Get(string key);

        public virtual string Rebuilt()
        {
            return "";
        }

        public abstract int GetInt(string key);
        public abstract void RemoveKeys(string prefix);


        /// <summary>
        /// 获取缓存结果
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="func"></param>
        /// <param name="expiresTime"></param>
        /// <returns></returns>
        public T GetCachedResult<T>(string cacheKey, BuiltCacheResultHandler<T> func, DateTime expiresTime)
        {
            return GetCacheResult<T>(cacheKey, func, true, expiresTime);
        }


        public abstract void Reset(CmsHandler handler);
    }
}