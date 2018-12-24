/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 23:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Web;
using JR.Cms.Cache;
using JR.Cms.Cache.CacheCompoment;
using JR.Cms.CacheService;
using JR.Cms.Cache;
using JR.Cms.Web.WebManager;

namespace JR.Cms.WebManager
{
	/// <summary>
	/// Description of MCacheUpdateAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class MCacheUpdateAttribute:Attribute,ICacheUpdatePolicy
	{
        private CacheSign sign;
		public MCacheUpdateAttribute(string cacheKey)
		{
			this.Key=cacheKey;
		}
		public MCacheUpdateAttribute(CacheSign sign)
		{
			this.Key=sign.ToString();
            this.sign = sign;
		}
		public string Key{get;private set;}
		
		public void Clear(string cacheKey)
		{
            CacheFactory.Sington.Clear(cacheKey);
		}
		
		public void Clear()
        {
            int siteId = CmsWebMaster.CurrentManageSite.SiteId;

            if (this.sign != CacheSign.Unknown)
            {
                if ((this.sign & CacheSign.Link)!=0)
                {
                    SiteLinkCache.ClearForSite(siteId);
                }
            }
            CacheFactory.Sington.Clear(this.Key);
		}
	}
}
