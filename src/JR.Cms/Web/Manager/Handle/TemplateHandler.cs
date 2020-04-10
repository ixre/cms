//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
// 
// Project: jr.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.Utility;
using JR.Stand.Core;
using JR.Stand.Core.Framework;
using Newtonsoft.Json;
using SharpCompress.Archive;
using SharpCompress.Common;

namespace JR.Cms.Web.Manager.Handle
{
    public class TemplateHandler : BasePage
    {
        /// <summary>
        /// 编辑模板
        /// </summary>
        public void Edit()
        {
            string tpl = Request.Query("tpl");
            if (string.IsNullOrEmpty(tpl)) tpl = CurrentSite.Tpl;
            var sb = new StringBuilder();

            var dir = new DirectoryInfo($"{EnvUtil.GetBaseDirectory()}/templates/{tpl}/");
            if (!dir.Exists)
            {
                Response.Redirect(Request.GetPath() + "?module=template&action=templates", false);
                return;
            }

            EachClass.WalkTemplateFiles(dir, sb, tpl);

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Template_Edit), new
            {
                tplfiles = sb.ToString(),
                tpl = tpl
            });
        }

        public string GetListJson_POST()
        {
            string tpl = Request.Form("tpl");
            var dir = new DirectoryInfo(string.Format("{0}templates/{1}/", Cms.PhysicPath, tpl));
            if (dir.Exists)
            {
                IList<EachClass.TemplateNames> list = new List<EachClass.TemplateNames>();
                var nameDict = Cms.TemplateManager.Get(tpl).GetNameDictionary();
                EachClass.IterTemplateFiles2(dir, dir.FullName.Length, list, nameDict);
                return JsonConvert.SerializeObject(list);
            }

            return "[]";
        }

        public string SaveNames_POST()
        {
            string tpl = Request.Query("tpl");
            IDictionary<string, string> list = new Dictionary<string, string>();
            int row;
            foreach (var pk in Request.FormKeys())
                if (pk.StartsWith("k_"))
                    if (int.TryParse(pk.Substring(2), out row)) //获取行号
                        list.Add(Request.Form(pk), Request.Form("v_" + row.ToString()));

            try
            {
                Cms.TemplateManager.Get(tpl).SaveTemplateNames(JsonConvert.SerializeObject(list));
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            return ReturnSuccess();
        }

        /// <summary>
        /// 编辑文件
        /// </summary>
        public string EditFile()
        {
            string path = Request.Query("path");
            string content,
                bakinfo;

            FileInfo file, bakfile;

            file = new FileInfo(EnvUtil.GetBaseDirectory()+ path);
            bakfile = new FileInfo(EnvUtil.GetBaseDirectory() + Helper.GetBackupFilePath(path));


            if (!file.Exists)
            {
                return "文件不存在!";
            }
            else
            {
                if (bakfile.Exists)
                    bakinfo = string.Format(@"上次修改时间日期：{0:yyyy-MM-dd HH:mm:ss}&nbsp;
                                <a style=""margin-right:20px"" href=""javascript:;"" onclick=""process('restore')"">还原</a>",
                        bakfile.LastWriteTime);
                else
                    bakinfo = "";
            }

            var sr = new StreamReader(file.FullName);
            content = sr.ReadToEnd();
            sr.Dispose();


            content = content.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            ViewData["raw_content"] = content;
            ViewData["bakinfo"] = bakinfo;
            ViewData["file"] = path;
            ViewData["path"] = path;

            return RequireTemplate(CompressHtml(ResourceMap.GetPageContent(ManagementPage.Template_EditFile)));
        }

        public void EditFile_POST()
        {
            //修改系统文件
            if (Request.Query("path")[0].IndexOf("templates/", StringComparison.Ordinal) == -1)
            {
                Response.WriteAsync("不允许修改!");
                return;
            }

            string action = Request.Form("action");
            string path = Request.Form("path");
            string content = Request.Form("content");

            var file = new FileInfo(Cms.PhysicPath + path);
            if (file.Exists)
            {
                if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    Response.WriteAsync("文件只读，无法修改!");
                    return;
                }
                else
                {
                    var backFile = string.Concat(Cms.PhysicPath, Helper.GetBackupFilePath(path));

                    if (action == "save")
                    {
                        var backupDir = backFile.Substring(0, backFile.LastIndexOfAny(new char[] {'/', '\\'}) + 1);

                        if (!Directory.Exists(backupDir))
                        {
                            Directory.CreateDirectory(backupDir).Create();
                            File.SetAttributes(backupDir, FileAttributes.Hidden);
                        }
                        else
                        {
                            if (File.Exists(backFile)) File.Delete(backFile);
                        }

                        //生成备份文件
                        file.CopyTo(backFile, true);
                        //global::System.IO.File.SetAttributes(backFile,file.Attributes&FileAttributes.Hidden);

                        //重写现有文件
                        var fs = new FileStream(file.FullName, FileMode.Truncate, FileAccess.Write, FileShare.Read);
                        var data = Encoding.UTF8.GetBytes(content);
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                        fs.Dispose();

                        Response.WriteAsync("保存成功!");
                    }
                    else if (action == "restore")
                    {
                        FileInfo bakfile = new FileInfo(backFile),
                            tmpfile = new FileInfo(backFile + ".tmp");

                        var _fpath = file.FullName;

                        if (bakfile.Exists)
                        {
                            file.MoveTo(backFile + ".tmp");
                            bakfile.MoveTo(_fpath);
                            tmpfile.MoveTo(backFile);

                            //global::System.IO.File.SetAttributes(_fpath + ".bak",file.Attributes & FileAttributes.Hidden);
                            File.SetAttributes(_fpath, file.Attributes & FileAttributes.Normal);
                        }

                        Response.WriteAsync("还原成功!");
                    }
                }
            }
            else
            {
                Response.WriteAsync("文件不存在,请检查!");
            }
        }

        public void CreateView_POST()
        {
            var tplname = string.Format("templates/{0}/{1}.{2}",
                CurrentSite.Tpl,
                Request.Form("name"),
                Request.Form("type") == "1" ? "part.html" : "html");

            var tplPath = string.Format("{0}/{1}",
                EnvUtil.GetBaseDirectory(),
                tplname);

            if (File.Exists(tplPath))
                Response.WriteAsync("文件已经存在!");
            else
                try
                {
                    //global::System.IO.Directory.CreateDirectory(tplPath).Create();   //创建目录
                    File.Create(tplPath).Dispose(); //创建文件

                    Cms.Template.Reload(); //重新注册模板

                    Response.WriteAsync(tplname);
                }
                catch (Exception e)
                {
                    // Response.Write(e.Message);
                    Response.WriteAsync("无权限创建文件，请设置视图目录(templates)可写权限！");
                }
        }

        /// <summary>
        /// 模板设置
        /// </summary>
        public void Settings()
        {
            string tpl = Request.Query("tpl");
            if (string.IsNullOrEmpty(tpl)) tpl = CurrentSite.Tpl;

            var tplSetting = Cms.TemplateManager.Get(tpl);

            ViewData["json"] = JsonConvert.SerializeObject(new
            {
                //模板
                tpl_CFG_OutlineLength = tplSetting.CfgOutlineLength.ToString(),
                tpl_CFG_AllowAmousComment = tplSetting.CFG_AllowAmousComment,
                tpl_CFG_CommentEditorHtml = tplSetting.CFG_CommentEditorHtml,
                tpl_CFG_ArchiveTagsFormat = tplSetting.CFG_ArchiveTagsFormat,
                tpl_CFG_FriendLinkFormat = tplSetting.CFG_FriendLinkFormat,
                tpl_CFG_FriendShowNum = tplSetting.CFG_FriendShowNum,
                tpl_CFG_NavigatorLinkFormat = tplSetting.CFG_NavigatorLinkFormat,
                tpl_CFG_NavigatorChildFormat = tplSetting.CFG_NavigatorChildFormat,
                tpl_CFG_SitemapSplit = tplSetting.CFG_SitemapSplit,
                tpl_CFG_TrafficFormat = tplSetting.CFG_TrafficFormat,
                tpl_CFG_Enabled_Mobi = tplSetting.CfgEnabledMobiPage ? "1" : "0",
                tpl_CFG_Show_Error = tplSetting.CfgShowError ? "1" : "0",
            });
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Template_Setting));
        }

        public void Settings_POST()
        {
            string tpl = Request.Query("tpl");
            if (string.IsNullOrEmpty(tpl))
            {
                tpl = CurrentSite.Tpl;
            }
            else
            {
                if (!UserState.Administrator.Current.IsMaster)
                {
                    RenderError("无权执行此操作!");
                    return;
                }
            }

            var tplSetting = Cms.TemplateManager.Get(tpl);
            var req = Request;
            int outlineLength;
            int.TryParse(req.Form("tpl_CFG_OutlineLength"), out outlineLength);
            tplSetting.CfgOutlineLength = outlineLength;
            tplSetting.CFG_AllowAmousComment = req.Form("tpl_CFG_AllowAmousComment") == "on";
            tplSetting.CFG_CommentEditorHtml = req.Form("tpl_CFG_CommentEditorHtml");
            tplSetting.CFG_ArchiveTagsFormat = req.Form("tpl_CFG_ArchiveTagsFormat");
            tplSetting.CFG_FriendLinkFormat = req.Form("tpl_CFG_FriendLinkFormat");
            int friendlinkNum;
            int.TryParse(req.Form("tpl_CFG_FriendShowNum"), out friendlinkNum);
            tplSetting.CFG_FriendShowNum = friendlinkNum;
            tplSetting.CFG_NavigatorLinkFormat = req.Form("tpl_CFG_NavigatorLinkFormat");
            tplSetting.CFG_NavigatorChildFormat = req.Form("tpl_CFG_NavigatorChildFormat");
            tplSetting.CFG_SitemapSplit = req.Form("tpl_CFG_SitemapSplit");
            tplSetting.CFG_TrafficFormat = req.Form("tpl_CFG_TrafficFormat");
            tplSetting.CfgEnabledMobiPage = req.Form("tpl_CFG_Enabled_Mobi") == "1";
            tplSetting.CfgShowError = req.Form("tpl_CFG_Show_Error") == "1";
            tplSetting.Save();

            RenderSuccess("修改成功!");
        }

        /// <summary>
        /// 模板列表
        /// </summary>
        public void Templates()
        {
            if (!UserState.Administrator.Current.IsMaster) return;
            var curTemplate = CurrentSite.Tpl;

            var tplRootPath = string.Format("{0}/templates/",EnvUtil.GetBaseDirectory());
            var tplList = new string[0];
            var dir = new DirectoryInfo(tplRootPath);
            if (dir.Exists)
            {
                var dirs = dir.GetDirectories();
                tplList = new string[dirs.Length];
                var i = -1;
                foreach (var d in dirs) tplList[++i] = d.Name;
            }

            SettingFile sf;
            string currentName = "",
                currentThumbnail = "",
                tplConfigFile,
                tplName,
                tplDescrpt,
                tplThumbnail;

            var sb = new StringBuilder();
            foreach (var tpl in tplList)
            {
                tplName = tpl;
                tplThumbnail = null;
                tplDescrpt = null;

                tplConfigFile = $"{tplRootPath}{tpl}/tpl.conf";
                if (File.Exists(tplConfigFile))
                {
                    sf = new SettingFile(tplConfigFile);
                    if (sf.Contains("name")) tplName = sf["name"];

                    if (sf.Contains("thumbnail")) tplThumbnail = sf["thumbnail"];
                    if (sf.Contains("descript")) tplDescrpt = sf["descript"];
                }

                if (string.CompareOrdinal(tpl, curTemplate) != 0)
                {
                    sb.Append("<li><p><a href=\"javascript:;\">");
                    if (tplThumbnail != null)
                        sb.Append("<img src=\"").Append(tplThumbnail).Append("\" alt=\"点击切换模板\" class=\"shot ")
                            .Append(tpl).Append("\"/>");
                    else
                        sb.Append("<span title=\"点击切换模板\" class=\"shot ").Append(tpl)
                            .Append(" thumbnail\">无缩略图</span>");

                    sb.Append("</a></p><p><a href=\"javascript:;\" class=\"t\">")
                        .Append(tplName).Append("</a></p><p><a class=\"btn edit\" href=\"tpl:")
                        .Append(tpl).Append("\">编辑</a>&nbsp;<a class=\"btn down\" href=\"tpl:")
                        .Append(tpl).Append("\">下载</a></p>")
                        .Append("<p class=\"hidden\">").Append(tplDescrpt).Append("</p>")
                        .Append("</li>");
                }
                else
                {
                    currentName = string.IsNullOrEmpty(tplName) ? curTemplate : tplName;
                    if (tplThumbnail != null)
                        currentThumbnail = "<img src=\"" + tplThumbnail + "\" alt=\"点击切换模板\" class=\"shot1 " + tpl +
                                           "\"/>";
                    else
                        currentThumbnail = "<span class=\"shot1 " + tpl + " thumbnail\">无缩略图</span>";
                }
            }

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Template_Manager), new
            {
                list = sb.ToString(),
                current = curTemplate,
                currentName = currentName,
                currentThumbnail = currentThumbnail
            });
        }

        /// <summary>
        /// 设置默认模板
        /// </summary>
        /// <returns></returns>
        public void TemplateAsDefault_POST()
        {
            string tpl = Request.Query("tpl");
            var site = CurrentSite;
            site.Tpl = tpl;
            ServiceCall.Instance.SiteService.SaveSite(site);
            RenderSuccess("");
        }

        /// <summary>
        /// 解压上传模板文件
        /// </summary>
        /// <returns></returns>
        public void UploadInstall_POST()
        {
            var tplRootPath = $"{EnvUtil.GetBaseDirectory()}/templates/";
            var tempTplPath = tplRootPath + "~.tpl";
            const string jsTip = "{\"msg\":\"{0}\"}";
            string resultMessage;

            /*
            Stream upStrm = Request.Files[0].InputStream;

            MemoryStream ms = new MemoryStream();
            byte[] data=new byte[100];
            int totalReadBytes=0;
            int readBytes = 0;

            while (totalReadBytes<upStrm.Length)
            {
                readBytes = upStrm.Read(data,0,data.Length);
                ms.Write(data,0, readBytes);
                totalReadBytes += readBytes;
            }
            */
            
            //保存文件
            var file = Request.FileIndex(0);
            file.Save(tempTplPath);


            var dirIndex = 0;
            var entryCount = 0;

            var archive = ArchiveFactory.Open(tempTplPath);


            foreach (var entry in archive.Entries)
            {
                ++entryCount;

                if (dirIndex == 0)
                {
                    dirIndex++;
                    var dirName = entry.Key.Split('\\', '/')[0];
                    if (Directory.Exists(tplRootPath + dirName))
                    {
                        resultMessage = "模板已经存在!";
                        goto handleOver;
                    }
                }

                if (!entry.IsDirectory)
                    entry.WriteToDirectory(tplRootPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
            }

            if (entryCount == 0)
                resultMessage = "上传的模板不包含任何内容!";
            else
                resultMessage = "模板安装成功!";


            handleOver:

            archive.Dispose();
            File.Delete(tempTplPath);

            //重新注册模板
            Cms.Template.Reload();

            Response.WriteAsync(string.Format(jsTip, resultMessage));
        }

        /// <summary>
        /// 网络安装模板
        /// </summary>
        public void NetInstall_POST()
        {
            string url = Request.Query("url");
            var name = Regex.Match(url, "/([^/]+)\\.zip", RegexOptions.IgnoreCase).Groups[1].Value;
            var result = Updater.InstallTemplate(name, url);
            if (result == 1)
            {
                //重新注册模板
                Cms.Template.Reload();
                RenderSuccess("安装成功!");
            }
            else if (result == -1)
            {
                RenderError("获取模板包失败!");
            }
            else if (result == -2)
            {
                RenderError("模板已经安装!");
            }
        }

        /// <summary>
        /// 下载作为一个压缩文件
        /// </summary>
        public void DownloadZip()
        {
            string tpl = Request.Query("tpl");
            var bytes = ZipHelper.Compress($"{Cms.PhysicPath}templates/{tpl}/", tpl);
            Response.WriteAsync(bytes);
            Response.ContentType ("application/octet-stream");
            Response.AddHeader("Content-Disposition", "attachment;filename=template_" + tpl + ".zip");
            Response.AddHeader("Content-Length", bytes.Length.ToString());
        }

        /// <summary>
        /// 备份模板
        /// </summary>
        public void BackupTemplate()
        {
            string tpl = Request.Query("tpl");

            //设置目录
            var dir = new DirectoryInfo(string.Format("{0}backups/templates/", Cms.PhysicPath));
            if (!dir.Exists)
                Directory.CreateDirectory(dir.FullName).Create();
            else
                Cms.Utility.SetDirCanWrite("backups/templates/");

            ZipHelper.ZipAndSave(string.Format("{0}/templates/{1}/", Cms.PhysicPath, tpl)
                , string.Format("{0}/backups/templet/{1}_{2:yyyyMMddHHss}.zip", Cms.PhysicPath, tpl, DateTime.Now),
                tpl
            );
        }

        /// <summary>
        /// 自动升级模板语法
        /// </summary>
        public void AutoInstall()
        {
            //Automation.TemplateAuto.AutoInstall(base.Request.Query("tpl"));
        }
    }
}