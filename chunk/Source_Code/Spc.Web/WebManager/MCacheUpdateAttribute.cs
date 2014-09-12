/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 23:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Ops.Cms.Cache;
using Ops.Cms.Cache.CacheCompoment;

namespace Ops.Cms.WebManager
{
	/// <summary>
	/// Description of MCacheUpdateAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class MCacheUpdateAttribute:Attribute,ICacheUpdatePolicy
	{
		public MCacheUpdateAttribute(string cacheKey)
		{
			this.Key=cacheKey;
		}
		public MCacheUpdateAttribute(CacheSign sign)
		{
			this.Key=sign.ToString();
		}
		public string Key{get;private set;}
		
		public void Clear(string cacheKey)
		{
            CacheFactory.Sington.Clear(cacheKey);
		}
		
		public void Clear()
        {
            CacheFactory.Sington.Clear(this.Key);
		}
	}
}
