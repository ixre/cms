//
// Copyright (C) 2007-2013 S1N1.COM,All rights reseved.
// 
// Project: Cms.Cms.Config
// FileName : Settings.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2012/5/26 19:25:33
// Description :
//
// Get infromation of this software,please visit our site http://Cms.Cms.cc
// Modify:
//  2013-02-21  17:28   newmin [+]: SYS_UseFullPath ，SYS_UseCompress
//  2013-05-30  21:30   newmin [+]: 检测是否gzip,支持则默认开启动
//

using System;
using System.IO;
using System.IO.Compression;

namespace J6.Cms.Conf
{
    public struct Settings
    {
        static Settings()
        {

            //检测是否支持开启GZIP
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream gs = new GZipStream(ms, CompressionMode.Compress);
                gs.Dispose();
                ms.Dispose();
                Settings.Opti_SupportGZip = true;
            }
            catch
            {
                Settings.Opti_SupportGZip = false;
            }
        }

        /// <summary>
        /// 是否已经被加载
        /// </summary>
        public static bool loaded = false;

        /// <summary>
        /// 授权用户名称
        /// </summary>
        public static string License_NAME;

        /// <summary>
        /// 系统激活码
        /// </summary>
        public static string License_KEY;

        
        /// <summary>
        /// 后台管理地址
        /// </summary>
        public static string SYS_ADMIN_TAG="admin";
        
        /// <summary>
        /// 是否启用静态服务器
        /// </summary>
        public static Boolean SERVER_STATIC_ENABLED=false;
        
        /// <summary>
        /// 静态服务器地址
        /// </summary>
        public static String SERVER_STATIC="";

        /// <summary>
        /// 更新服务器
        /// </summary>
        public static string SERVER_UPGRADE = null;

        /// <summary>
        /// 系统虚拟路径(默认为空)
        /// </summary>
        //public static string SYS_VIRTHPATH;

        /// <summary>
        /// 自动301跳转到www
        /// </summary>
        public static bool SYS_AUTOWWW;


        #region 模板相关
        /// <summary>
        /// 是否使用完整路径
        /// </summary>
        public static bool TPL_UseFullPath;

        /// <summary>
        /// 是否使用压缩
        /// </summary>
        public static bool TPL_UseCompress;

        /// <summary>
        /// 多模板公享模式
        /// </summary>
        public static bool TPL_MultMode = false;

        #endregion



        /***** 数据库配置 *****/

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string DB_TYPE="mysql";

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DB_CONN;

        /// <summary>
        /// 数据库表前缀
        /// </summary>
        public static string DB_PREFIX = "cms_";



       

        /***** 会员配置  *****/

        /// <summary>
        /// 会员头像路径
        /// </summary>
        public static string MM_AVATAR_PATH;

        /******* 优化 **********/

        /// <summary>
        /// 是否支持GZip压缩
        /// </summary>
        public static bool Opti_SupportGZip = false;

        /// <summary>
        /// 调试模式
        /// </summary>
        public static bool Opti_Debug = false;

        /// <summary>
        /// 客户端缓存
        /// </summary>
        public static bool Opti_ClientCache { get { return Opti_ClientCacheSeconds > 0; } }

        /// <summary>
        /// 客户端缓存秒数
        /// </summary>
        public static int Opti_ClientCacheSeconds =0;

        /// <summary>
        /// 首页缓存秒数
        /// </summary>
        public static int Opti_IndexCacheSeconds = 0;

        /// <summary>
        /// GC回收间隔(默认30分钟回收一次)
        /// </summary>
        public static int Opti_GC_Collect_Interval = 3600000 * 30;


        public static void TurnOnDebug()
        {
            Opti_Debug = true;
            WebConfig.SetDebug(true);
        }

        public static void TurnOffDebug()
        {
            Opti_Debug = !true;
            WebConfig.SetDebug(!true);
        }
    }
}
