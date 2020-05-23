/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Cms.Infrastructure;

namespace JR.Cms.Library.CacheProvider.CacheComponent
{
    public interface ICmsCache
    {
        void Insert(string key, object value, DateTime absoluteExpireTime);
        void Insert(string key, object value, string filename);
        void Insert(string key, object value);


        /// <summary>
        /// 清除缓存KEY带指定符号的缓存
        /// </summary>
        /// <param name="keySign"></param>
        void Clear(string keySign);

        object Get(string cacheKey);

        void Reset(CmsHandler handler);

        /// <summary>
        /// 获取缓存过的结果，如果没有缓存则自动缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="func"></param>
        /// <param name="expiresTime"></param>
        /// <returns></returns>
        T GetCachedResult<T>(string cacheKey, BuiltCacheResultHandler<T> func, DateTime expiresTime);


        string Rebuilt();
        
        /// <summary>
        /// 获取Int类型的缓存,如果缓存不存在或类型不匹配,返回-1
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        int GetInt(string key);
    }
}