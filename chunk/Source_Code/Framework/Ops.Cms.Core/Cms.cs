
/*
 * Copyright(C) 2010-2013 OPSoft Inc
 * 
 * File Name	: TemplateUrlRule
 * Author	: Newmin (new.min@msn.com)
 * Create	: 2013/05/21 19:59:54
 * Description	:
 *
 */

using Ops.Cms.Cache.CacheCompoment;
using Ops.Cms.Conf;
using Ops.Cms.Core;

namespace Ops.Cms
{
    using Com.PluginKernel;
    using Ops.Cms.Cache;
    using Ops.Cms.DataTransfer;
    using Ops.Cms.DB;
    using Ops.Cms.Infrastructure;
    using Ops.Template;
    using System;
    using System.IO;
    using System.Web;

	public static class Cms
	{
		/// <summary>
		/// 版本
		/// </summary>
		public static readonly string Version=CmsVariables.VERSION;
		
		/// <summary>
		/// 最后生成时间
		/// </summary>
		//public static readonly DateTime BuiltTime;
		
		/// <summary>
		/// 是否为Mono平台
		/// </summary>
		public static Boolean RunAtMono;

		/// <summary>
		/// 物理路径
		/// </summary>
		public static readonly string PyhicPath;

		/// <summary>
		/// 是否为多站点版本
		/// </summary>
		public static bool MultSiteVersion=true;

		/// <summary>
		/// 所有站点
		/// </summary>
		private static SiteDto[] sites;


		/// <summary>
		/// CMS实用工具
		/// </summary>
		public static readonly CmsUtility Utility;

		/// <summary>
		/// CMS缓存
		/// </summary>
		public static readonly CmsCache Cache;

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
		public static readonly CmsTemplate Template ;

		/// <summary>
		/// 插件
		/// </summary>
		public static readonly CmsPluginContext Plugins ;

        /// <summary>
        /// 语言包
        /// </summary>
        public static CmsLanguagePackage Language
        {
            get
            {
                return _cmsLang ?? (_cmsLang = new CmsLanguagePackage());
            }
        }

		/// <summary>
		/// 开始初始化
		/// </summary>
		public static event CmsHandler OnInit;

		/// <summary>
		/// 重置缓存时发生
		/// </summary>
		public static event CmsHandler OnResetCache;
        private static CmsLanguagePackage _cmsLang;


		static Cms()
		{

			PyhicPath = AppDomain.CurrentDomain.BaseDirectory;
			
			//获得版本号
			//AssemblyFileVersionAttribute ver = (AssemblyFileVersionAttribute)typeof(Cms).Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0];
			
			//string[] verarr = ver.Version.Split('.');
			//Version = String.Format("{0}.{1}.{2}", verarr[0], verarr[1], verarr[2]);

			//获取编译生成的时间
			//Version ver=typeof(Cms).Assembly.GetName().Version;
			//BuiltTime= new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision * 2);
			//BuiltTime=System.IO.File.GetLastWriteTime(typeof(Cms).Assembly.Location);
			
			//获取平台
			
            Int32 platFormID = (Int32)Environment.OSVersion.Platform;
            if (platFormID == 4 || platFormID == 6 || platFormID == 128)
            {
                RunAtMono = true;
            }

			//初始化
			Plugins= new CmsPluginContext();
			Template= new CmsTemplate();
			Cache=CacheFactory.Sington as CmsCache; 
			Utility=new CmsUtility();

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

            //判断是否已经安装
            FileInfo insLockFile = new FileInfo(String.Format("{0}config/install.lock", Cms.PyhicPath));
            if (!insLockFile.Exists)
            {
                //HttpRuntime.UnloadAppDomain();

                global::System.Web.HttpContext context = global::System.Web.HttpContext.Current;
                string installUrl = "/install/install.html";
                context.Response.Redirect(installUrl, true);
                context.Response.End();
                return;
            }

            //初始化设置
            string cmsConfigFile = String.Format("{0}config/cms.config", Cms.PyhicPath);
            FileInfo cfgFile = new FileInfo(cmsConfigFile);
            if (cfgFile.Exists)
            {
                Configuration.Load(cmsConfigFile);
            }
            else
            {
                throw new Exception("CMS配置文件不存在");
            }

            //设置数据库
            CmsDataBase.Initialize(String.Format("{0}://{1}", Settings.DB_TYPE.ToString(), Settings.DB_CONN.ToString()), Settings.DB_PREFIX);

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


            //清空临时文件
            resetTempFiles();

            PluginConfig.PLUGIN_DIRECTORY = CmsVariables.PLUGIN_PATH;
            PluginConfig.PLUGIN_TMP_DIRECTORY = CmsVariables.TEMP_PATH;
            PluginConfig.PLUGIN_LOG_OPENED = true;
            PluginConfig.PLUGIN_LOG_EXCEPT_FORMAT = "** {time} **:{message}\r\nSource:{source}\r\nAddress:{addr}\r\nStack:{stack}\r\n\r\n";

            //连接插件
            CmsPluginContext.Connect();


            if (OnInit != null)
            {
                Cms.OnInit();
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

		/// <summary>
		/// 所有站点
		/// </summary>
		public static SiteDto[] Sites { get { return sites; } }

		/// <summary>
		/// 更新服务器信息
		/// </summary>
		internal static void UpdateServerInfo()
		{
			
			Ops.Framework.Net.WebClient client = new Ops.Framework.Net.WebClient("http://ct.ops.cc/control.axd");
			try
			{
				//Server.StaticServer = client.Post("task=server,getStaticServer," + Settings.License_KEY);
			}
			catch { }
		}

		/// <summary>
		/// 重新注册设置站点
		/// </summary>
		/// <param name="sites"></param>
        public static void RegSites(SiteDto[] sites)
        {
            Cms.sites = sites;
        }


        public static void TraceLog(string p)
        {

            //
            //TODO:log
            //

           // throw new NotImplementedException();
        }
    }
}
