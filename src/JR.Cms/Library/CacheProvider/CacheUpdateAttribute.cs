/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using JR.Cms.Library.CacheProvider.CacheComponent;
using JR.Stand.Abstracts.Cache;

namespace JR.Cms.Library.CacheProvider
{
    /// <summary>
    /// Description of CacheUpdateAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : Attribute, ICachePolicy
    {
        public CacheAttribute(string cacheKey)
        {
            Key = cacheKey;
        }

        public CacheAttribute(CacheSign sign)
        {
            Key = sign.ToString();
        }

        public string Key { get; private set; }


        public void Clean()
        {
            CmsCacheFactory.Singleton.Clear(Key);
        }
    }
}