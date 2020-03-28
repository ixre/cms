using System;
using System.Collections.Generic;

namespace JR.Stand.Core.Utils
{
    public interface IMemoryCacheWrapper
    {
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiresAbsolute">绝对过期时长</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiresAbsolute);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresAbsolute">绝对过期时长</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan expiresAbsolute);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        void Remove(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        void RemoveCacheAll();
        
        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        List<string> GetCacheKeys();
    }
}