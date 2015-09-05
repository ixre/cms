
/*
* Copyright(C) 2010-2013 Z3Q.NET
* 
* File Name	: TemplateUrlRule
* publisher_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using J6.Cms.Infrastructure;
using J6.DevFw.Framework;
using J6.DevFw.Framework.IO;
using J6.DevFw.Framework.Web.Cache;

namespace J6.Cms.Cache.CacheCompoment
{
    /// <summary>
    /// Cms注入缓存
    /// </summary>
    public class CmsDependCache : CmsCacheBase
    {
        private readonly string _cacheDependFile;

        internal CmsDependCache()
        {
            _cacheDependFile = Variables.PhysicPath + "config/cache.pid";
        }


        public override void Insert(string key, object value, DateTime absoluteExpireTime)
        {
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(_cacheDependFile), absoluteExpireTime, TimeSpan.Zero);
        }

        public override void Insert(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(_cacheDependFile),
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                System.Web.Caching.Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 重建缓存
        /// </summary>
        public override string Rebuilt()
        {
            //初始化config文件夹
            if (!Directory.Exists(String.Concat(Variables.PhysicPath, "config/")))
            {
                Directory.CreateDirectory(String.Concat(Variables.PhysicPath, "config/")).Create();
            }

            using (FileStream fs = new FileStream(_cacheDependFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] pid = Encoding.UTF8.GetBytes(new Random().Next(1000, 5000).ToString());
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(pid, 0, pid.Length);
                fs.Flush();
            }

            return  IoUtil.GetFileSHA1(_cacheDependFile);
            
            //FileInfo file = new FileInfo(cacheDependFile);
            //file.LastWriteTimeUtc = DateTime.UtcNow;
        }

    }



    /// <summary>
    /// CMS缓存处理
    /// </summary>
    public class CmsCache : ICmsCache
    {
        private readonly ICmsCache _dependCache;
        private static string _cacheSha1ETag;    //客户端ETag

        internal CmsCache(ICmsCache cache)
        {
            _dependCache = cache;
            this.Reset(null);
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
            return _dependCache.GetCachedResult<T>(cacheKey, func,expiresTime);
        }

        /// <summary>
        /// 重置系统缓存(不包括Framework Cache)
        /// </summary>
        /// <param name="handler"></param>
        public void Reset(CmsHandler handler)
        {
            //清除系统缓存
            _cacheSha1ETag = _dependCache.Rebuilt();

            if (handler != null)
            {
                handler();
            }
        }
         
        public bool CheckClientCacheExpires(int seconds)
        {
            return CacheUtil.CheckClientCacheExpires(seconds);
        }

        public bool CheckClientCacheExpiresByEtag()
        {
            return CacheUtil.CheckClientCacheExpires(_cacheSha1ETag);
        }


        public void SetClientCache(HttpResponse response, int seconds)
        {
            CacheUtil.SetClientCache(response, seconds);
        }

        public void SetClientCacheByEtag(HttpResponse response)
        {
            CacheUtil.SetClientCache(response, _cacheSha1ETag);
        }
        
        public void ETagOutput(HttpResponse response,StringCreatorHandler handler)
        {
        	CacheUtil.Output(response,_cacheSha1ETag,handler);
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

        public void Clear(string keySign)
        {
            _dependCache.Clear(keySign);
        }

        public object Get(string cacheKey)
        {
            return _dependCache.Get(cacheKey);
        }

        public string Rebuilt()
        {
            return _dependCache.Rebuilt();
        }

        #endregion


    }
}
