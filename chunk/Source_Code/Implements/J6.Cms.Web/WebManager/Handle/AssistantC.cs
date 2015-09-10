
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.DataTransfer;
using J6.Cms.WebManager;
using SharpCompress.Archive;
using SharpCompress.Common;

namespace J6.Cms.Web.WebManager.Handle
{
    internal  class AssistantC:BasePage
    {
        public string Category_Clone_GET()
        {
            int fromSiteId;
            int.TryParse(Request["target_site"], out fromSiteId);

            String siteOpt;
            SiteDto targetSite = default(SiteDto);
            
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var e in SiteCacheManager.GetAllSites())
            {
                if (e.SiteId != fromSiteId)
                {
                    dict.Add(e.SiteId.ToString(), e.Name);
                }
                else
                {
                    targetSite = e;
                }
            }
            siteOpt = Helper.GetOptions(dict, null);

            ViewData["site_opt"] = siteOpt;
            ViewData["target_name"] = targetSite.Name;
            ViewData["target_site"] = targetSite.SiteId;

            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Assistant_Category_Clone));
        }


        #region 系统补丁

        /// <summary>
        /// 系统补丁页面
        /// </summary>
        public string Local_patch_GET()
        {
            string patchlistHtml;

            DirectoryInfo dir = new DirectoryInfo(Cms.PyhicPath + CmsVariables.TEMP_PATH + "patch");
            if (!dir.Exists)
            {
                dir.Create();
                patchlistHtml = "<div style=\"color:red\">没有补丁可供安装！</div>";
            }
            else
            {
                FileInfo[] files = dir.GetFiles("*.zip");
                if (files.Length == 0)
                {
                    patchlistHtml = "<div style=\"color:red\">没有补丁可供安装！</div>";
                }
                else
                {
                    //对文件排序
                    //Array.Sort(files, new Comparison<FileInfo>((a, b) => { return a.CreationTime > b.CreationTime ? 1 : -1; }));

                    StringBuilder sb = new StringBuilder();
                    int i = 0;
                    foreach (FileInfo file in files)
                    {
                        sb.Append("<div><input type=\"radio\" name=\"applypatchfile\"")
                            .Append(++i == 1 ? " checked=\"checked\"" : String.Empty)
                            .Append(" value=\"")
                            .Append(file.Name).Append("\"/><label>")
                            .Append(file.Name).Append("</label></div>");
                    }

                    patchlistHtml = sb.ToString();
                }
            }
            ViewData["patchlist"] = patchlistHtml;

            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Assistant_Local_Patch));
        }

        /// <summary>
        /// 上传补丁文件
        /// </summary>
        public void Patch_upload_POST()
        {
            if (base.Request.Files.Count != 0)
            {
                HttpPostedFile file = base.Request.Files[0];

                try
                {
                    file.SaveAs(String.Format("{0}{1}patch/{2}", Cms.PyhicPath,CmsVariables.TEMP_PATH, file.FileName));
                    base.Response.Write("<script>window.parent.location.reload();</script>");
                }
                catch
                {
                    base.Response.Write("<script>alert('无权限保存补丁文件!');</script>");
                }
            }
        }

        /// <summary>
        /// 安装补丁
        /// </summary>
        public void Local_patch_POST()
        {
            string appDir = Cms.PyhicPath;
            string filePath = String.Format(@"{0}{1}patch\{2}", appDir, CmsVariables.TEMP_PATH,base.Request["file"]);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                //bool result=ZipUtility.UncompressFile(@"C:\", filePath, true);
                try
                {
                    System.Threading.Thread.Sleep(1000);

                    #region dotnetzip
                    /*

                    // var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                    using (ZipFile zip = ZipFile.Read(filePath))//, options))
                    {
                        // This call to ExtractAll() assumes:
                        //   - none of the entries are password-protected.
                        //   - want to extract all entries to current working directory
                        //   - none of the files in the zip already exist in the directory;
                        //     if they do, the method will throw.
                        zip.ExtractAll(appDir,ExtractExistingFileAction.OverwriteSilently);
                    }

					 */
                    #endregion

                    #region sharpcompress

                    IArchive archive = ArchiveFactory.Open(filePath);
                    foreach (IArchiveEntry entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            //Console.WriteLine(entry.FilePath);
                            entry.WriteToDirectory(appDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                    }
                    archive.Dispose();

                    #endregion


                }
                catch (System.Exception ex1)
                {
                    base.Response.Write(ex1.Message);
                }
            }
        }

        #endregion
    }
}
