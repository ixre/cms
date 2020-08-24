/*
 * Created by SharpDevelop.
 * UserBll: newmin
 * Date: 2013/12/14
 * Time: 10:33
 * 
 *  KindEditor ASP.NET
 *
 * 本ASP.NET程序是演示程序，建议不要直接在实际项目中使用。
 * 如果您确定直接使用本程序，使用之前请仔细确认相关安全设置。
 *
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JR.Cms.Web.Util;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core;
using JR.Stand.Core.Utils;

namespace JR.Cms.Web.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public class KindEditor
    {
        private readonly string _appPath;
        private readonly string _rootPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="rootPath"></param>
        public KindEditor(string appPath, string rootPath)
        {
            this._appPath = appPath;
            this._rootPath = rootPath;
        }

        /// <summary>
        /// 文件浏览
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task FileManagerRequest(ICompatibleHttpContext context)
        {
            context.Response.ContentType("application/json; charset=utf-8");

            //根目录路径，相对路径
            //String rootPath = $"{CmsVariables.RESOURCE_PATH}{siteId}/";
            //根目录URL，可以指定绝对路径，比如 http://www.yoursite.com/attached/
            // string appPath = Cms.Context.ApplicationPath;

            String rootUrl = $"{(this._appPath == "/" ? "" : this._appPath)}/{this._rootPath}";

            //图片扩展名
            String fileTypes = "gif,jpg,jpeg,png,bmp";

            String currentPath = "";
            String currentUrl = "";
            String currentDirPath = "";
            String moveUpDirPath = "";

            String dirPath = EnvUtil.GetBaseDirectory() + this._rootPath;
            String dirName = context.Request.Query("dir");
            if (!String.IsNullOrEmpty(dirName))
            {
                if (Array.IndexOf("image,flash,media,file".Split(','), dirName) == -1)
                {
                    return context.Response.WriteAsync("Invalid Directory name.");
                }

                dirPath += dirName + "/";
                rootUrl += dirName + "/";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath).Create();
                }
            }

            //根据path参数，设置各路径和URL
            String path = context.Request.Query("path");
            path = String.IsNullOrEmpty(path) ? "" : path;
            if (path == "")
            {
                currentPath = dirPath;
                currentUrl = rootUrl;
                currentDirPath = "";
                moveUpDirPath = "";
            }
            else
            {
                currentPath = dirPath + path;
                currentUrl = rootUrl + path;
                currentDirPath = path;
                moveUpDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
            }

            //排序形式，name or size or type
            String order = context.Request.Query("order");
            order = String.IsNullOrEmpty(order) ? "" : order.ToLower();

            //不允许使用..移动到上一级目录
            if (Regex.IsMatch(path, @"\.\."))
            {
                return context.Response.WriteAsync("Access is not allowed.");
            }

            //最后一个字符不是/
            if (path != "" && !path.EndsWith("/"))
            {
                return context.Response.WriteAsync("Parameter is not valid.");
            }

            //目录不存在或不是目录
            if (!Directory.Exists(currentPath))
            {
                return context.Response.WriteAsync("Directory does not exist.");
            }

            //遍历目录取得文件信息
            string[] dirList = Directory.GetDirectories(currentPath);
            string[] fileList = Directory.GetFiles(currentPath);

            switch (order)
            {
                case "size":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new SizeSorter());
                    break;
                case "type":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new TypeSorter());
                    break;
                case "name":
                default:
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new NameSorter());
                    break;
            }

            Hashtable result = new Hashtable();
            result["moveup_dir_path"] = moveUpDirPath;
            result["current_dir_path"] = currentDirPath;
            result["current_url"] = currentUrl;
            result["total_count"] = dirList.Length + fileList.Length;
            List<Hashtable> dirFileList = new List<Hashtable>();
            for (int i = 0; i < dirList.Length; i++)
            {
                DirectoryInfo dir = new DirectoryInfo(dirList[i]);
                Hashtable hash = new Hashtable
                {
                    ["is_dir"] = true,
                    ["has_file"] = (dir.GetFileSystemInfos().Length > 0),
                    ["filesize"] = 0,
                    ["is_photo"] = false,
                    ["filetype"] = "",
                    ["filename"] = dir.Name,
                    ["datetime"] = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                };
                dirFileList.Add(hash);
            }

            for (int i = 0; i < fileList.Length; i++)
            {
                FileInfo file = new FileInfo(fileList[i]);
                if (file.Extension.Equals("")) continue;
                Hashtable hash = new Hashtable
                {
                    ["is_dir"] = false,
                    ["has_file"] = false,
                    ["filesize"] = file.Length,
                    ["is_photo"] = (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) >= 0),
                    ["filetype"] = file.Extension.Substring(1),
                    ["filename"] = file.Name,
                    ["datetime"] = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                };
                dirFileList.Add(hash);
            }

            string files = String.Empty;
            int j = 0;
            foreach (Hashtable h in dirFileList)
            {
                files += JsonAnalyzer.ToJson(h);
                if (++j < dirFileList.Count)
                {
                    files += ",";
                }
            }

            result["file_list"] = "[" + files + "]";
            context.Response.ContentType("application/json; charset=utf-8");
            return context.Response.WriteAsync(JsonAnalyzer.ToJson(result));
        }


        /// <summary>
        /// 处理上传
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task UploadRequest(ICompatibleHttpContext context)
        {
            context.Response.ContentType("application/json; charset=utf-8");
            var path = context.Request.GetPath();
            //String aspxUrl = path.Substring(0, path.LastIndexOf("/") + 1);
            String saveUrl = $"{(this._appPath == "/" ? "" : this._appPath)}/{this._rootPath}";

            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp,webp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2,7z");

            //最大文件大小
            int maxSize = 10240000;

            ICompatiblePostedFile imgFile = context.Request.File("imgFile");
            if (imgFile == null)
            {
                return this.showError(context, "请选择文件。");
            }

            String dirPath = EnvUtil.GetBaseDirectory() + this._rootPath;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath).Create();
                //return showError(context,"上传目录不存在。");
            }

            String dirName = context.Request.Query("dir");
            if (String.IsNullOrEmpty(dirName))
            {
                dirName = "image";
            }

            if (!extTable.ContainsKey(dirName))
            {
                return this.showError(context, "目录名不正确。");
            }

            String fileName = imgFile.GetFileName();
            String fileExt = Path.GetExtension(fileName).ToLower();

            if (imgFile.GetLength() > maxSize)
            {
                return this.showError(context, "上传文件大小超过限制。");
            }

            if (String.IsNullOrEmpty(fileExt) ||
                Array.IndexOf(((String) extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                return this.showError(context, "上传文件扩展名是不允许的扩展名。\n只允许" + ((String) extTable[dirName]) + "格式。");
            }

            //创建文件夹
            dirPath += dirName + "/";
            saveUrl += dirName + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath).Create();
            }

            String ymd = DateTime.Now.ToString("yyyyMM", DateTimeFormatInfo.InvariantInfo);
            dirPath += ymd + "/";
            saveUrl += ymd + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            String originName = UploadUtils.GetUploadFileName(imgFile);
            String newFileName = originName + fileExt;
            //String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;

            // 自动将重复的名称命名
            String targetPath = dirPath + newFileName;
            int i = 0;
            while (File.Exists(targetPath))
            {
                i++;
                newFileName = $"{originName}_{i.ToString()}{fileExt}";
                targetPath = dirPath + newFileName;
            }

            SaveFile(imgFile, targetPath);

            String fileUrl = saveUrl + newFileName;

            Hashtable hash = new Hashtable();
            hash["error"] = 0;
            hash["url"] = fileUrl;
            return context.Response.WriteAsync(JsonAnalyzer.ToJson(hash));
        }

        private async void SaveFile(ICompatiblePostedFile imgFile, string targetPath)
        {
            using (FileStream fs = new FileStream(targetPath, FileMode.Create))
            {
                imgFile?.CopyToAsync(fs);
                fs.Flush();
            }
        }

        private Task showError(ICompatibleHttpContext context, string message)
        {
            Hashtable hash = new Hashtable {["error"] = 1, ["message"] = message};
            return context.Response.WriteAsync(JsonAnalyzer.ToJson(hash));
        }

        public class NameSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                FileInfo xInfo = new FileInfo(x.ToString());
                FileInfo yInfo = new FileInfo(y.ToString());

                return xInfo.FullName.CompareTo(yInfo.FullName);
            }
        }

        public class SizeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                FileInfo xInfo = new FileInfo(x.ToString());
                FileInfo yInfo = new FileInfo(y.ToString());

                return xInfo.Length.CompareTo(yInfo.Length);
            }
        }

        public class TypeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                FileInfo xInfo = new FileInfo(x.ToString());
                FileInfo yInfo = new FileInfo(y.ToString());

                return xInfo.Extension.CompareTo(yInfo.Extension);
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}