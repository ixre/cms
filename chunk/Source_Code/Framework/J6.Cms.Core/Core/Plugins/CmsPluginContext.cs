using System;
using System.IO;
using System.Web;
using J6.Cms.Conf;
using J6.Cms;
using J6.DevFw.PluginKernel;
using J6.DevFw.PluginKernel.Web;
using J6.DevFw.Template;
using J6.DevFw.Web;

namespace J6.Cms.Core.Plugins
{
    public class CmsPluginContext
    {
        private static CmsPlugin _cmsPlugin;
        private static PortalPlugin _portal;
        private static CategoryPlugin _category;
        private static ArchivePlugin _archive;
        //private static ExtendsPlugin _extends;
        private static WebPluginHandleProxy<HttpContext> _webHandler;
        private string _domain;
        private string _fpath;
        private string _pluginPath;
        private string _admJs;
        private string _admCss;
        private bool _loaded;


        internal CmsPluginContext()
        {
            if (_webHandler == null)
            {
                _webHandler = new WebPluginHandleProxy<HttpContext>();
            }
        }

        #region  插件对象

        /// <summary>
        /// 插件管理
        /// </summary>
        public CmsPlugin Manager { get { return _cmsPlugin ?? (_cmsPlugin = new CmsPlugin()); } }

        /// <summary>
        /// 入口插件
        /// </summary>
        public PortalPlugin Portal { get { return _portal ?? (_portal = new PortalPlugin()); } }

        /// <summary>
        /// 栏目插件
        /// </summary>
        public CategoryPlugin Category { get { return _category ?? (_category = new CategoryPlugin()); } }


        /// <summary>
        /// 文档插件
        /// </summary>
        public ArchivePlugin Archive { get { return _archive ?? (_archive = new ArchivePlugin()); } }

        /// <summary>
        /// 扩展(模块)插件
        /// </summary>
        //public IPluginApp Extends { get { return _extends ?? (_extends = new ExtendsPlugin(_webHandler)); } }

        #endregion


        /// <summary>
        /// 连接插件
        /// </summary>
        public static void Connect()
        {
            Cms.Plugins.Portal.Connect();
            Cms.Plugins.Category.Connect();
            Cms.Plugins.Archive.Connect();
            //Cms.Plugins.Extends.Connect();
            Cms.Plugins.Manager.Connect();
        }

      

        /// <summary>
        /// 获取模板页
        /// </summary>
        /// <param name="filePath">相对于插件目录的文件路径，如插件com.demo引用模板test.html,将test.html放在/plugins/com.demo/下就可以了</param>
        /// <returns></returns>
        public TemplatePage GetPage<T>(string filePath) where T : IPlugin
        {
            this.Load();
            PluginPackAttribute attr = PluginUtil.GetPluginByType(typeof(T)).GetAttribute();
            return this.GetTemplatePage(filePath, attr);
        }

        /// <summary>
        /// 获取模板页
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="filePath">相对于插件目录的文件路径，如插件com.demo引用模板test.html,将test.html放在/plugins/com.demo/下就可以了</param>
        /// <returns></returns>
        public TemplatePage GetPage(IPlugin plugin, string filePath)
        {
            this.Load();

            PluginPackAttribute attr = plugin.GetAttribute();
            return this.GetTemplatePage(filePath, attr);
        }

        private TemplatePage GetTemplatePage(string filePath, PluginPackAttribute attr)
        {
            string pluginDirPath = attr.WorkSpace;


            string cacheId = String.Concat("plugin", "_tpl_", attr.WorkIndent, filePath);
            string html = Cms.Cache.Get(cacheId) as string;

            if (html == null)
            {
                //模板文件放在/plugins/com.spdepository/pages/下
                string tplFilePath = pluginDirPath + filePath;

                try
                {
                    using (TextReader tr = new StreamReader(tplFilePath))
                    {
                        html = tr.ReadToEnd();
                        tr.Dispose();
                    }

                    Cms.Cache.Insert(cacheId, html, tplFilePath);
                }
                catch (Exception exc)
                {
                    throw new Exception(exc.Message+", File:"+tplFilePath);
                }
               
            }

            TemplatePage tpl = new TemplatePage();
            tpl.TemplateContent = html;

            string pluginPath =this._pluginPath + attr.WorkIndent;

            tpl.AddVariable("os", new
            {
                version = Cms.Version,
                domain = this._domain,
                ppath = pluginPath,
                fpath = this._fpath,
                mjs = this._admJs,
                mcss = this._admCss
            });

            tpl.AddVariable("plugin",new{
                path = pluginPath,
                indent = attr.WorkIndent,
                name = attr.Name
            });

            return tpl;
        }

        private void Load()
        {
            if (!this._loaded)
            {
                this._domain = WebCtx.Domain;
                this._fpath = String.Format("{0}/{1}", _domain,
                       CmsVariables.FRAMEWORK_ASSETS_PATH
                           .Substring(0, CmsVariables.FRAMEWORK_ASSETS_PATH.Length - 1)
                       );
                this._admJs = String.Concat(this._domain, "/",Settings.SYS_ADMIN_TAG, "?res=c2NyaXB0&amp;0.5.1.js&amp;ver=", Cms.Version);
                this._admCss = String.Concat(this._domain,"/", Settings.SYS_ADMIN_TAG, "?res=c3R5bGU=&amp;0.5.1.css&amp;ver=", Cms.Version);
                this._pluginPath =String.Concat(this._domain ,"/", PluginConfig.PLUGIN_DIRECTORY);
                this._loaded = true;
            
            }
        }


        /// <summary>
        /// 在获取用户失败的情况下，则调用此方法
        /// </summary>
        /// <param name="context"></param>
        public void LoadSession(HttpContext context)
        {
            context.Response.Write("<script>location.href='/" +
                Settings.SYS_ADMIN_TAG +
                "?ls" +
                "';</script>");
            context.Response.End();
        }
    }
}
