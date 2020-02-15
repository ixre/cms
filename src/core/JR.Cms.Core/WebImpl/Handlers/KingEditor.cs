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
using System.Web;
using JR.Cms.Conf;
using JR.Cms.WebImpl.Util;
using JR.Cms.WebImpl.WebManager;
using JR.DevFw.Utils;

namespace JR.Cms.WebImpl.Handlers
{

    public class EditorUploadHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
	{
		private HttpContext context;

		public void ProcessRequest(HttpContext context)
		{
			String aspxUrl = context.Request.Path.Substring(0, context.Request.Path.LastIndexOf("/") + 1);
			
			string siteId=Logic.CurrentSite.SiteId.ToString();
			//文件保存目录路径
			String savePath =String.Format("/{0}{1}/",CmsVariables.RESOURCE_PATH, siteId);

			//文件保存目录URL
			string appPath=Cms.Context.ApplicationPath;
			String saveUrl = String.Format("{0}/{1}{2}/",appPath=="/"?"":appPath, CmsVariables.RESOURCE_PATH,siteId);

			//定义允许上传的文件扩展名
			Hashtable extTable = new Hashtable();
			extTable.Add("image", "gif,jpg,jpeg,png,bmp");
			extTable.Add("flash", "swf,flv");
			extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
			extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2,7z");

			//最大文件大小
            int maxSize = 10240000;
			this.context = context;

			HttpPostedFile imgFile = context.Request.Files["imgFile"];
			if (imgFile == null)
			{
				showError("请选择文件。");
			}

			String dirPath = AppDomain.CurrentDomain.BaseDirectory+savePath;

		checkDir:
			bool isCreate = false;
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath).Create();
				if (!isCreate)
				{
					isCreate = true;
					goto checkDir;
				}
				
				showError("上传目录不存在。");
			}

			String dirName = context.Request.QueryString["dir"];
			if (String.IsNullOrEmpty(dirName)) {
				dirName = "image";
			}
			if (!extTable.ContainsKey(dirName)) {
				showError("目录名不正确。");
			}

			String fileName = imgFile.FileName;
			String fileExt = Path.GetExtension(fileName).ToLower();

			if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
			{
				showError("上传文件大小超过限制。");
			}

			if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
			{
				showError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。");
			}

			//创建文件夹
			dirPath += dirName + "/";
			saveUrl += dirName + "/";
			if (!Directory.Exists(dirPath)) {
				Directory.CreateDirectory(dirPath).Create();
			}
			String ymd = DateTime.Now.ToString("yyyyMM", DateTimeFormatInfo.InvariantInfo);
			dirPath += ymd + "/";
			saveUrl += ymd + "/";
			if (!Directory.Exists(dirPath)) {
				Directory.CreateDirectory(dirPath);
			}
            String originName = UploadUtils.GetUploadFileName(context.Request);
            String newFileName = originName + fileExt;
            //String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;

            // 自动将重复的名称命名
            String targetPath = dirPath + newFileName;
            int i = 0;
            while (File.Exists(targetPath))
            {
                i++;
                newFileName =  String.Format("{0}_{1}{2}",originName, i.ToString(), fileExt);
                targetPath = dirPath + newFileName;
            }
            imgFile.SaveAs(targetPath);

			String fileUrl = saveUrl + newFileName;

			Hashtable hash = new Hashtable();
			hash["error"] = 0;
			hash["url"] = fileUrl;
			context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");


			context.Response.Write(JsonAnalyzer.ToJson(hash));
			context.Response.End();
		}

		private void showError(string message)
		{
			Hashtable hash = new Hashtable();
			hash["error"] = 1;
			hash["message"] = message;
			context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
			context.Response.Write(JsonAnalyzer.ToJson(hash));
			context.Response.End();
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}
	}
	
	public class EditorFileManager : IHttpHandler,System.Web.SessionState.IRequiresSessionState
	{
		public void ProcessRequest(HttpContext context)
		{
			String aspxUrl = context.Request.Path.Substring(0, context.Request.Path.LastIndexOf("/") + 1);

			string siteId = Logic.CurrentSite.SiteId.ToString();
			
			//根目录路径，相对路径
			String rootPath = String.Format("{0}{1}/", CmsVariables.RESOURCE_PATH,siteId);
			//根目录URL，可以指定绝对路径，比如 http://www.yoursite.com/attached/
			string appPath=Cms.Context.ApplicationPath;
			String rootUrl =String.Format("{0}/{1}{2}/",appPath=="/"?"":appPath,
                CmsVariables.RESOURCE_PATH, siteId);
			
			//图片扩展名
			String fileTypes = "gif,jpg,jpeg,png,bmp";

			String currentPath = "";
			String currentUrl = "";
			String currentDirPath = "";
			String moveupDirPath = "";

			String dirPath = AppDomain.CurrentDomain.BaseDirectory+rootPath;
			String dirName = context.Request.QueryString["dir"];
			if (!String.IsNullOrEmpty(dirName)) {
				if (Array.IndexOf("image,flash,media,file".Split(','), dirName) == -1) {
					context.Response.Write("Invalid Directory name.");
					context.Response.End();
				}
				dirPath += dirName + "/";
				rootUrl += dirName + "/";
				if (!Directory.Exists(dirPath)) {
					Directory.CreateDirectory(dirPath).Create();
				}
			}

			//根据path参数，设置各路径和URL
			String path = context.Request.QueryString["path"];
			path = String.IsNullOrEmpty(path) ? "" : path;
			if (path == "")
			{
				currentPath = dirPath;
				currentUrl = rootUrl;
				currentDirPath = "";
				moveupDirPath = "";
			}
			else
			{
				currentPath = dirPath + path;
				currentUrl = rootUrl + path;
				currentDirPath = path;
				moveupDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
			}

			//排序形式，name or size or type
			String order = context.Request.QueryString["order"];
			order = String.IsNullOrEmpty(order) ? "" : order.ToLower();

			//不允许使用..移动到上一级目录
			if (Regex.IsMatch(path, @"\.\."))
			{
				context.Response.Write("Access is not allowed.");
				context.Response.End();
			}
			//最后一个字符不是/
			if (path != "" && !path.EndsWith("/"))
			{
				context.Response.Write("Parameter is not valid.");
				context.Response.End();
			}
			//目录不存在或不是目录
			if (!Directory.Exists(currentPath))
			{
				context.Response.Write("Directory does not exist.");
				context.Response.End();
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
			result["moveup_dir_path"] = moveupDirPath;
			result["current_dir_path"] = currentDirPath;
			result["current_url"] = currentUrl;
			result["total_count"] = dirList.Length + fileList.Length;
			List<Hashtable> dirFileList = new List<Hashtable>();
			for (int i = 0; i < dirList.Length; i++)
			{
				DirectoryInfo dir = new DirectoryInfo(dirList[i]);
				Hashtable hash = new Hashtable();
				hash["is_dir"] = true;
				hash["has_file"] = (dir.GetFileSystemInfos().Length > 0);
				hash["filesize"] = 0;
				hash["is_photo"] = false;
				hash["filetype"] = "";
				hash["filename"] = dir.Name;
				hash["datetime"] = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
				dirFileList.Add(hash);
			}
			for (int i = 0; i < fileList.Length; i++)
			{
				FileInfo file = new FileInfo(fileList[i]);
                if (file.Extension.Equals(""))continue;
				Hashtable hash = new Hashtable();
				hash["is_dir"] = false;
				hash["has_file"] = false;
				hash["filesize"] = file.Length;
				hash["is_photo"] = (Array.IndexOf(fileTypes.Split(','), file.Extension.Substring(1).ToLower()) >= 0);
				hash["filetype"] = file.Extension.Substring(1);
				hash["filename"] = file.Name;
				hash["datetime"] = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
				dirFileList.Add(hash);
			}

			string files = String.Empty;
			int j=0;
			foreach (Hashtable h in dirFileList)
			{
				files += JsonAnalyzer.ToJson(h);
				if (++j< dirFileList.Count)
				{
					files += ",";
				}
			}
			result["file_list"] = "["+files+"]";
			context.Response.AddHeader("Content-Type", "application/json; charset=UTF-8");
			context.Response.Write(JsonAnalyzer.ToJson(result));
			context.Response.End();
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
			get
			{
				return true;
			}
		}
	}

}




