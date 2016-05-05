/**
 * Copyright (C) 2007-2015 Z3Q.NET,All rights reseved.
 * Get more infromation of this software,please visit site http://k3f.net/cms
 * 
 * name : HttpApp.cs
 * author : newmin (new.min@msn.com)
 * date : 2014/12/01 23:00:00
 * description : 
 * history : 
 */
using System.Web;

namespace JR.Cms.Infrastructure
{
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
            {
                if (HttpContext.Current == null || HttpContext.Current.Handler == null)
                {
                    return "/";
                }
                _appPath = HttpContext.Current.Request.ApplicationPath;
            }
            return _appPath;
        }
    }
}
