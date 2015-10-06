//
// { newmin,OPS } Server.cs     2011/03/06 00:41
//
// Copyright 2011 @ Z3Q.NET,All rights reseved!
// Note: 需做一个调用更新服务器的功能
//
//

using System;

namespace J6.Cms.Conf
{
    public class Server
    {
    	/// <summary>
    	/// 默认静态服务器
    	/// 
    	/// </summary>
        internal static string DefaultStaticServer="ssp-sz.z3q.net/cms";


        /// <summary>
        /// 短域名服务器
        /// </summary>
        public static String ShortUrlServer = "z3q.net/cms_go";

        /// <summary>
        /// 更新服务器
        /// </summary>
        public static string DefaultUpgradeServer = "http://ssp-sz.z3q.net/cms/patch/latest/";
    }
}