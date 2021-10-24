/*
 * Copyright(C) 2010-2013 OPS.CC
 * 
 * File Name	: TemplateUrlRule
 * author_id	: Newmin (new.min@msn.com)
 * Create	: 2013/05/21 19:59:54
 * Description	:
 *
 */

using System;
using System.IO;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheProvider;
using JR.Cms.Library.CacheProvider.CacheComponent;
using JR.Cms.Library.DataAccess.DB;
using JR.Stand.Abstracts;
using JR.Stand.Core;
using JR.Stand.Core.Cache;
using JR.Stand.Core.Framework.Web.UI;
using JR.Stand.Core.PluginKernel;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Web;
using JR.Stand.Core.Web.Cache;

namespace JR.Cms
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum BootFlag
    {
        /// <summary>
        /// 
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 
        /// </summary>
        UnitTest = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Cms
    {
        private static bool isInstalled;
        private static  IMemoryCacheWrapper _cache;
        /// <summary>
        /// 版本
        /// </summary>
        public static string Version { get; private set; }

        /// <summary>
        /// 最后生成时间
        /// </summary>
        public static long BuiltTime;

        /// <summary>
        /// 是否为Mono平台
        /// </summary>
        public static  bool RunAtMono { get; private set; }

        /// <summary>
        /// 目标是否为.NET Stand
        /// </summary>
        public static bool IsNetStandard { get;private set; }

        /// <summary>
        /// 是否已经安装
        /// </summary>
        public static bool IsInstalled()
        {
            if (isInstalled == false)
            {
                var insLockFile = new FileInfo($"{PhysicPath}/config/install.lock");
                isInstalled = insLockFile.Exists;
            }

            return isInstalled;
        }

        /// <summary>
        /// 是否为正式环境
        /// </summary>
        public static bool OfficialEnvironment = true;

        /// <summary>
        /// 物理路径
        /// </summary>
        public static  string PhysicPath { get; private set; }
        
        /// <summary>
        /// CMS实用工具
        /// </summary>
        public static  CmsUtility Utility { get; private set; }

        /// <summary>
        /// CMS缓存
        /// </summary>
        public static  CmsCache Cache { get; private set; }

        private static TemplateManager _templateManager;


        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        internal static bool IsInitFinish { get; private set; }

        /// <summary>
        /// 请求是否为静态资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsStaticRequest(string path)
        {
            if (path.EndsWith(".ashx") || path.EndsWith(".aspx")) return false;
            return path.StartsWith("/public/") || path.StartsWith("/resources/") ||
                   path.StartsWith("/uploads/") || path.StartsWith("/plugins/") ||
                   path.StartsWith("/templates/");
        }

        /// <summary>
        /// Cms上下文对象
        /// </summary>
        public static CmsContext Context
        {
            get
            {
                var httpCtx = HttpHosting.Context;
                if (IsStaticRequest(httpCtx.RequestPath())) return null;
                if(!httpCtx.TryGetItem<CmsContext>("cms.context",out var context)){
                    context = new CmsContext(httpCtx);
                    httpCtx.SaveItem("cms.context", context);
                }

                return context;
            }
        }

        /// <summary>
        /// Cms模板
        /// </summary>
        public static  CmsTemplate Template { get; private set; }

        /// <summary>
        /// 插件
        /// </summary>
        //public static readonly CmsPluginContext Plugins;

        /// <summary>
        /// 语言包
        /// </summary>
        public static  CmsLanguagePackage Language { get; private set; }

        /// <summary>
        /// 模版管理器
        /// </summary>
        public static TemplateManager TemplateManager => _templateManager;

        /// <summary>
        /// 定制设置
        /// </summary>
        public static BuildOEM BuildOEM { get; private set; }


        /// <summary>
        /// 开始初始化
        /// </summary>
        public static event CmsHandler OnInit;


        private static void PrepareCms()
        {
#if NETSTANDARD
            IsNetStandard = true;
            if (_cache == null) _cache = new MemoryCacheWrapper();
#endif
            
            Version = CmsVariables.VERSION;
            PhysicPath = EnvUtil.GetBaseDirectory();

            //获取编译生成的时间
            //DateTime builtDate = new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision*2);
            var filePath = typeof(Cms).Assembly.Location;
            if (string.IsNullOrEmpty(filePath))
                filePath = PhysicPath + CmsVariables.FRAMEWORK_ASSEMBLY_PATH + "jrcms.dll";
            var builtDate = File.GetLastWriteTime(filePath);
            BuiltTime = DateHelper.ToUnix(builtDate);


            //获取平台
            var platFormId = (int) Environment.OSVersion.Platform;
            if (platFormId == 4 || platFormId == 6 || platFormId == 128) RunAtMono = true;
            //初始化
            //todo: plugin
            //Plugins = new CmsPluginContext();
            
            // 初始化缓存
            CmsCacheFactory.Configure(_cache);
            CacheFactory.Configure(_cache);
            Cache = new CmsCache(CmsCacheFactory.Singleton);
            // 初始化内存缓存
            Utility = new CmsUtility();
            Language = new CmsLanguagePackage();

            #region 缓存清除

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
        public static void Init(BootFlag flag, string confPath)
        {
            PrepareCms();
            BeforeInit();
            if (!IsInstalled()) return;
            //初始化目录
            ChkCreate(CmsVariables.TEMP_PATH);
            // 加载配置
            Configuration.LoadCmsConfig(confPath);
            //设置数据库
            CmsDataBase.Initialize($"{Settings.DB_TYPE}://{Settings.DB_CONN}", Settings.DB_PREFIX, Settings.SQL_PROFILE_TRACE);
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
                BuildOEM = new BuildOEM();

                //
                //TODO:
                //
                //检查网站激活状态
                //SoftwareActivator.VerifyActivation();
                
                //如果不存在模板文件夹，则创建目录
                if (!Directory.Exists(PhysicPath + "templates/"))
                {
                    Directory.CreateDirectory(PhysicPath + "templates/").Create();
                    //暂时网络安装默认模板(后可使用资源代替)
                    Updater.InstallTemplate("default", "tpl_default.zip");
                }

                //　初始化模板
                Template = new CmsTemplate(_cache, TemplateNames.FileName);
                // 注册模板
                Template.Register(CmsVariables.TEMPLATE_PATH);
                // 模板管理器
                _templateManager = new TemplateManager(PhysicPath + CmsVariables.TEMPLATE_PATH);
                
                // 注册插件
                //PluginConfig.PLUGIN_FILE_PARTTERN = "*.dll,*.so";
                PluginConfig.PLUGIN_DIRECTORY = CmsVariables.PLUGIN_PATH;
                PluginConfig.PLUGIN_TMP_DIRECTORY = CmsVariables.TEMP_PATH + "plugin/";
                PluginConfig.PLUGIN_LOG_OPENED = true;
                PluginConfig.PLUGIN_LOG_EXCEPT_FORMAT =
                    "** {time} **:{message}\r\nSource:{source}\r\nAddress:{addr}\r\nStack:{stack}\r\n\r\n";
                var pluginPhysicPath = PhysicPath + PluginConfig.PLUGIN_TMP_DIRECTORY;
                if (!Directory.Exists(pluginPhysicPath)) Directory.CreateDirectory(pluginPhysicPath).Create();
                // 连接插件
                //todo: plugin
                //CmsPluginContext.Connect();
                // 设置验证码字体
                VerifyCodeGenerator.SetFontFamily(PhysicPath + CmsVariables.FRAMEWORK_ASSETS_PATH + "fonts/comic.ttf");
            }
            OnInit?.Invoke();
            IsInitFinish = true;
        }

        /// <summary>
        /// 重新加载CMS
        /// </summary>
        public static void Reload()
        {
            Init(BootFlag.Normal, null);
            Cache.Reset(null);
        }
        
        private static void InitKvDb()
        {
            //注册KvDB
            var kvDir = PhysicPath + CmsVariables.TEMP_PATH + "data/gcp";
            if (Directory.Exists(kvDir))
                try
                {
                    Directory.Delete(kvDir, true);
                }
                catch
                {
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
        public static void TraceLog(string log)
        {
            //
            //TODO:log
            //
        }


        private static void ChkCreate(string path)
        {
            var d = PhysicPath + path;
            if (!Directory.Exists(d)) Directory.CreateDirectory(d).Create();
        }

        private static void resetTempFiles()
        {
            var dir = new DirectoryInfo(PhysicPath + CmsVariables.TEMP_PATH);
            if (dir.Exists)
                foreach (var file in dir.GetFiles())
                    file.Delete();
            else
                Directory.CreateDirectory(dir.FullName).Create();
        }

        public static void ConfigCache(IMemoryCacheWrapper cache)
        {
            _cache = cache;
        }
    }
}