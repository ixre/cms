/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 23:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Cms.Library.CacheProvider.CacheComponent;
using JR.Cms.Library.CacheService;
using JR.Stand.Abstracts.Cache;

namespace JR.Cms.Web.Manager
{
    /// <summary>
    /// Description of MCacheUpdateAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MCacheAttribute : Attribute, ICachePolicy
    {
        private readonly CacheSign _sign;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        public MCacheAttribute(string cacheKey)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sign"></param>
        public MCacheAttribute(CacheSign sign)
        {
            this._sign = sign;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clean()
        {

            if (_sign == CacheSign.Unknown)return;
            if((this._sign & CacheSign.Site) == CacheSign.Site){
                // 重置
                SiteCacheManager.Reset();
            }
            if ((_sign & CacheSign.Link) == CacheSign.Link)
            {
                var siteId = CmsWebMaster.CurrentManageSite.SiteId;
                SiteLinkCache.ClearForSite(siteId);
                Cms.Template.CleanPageCache();
            }
            //CmsCacheFactory.Singleton.Clear(Key);
        }
    }
}