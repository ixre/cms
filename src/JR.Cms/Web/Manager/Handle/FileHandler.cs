//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : File.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 18:10:55
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.IO;
using System.Text;
using JR.Cms.Conf;
using JR.Stand.Core;
using JR.Stand.Core.Utils;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHandler : BasePage
    {
        /// <summary>
        /// 游客头像
        /// </summary>
        public void GuestAvatar()
        {
            //MemoryStream ms = new MemoryStream();
            //ManagerResouces.loading_process.Save(ms, ImageFormat.Gif);
            //string x = Convert.ToBase64String(ms.ToArray());
            //ms.Dispose();
            //throw new Exception(x);

            //图片数据数据
            const string base64Str =
                "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";

            Response.ContentType("Image/Jpeg");
            Response.AppendHeader("Content-Disposition",
                "attachment;filename=guest_avatar.jpg");
            
            var bytes = Convert.FromBase64String(base64Str);
            Response.Write(bytes, 0, bytes.Length);
        }

        public void Loading()
        {
            //MemoryStream ms = new MemoryStream();
            //ManagerResouces.ajax_loader.Save(ms, ImageFormat.Gif);
            //string x = Convert.ToBase64String(ms.ToArray());
            //ms.Dispose();
            //throw new Exception(x);

            //图片数据
            const string loadingBase64String =
                "R0lGODlhEAAQAPIAAP///wBg/8LY/kKJ/gBg/2Kc/oKw/pK6/iH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6ImKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA== ";
         
            Response.ContentType("Image/Gif");
            Response.AppendHeader("Content-Disposition",
                "attachment;filename=loading.gif");
            var bytes = Convert.FromBase64String(loadingBase64String);
            Response.Write(bytes, 0, bytes.Length);
        }

        public void FileManager()
        {
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.File_Manager), null);
        }

        /// <summary>
        /// 文件管理器
        /// </summary>
        public void FileManager_POST()
        {
            string dir = Request.GetParameter("dir");
            string action = Request.GetParameter("act");
            string file = Request.GetParameter("file");
            bool isDir = Request.GetParameter("is_dir") == "1" || Request.GetParameter("is_dir") == "true";
            if (!dir.StartsWith(".")) dir = "./" + dir;
            if (action == "list")
            { 
                Response.WriteAsync(FileJsonExplore.GetJson(dir));
                return;
            }

            if (action == "del")
            {
                var result = FileJsonExplore.Delete(dir, file, isDir) ? "1" : "0";
                Response.WriteAsync(result);
                return;
            }

            if (action == "rename")
            {
                var result = FileJsonExplore.Rename(dir, file, Request.Form("newfile"), isDir)
                    ? "1"
                    : "0";
                Response.WriteAsync(result);
                return;
            }

            if (action == "create")
            {
                var result = FileJsonExplore.Create(dir, file, isDir);
                Response.WriteAsync(result);
                return;
            }

            if (action == "upload")
            {
                var result = FileJsonExplore.Upload(dir, Request.FileIndex(0));
                Response.WriteAsync(result);
                return;
            }

            Response.WriteAsync("");
        }

        /// <summary>
        /// 编辑模板
        /// </summary>
        public void Edit()
        {
            var sb = new StringBuilder();

            var cssDir = new DirectoryInfo($"{EnvUtil.GetBaseDirectory()}/style/");
            var cfgDir = new DirectoryInfo($"{EnvUtil.GetBaseDirectory()}/config/");
            //sb.Append("<optgroup label=\"样式表\">");
            //EachClass.EachFiles(cssDir, sb, "style",".css", true);
            sb.Append("</optgroup><optgroup label=\"设置文件\">");
            EachClass.EachFiles(cfgDir, sb, "config", ".xml ; .ini ; .conf", true);
            sb.Append("</optgroup>");

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.File_SelectEdit), new
            {
                files = sb.ToString(),
                systpl = CurrentSite.Tpl
            });
        }

        /// <summary>
        /// 编辑文件
        /// </summary>
        public string EditFile()
        {
            string path = Request.Query("path");
            string bakInfo;

            if (path.ToLower().IndexOf("config/cms.conf", StringComparison.Ordinal) != -1)
                throw new ArgumentException();

            var mode = "html";
            var dependJs = "/public/assets/code_editor/mode/htmlmixed/htmlmixed.js";

            var file = new FileInfo(EnvUtil.GetBaseDirectory() + "/" + path);
            var bakFile = new FileInfo(Cms.PhysicPath+Helper.GetBackupFilePath(path));

            switch (file.Extension.ToLower())
            {
                case ".css":
                    dependJs = "/public/assets/code_editor/mode/css/css.js";
                    mode = "css";
                    break;
                case ".conf":
                case ".config":
                case ".xml":
                    dependJs = "/public/assets/code_editor/mode/xml/xml.js";
                    mode = "xml";
                    break;
            }

            if (!file.Exists)
            {
                return "文件不存在!";
            }
            else
            {
                if (bakFile.Exists)
                    bakInfo = string.Format(@"上次修改时间日期：{0:yyyy-MM-dd HH:mm:ss}&nbsp;
                                <a style=""margin-right:20px"" href=""javascript:;"" onclick=""process('restore')"">还原</a>",
                        bakFile.LastWriteTime);
                else
                    bakInfo = "";
            }

            var sr = new StreamReader(file.FullName);
            var content = sr.ReadToEnd();
            sr.Dispose();
            content = content.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

            ViewData["bakInfo"] = bakInfo;
            ViewData["file"] = path;
            ViewData["mode"] = mode;
            ViewData["dependJs"] = dependJs;
            ViewData["path"] = path;
            ViewData["raw_content"] = content;

            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.File_Edit));
        }

        public void EditFile_POST()
        {
            //修改系统文件
            // if (Request.Query("path").IndexOf("templates/") == -1 && Request.Query("pwd") != "$Newmin")
            // {
            //     Response.Write("不允许修改!"); return;
            // }


            string action = Request.Form("action");
            string path = Request.Form("path");
            string content = Request.Form("content");


            if (path.ToLower().IndexOf("config/cms.conf", StringComparison.Ordinal) != -1)
            {
                throw new ArgumentException();
            }

            var file = new FileInfo(EnvUtil.GetBaseDirectory() + path);


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
                        FileInfo bakFile = new FileInfo(backFile),
                            tmpFile = new FileInfo(backFile + ".tmp");

                        var filePath = file.FullName;

                        if (bakFile.Exists)
                        {
                            file.MoveTo(backFile + ".tmp");
                            bakFile.MoveTo(filePath);
                            tmpFile.MoveTo(backFile);
                            File.SetAttributes(filePath, file.Attributes & FileAttributes.Normal);
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
    }
}