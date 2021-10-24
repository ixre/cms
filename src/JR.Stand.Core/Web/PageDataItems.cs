/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reserved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : CmsContext.cs
 * author : newmin (new.min@msn.com)
 * date : 2014/12/01 23:00:00
 * description : 
 * history : 
 */

using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Web;

namespace JR.Stand.Core.Web
{
    /// <summary>
    /// 页面数据项
    /// </summary>
    public class PageDataItems
    {
        private readonly ICompatibleHttpContext _context;

        internal PageDataItems(ICompatibleHttpContext ctx)
        {
            this._context = ctx;
        }
        public object this[string key]
        {
            get
            {
                if (this._context.TryGetItem<object>(key, out var v))
                {
                    return v;
                }
                return null;
            }
            set
            {
                this._context.SaveItem(key,value);
            }
        }
    }

}