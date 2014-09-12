using Com.PluginKernel;
using Com.PluginKernel.Web;
using Ops.Cms.Conf;
using Ops.Cms.Core.Plugins;
using Ops.Template;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Ops.Cms
{
    public class CmsPluginContext
    {
        private static CmsPlugin cmsPlugin;
        private static PortalPlugin portal;
        private static CategoryPlugin category;
        private static ArchivePlugin archive;
        private static ExtendsPlugin extends;
        private static PluginWebHandleProxy<HttpContext> webHandler;


        static CmsPluginContext()
        {
            webHandler = new PluginWebHandleProxy<HttpContext>();
        }

        internal CmsPluginContext()
        {
        }

        #region  插件对象

        /// <summary>
        /// 插件管理
        /// </summary>
        public CmsPlugin Manager { get { return cmsPlugin ?? (cmsPlugin = new CmsPlugin()); } }

        /// <summary>
        /// 入口插件
        /// </summary>
        public PortalPlugin Portal { get { return portal ?? (portal = new PortalPlugin()); } }

        /// <summary>
        /// 栏目插件
        /// </summary>
        public CategoryPlugin Category { get { return category ?? (category = new CategoryPlugin()); } }


        /// <summary>
        /// 文档插件
        /// </summary>
        public ArchivePlugin Archive { get { return archive ?? (archive = new ArchivePlugin()); } }

        /// <summary>
        /// 扩展(模块)插件
        /// </summary>
        public IExtendApp Extends { get { return extends ?? (extends = new ExtendsPlugin(webHandler)); } }

        #endregion


        /// <summary>
        /// 连接插件
        /// </summary>
        public static void Connect()
        {
            Cms.Plugins.Portal.Connect();
            Cms.Plugins.Category.Connect();
            Cms.Plugins.Archive.Connect();
            Cms.Plugins.Extends.Connect();
            Cms.Plugins.Manager.Connect();
        }

      

        /// <summary>
        /// 获取模板页
        /// </summary>
        /// <param name="filePath">相对于插件目录的文件路径，如插件com.demo引用模板test.html,将test.html放在/plugins/com.demo/下就可以了</param>
        /// <returns></returns>
        public TemplatePage GetPage<T>(string filePath) where T : IPlugin
        {
            PluginPackAttribute attr = PluginUtil.GetAttribute<T>();
            string pluginDirPath = String.Concat(PluginConfig.PLUGIN_DIRECTORY, attr.WorkIndent);

            string cacheID = String.Concat("plugin", "_tpl_", attr.WorkIndent, filePath);
            string html = Cms.Cache.Get(cacheID) as string;

            if (html == null)
            {
                //模板文件放在/plugins/com.spdepository/pages/下
                string tplFilePath = String.Concat(
                    Cms.PyhicPath,
                    pluginDirPath, "/",
                    filePath);

                using (TextReader tr = new StreamReader(tplFilePath))
                {
                    html = tr.ReadToEnd();
                    tr.Dispose();
                }
                Cms.Cache.Insert(cacheID, html, tplFilePath);
            }


            TemplatePage tpl = new TemplatePage();
            tpl.TemplateContent = html;
            tpl.AddVariable("os", new
            {
                version = Cms.Version,
                ppath = String.Format("/{0}", pluginDirPath),
                fpath = String.Format("{0}{1}", Cms.Context.StaticDomain,
                        CmsVariables.FRAMEWORK_ASSETS_PATH
                        .Substring(0, CmsVariables.FRAMEWORK_ASSETS_PATH.Length - 1)
                        )
            });
            return tpl;
        }
    }
}
