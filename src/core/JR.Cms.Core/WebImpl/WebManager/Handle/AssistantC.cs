using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.DataTransfer;
using JR.Cms.Library.CacheService;
using SharpCompress.Archive;
using SharpCompress.Common;
using SiteDto = JR.Cms.ServiceDto.SiteDto;

namespace JR.Cms.WebImpl.WebManager.Handle
{
    internal  class AssistantC:BasePage
    {
        public string Category_Clone_GET()
        {
            int targetSiteId;
            int.TryParse(Request["target_site"], out targetSiteId);

            String siteOpt;
            SiteDto targetSite = default(SiteDto);
            
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var e in SiteCacheManager.GetAllSites())
            {
                if (e.SiteId != targetSiteId)
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

        public string Category_Clone_POST()
        {
            int sourceSiteId, targetSiteId, fromCid, toCid;
            int.TryParse(Request.Form["sourceSiteId"], out sourceSiteId);
            int.TryParse(Request.Form["targetSiteId"], out targetSiteId);
            int.TryParse(Request.Form["fromCid"], out fromCid);
            int.TryParse(Request.Form["toCid"], out toCid);
            bool includeChild = Request.Form["includeChild"] == "true";
            bool includeExtend = Request.Form["includeExtend"] == "true";
            bool includeTempateBind = Request.Form["includeTemplateBind"] == "true";

            if (sourceSiteId == 0 || targetSiteId == 0 || fromCid == 0 || toCid == 0)
            {
                return base.ReturnError("参数不正确");
            }
            else if (targetSiteId != this.CurrentSite.SiteId)
            {
                return base.ReturnError("非法操作");
            }

            try
            {
                ServiceCall.Instance.SiteService.CloneCategory(sourceSiteId, targetSiteId, fromCid, toCid, 
                    includeChild,includeExtend,includeTempateBind);
                return base.ReturnSuccess();
            }
            catch(Exception exc)
            {
                return base.ReturnError(exc.Message);
            }
        }

        public string Pub_Archive_GET()
        {
            int sourceSiteId;
            int.TryParse(Request["source_site"], out sourceSiteId);

            String siteOpt;
            SiteDto sourceSite = default(SiteDto);

            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var e in SiteCacheManager.GetAllSites())
            {
                if (e.SiteId != sourceSiteId)
                {
                    dict.Add(e.SiteId.ToString(), e.Name);
                }
                else
                {
                    sourceSite = e;
                }
            }
            siteOpt = Helper.GetOptions(dict, null);

            ViewData["site_opt"] = siteOpt;
            ViewData["source_name"] = sourceSite.Name;
            ViewData["source_site"] = sourceSite.SiteId;
            ViewData["id_array"] = Request["id_array"] ?? "";
            ViewData["post_url"] = Request.Url.PathAndQuery;

            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Assistant_Archive_Clone_Pub));
        }


        public string Pub_Archive_POST()
        {
            int sourceSiteId, targetSiteId, toCid;
            int.TryParse(Request.Form["sourceSiteId"], out sourceSiteId);
            int.TryParse(Request.Form["targetSiteId"], out targetSiteId);

            int.TryParse(Request.Form["toCid"], out toCid);
            bool includeExtend = Request.Form["includeExtend"] == "true";
            bool includeTempateBind = Request.Form["includeTemplateBind"] == "true";
            bool includeRelatedLink = Request.Form["includeRelatedLink"] == "true";

            string idArrParam = Request.Form["idArr"];
            if (sourceSiteId != this.CurrentSite.SiteId || string.IsNullOrEmpty(idArrParam) || toCid ==0)
            {
                return base.ReturnError("参数不正确");
            }

            int[] idArray = Array.ConvertAll(idArrParam.Split(','), a =>int.Parse(a));
            Array.Reverse(idArray,0,idArray.Length);  //反转顺序

            try
            {
                IDictionary<int, string> errs = ServiceCall.Instance.SiteService.ClonePubArchive(sourceSiteId, targetSiteId, toCid,
                  idArray, includeExtend, includeTempateBind,includeRelatedLink);
                if (errs.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var err in errs)
                    {
                        sb.Append(err.Value.Replace("<break>", "\\n")).Append("\\n");
                    }
                    return "{\"result\":false,\"message\":\"" + sb.ToString() + "\"}";
                }
                return base.ReturnSuccess();
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }
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
