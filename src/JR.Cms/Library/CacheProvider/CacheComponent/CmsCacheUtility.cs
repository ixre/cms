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
using JR.Stand.Abstracts.Cache;

namespace JR.Cms.Library.CacheProvider.CacheComponent
{
    /// <summary>
    /// Description of CmsCacheUtility.
    /// </summary>
    public static class CmsCacheUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <typeparam name="T"></typeparam>
        public static void EvalCacheUpdate<T>(MethodInfo method) where T : Attribute, ICachePolicy
        {
            var attrs = method.GetCustomAttributes(typeof(T), false);
            if (attrs.Length == 0) return;
            (attrs[0] as ICachePolicy)?.Clean();
        }
    }
}