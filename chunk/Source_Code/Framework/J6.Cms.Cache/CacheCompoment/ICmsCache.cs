/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using J6.Cms.Infrastructure;

namespace J6.Cms.Cache.CacheCompoment
{
	public interface ICmsCache
    {
        void Insert(string key, object value, DateTime absoluteExpireTime);
        void Insert(string key, object value, String filename);
        void Insert(string key, object value);


        /// <summary>
        /// 清除缓存KEY带指定符号的缓存
        /// </summary>
        /// <param name="keySign"></param>
        void Clear(string keySign);

        object Get(string cacheKey);

        void Reset(CmsHandler handler);

        /// <summary>
        /// 获取缓存过的结果，如果没有缓存则自动缓存永不失效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        T GetCachedResult<T>(string cacheKey, BuiltCacheResultHandler<T> func);

        /// <summary>
        /// 获取缓存结果，如果未缓存则调用委托产生的缓存结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        T GetResult<T>(string cacheKey, BuiltCacheResultHandler<T> func);

        string Rebuilt();
    }
}
