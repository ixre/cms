/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Stand.Abstracts;
using JR.Stand.Core.Utils;

namespace JR.Stand.Core.Web.Cache.Component
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpireTime"></param>
        void Insert(string key, object value, DateTime absoluteExpireTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filename"></param>
        void Insert(string key, object value, String filename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Insert(string key, object value);


        /// <summary>
        /// 清除缓存KEY带指定符号的缓存
        /// </summary>
        /// <param name="keySign"></param>
        void Clear(string keySign);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        object Get(string cacheKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        void Reset(FwHandler handler);

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string Rebuilt();

        IMemoryCacheWrapper RawCache();
    }
}
