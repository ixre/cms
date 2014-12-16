using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ops.Json;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;



public class FileJsonExplor
{

	private delegate void DirectoryHandler(DirectoryInfo dir);
	private delegate void FileHandler(FileInfo file);
	private static readonly string[] sysInts = new string[] {"bin","config", "templates", "libs", "global\\.asax", "web\\.config","cms.config" };

	private static string[] readOnlyFiles;
	
	static FileJsonExplor()
	{
		readOnlyFiles=new string[]{"cms.config","web.config","install.lock","global.asax"};
	}
	
	/// <summary>
	/// 返回错误信息
	/// </summary>
	/// <param name="message"></param>
	/// <returns></returns>
	private static string ReturnError(string message)
	{
		return "{error:'"+message.Replace("'","\\'")+"'}";
	}

	public static string GetJson(string dir_abs_path)
	{
		DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + dir_abs_path);
		if (!dir.Exists)
		{
			return ReturnError(String.Format("目录:{0}不存在!",dir_abs_path));
		}
		return GetJson(dir,dir_abs_path=="/");
	}

	private static string GetJson(DirectoryInfo dir,bool isRoot)
	{
		//System.Threading.Thread.Sleep(500);
		//
		//  [
		//   pdir:{path='/templates/'},
		//     dirs:[
		//             name:'text',
		//             dirnum:0,
		//             filenum:0
		//          ],
		//     files:[
		//             {name:'1.txt',len:'123400',date:'2013-04-05',mdate:'2013-04-05',readonly:1,system:1}
		//          ]
		//   ]
		//

		StringBuilder sb = new StringBuilder();

		DirectoryHandler dh = d =>
		{
		};

		FileHandler fh = f =>
		{
		};

		//父目录
		sb.Append("{parent:'").Append(isRoot ? "/" : dir.Parent.Name).Append("',");

		//获取目录下的文件夹
		int i = 0;
		sb.Append("dirs:[");
		DirectoryInfo[] dirs = dir.GetDirectories();

		foreach (DirectoryInfo d in dirs)
		{
			if((d.Attributes & FileAttributes.Hidden)!=FileAttributes.Hidden)
			{
				sb.Append(i++==0?"":",");
				sb.Append("{name:'").Append(d.Name).Append("',dirnum:")
					.Append(d.GetDirectories().Length).Append(",filenum:")
					.Append(d.GetFiles().Length).Append(",system:0}");
			}
		}
		sb.Append("],");

		//获取目录下的文件
		i = 0;
		sb.Append("files:[");
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo f in files)
		{
			if((f.Attributes & FileAttributes.Hidden)!=FileAttributes.Hidden)
			{
				sb.Append(i++==0?"":",");
				sb.Append("{name:'").Append(f.Name).Append("',len:")
					.Append(f.Length.ToString()).Append(",date:'")
					.Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}",f.CreationTime))
					.Append("',mdate:'").Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}",f.LastWriteTime))
					.Append("',readonly:").Append(
						(Array.Find(readOnlyFiles,str=>String.Compare(str,f.Name,true)==0)!=null || (f.Attributes|FileAttributes.ReadOnly)==FileAttributes.ReadOnly?1:0).ToString())
					.Append(",system:0}");
			}
		}
		sb.Append("]}");

		string result = sb.ToString();

		if (isRoot)
		{
			string sysPat="";
			int j = 0;
			Array.ForEach(sysInts, a =>
			              {
			              	if (++j != 1)
			              	{
			              		sysPat += "|"+a ;
			              	}
			              	else
			              	{
			              		sysPat += a;
			              	}
			              });

			Regex reg = new Regex("\\{name:'(" + sysPat + ")'([^(sys)]+)system:0\\}", RegexOptions.IgnoreCase);
			result = reg.Replace(result, "{name:'$1'$2system:1}");
		}
		return result;
	}

	private static void EachExpolor(DirectoryInfo dir,DirectoryHandler dh)
	{
		DirectoryInfo[] dirs = dir.GetDirectories();
		
	}

	/// <summary>
	/// 删除文件或目录
	/// </summary>
	/// <param name="dir"></param>
	/// <param name="file"></param>
	/// <param name="isdir"></param>
	/// <returns></returns>
	internal static bool Delete(string dir, string file, bool isdir)
	{
		dir =Regex.Replace(dir,"^(\\/)*([\\S\\s]+)(\\/)$", "$2");

		string path = AppDomain.CurrentDomain.BaseDirectory + dir+"/"+ file;

		if (isdir)
		{
			//如果为系统文件,则返回false
			foreach (string s in sysInts)
			{
				if (String.Compare(s,file) == 0)
				{
					return false;
				}
			}
			Directory.Delete(path + "/");  //目录下有文件,不能删除
		}
		else
		{
			//如果为系统文件,则返回false
			foreach (string s in sysInts)
			{
				if (string.Compare(s, file.Replace("\\","")) == 0)
				{
					return false;
				}
			}

			File.Delete(path);
		}

		return true;

	}

	/// <summary>
	/// 重命名
	/// </summary>
	/// <param name="dir"></param>
	/// <param name="file"></param>
	/// <param name="newfile"></param>
	/// <param name="isdir"></param>
	/// <returns></returns>
	internal static bool Rename(string dir, string file, string newfile, bool isdir)
	{

		dir = Regex.Replace(dir, "^(\\/)*([\\S\\s]+)(\\/)$", "$2");

		string path = AppDomain.CurrentDomain.BaseDirectory + dir + "/" + file;
		string newPath = AppDomain.CurrentDomain.BaseDirectory + dir + "/" + newfile;

		if (isdir)
		{
			Directory.Move(path + "/", newPath + "/");
		}
		else
		{
			File.Move(path, newPath);
		}
		return true;
	}

	/// <summary>
	/// 创建文件
	/// </summary>
	/// <param name="dir"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	internal static string Create(string dir, string path)
	{
		string file = AppDomain.CurrentDomain.BaseDirectory + dir + path;
		if (File.Exists(file))
		{
			return ReturnError("文件已存在!");
		}
		else
		{
			File.Create(file).Dispose();
			return "{}";
		}
	}
}