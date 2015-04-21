/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : HttpApp.cs
 * author : newmin (new.min@msn.com)
 * date : 2014/12/01 23:00:00
 * description : 
 * history : 
 */
namespace AtNet.Cms.Infrastructure
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
            return _appPath ?? (_appPath = "/");
        }

        public static void SetApplicationPath(string appPath)
        {
            _appPath = appPath;
        }
    }
}
