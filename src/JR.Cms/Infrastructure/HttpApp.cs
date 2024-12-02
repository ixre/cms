/**
 * Copyright (C) 2007-2015 fze.NET,All rights reserved.
 * Get more infromation of this software,please visit site http://fze.NET/cms
 * 
 * name : HttpApp.cs
 * author : newmin (new.min@msn.com)
 * date : 2014/12/01 23:00:00
 * description : 
 * history : 
 */

using JR.Stand.Core.Web;

namespace JR.Cms.Infrastructure
{
    /// <summary>
    /// HttpApp
    /// </summary>
    public static class HttpApp
    {
        private static string _appPath;

        /// <summary>
        /// 返回ApplicationPath,如果有虚拟目录则返回虚拟目录路径
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationPath()
        {
            if (_appPath == null)
                _appPath = WebCtx.Current.ApplicationPath;
            // if (HttpContextW.Current == null || HttpContext.Current.Handler == null)
            // {
            //     return "/";
            // }
            // _appPath = HttpContext.Current.Request.ApplicationPath;
            return _appPath;
        }
    }
}