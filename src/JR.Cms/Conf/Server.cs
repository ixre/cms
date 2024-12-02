//
// { newmin,OPS } Server.cs     2011/03/06 00:41
//
// Copyright 2011 @ fze.NET,All rights reserved!
// Note: 需做一个调用更新服务器的功能
//
//

using System;

namespace JR.Cms.Conf
{
    public class Server
    {
        /// <summary>
        /// 默认静态服务器
        /// 
        /// </summary>
        internal static string DefaultStaticServer = "cdn.cms.fze.NET/cms";


        /// <summary>
        /// 短域名服务器
        /// </summary>
        public static String ShortUrlServer = "fze.NET/cms/pages";

        /// <summary>
        /// 更新服务器
        /// </summary>
        public static string DefaultUpgradeServer = "http://fze.NET/cms/update/latest";
    }
}