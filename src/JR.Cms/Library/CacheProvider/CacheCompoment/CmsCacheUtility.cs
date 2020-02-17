/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Reflection;

namespace JR.Cms.Library.CacheProvider.CacheCompoment
{
	/// <summary>
	/// Description of CmsCacheUtility.
	/// </summary>
	public class CmsCacheUtility
	{
		public static void EvalCacheUpdate<T>(MethodInfo method) where T:Attribute,ICacheUpdatePolicy
		{
			object[] attrs=method.GetCustomAttributes(typeof(T),false);
			if(attrs.Length!=0)
			{
				(attrs[0] as ICacheUpdatePolicy).Clear();
			}
		}
	}
}
