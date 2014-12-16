using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ops.Cms.Infrastructure;
using Ops.Cms.Infrastructure.KV;
using kv = Ops.Cms.Infrastructure;

namespace Ops.Cms.CacheService
{
    public static class Kvdb
    {
        /// <summary>
        /// 全局缓存
        /// </summary>
        public static LevelDb GCA
        {
            get { return kv.Kvdb._currentInstance;}
        }
    }
}
