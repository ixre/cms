
/*
* Copyright(C) 2010-2013 S1N1.COM
* 
* File Name	: TemplateUrlRule
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using System;
using JR.Stand.Abstracts;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Utils;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Web.Cache.Component
{
    /// <summary>
    /// CMS缓存处理
    /// </summary>
    public class BasicCache : ICache
    {
        private readonly ICache dependCache;
        private static string _cacheSha1ETag;    //客户端ETag

        internal BasicCache(ICache cache)
        {
            dependCache = cache;
            this.Reset(null);
        }

        /// <summary>
        /// 获取缓存结果
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetCachedResult<T>(string cacheKey, BuiltCacheResultHandler<T> func)
        {
            return dependCache.GetCachedResult<T>(cacheKey, func);
        }

        public T GetResult<T>(string cacheKey, BuiltCacheResultHandler<T> func)
        {
            return dependCache.GetResult<T>(cacheKey, func);
        }

        /// <summary>
        /// 重置系统缓存(不包括Framework Cache)
        /// </summary>
        /// <param name="handler"></param>
        public void Reset(FwHandler handler)
        {
            //清除系统缓存
            _cacheSha1ETag = dependCache.Rebuilt();

            if (handler != null)
            {
                handler();
            }
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool CheckClientCacheExpires(int seconds)
        {
            throw new NotImplementedException();
            //return CacheUtil.CheckClientCacheExpires(seconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckClientCacheExpiresByEtag()
        {            throw new NotImplementedException();

            //return CacheUtil.CheckClientCacheExpires(_cacheSha1ETag);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="seconds"></param>
        public void SetClientCache(HttpResponse response, int seconds)
        {
            throw new NotImplementedException();
            //CacheUtil.SetClientCache(response, seconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        public void SetClientCacheByEtag(HttpResponse response)
        {
            throw new NotImplementedException();
            //CacheUtil.SetClientCache(response, _cacheSha1ETag);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="handler"></param>
        public void ETagOutput(HttpResponse response,StringCreatorHandler handler)
        {
            throw new NotImplementedException();
            //CacheUtil.Output(response,_cacheSha1ETag,handler);
        }

        #region 接口方法

        public void Insert(string key, object value, DateTime absoluteExpireTime)
        {
            dependCache.Insert(key, value, absoluteExpireTime);
        }

        public void Insert(string key, object value, string filename)
        {
            dependCache.Insert(key, value, filename);
        }

        public void Insert(string key, object value)
        {
            dependCache.Insert(key, value);
        }

        public void Clear(string keySign)
        {
            dependCache.Clear(keySign);
        }

        public object Get(string cacheKey)
        {
            return dependCache.Get(cacheKey);
        }

        public string Rebuilt()
        {
            return dependCache.Rebuilt();
        }

        public IMemoryCacheWrapper RawCache()
        {
            return this.dependCache.RawCache();
        }

        #endregion


    }
}
