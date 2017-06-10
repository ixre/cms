//
// Copyright 2011 @ OPS Inc,All rights reseved.
// Name: CacheUtility.cs
// Author: newmin
//
namespace Spc
{
    using System;
    using System.Web;
    using System.Collections.Generic;
    using Models;
    using System.Data.Extensions;

    [Obsolete]
    public class CacheUtility
    {
        private static ArchiveBLL bll = new ArchiveBLL();
        /// <summary>
        /// 获取单个文章并缓存
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public static Archive GetArchive(string idOrAlias)
        {
            string cacheName = "cache_archive_" + idOrAlias;
            Archive a = HttpRuntime.Cache[cacheName] as Archive;
            if (a == null)
            {
                a = bll.Get(idOrAlias);
                if (a == null) return null;
                HttpRuntime.Cache.Insert(cacheName, a, null, DateTime.Now.AddMinutes(100), TimeSpan.Zero);
            }
            a.ViewCount++;
            return a;
        }
        /// <summary>
        /// 清除单个文章的缓存
        /// </summary>
        [Obsolete]
        public static void ClearSingleArchiveCache(string idOrAlias)
        {
            Archive a = bll.Get(idOrAlias);
            HttpRuntime.Cache.Remove("cache_archive_" + a.ID.ToString());
            HttpRuntime.Cache.Remove("cache_archive_" + a.Alias.ToString());
        }

        [Obsolete]
        public static IList<Archive> GetArchives(int categoryID, int number)
        {
            string cacheName = "cache_archive_" + categoryID.ToString() + "_" + number.ToString();
            IList<Archive> archives = HttpRuntime.Cache[cacheName] as IList<Archive>;
            if (archives == null)
            {
                archives = bll.GetArchives(categoryID, number).ToEntityList<Archive>();
                HttpRuntime.Cache.Insert(cacheName, archives, null, DateTime.Now.AddMinutes(100), TimeSpan.Zero);
            }
            return archives;
        }

    }
}