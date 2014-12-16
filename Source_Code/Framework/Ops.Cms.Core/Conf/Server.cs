//
// { newmin,OPS } Server.cs     2011/03/06 00:41
//
// Copyright 2011 @ OPS Inc,All rights reseved!
// Note: 需做一个调用更新服务器的功能
//
//

using System;

namespace Ops.Cms.Conf
{
    public class Server
    {
    	/// <summary>
    	/// 默认静态服务器
    	/// 
    	/// </summary>
        internal static string _defaultStaticServer="cdn.ops.cc/cms";

        /// <summary>
        /// 更新服务器
        /// </summary>
        public static String UpgradeServer = "http://cdn.ops.cc/cms/release/last_patch/";

        /// <summary>
        /// 控制台订阅服务器
        /// </summary>
        public static String DashbordRssServer = "http://www.ops.cc/cms/";
    }
}