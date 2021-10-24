/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Stand.Core.Web.Cache.Component;

namespace JR.Stand.Core.Web.Cache
{
	/// <summary>
	/// Description of CacheUpdateAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CacheUpdateAttribute:Attribute,ICacheUpdatePolicy
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cacheKey"></param>
		public CacheUpdateAttribute(string cacheKey)
		{
			this.Key=cacheKey;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sign"></param>
		public CacheUpdateAttribute(CacheSign sign)
		{
			this.Key=sign.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public string Key{get;private set;}
		
		
		public void Clear()
		{
            CacheFactory.Sington.Clear(this.Key);
		}
	}
}
