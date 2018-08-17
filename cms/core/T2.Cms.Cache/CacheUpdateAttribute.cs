/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using T2.Cms.Cache.CacheCompoment;

namespace T2.Cms.Cache
{
	/// <summary>
	/// Description of CacheUpdateAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CacheUpdateAttribute:Attribute,ICacheUpdatePolicy
	{
		public CacheUpdateAttribute(string cacheKey)
		{
			this.Key=cacheKey;
		}
		public CacheUpdateAttribute(CacheSign sign)
		{
			this.Key=sign.ToString();
		}
		public string Key{get;private set;}
		
		
		public void Clear()
		{
            CacheFactory.Sington.Clear(this.Key);
		}
	}
}
