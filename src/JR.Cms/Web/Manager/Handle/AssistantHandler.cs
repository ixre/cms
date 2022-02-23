using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using JR.Cms.Conf;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace JR.Cms.Web.Manager.Handle
{
    internal class AssistantHandler : BasePage
    {
        public string Category_Clone()
        {
            int targetSiteId;
            int.TryParse(Request.Query("target_site"), out targetSiteId);

            string siteOpt;
            var targetSite = default(SiteDto);

            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var e in SiteCacheManager.GetAllSites())
                if (e.SiteId != targetSiteId)
                    dict.Add(e.SiteId.ToString(), e.Name);
                else
                    targetSite = e;
            siteOpt = Helper.GetOptions(dict, null);

            ViewData["site_opt"] = siteOpt;
            ViewData["target_name"] = targetSite.Name;
            ViewData["target_site"] = targetSite.SiteId;

            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Assistant_Category_Clone));
        }

        public string Category_Clone_POST()
        {
            int sourceSiteId, targetSiteId, fromCid, toCid;
            int.TryParse(Request.Form("sourceSiteId"), out sourceSiteId);
            int.TryParse(Request.Form("targetSiteId"), out targetSiteId);
            int.TryParse(Request.Form("fromCid"), out fromCid);
            int.TryParse(Request.Form("toCid"), out toCid);
            var includeChild = Request.Form("includeChild") == "true";
            var includeExtend = Request.Form("includeExtend") == "true";
            var includeTempateBind = Request.Form("includeTemplateBind") == "true";

            if (sourceSiteId == 0 || targetSiteId == 0 || fromCid == 0 || toCid == 0)
                return ReturnError("参数不正确");
            else if (targetSiteId != CurrentSite.SiteId) return ReturnError("非法操作");

            try
            {
                ServiceCall.Instance.SiteService.CloneCategory(sourceSiteId, targetSiteId, fromCid, toCid,
                    includeChild, includeExtend, includeTempateBind);
                return ReturnSuccess();
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }
        }

        public string Pub_Archive()
        {
            int sourceSiteId;
            int.TryParse(Request.Query("source_site"), out sourceSiteId);

            string siteOpt;
            var sourceSite = default(SiteDto);

            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var e in SiteCacheManager.GetAllSites())
                if (e.SiteId != sourceSiteId)
                    dict.Add(e.SiteId.ToString(), e.Name);
                else
                    sourceSite = e;
            siteOpt = Helper.GetOptions(dict, null);

            var path = Request.GetPath();
            var query = Request.GetQueryString();
            ViewData["site_opt"] = siteOpt;
            ViewData["source_name"] = sourceSite.Name;
            ViewData["source_site"] = sourceSite.SiteId;
            ViewData["id_array"] = Request.Query("id_array")[0] ?? "";
            ViewData["post_url"] = path + query;
            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Assistant_Archive_Clone_Pub));
        }


        public string Pub_Archive_POST()
        {
            int sourceSiteId, targetSiteId, toCid;
            int.TryParse(Request.Form("sourceSiteId"), out sourceSiteId);
            int.TryParse(Request.Form("targetSiteId"), out targetSiteId);

            int.TryParse(Request.Form("toCid"), out toCid);
            var includeExtend = Request.Form("includeExtend") == "true";
            var includeTempateBind = Request.Form("includeTemplateBind") == "true";
            var includeRelatedLink = Request.Form("includeRelatedLink") == "true";

            string idArrParam = Request.Form("idArr");
            if (sourceSiteId != CurrentSite.SiteId || string.IsNullOrEmpty(idArrParam) || toCid == 0)
                return ReturnError("参数不正确");

            var idArray = Array.ConvertAll(idArrParam.Split(','), a => int.Parse(a));
            Array.Reverse(idArray, 0, idArray.Length); //反转顺序

            try
            {
                var errs = ServiceCall.Instance.SiteService.ClonePubArchive(sourceSiteId, targetSiteId, toCid,
                    idArray, includeExtend, includeTempateBind, includeRelatedLink);
                if (errs.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var err in errs) sb.Append(err.Value.Replace("<break>", "\\n")).Append("\\n");
                    return "{\"result\":false,\"message\":\"" + sb.ToString() + "\"}";
                }

                return ReturnSuccess();
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }
        }

        #region 系统补丁

        /// <summary>
        /// 系统补丁页面
        /// </summary>
        public string Local_patch()
        {
            string patchlistHtml;

            var dir = new DirectoryInfo(Cms.PhysicPath + CmsVariables.TEMP_PATH + "patch");
            if (!dir.Exists)
            {
                dir.Create();
                patchlistHtml = "<div style=\"color:red\">没有补丁可供安装！</div>";
            }
            else
            {
                var files = dir.GetFiles("*.zip");
                if (files.Length == 0)
                {
                    patchlistHtml = "<div style=\"color:red\">没有补丁可供安装！</div>";
                }
                else
                {
                    //对文件排序
                    //Array.Sort(files, new Comparison<FileInfo>((a, b) => { return a.CreationTime > b.CreationTime ? 1 : -1; }));

                    var sb = new StringBuilder();
                    var i = 0;
                    foreach (var file in files)
                        sb.Append("<div><input type=\"radio\" name=\"applypatchfile\"")
                            .Append(++i == 1 ? " checked=\"checked\"" : string.Empty)
                            .Append(" value=\"")
                            .Append(file.Name).Append("\"/><label>")
                            .Append(file.Name).Append("</label></div>");

                    patchlistHtml = sb.ToString();
                }
            }

            ViewData["patchlist"] = patchlistHtml;

            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Assistant_Local_Patch));
        }

        /// <summary>
        /// 上传补丁文件
        /// </summary>
        public void Patch_upload_POST()
        {
            var file = Request.FileIndex(0);
            try
            {
                var path = $"{Cms.PhysicPath}{CmsVariables.TEMP_PATH}patch/{file.GetFileName()}";
                file.Save(path);
                Response.WriteAsync("<script>window.parent.location.reload();</script>");
            }
            catch
            {
                Response.WriteAsync("<script>alert('无权限保存补丁文件!');</script>");
            }
        }

        /// <summary>
        /// 安装补丁
        /// </summary>
        public void Local_patch_POST()
        {
            var appDir = Cms.PhysicPath;
            var filePath = $@"{appDir}{CmsVariables.TEMP_PATH}patch\{Request.Query("file")}";
            var file = new FileInfo(filePath);
            if (file.Exists)
                //bool result=ZipUtility.UncompressFile(@"C:\", filePath, true);
                try
                {
                    Thread.Sleep(1000);

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

                    var archive = ArchiveFactory.Open(filePath);
                    foreach (var entry in archive.Entries)
                        if (!entry.IsDirectory)
                        {
                            //Console.WriteLine(entry.FilePath);
                            entry.WriteToDirectory(appDir,new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }

                    archive.Dispose();

                    #endregion
                }
                catch (Exception ex1)
                {
                    Response.WriteAsync(ex1.Message);
                }
        }

        #endregion
    }
}