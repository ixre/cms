/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace T2.Cms.Cache.CacheCompoment
{
	  /// <summary>
    /// 缓存符号
    /// </summary>
    [Flags]
    public enum CacheSign:int
    {
        Unknown =1,
        Site = 1 << 1,
        Comm = 1 << 2,
        Archive = 1 << 3,

        /// <summary>
        /// 栏目
        /// </summary>
        Category= 1 << 4,
        Link = 1 << 5,
        Property = 1 << 6,
        Template =  1 << 7,
        TemplateBind = 1 << 8,
        Module = 1 << 9
    }
}
