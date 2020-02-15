
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
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Core.Plugins;
using JR.Cms.DB;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheProvider;
using JR.Cms.Library.CacheProvider.CacheCompoment;
using JR.Cms.ServiceDto;
using JR.DevFw.Framework.IO;
using JR.DevFw.Framework.Web.UI;
using JR.DevFw.PluginKernel;

namespace JR.Cms
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
            if (path.EndsWith(".ashx") || path.EndsWith(".aspx")) return false;
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
        public static BuildOEM BuildOEM
        {
            get; private set;
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
            string filePath = typeof(Cms).Assembly.Location;
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
            Cache = CacheFactory.Singleton as CmsCache;
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
        public static void Init(BootFlag flag, String confPath)
        {
            BeforeInit();
            if (!IsInstalled()) return;
            //初始化目录
            ChkCreate(CmsVariables.TEMP_PATH);
            // 拷贝OEM文件到根目录
            CopyOEMToRoot("root");
            // 加载配置
            Configuration.LoadCmsConfig(confPath);
            //设置数据库
            CmsDataBase.Initialize(String.Format("{0}://{1}", Settings.DB_TYPE.ToString(),
                Settings.DB_CONN.ToString()), Settings.DB_PREFIX, Settings.SQL_PROFILE_TRACE);
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
                if (!Directory.Exists(PyhicPath + "templates/"))
                {
                    Directory.CreateDirectory(PyhicPath + "templates/").Create();
                    //暂时网络安装默认模板(后可使用资源代替)
                    Updater.InstallTemplate("default", "tpl_default.zip");
                }
                _templateManager = new TemplateManager(PyhicPath + CmsVariables.TEMPLATE_PATH);

                // 注册模板
                Template.Register("/" + CmsVariables.TEMPLATE_PATH, true);
                // 注册插件
                //PluginConfig.PLUGIN_FILE_PARTTERN = "*.dll,*.so";
                PluginConfig.PLUGIN_DIRECTORY = CmsVariables.PLUGIN_PATH;
                PluginConfig.PLUGIN_TMP_DIRECTORY = CmsVariables.TEMP_PATH + "plugin/";
                PluginConfig.PLUGIN_LOG_OPENED = true;
                PluginConfig.PLUGIN_LOG_EXCEPT_FORMAT = "** {time} **:{message}\r\nSource:{source}\r\nAddress:{addr}\r\nStack:{stack}\r\n\r\n";
                string pluginPhysicPath = PyhicPath + PluginConfig.PLUGIN_TMP_DIRECTORY;
                if (!Directory.Exists(pluginPhysicPath))
                {
                    Directory.CreateDirectory(pluginPhysicPath).Create();
                }
                // 连接插件
                CmsPluginContext.Connect();

                // 设置验证码字体
                VerifyCodeGenerator.SetFontFamily(PyhicPath + CmsVariables.FRAMEWORK_ASSETS_PATH + "fonts/comic.ttf");
            }
            if (OnInit != null)
            {
                OnInit();
            }

            IsInitFinish = true;
        }

        /// <summary>
        /// 拷贝OEM文件到根目录
        /// </summary>
        /// <param name="v"></param>
        private static void CopyOEMToRoot(string dir)
        {
            try
            {
                IOUtils.CopyFolder(PyhicPath + "/" + CmsVariables.OEM_PATH + dir, PyhicPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ CMS][ OEM]: 拷贝OEM文件失败:" + ex.Message);
            }
        }

        private static void InitKvDb()
        {
            //注册KvDB
            string kvDir = PyhicPath + CmsVariables.TEMP_PATH + "data/gcp";
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
            _sites = sites;
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
            DirectoryInfo dir = new DirectoryInfo(PyhicPath + CmsVariables.TEMP_PATH);
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
