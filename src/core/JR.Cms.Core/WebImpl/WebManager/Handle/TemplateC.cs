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
using JR.Cms.CacheService;
using JR.Cms.Core;
using JR.Cms.DataTransfer;
using JR.DevFw.Framework;
using Newtonsoft.Json;
using SharpCompress.Archive;
using SharpCompress.Common;
using SiteDto = JR.Cms.ServiceDto.SiteDto;

namespace JR.Cms.WebImpl.WebManager.Handle
{
    public class TemplateC: BasePage
    {

        /// <summary>
        /// 编辑模板
        /// </summary>
        public void Edit_GET()
        {
            string tpl = Request["tpl"];
            if (String.IsNullOrEmpty(tpl))
            {
                tpl = base.CurrentSite.Tpl;
            }
            StringBuilder sb = new StringBuilder();
           
            DirectoryInfo dir = new DirectoryInfo(String.Format("{0}templates/{1}/", AppDomain.CurrentDomain.BaseDirectory,tpl));
            if (!dir.Exists)
            {
                Response.Redirect(Request.Path+"?module=template&action=templates",true);
                return;
            }

            EachClass.IterialTemplateFiles(dir, sb,tpl);

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Template_Edit), new
            {
                tplfiles= sb.ToString(),
                tpl=tpl
            });
        }

        public string GetListJson_POST()
        {
            String tpl = Request.Form.Get("tpl");
            DirectoryInfo dir = new DirectoryInfo(String.Format("{0}templates/{1}/", Cms.PyhicPath, tpl));
            if (dir.Exists)
            {
                IList<EachClass.TemplateNames> list = new List<EachClass.TemplateNames>();
                IDictionary<String, String> nameDict = Cms.TemplateManager.Get(tpl).GetNameDictionary();
               EachClass.IterTemplateFiles2(dir,dir.FullName.Length,list, nameDict);
                return JsonConvert.SerializeObject(list);
            }
            return "[]";
        }

        public string SaveNames_POST()
        {
            String tpl = Request["tpl"];
            var form = Request.Form;
            IDictionary<String,String> list = new Dictionary<string, string>();
            int row;
            foreach (String pk in form.Keys)
            {
                if (pk.StartsWith("k_"))
                {
                    if (int.TryParse(pk.Substring(2), out row)) //获取行号
                    {
                        list.Add( form[pk], form["v_" + row.ToString()]);
                    }
                }
            }

            try
            {
                Cms.TemplateManager.Get(tpl).SaveTemplateNames(JsonConvert.SerializeObject(list));
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }
            return base.ReturnSuccess();
        }
    
        /// <summary>
        /// 编辑文件
        /// </summary>
        public string EditFile_GET()
        {
            string path = Request["path"];
            string content,
                   bakinfo;

            FileInfo file, bakfile;

            file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + path);
            bakfile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + Helper.GetBackupFilePath(path));


            if (!file.Exists)
            {
                return "文件不存在!"; 
            }
            else
            {
                if (bakfile.Exists)
                {
                    bakinfo = String.Format(@"上次修改时间日期：{0:yyyy-MM-dd HH:mm:ss}&nbsp;
                                <a style=""margin-right:20px"" href=""javascript:;"" onclick=""process('restore')"">还原</a>",
                        bakfile.LastWriteTime);
                }
                else
                {
                    bakinfo = "";
                }
            }

            StreamReader sr = new StreamReader(file.FullName);
            content = sr.ReadToEnd();
            sr.Dispose();


            content = content.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            ViewData["raw_content"] = content;
            ViewData["bakinfo"] = bakinfo;
            ViewData["file"] = path;
            ViewData["path"] = path;

            return base.RequireTemplate(BasePage.CompressHtml(ResourceMap.GetPageContent(ManagementPage.Template_EditFile)));
        }
        public void EditFile_POST()
        {
            //修改系统文件
            if (Request["path"].IndexOf("templates/", StringComparison.Ordinal) == -1 && Request["pwd"] != "$Newmin")
            {
                Response.Write("不允许修改!"); return;
            }


            string action = Request.Form["action"];
            string path = Request.Form["path"];
            string content = Request.Form["content"];

            FileInfo file = new FileInfo(Cms.PyhicPath + path);



            if (file.Exists)
            {

                if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    Response.Write("文件只读，无法修改!"); return;
                }
                else
                {
                	string backFile=String.Concat(Cms.PyhicPath,Helper.GetBackupFilePath(path));
                    
                    if (action == "save")
                    {
                    	string backupDir=backFile.Substring(0,backFile.LastIndexOfAny(new char[]{'/','\\'})+1);
                    	
		        		if(!Directory.Exists(backupDir))
		        		{
		        			Directory.CreateDirectory(backupDir).Create();
		        			File.SetAttributes(backupDir,FileAttributes.Hidden);
		        		}
		        		else
		        		{
		        			if(File.Exists(backFile))
		        			{
                        		File.Delete(backFile);
		        			}
		        		}
                        //生成备份文件
                        file.CopyTo(backFile, true);
                        //global::System.IO.File.SetAttributes(backFile,file.Attributes&FileAttributes.Hidden);

                        //重写现有文件
                        FileStream fs = new FileStream(file.FullName, FileMode.Truncate, FileAccess.Write, FileShare.Read);
                        byte[] data = Encoding.UTF8.GetBytes(content);
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                        fs.Dispose();

                        Response.Write("保存成功!");
                    }
                    else if (action == "restore")
                    {
                        FileInfo bakfile = new FileInfo(backFile),
                        tmpfile = new FileInfo(backFile+".tmp");

                        string _fpath = file.FullName;

                        if (bakfile.Exists)
                        {
                            file.MoveTo(backFile+".tmp");
                            bakfile.MoveTo(_fpath);
                            tmpfile.MoveTo(backFile);

                            //global::System.IO.File.SetAttributes(_fpath + ".bak",file.Attributes & FileAttributes.Hidden);
                            File.SetAttributes(_fpath,file.Attributes & FileAttributes.Normal);
                        }
                        Response.Write("还原成功!");
                    }
                }
            }
            else
            {
                Response.Write("文件不存在,请检查!");
            }
        }

        public void CreateView_POST()
        {
            string tplname = String.Format("templates/{0}/{1}.{2}",
                base.CurrentSite.Tpl,
                Request.Form["name"],
                Request.Form["type"] == "1" ? "part.html" : "html");

            string tplPath = String.Format("{0}{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                tplname);

            if (File.Exists(tplPath))
            {
                Response.Write("文件已经存在!");
            }
            else
            {
                try
                {
                    //global::System.IO.Directory.CreateDirectory(tplPath).Create();   //创建目录
                    File.Create(tplPath).Dispose();                           //创建文件

                    Cms.Template.Register();           //重新注册模板

                    Response.Write(tplname);
                }
                catch (Exception e)
                {
                    // Response.Write(e.Message);
                    Response.Write("无权限创建文件，请设置视图目录(templates)可写权限！");
                }
            }
        }

        /// <summary>
        /// 模板设置
        /// </summary>
        public void Settings_GET()
        {
            string tpl = Request["tpl"];
            if (String.IsNullOrEmpty(tpl))
            {
                tpl = base.CurrentSite.Tpl;
            }

            var tplSetting = Cms.TemplateManager.Get(tpl);

            base.ViewData["json"] = JsonConvert.SerializeObject(new
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
                tpl_CFG_Enabled_Mobi = tplSetting.CfgEnabledMobiPage?"1":"0",
                tpl_CFG_Show_Error = tplSetting.CfgShowError ? "1" : "0",
            });
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Template_Setting));
        }

        public void Settings_POST()
        {
            string tpl = Request["tpl"];
            if (String.IsNullOrEmpty(tpl))
            {
                tpl = base.CurrentSite.Tpl;
            }
            else
            {
                if (!UserState.Administrator.Current.IsMaster)
                {
                    base.RenderError("无权执行此操作!");
                    return;
                }
            }

            TemplateSetting tplSetting = Cms.TemplateManager.Get(tpl);
            var req = base.Request;
            int outlineLength;
            int.TryParse(req["tpl_CFG_OutlineLength"], out outlineLength);
            tplSetting.CfgOutlineLength = outlineLength;
            tplSetting.CFG_AllowAmousComment = req["tpl_CFG_AllowAmousComment"] == "on";
            tplSetting.CFG_CommentEditorHtml = req["tpl_CFG_CommentEditorHtml"];
            tplSetting.CFG_ArchiveTagsFormat = req["tpl_CFG_ArchiveTagsFormat"];
            tplSetting.CFG_FriendLinkFormat = req["tpl_CFG_FriendLinkFormat"];
            int friendlinkNum;
            int.TryParse(req["tpl_CFG_FriendShowNum"], out friendlinkNum);
            tplSetting.CFG_FriendShowNum = friendlinkNum;
            tplSetting.CFG_NavigatorLinkFormat = req["tpl_CFG_NavigatorLinkFormat"];
            tplSetting.CFG_NavigatorChildFormat = req["tpl_CFG_NavigatorChildFormat"];
            tplSetting.CFG_SitemapSplit = req["tpl_CFG_SitemapSplit"];
            tplSetting.CFG_TrafficFormat = req["tpl_CFG_TrafficFormat"];
            tplSetting.CfgEnabledMobiPage = req["tpl_CFG_Enabled_Mobi"] == "1";
            tplSetting.CfgShowError = req["tpl_CFG_Show_Error"] == "1";
            tplSetting.Save();

            base.RenderSuccess("修改成功!");
        }

        /// <summary>
        /// 模板列表
        /// </summary>
        public void Templates_GET()
        {
            if (!UserState.Administrator.Current.IsMaster) return;
            string curTemplate = base.CurrentSite.Tpl;

            string tplRootPath=String.Format("{0}templates/",AppDomain.CurrentDomain.BaseDirectory);
            string[] tplList = new string[0];
            DirectoryInfo dir = new DirectoryInfo(tplRootPath);
            if (dir.Exists)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                tplList = new string[dirs.Length];
                int i = -1;
                foreach (DirectoryInfo d in dirs)
                {
                    tplList[++i] = d.Name;
                }
            }

            SettingFile sf;
            string  currentName="",
                currentThumbnail="",
                    tplConfigFile,
                   tplName,
                   tplDescrpt,
                   tplThumbnail;

            StringBuilder sb = new StringBuilder();
            foreach (string tpl in tplList)
            {

                tplName = tpl;
                tplThumbnail = null;
                tplDescrpt = null;

                tplConfigFile = String.Format("{0}{1}/tpl.conf", tplRootPath, tpl);
                if (File.Exists(tplConfigFile))
                {
                    sf = new SettingFile(tplConfigFile);
                    if (sf.Contains("name"))
                    {
                        tplName = sf["name"];
                    }

                    if (sf.Contains("thumbnail"))
                    {
                        tplThumbnail = sf["thumbnail"];
                    }
                    if (sf.Contains("descript"))
                    {
                        tplDescrpt = sf["descript"];
                    }
                }

                if (String.CompareOrdinal(tpl, curTemplate) != 0)
                {
                    sb.Append("<li><p><a href=\"javascript:;\">");
                    if (tplThumbnail != null)
                    {
                        sb.Append("<img src=\"").Append(tplThumbnail).Append("\" alt=\"点击切换模板\" class=\"shot ").Append(tpl).Append("\"/>");
                    }
                    else
                    {
                        sb.Append("<span title=\"点击切换模板\" class=\"shot ").Append(tpl).Append(" thumbnail\">无缩略图</span>");
                    }

                    sb.Append("</a></p><p><a href=\"javascript:;\" class=\"t\">")
                        .Append(tplName).Append("</a></p><p><a class=\"btn edit\" href=\"tpl:")
                       .Append(tpl).Append("\">编辑</a>&nbsp;<a class=\"btn down\" href=\"tpl:")
                       .Append(tpl).Append("\">下载</a></p>")
                       .Append("<p class=\"hidden\">").Append(tplDescrpt).Append("</p>")
                       .Append("</li>");
                }
                else
                {

                    currentName = String.IsNullOrEmpty(tplName) ? curTemplate : tplName;
                    if (tplThumbnail != null)
                    {
                       currentThumbnail="<img src=\""+tplThumbnail+"\" alt=\"点击切换模板\" class=\"shot1 "+tpl+"\"/>";
                    }
                    else
                    {
                        currentThumbnail="<span class=\"shot1 "+tpl+" thumbnail\">无缩略图</span>";
                    }
                }
            }

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Template_Manager), new
            {
                list=sb.ToString(),
                current = curTemplate,
                currentName=currentName,
                currentThumbnail=currentThumbnail
            });

        }

        /// <summary>
        /// 设置默认模板
        /// </summary>
        /// <returns></returns>
        public void TemplateAsDefault_POST()
        {
            string tpl = Request["tpl"];
            SiteDto site = base.CurrentSite;
            site.Tpl = tpl;
            ServiceCall.Instance.SiteService.SaveSite(site);
            base.RenderSuccess("");
        }

        /// <summary>
        /// 解压上传模板文件
        /// </summary>
        /// <returns></returns>
        public void UploadInstall_POST()
        {
            string tplRootPath =String.Format("{0}templates/",AppDomain.CurrentDomain.BaseDirectory);
            string tempTplPath = tplRootPath + "~.tpl";
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
            Request.Files[0].SaveAs(tempTplPath);


            int dirIndex = 0;
            int entryCount = 0;

            IArchive archive = ArchiveFactory.Open(tempTplPath);


            foreach (IArchiveEntry entry in archive.Entries)
            {
                ++entryCount;

                if (dirIndex == 0)
                {
                    dirIndex++;
                    string dirName = entry.FilePath.Split('\\', '/')[0];
                    if (global::System.IO.Directory.Exists(tplRootPath + dirName))
                    {
                        resultMessage = "模板已经存在!";
                        goto handleOver;
                    }
                }

                if (!entry.IsDirectory)
                {
                    entry.WriteToDirectory(tplRootPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }

            if (entryCount == 0)
            {
                resultMessage = "上传的模板不包含任何内容!";
            }
            else
            {

                resultMessage = "模板安装成功!";
            }


        handleOver:

            archive.Dispose();
            global::System.IO.File.Delete(tempTplPath);

            //重新注册模板
            Cms.Template.Register();

            base.Response.Write(String.Format(jsTip, resultMessage));
        }

        /// <summary>
        /// 网络安装模板
        /// </summary>
        public void NetInstall_POST()
        {
            string url = Request["url"];
            string name=Regex.Match(url,"/([^/]+)\\.zip",RegexOptions.IgnoreCase).Groups[1].Value;
            int result= Updater.InstallTemplate(name, url);
            if (result == 1)
            {      
                //重新注册模板
                Cms.Template.Register();    

                base.RenderSuccess("安装成功!");
            }
            else if (result == -1)
            {
                base.RenderError("获取模板包失败!");
            }
            else if (result == -2)
            {
                base.RenderError("模板已经安装!");
            }
        }

        /// <summary>
        /// 下载作为一个压缩文件
        /// </summary>
        public void DownloadZip_GET()
        {
            string tpl = Request["tpl"];
            byte[] bytes =  ZipHelper.Compress(String.Format("{0}templates/{1}/", Cms.PyhicPath, tpl),tpl);
            Response.BinaryWrite(bytes);
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=template_" + tpl + ".zip");
            Response.AddHeader("Content-Length", bytes.Length.ToString());
        }

        /// <summary>
        /// 备份模板
        /// </summary>
        public void BackupTemplate_GET()
        {
            string tpl = base.Request["tpl"];

            //设置目录
            DirectoryInfo dir = new DirectoryInfo(String.Format("{0}backups/templates/", Cms.PyhicPath));
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dir.FullName).Create();
            }
            else
            {
                Cms.Utility.SetDirCanWrite("backups/templates/");
            }

            ZipHelper.ZipAndSave(String.Format("{0}/templates/{1}/", Cms.PyhicPath, tpl)
                , String.Format("{0}/backups/templet/{1}_{2:yyyyMMddHHss}.zip", Cms.PyhicPath, tpl, DateTime.Now),
                tpl
                );

        }

        /// <summary>
        /// 自动升级模板语法
        /// </summary>
        public void AutoInstall_GET()
        {
            //Automation.TemplateAuto.AutoInstall(base.Request["tpl"]);
        }
    }
}

