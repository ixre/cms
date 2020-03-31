using System;
using System.IO;
using System.Reflection;
using System.Text;
using JR.Cms.Conf;
using JR.Stand.Core;

namespace JR.Cms.WebImpl.Resource
{
    /// <summary>
    /// 站点静态资源初始化
    /// </summary>
    public class SiteResourceInit
    {
        const string comment = "/* 警告:此文件由系统自动生成,请勿修改,因为可能导致您的更改丢失! */\r\n";
        const string cssComment = " /* 此文件由系统自动生成,所有样式表请引用此文件!*/\r\n";

        public static void Init()
        {
            ExtraMasterAssets();
            
            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "base.css",
                cssComment + "@import url(\"icon-font.css\");\r\n"
                           + ResourceUtility.CompressHtml(GetResource("Web/Resource/SiteResources/site-base.css"))
                                                          + "\n /* merge page.css */\n"
                                                          + ResourceUtility.CompressHtml(GetResource("Web/Resource/SiteResources/site-page.css")), false);

            //Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "cms.js", 
            //    comment + SiteResource.cms_core_min, !true);
            
            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "base.min.js",
                comment + GetResource("Web/Resource/SiteResources/base.min.js") +
                "\n" + ResourceUtility.CompressHtml(GetResource("Web/Resource/SiteResources/cms.core.js")), false);
            
            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "api.js",
                comment + GetResource("Web/Resource/SiteResources/js_cms_api.js"), false);
            
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/ui.js",
            //     comment + GetResource("js_lib_ui.js"), false);
            //
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/dialog.js",
            //     comment + GetResource("js_lib_dialog.js"), false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/form.js",
            //     comment + GetResource("js_lib_form.js"), false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/datagrid.js",
            //     comment + GetResource("js_lib_datagrid.js"), false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/scroller.js",
            //     comment + GetResource("js_lib_scroller.js"), false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/scrollbar.js",
            //     comment +GetResource("js_lib_scrollbar.js"), false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/roller.js",
            //     comment + GetResource("js_lib_roller.js"), false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/table.js",
            //     comment + GetResource("js_lib_table.js"), false);
            //
            // //Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/validate.js",
            // //    comment + SiteResource.js_lib_validate, false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/upload.js",
            //     comment + GetResource("js_lib_upload.js"), false);
            //
            // Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/animation.js",
            //     comment + GetResource("js_lib_animation.js"), false);
            
            // animate.css
            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "animate.css", comment + 
                                                                      GetResource("Web/Resource/SiteResources/Assets/animate.css"), false);
            // wow.js
            Reset(CmsVariables.FRAMEWORK_ASSETS_PATH + "js/wow.js", comment +GetResource("Web/Resource/SiteResources/Assets/wow.js"), false);
        }

        private static void ExtraMasterAssets()
        {
             Reset(CmsVariables.FRAMEWORK_PATH + "mui/js/base.js",
                 comment + GetResource("Web/Resource/ManageResources/manage_js.js"), false);
             Reset(CmsVariables.FRAMEWORK_PATH + "mui/js/component.js",
                 comment +GetResource("Web/Resource/ManageResources/ui_component.source.js"), false);
        }

        private static string GetResource(string fileName)
        {
          return  ResourcesReader.Read(typeof(SiteResourceInit).Assembly, fileName);
        }

        private static void Reset(string filePath, string content, bool keepBak)
        {
            var fileName = $"{EnvUtil.GetBaseDirectory()}/{filePath}";
            var file = new FileInfo(fileName);

            try
            {
                if (keepBak && file.Exists)
                {
                    var bakFile = new FileInfo(fileName + ".bak");
                    if (bakFile.Exists) bakFile.Delete();
                    File.Move(fileName, fileName + ".bak");
                }

                var data = Encoding.UTF8.GetBytes(content);

                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
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
            string tplRootPath= String.Format("{0}templates/", EnvUtil.GetBaseDirectory());
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