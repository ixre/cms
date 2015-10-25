
/*
 * Copyright(C) 2010-2013 Z3Q.NET
 * 
 * File Name	: TemplateUrlRule
 * publisher_id	: Newmin (new.min@msn.com)
 * Create	: 2013/05/21 19:59:54
 * Description	:
 *
 */

using System;
using System.IO;
using System.Web;
using J6.Cms.Cache;
using J6.Cms.Cache.CacheCompoment;
using J6.Cms.Conf;
using J6.Cms.Core;
using J6.Cms.Core.Plugins;
using J6.Cms.Infrastructure;
using J6.DevFw.Framework;
using J6.DevFw.PluginKernel;
using J6.Cms.DataTransfer;
using J6.Cms.DB;
using J6.DevFw.Framework.IO;

namespace J6.Cms
{
    public static class Cms
    {
        /// <summary>
        /// 版本
        /// </summary>
        public static readonly string Version;

        /// <summary>
        /// 最后生成时间
        /// </summary>
        public static long BuiltTime;

        /// <summary>
        /// 是否为Mono平台
        /// </summary>
        public static readonly Boolean RunAtMono;

        /// <summary>
        /// 是否已经安装
        /// </summary>
        public static readonly Boolean Installed;

        /// <summary>
        /// 是否为正式环境
        /// </summary>
        public static bool OfficialEnvironment = true;

        /// <summary>
        /// 物理路径
        /// </summary>
        public static readonly string PyhicPath;

        /// <summary>
        /// 是否为多站点版本
        /// </summary>
        public static bool MultSiteVersion = true;

        /// <summary>
        /// 所有站点
        /// </summary>
        private static SiteDto[] _sites;


        /// <summary>
        /// CMS实用工具
        /// </summary>
        public static readonly CmsUtility Utility;

        /// <summary>
        /// CMS缓存
        /// </summary>
        public static readonly CmsCache Cache;

        private static LogFile _logFile;

        /// <summary>
        /// Cms上下文对象
        /// </summary>
        public static CmsContext Context
        {
            get
            {
                CmsContext context = HttpContext.Current.Items["cms.context"] as CmsContext;
                if (context == null)
                {
                    context = new CmsContext();
                    HttpContext.Current.Items["cms.context"] = context;
                }
                return context;
            }
        }

        /// <summary>
        /// Cms模板
        /// </summary>
        public static readonly CmsTemplate Template;

        /// <summary>
        /// 插件
        /// </summary>
        public static readonly CmsPluginContext Plugins;

        /// <summary>
        /// 语言包
        /// </summary>
        public static CmsLanguagePackage Language
        {
            get
            {
                return CmsLanguagePackage.Create();
            }
        }

        /// <summary>
        /// 开始初始化
        /// </summary>
        public static event CmsHandler OnInit;



        static Cms()
        {
            Version = CmsVariables.VERSION;
            PyhicPath = AppDomain.CurrentDomain.BaseDirectory;

            //获取编译生成的时间
            //DateTime builtDate = new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision*2);
            string filePath = typeof (Cms).Assembly.Location;
            if (String.IsNullOrEmpty(filePath))
            {
                filePath = PyhicPath + CmsVariables.FRAMEWORK_ASSEMBLY_PATH + "j6.cms.dll";
            }
            DateTime builtDate = File.GetLastWriteTime(filePath);
            BuiltTime = DateHelper.ToUnix(builtDate);

            //获取平台

            Int32 platFormId = (Int32)Environment.OSVersion.Platform;
            if (platFormId == 4 || platFormId == 6 || platFormId == 128)
            {
                RunAtMono = true;
            }


            //判断是否已经安装
            FileInfo insLockFile = new FileInfo(String.Format("{0}config/install.lock", Cms.PyhicPath));
            Installed = insLockFile.Exists;

            //初始化
            Plugins = new CmsPluginContext();
            Template = new CmsTemplate();
            Cache = CacheFactory.Sington as CmsCache;
            Utility = new CmsUtility();

            #region  缓存清除

            //
            //UNDONE: 弱引用
            //

            /*
            WeakRefCache.OnLinkBuilting += () =>
            {
                Cms.Cache.Clear(CacheSign.Link.ToString());
            };

            WeakRefCache.OnModuleBuilting += () =>
            {
                Cms.Cache.Clear(CacheSign.Module.ToString());
            };

            WeakRefCache.OnPropertyBuilting += () =>
            {
                Cms.Cache.Clear(CacheSign.Property.ToString());
            };

            WeakRefCache.OnTemplateBindBuilting += () =>
            {
                Cms.Cache.Clear(CacheSign.TemplateBind.ToString());
            };

             */
            #endregion
        }


        /// <summary>
        /// 设置应用程序，如在过程中发生异常则重启并提醒！
        /// </summary>
        public static void Init()
        {
            BeforeInit();

            if (!Installed) return;

            //初始化目录
            ChkCreate(CmsVariables.TEMP_PATH);


            Configuration.LoadCmsConfig();

            //设置数据库
            CmsDataBase.Initialize(
                String.Format("{0}://{1}", Settings.DB_TYPE.ToString(),
                Settings.DB_CONN.ToString()),
                Settings.DB_PREFIX);


            LoadOtherConfig();



            //清空临时文件
            //resetTempFiles();


            InitKvDb();

            //获取静态服务器
            //UpdateServerInfo();

            //
            //TODO:
            //
            //检查网站激活状态
            //SoftwareActivator.VerifyActivation();

            //如果不存在模板文件夹，则创建目录
            if (!Directory.Exists(Cms.PyhicPath + "templates/"))
            {
                Directory.CreateDirectory(Cms.PyhicPath + "templates/").Create();

                //暂时网络安装默认模板(后可使用资源代替)
                Updater.InstallTemplate("default", "tpl_default.zip");
            }

            //注册模板
            Template.Register("/templates/", true);

            //PluginConfig.PLUGIN_FILE_PARTTERN = "*.dll,*.so";
            PluginConfig.PLUGIN_DIRECTORY = CmsVariables.PLUGIN_PATH;
            PluginConfig.PLUGIN_TMP_DIRECTORY = CmsVariables.TEMP_PATH + "plugin/";
            PluginConfig.PLUGIN_LOG_OPENED = true;
            PluginConfig.PLUGIN_LOG_EXCEPT_FORMAT = "** {time} **:{message}\r\nSource:{source}\r\nAddress:{addr}\r\nStack:{stack}\r\n\r\n";

            string pluginPhysicPath = Cms.PyhicPath + PluginConfig.PLUGIN_TMP_DIRECTORY;
            if (!Directory.Exists(pluginPhysicPath))
            {
                Directory.CreateDirectory(pluginPhysicPath).Create();
            }

            //连接插件
            CmsPluginContext.Connect();


            if (OnInit != null)
            {
                Cms.OnInit();
            }
        }

        private static void InitKvDb()
        {
            //注册KvDB
            string kvDir = Cms.PyhicPath + CmsVariables.TEMP_PATH + "data/gcp";
            if (Directory.Exists(kvDir))
            {
                try
                {
                    Directory.Delete(kvDir, true);
                }
                catch
                {
                }
            }
            Kvdb.SetPath(kvDir);
            Kvdb.Clean();
        }

        private static void BeforeInit()
        {
            //初始化设置
            //            string cmsConfigFile = String.Format("{0}config/cms.config", Cms.PyhicPath);
            //            if (File.Exists(cmsConfigFile))
            //            {
            //                File.Move(cmsConfigFile, cmsConfigFile.Replace("cms.config", "cms.conf"));
            //            }
        }

        /// <summary>
        /// 从文件中加载配置
        /// </summary>
        private static void LoadOtherConfig()
        {
            Configuration.LoadRelatedIndent();
        }



        /// <summary>
        /// 所有站点
        /// </summary>
        public static SiteDto[] Sites { get { return _sites; } }

        /// <summary>
        /// 更新服务器信息
        /// </summary>
        //internal static void UpdateServerInfo()
        //{

        //    Cms.Framework.Net.WebClient client = new Cms.Framework.Net.WebClient("http://ct.Cms.cc/control.axd");
        //    try
        //    {
        //        Server.StaticServer = client.Post("task=server,getStaticServer," + Settings.License_KEY);
        //    }
        //    catch { }
        //}

        /// <summary>
        /// 重新注册设置站点
        /// </summary>
        /// <param name="sites"></param>
        public static void RegSites(SiteDto[] sites)
        {
            Cms._sites = sites;
        }


        public static void TraceLog(string log)
        {
            //
            //TODO:log
            //

        }


        private static void ChkCreate(string path)
        {
            string d = PyhicPath + path;
            if (!Directory.Exists(d))
            {
                Directory.CreateDirectory(d).Create();
            }
        }

        private static void resetTempFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(Cms.PyhicPath + CmsVariables.TEMP_PATH);
            if (dir.Exists)
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(dir.FullName).Create();
            }
        }
    }
}
