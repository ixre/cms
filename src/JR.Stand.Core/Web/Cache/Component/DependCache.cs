using System;
using System.IO;
using System.Text;
using JR.Stand.Abstracts;
using JR.Stand.Core.Utils;

namespace JR.Stand.Core.Web.Cache.Component
{
    /// <summary>
    /// 注入缓存
    /// </summary>
    public class DependCache : CacheBase
    {
        private static readonly string CacheDependFile;
        private readonly IMemoryCacheWrapper _cacheWrapper;

        internal DependCache(IMemoryCacheWrapper cache)
        {
            this._cacheWrapper = cache;
        }
        static DependCache()
        {
            CacheDependFile = Variables.PhysicPath + "tmp/cache.pid";
        }

        public override void Insert(string key, object value, DateTime absoluteExpireTime)
        {
            this._cacheWrapper.Set(key, value, TimeSpan.Zero, absoluteExpireTime - DateTime.Now);
            // HttpRuntime.Cache.Insert(key, value, new CacheDependency(CacheDependFile), absoluteExpireTime, TimeSpan.Zero);
        }

        public override void Insert(string key, object value, string filename)
        {
            throw new NotImplementedException();
        }

        public override void Insert(string key, object value)
        {
            this._cacheWrapper.Set(key, value, TimeSpan.Zero,TimeSpan.Zero);

            // HttpRuntime.Cache.Insert(key, value, new CacheDependency(CacheDependFile),
            //     System.Web.Caching.Cache.NoAbsoluteExpiration,
            //     System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public override void Clear(string keySign)
        {
            this._cacheWrapper.GetCacheKeys().ForEach((k) =>
            {
                if(k.StartsWith(keySign))this._cacheWrapper.Remove(k);
            });
        }

        public override object Get(string cacheKey)
        {
            return this._cacheWrapper.Get(cacheKey);
        }

        public override void Reset(FwHandler handler)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 重建缓存
        /// </summary>
        public override string Rebuilt()
        {
            //初始化config文件夹
            if (!Directory.Exists(CacheDependFile))
            {
                var folder = new FileInfo(CacheDependFile).Directory.FullName;
                Directory.CreateDirectory(folder).Create();
            }

            using (FileStream fs = new FileStream(CacheDependFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] pid = Encoding.UTF8.GetBytes(new Random().Next(1000, 5000).ToString());
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(pid, 0, pid.Length);
                fs.Flush();
            }

            return  IoUtil.GetFileSha1(CacheDependFile);
            
            //FileInfo file = new FileInfo(cacheDependFile);
            //file.LastWriteTimeUtc = DateTime.UtcNow;
        }

        public override IMemoryCacheWrapper RawCache()
        {
            return this._cacheWrapper;
        }
    }
}