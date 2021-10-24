/*
* Copyright(C) 2010-2013 OPS.CC
* 
* File Name	: TemplateUrlRule
* author_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using System;
using System.IO;
using System.Text;
using JR.Cms.Infrastructure;
using JR.Stand.Abstracts;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.Web.Cache;
using JR.Stand.Core.Utils;

namespace JR.Cms.Library.CacheProvider.CacheComponent
{
    /// <summary>
    /// Cms注入缓存
    /// </summary>
    public class CmsDependCache : CmsCacheBase
    {
        private readonly string _cacheDependFile;
        private IMemoryCacheWrapper cache;

        internal CmsDependCache(IMemoryCacheWrapper cache)
        {
            _cacheDependFile = Variables.PhysicPath + "config/cache.pid";
            this.cache = cache;
        }


        public override void Insert(string key, object value, DateTime absoluteExpireTime)
        {
            cache.Set(key, value, TimeSpan.Zero,absoluteExpireTime - DateTime.Now);
            // HttpRuntime.Cache.Insert(key, value, new CacheDependency(_cacheDependFile), absoluteExpireTime, TimeSpan.Zero);
        }

        public override void Insert(string key, object value, string filename)
        {
            cache.Set(key, value, TimeSpan.Zero);
        }

        public override void Clear(string keySign)
        {
            if (keySign == null) return;
            foreach (var key in cache.GetCacheKeys())
            {
                if (key.StartsWith(keySign))
                {
                    cache.Remove(key);
                }
            }
        }

        public override object Get(string key)
        {
            return this.cache.Get(key);
        }

        public override void Reset(CmsHandler handler)
        {
            this.cache.Reset();
            handler?.Invoke();
        }

        public override void Insert(string key, object value)
        {
            cache.Set(key, value, TimeSpan.Zero);
            // HttpRuntime.Cache.Insert(key, value, new CacheDependency(_cacheDependFile),
            //     System.Web.Caching.Cache.NoAbsoluteExpiration,
            //     System.Web.Caching.Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 重建缓存
        /// </summary>
        public override string Rebuilt()
        {
            //初始化config文件夹
            if (!Directory.Exists(string.Concat(Variables.PhysicPath, "config/")))
                Directory.CreateDirectory(string.Concat(Variables.PhysicPath, "config/")).Create();
            using (var fs = new FileStream(_cacheDependFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var pid = Encoding.UTF8.GetBytes(new Random().Next(100000, 500000).ToString());
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(pid, 0, pid.Length);
                fs.Flush();
            }

            return IoUtil.GetFileSha1(_cacheDependFile);

            //FileInfo file = new FileInfo(cacheDependFile);
            //file.LastWriteTimeUtc = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public override int GetInt(string key)
        {
            return this.cache.GetInt(key);
        }

        /// <summary>
        /// 删除指定前缀的键
        /// </summary>
        /// <param name="prefix"></param>
        public override void RemoveKeys(string prefix)
        {
            var list = this.cache.SearchKeys("^" + prefix);
            foreach (var l in list)
            {
                this.cache.Remove(l);
            }
        }
    }


    /// <summary>
    /// CMS缓存处理
    /// </summary>
    public class CmsCache : ICmsCache
    {
        private readonly ICmsCache _dependCache;
        private static string _cacheSha1ETag; //客户端ETag

        internal CmsCache(ICmsCache cache)
        {
            _dependCache = cache;
            Reset(null);
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
            return _dependCache.GetCachedResult<T>(cacheKey, func, expiresTime);
        }

        /// <summary>
        /// 重置系统缓存(不包括Framework Cache)
        /// </summary>
        /// <param name="handler"></param>
        public void Reset(CmsHandler handler)
        {
            //清除系统缓存
            _cacheSha1ETag = this._dependCache.Rebuilt();
            this._dependCache.Reset(handler);
        }

        public bool CheckClientCacheExpires(int seconds)
        {
            return CacheUtil.CheckClientCacheExpires(seconds);
        }

        public bool CheckClientCacheExpiresByEtag()
        {
            return CacheUtil.CheckClientCacheExpires(_cacheSha1ETag);
        }


        public void SetClientCache(ICompatibleResponse response, int seconds)
        {
            CacheUtil.SetClientCache(response, seconds);
        }

        public void SetClientCacheByEtag(ICompatibleResponse response)
        {
            CacheUtil.SetClientCache(response, _cacheSha1ETag);
        }

        public void ETagOutput(ICompatibleResponse response, StringCreatorHandler handler)
        {
            CacheUtil.Output(response, _cacheSha1ETag, handler);
        }

        #region 接口方法

        public void Insert(string key, object value, DateTime absoluteExpireTime)
        {
            _dependCache.Insert(key, value, absoluteExpireTime);
        }

        public void Insert(string key, object value, string filename)
        {
            _dependCache.Insert(key, value, filename);
        }

        public void Insert(string key, object value)
        {
            _dependCache.Insert(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySign"></param>
        public void Clear(string keySign)
        {
            _dependCache.Clear(keySign);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public object Get(string cacheKey)
        {
            return _dependCache.Get(cacheKey);
        }

        public string Rebuilt()
        {
            return _dependCache.Rebuilt();
        }

        #endregion

        /// <inheritdoc />
        public int GetInt(string key)
        {
            return this._dependCache.GetInt(key);
        }

        public void RemoveKeys(string prefix)
        {
            this._dependCache.RemoveKeys(prefix);
        }
    }
}