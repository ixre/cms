
using J6.Cms.Conf;

namespace J6.Cms.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using J6.Cms.Web.Resource.WebManager;
    using SharpCompress.Archive;
    using SharpCompress.Common;
    using J6.Cms.Web.Resource;

    /// <summary>
    /// 站点静态资源初始化
    /// </summary>
    public class SiteResourceInit
    {

        public static void Init()
        {
            const string comment="/* 此文件由系统自动生成! */\r\n";
            const string cssComment = " /* 此文件由系统自动生成,所有样式表请引用此文件!*/\r\n";

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "template.css",
                cssComment + ResourceUtility.CompressHtml(SiteResource.template), !true);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "core.js", 
                comment + SiteResource.cms_core_min, !true);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "api.js",
               comment + SiteResource.js_cms_api, !true);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "ui-mini.js",
                comment + SiteResource.js_ui_min, !true);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/ui.js",
                comment + SiteResource.js_lib_ui, false);


            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/dialog.js", 
                comment + SiteResource.js_lib_dialog, false);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/form.js", 
                comment + SiteResource.js_lib_form, false);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/datagrid.js",
               comment + SiteResource.js_lib_datagrid, false);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/scroller.js", 
                comment + SiteResource.js_lib_scroller, false);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/scrollbar.js", 
                comment + SiteResource.js_lib_scrollbar, false);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/roller.js",
                comment + SiteResource.js_lib_roller, false);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/table.js", 
                comment + SiteResource.js_lib_table, false);

            //Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/validate.js",
            //    comment + SiteResource.js_lib_validate, false);

            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/upload.js", 
                comment + SiteResource.js_lib_upload, false);


            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/animation.js",
                comment + SiteResource.js_lib_animation, false);
        }

        private static void Reset(string filePath, string content,bool keepBak)
        {
            string fileName = String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, filePath);
            FileInfo file = new FileInfo(fileName);
            FileInfo bakFile;

            try
            {
                if (keepBak && file.Exists)
                {
                    bakFile = new FileInfo(fileName + ".bak");
                    if (bakFile.Exists)
                    {
                        bakFile.Delete();
                    }
                    File.Move(fileName, fileName + ".bak");
                }

                byte[] data = Encoding.UTF8.GetBytes(content);

                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.SetLength(0);
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                    fs.Dispose();
                }
            }
            catch
            {
            }
        }



        private delegate void ExtractTemplate(string templateName, byte[] data);

        /// <summary>
        /// 释放默认的模板包
        /// </summary>
        [Obsolete]
        private static void ExtractDefaultTemplatePack()
        {
            /*
            string tplRootPath= String.Format("{0}templates/", AppDomain.CurrentDomain.BaseDirectory);
            MemoryStream ms ;
            IArchiveModel archive;
            DirectoryInfo dir;

            ExtractTemplate et = (tpl, zipData) =>
            {
                dir = new DirectoryInfo(tplRootPath + tpl);

                //模板不存在,则解压
                if (!dir.Exists)
                {
                    ms = new MemoryStream(zipData);

                    archive = ArchiveFactory.Open(ms);

                    foreach (IArchiveModelEntry entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            entry.WriteToDirectory(tplRootPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                    }

                    archive.Dispose();
                    ms.Dispose();
                }
            };

            //Default Template
            et("default", SiteResource.tpl_default);

            //Diy Templates
            et("diy", SiteResource.tpl_diy);
            */
        }
    }
}
