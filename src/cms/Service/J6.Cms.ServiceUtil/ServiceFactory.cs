/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace J6.Cms.ServiceUtil
{
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public static class ServiceFactory
	{
	    /// <summary>
	    /// 
	    /// </summary>
	    private static ICmsServiceProvider _redirect;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ICmsServiceProvider GetService(ServiceCallMethod method)
        {
            if (method == ServiceCallMethod.Redirect)
                   return _redirect ??(_redirect = new RedirectCallService());
            throw new NotSupportedException("目前仅支持直接调用接口");

        }
	}
}