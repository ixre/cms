//
// Copyright 2011 (C) OPS, All right reseved.
// Name :ArchiveWatchEventRegister.cs
// Author:newmin
// Date:2011/07/25
//

namespace Cms.Web
{

    using System;
    using System.IO;
    using J6.Cms;
    using J6.Cms.BLL;
    using J6.Cms.Models;

    /// <summary>
    /// 文档监视事件注册
    /// </summary>
    public class WatchEventRegister
    {

        private static string htmlPath;

        private const string htmlPathFormat = "{0}{1}.html";
            

        /// <summary>
        /// 是否使用默认的Html生成路径
        /// </summary>
        public static bool UseDefaultHtmlPath = true;

        /// <summary>
        /// 存放生成html的路径
        /// </summary>
        public static string HtmlPath
        {
            get { return htmlPath; }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException("生成Html的路径不能为空!");
                else if (!value.StartsWith("/")) throw new ArgumentException("路径必须以\"/\"开头");
                htmlPath = value;
            }
        }


        public static void Register()
        {

            WatchService.OnClearingCache += new WatchBehavior(WatchService_OnClearingCache);

            //WatchService.OnArchivePublished += new ArchiveBehavior(WatchService_OnArchivePublished);
            //WatchService.OnPrevArchiveUpdate += new ArchiveBehavior(WatchService_OnPrevArchiveUpdate);
            //WatchService.OnArchiveUpdated += new ArchiveBehavior(WatchService_OnArchiveUpdated);
            //WatchService.OnArchiveDeleted += new ArchiveBehavior(WatchService_OnArchiveDeleted);
            //WatchService.OnPrevArchiveDelete += new ArchiveBehavior(WatchService_OnPrevArchiveDelete);
        }

        static void WatchService_OnClearingCache()
        {
            //重新注册模板
            ApplicationManager.Reload();

            //清除文档辅助方法缓存
            ArchiveUtility.ClearCache();

          

            //初始化数据
            //PageGenerator.Generate(PageGeneratorObject.InitData);

            //退出时候更新首页
           // PageGenerator.Generate(PageGeneratorObject.Default);

            //更新about页
            // PageGenerator.Generate(PageGeneratorObject.About);

        }


        /// <summary>
        /// 文档发布时候发生
        /// </summary>
        /// <param name="archive"></param>
        static void WatchService_OnArchivePublished(Archive archive)
        {
            return;
            if (archive == null) return;

            //更新标签链接
            ArchiveUtility.UpdateArchiveTagLinks("/tags/{0}",archive,true);
            Category category = new CategoryBLL().Get(a => a.ID == archive.Cid);
            PageGenerator.Generate(PageGeneratorObject.ArchivePage,category,archive);
        
        }


        /// <summary>
        /// 删除文档之前发生
        /// </summary>
        /// <param name="archive"></param>
        static void WatchService_OnPrevArchiveDelete(Archive archive)
        {
        }

        /// <summary>
        /// 文档删除之后发生
        /// </summary>
        /// <param name="archive"></param>
        static void WatchService_OnArchiveDeleted(Archive archive)
        {
            if (archive != null)
            {
                //先删除文件,避免修改分类后无法识别生成文件位置

                string basePath = AppDomain.CurrentDomain.BaseDirectory + (UseDefaultHtmlPath
                        ? ArchiveUtility.GetCategoryDirectoryPath(archive.Cid)
                        : HtmlPath);

                string fileID = String.IsNullOrEmpty(archive.Alias) ? archive.ID : archive.Alias;

                FileInfo file = new FileInfo(String.Format(htmlPathFormat,basePath,fileID));

                if (file.Exists) file.Delete();
            }
        }

        /// <summary>
        /// 文档删除之前发生
        /// </summary>
        /// <param name="archive"></param>
        static void WatchService_OnPrevArchiveUpdate(Archive archive)
        {
            //先删除文件,避免修改分类后无法识别生成文件位置

            string basePath =AppDomain.CurrentDomain.BaseDirectory+(UseDefaultHtmlPath
                    ?ArchiveUtility.GetCategoryDirectoryPath(archive.Cid)
                    :HtmlPath);

            FileInfo[] files = new FileInfo[2] {
                new FileInfo(String.Format(htmlPathFormat,basePath,archive.ID)),
                new FileInfo(String.Format(htmlPathFormat,basePath,archive.Alias))
            };
            foreach (FileInfo file in files)
            {
                if (file.Exists) file.Delete();
            }
        }

        /// <summary>
        /// 文档更新之后发生
        /// </summary>
        /// <param name="archive"></param>
        static void WatchService_OnArchiveUpdated(Archive archive)
        {
            return;
            if (archive == null) return;

            //更新标签链接
            ArchiveUtility.UpdateArchiveTagLinks("/tags/{0}",archive,true);
            Category category = new CategoryBLL().Get(a => a.ID == archive.Cid);
            PageGenerator.Generate(PageGeneratorObject.ArchivePage,category, archive);

        }
    }
}