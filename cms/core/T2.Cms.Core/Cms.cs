
/*
 * Copyright(C) 2010-2013 TO2.NET
 * 
 * File Name	: TemplateUrlRule
 * author_id	: Newmin (new.min@msn.com)
 * Create	: 2013/05/21 19:59:54
 * Description	:
 *
 */

using System;
using System.IO;
using System.Web;
using T2.Cms.Cache;
using T2.Cms.Cache.CacheCompoment;
using T2.Cms.Conf;
using T2.Cms.Core;
using T2.Cms.Core.Plugins;
using T2.Cms.Infrastructure;
using JR.DevFw.Framework;
using T2.Cms.DataTransfer;
using T2.Cms.DB;
using JR.DevFw.PluginKernel;
using JR.DevFw.Framework.Web.UI;

namespace T2.Cms
{
    public enum BootFlag
    {
        Normal = 1,
        UnitTest = 2
    }
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
        public static Boolean IsInstalled()
        {
            FileInfo insLockFile = new FileInfo(String.Format("{0}/config/install.lock", PyhicPath));
            return insLockFile.Exists;
        }

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

        private static TemplateManager _templateManager;
        private static CustomBuildSet _buildSet;



        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        internal static bool IsInitFinish { get; set; }

        /// <summary>
        /// 请求是否为静态资源
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static bool IsStaticRequest(HttpContext ctx)
        {
            String path = ctx.Request.Path;
           return path.StartsWith("/public/") || path.StartsWith("/resources/") ||
                path.StartsWith("/plugins/") || path.StartsWith("/templates/");
        }

        /// <summary>
        /// Cms上下文对象
        /// </summary>
        public static CmsContext Context
        {
            get
            {
                HttpContext httpCtx = HttpContext.Current;
                if (IsStaticRequest(httpCtx))
                {
                    return null;
                }
                CmsContext context = httpCtx.Items["cms.context"] as CmsContext;
                if (context == null)
                {
                    context = new CmsContext();
                    httpCtx.Items["cms.context"] = context;
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
        public static readonly CmsLanguagePackage Language;

        /// <summary>
        /// 模版管理器
        /// </summary>
        public static TemplateManager TemplateManager
        {
            get
            {
                return _templateManager;
            }
        }

        /// <summary>
        /// 定制设置
        /// </summary>
        public static  CustomBuildSet BuildSet
        {
            get
            {
                return _buildSet;
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
                filePath = PyhicPath + CmsVariables.FRAMEWORK_ASSEMBLY_PATH + "jrcms.dll";
            }
            DateTime builtDate = File.GetLastWriteTime(filePath);
            BuiltTime = DateHelper.ToUnix(builtDate);

            //获取平台
            Int32 platFormId = (Int32)Environment.OSVersion.Platform;
            if (platFormId == 4 || platFormId == 6 || platFormId == 128)
            {
                RunAtMono = true;
            }



            //初始化
            Plugins = new CmsPluginContext();
            Template = new CmsTemplate();
            Cache = CacheFactory.Sington as CmsCache;
            Utility = new CmsUtility();
            Language = new CmsLanguagePackage();
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
        public static void Init(BootFlag flag,String confPath)
        {
            BeforeInit();
            if (!IsInstalled()) return;
            //初始化目录
            ChkCreate(CmsVariables.TEMP_PATH);
            // 加载配置
            Configuration.LoadCmsConfig(confPath);
            //设置数据库
            CmsDataBase.Initialize(String.Format("{0}://{1}", Settings.DB_TYPE.ToString(),
                Settings.DB_CONN.ToString()), Settings.DB_PREFIX,Settings.SQL_PROFILE_TRACE);
            //清空临时文件
            //resetTempFiles();
            // 初始化键值存储
            InitKvDb();
            // 加载其他配置
            LoadOtherConfig();
            //获取静态服务器
            //UpdateServerInfo();

            // 正常模式启动
            if ((flag & BootFlag.Normal) != 0)
            {
                _buildSet = new CustomBuildSet();

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
                _templateManager = new TemplateManager(Cms.PyhicPath + CmsVariables.TEMPLATE_PATH);

                // 注册模板
                Template.Register("/" + CmsVariables.TEMPLATE_PATH, true);
                // 注册插件
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
                // 连接插件
                CmsPluginContext.Connect();

                // 设置验证码字体
                VerifyCodeGenerator.SetFontFamily(Cms.PyhicPath + CmsVariables.FRAMEWORK_ASSETS_PATH + "fonts/comic.ttf");
            }
            if (OnInit != null)
            {
                Cms.OnInit();
            }

            IsInitFinish = true;
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
