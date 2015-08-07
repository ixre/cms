//
// { newmin,OPS } Server.cs     2011/03/06 00:41
//
// Copyright 2011 @ K3F.NET,All rights reseved!
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
        internal static string DefaultStaticServer="cms-cdn.K3F.NET/cms";


        /// <summary>
        /// 控制台订阅服务器
        /// </summary>
        public static String DashbordRssServer = "http://g.k3f.net/cms_dash_rss";

        /// <summary>
        /// 更新服务器
        /// </summary>
        public static string DefaultUpgradeServer = "http://cms-cdn.K3F.NET/cms/patch/latest/";
    }
}