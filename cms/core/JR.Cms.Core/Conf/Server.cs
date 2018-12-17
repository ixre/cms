//
// { newmin,OPS } Server.cs     2011/03/06 00:41
//
// Copyright 2011 @ TO2.NET,All rights reseved!
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
        internal static string DefaultStaticServer="cdn.cms.to2.net/cms";


        /// <summary>
        /// 短域名服务器
        /// </summary>
        public static String ShortUrlServer = "to2.net/cms_go";

        /// <summary>
        /// 更新服务器
        /// </summary>
        public static string DefaultUpgradeServer = "http://to2.net/cms/patch/latest/";
    }
}