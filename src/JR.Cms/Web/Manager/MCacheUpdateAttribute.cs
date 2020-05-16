/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 23:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Cms.Library.CacheProvider;
using JR.Cms.Library.CacheProvider.CacheCompoment;
using JR.Cms.Library.CacheService;

namespace JR.Cms.Web.Manager
{
    /// <summary>
    /// Description of MCacheUpdateAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MCacheUpdateAttribute : Attribute, ICacheUpdatePolicy
    {
        private CacheSign sign;

        public MCacheUpdateAttribute(string cacheKey)
        {
            Key = cacheKey;
        }

        public MCacheUpdateAttribute(CacheSign sign)
        {
            Key = sign.ToString();
            this.sign = sign;
        }

        public string Key { get; private set; }

        public void Clear(string cacheKey)
        {
            CmsCacheFactory.Singleton.Clear(cacheKey);
        }

        public void Clear()
        {
            var siteId = CmsWebMaster.CurrentManageSite.SiteId;

            if (sign != CacheSign.Unknown)
                if ((sign & CacheSign.Link) != 0)
                    SiteLinkCache.ClearForSite(siteId);
            CmsCacheFactory.Singleton.Clear(Key);
        }
    }
}