//
// Copyright (C) 2007-2013 TO2.NET,All rights reserved.
// 
// Project: Cms.Core
// FileName : Settings.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2012/5/26 19:25:33
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/jr-cms
// Modify:
//  2013-02-21  17:28   newmin [+]: SYS_UseFullPath ，SYS_UseCompress
//  2013-05-30  21:30   newmin [+]: 检测是否gzip,支持则默认开启动
//

using System;
using System.IO;
using System.IO.Compression;

namespace JR.Cms.Conf
{
    /// <summary>
    /// 
    /// </summary>
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
        public static string LICENSE_NAME;

        /// <summary>
        /// 系统激活码
        /// </summary>
        public static string LICENSE_KEY;

        
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
        /// 是否使用原始上传文件名称
        /// </summary>
        public static bool SYS_USE_UPLOAD_RAW_NAME = false;

        /// <summary>
        /// 是否开启SQL跟踪
        /// </summary>
        public static bool SQL_PROFILE_TRACE = false;

        /// <summary>
        /// 是否加密配置文件
        /// </summary>
        public static bool SYS_ENCODE_CONF_FILE = false;

        /// <summary>
        /// 开放接口私钥
        /// </summary>
        public static String SYS_RSA_KEY = "";

        #region 模板相关
        /// <summary>
        /// 是否使用完整路径
        /// </summary>
        public static bool TPL_FULL_URL_PATH;

        /// <summary>
        /// 是否使用压缩
        /// </summary>
        public static bool TPL_USE_COMPRESS = true;

        /// <summary>
        /// 多模板公享模式
        /// </summary>
        [Obsolete]
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
        public static bool Opti_SupportGZip = true;

        /// <summary>
        /// 调试模式
        /// </summary>
        public static bool OPTI_DEBUG_MODE = false;

        /// <summary>
        /// 客户端缓存
        /// </summary>
        public static bool Opti_ClientCache { get { return Opti_ClientCacheSeconds > 0; } }

        /// <summary>
        /// 默认缓存小时数
        /// </summary>
        public static int OptiDefaultCacheHours = 12;

        /// <summary>
        /// 客户端缓存秒数
        /// </summary>
        public static int Opti_ClientCacheSeconds =0;

        /// <summary>
        /// 首页缓存秒数
        /// </summary>
        public static int PERM_INDEX_CACHE_SECOND = 0;

        /// <summary>
        /// GC回收间隔(默认30分钟回收一次)
        /// </summary>
        public static int opti_gc_collect_interval = 3600000 * 30;

        public static bool TPL_USE_CACHE = true;


        public static void TurnOnDebug()
        {
            OPTI_DEBUG_MODE = true;
            WebConfig.SetDebug(true);
        }

        public static void TurnOffDebug()
        {
            OPTI_DEBUG_MODE = !true;
            WebConfig.SetDebug(!true);
        }
    }
}
