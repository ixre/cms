using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core;

namespace JR.Cms.Web.Manager
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileJsonExplore
    {
        private delegate void DirectoryHandler(DirectoryInfo dir);

        private delegate void FileHandler(FileInfo file);

        private static readonly string[] SysIntArray =
        {
            "runtimes", "install", "config", "public", "templates", "libs", "global\\.asax", "web\\.config", "cms.conf"
        };

        private static readonly string[] ReadOnlyFiles;

        static FileJsonExplore()
        {
            ReadOnlyFiles = new[] {"cms.conf", "web.config", "install.lock", "global.asax"};
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string ReturnError(string message)
        {
            return "{\"error\":\"" + message.Replace("'", "\\'") + "\"}";
        }

        public static string GetJson(string dir_abs_path)
        {
            var dir = new DirectoryInfo(Path.Combine(EnvUtil.GetBaseDirectory(), dir_abs_path));
            if (!dir.Exists) return ReturnError($"目录:{dir_abs_path}不存在!");
            return GetJson(dir, dir_abs_path == "/");
        }

        private static string GetJson(DirectoryInfo dir, bool isRoot)
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

            var sb = new StringBuilder();
            //父目录
            sb.Append("{\"parent\":\"").Append(isRoot ? "/" : dir.Parent.Name).Append("\",");

            //获取目录下的文件夹
            var i = 0;
            sb.Append("\"dirs\":[");
            var dirs = dir.GetDirectories();

            foreach (var d in dirs)
                if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    sb.Append(i++ == 0 ? "" : ",");
                    sb.Append("{\"name\":\"").Append(d.Name).Append("\",\"dirnum\":")
                        .Append(d.GetDirectories().Length).Append(",\"filenum\":")
                        .Append(d.GetFiles().Length).Append(",\"system\":0}");
                }

            sb.Append("],");

            //获取目录下的文件
            i = 0;
            sb.Append("\"files\":[");
            var files = dir.GetFiles();
            foreach (var f in files)
                if ((f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    sb.Append(i++ == 0 ? "" : ",");
                    sb.Append("{\"name\":\"").Append(f.Name).Append("\",\"len\":")
                        .Append(f.Length.ToString()).Append(",\"date\":\"")
                        .Append(string.Format("{0:yyyy-MM-dd HH:mm:ss}", f.CreationTime))
                        .Append("\",\"mdate\":\"").Append($"{f.LastWriteTime:yyyy-MM-dd HH:mm:ss}")
                        .Append("\",\"readonly\":").Append(
                            (Array.Find(ReadOnlyFiles,
                                 str => string.Compare(str, f.Name, StringComparison.OrdinalIgnoreCase) == 0) != null ||
                             (f.Attributes | FileAttributes.ReadOnly) == FileAttributes.ReadOnly
                                ? 1
                                : 0).ToString())
                        .Append(",\"system\":0}");
                }

            sb.Append("]}");

            var result = sb.ToString();

            if (isRoot)
            {
                var sysPat = "";
                var j = 0;
                Array.ForEach(SysIntArray, a =>
                {
                    if (++j != 1)
                        sysPat += "|" + a;
                    else
                        sysPat += a;
                });

                var reg = new Regex("\\{\"name\":\"(" + sysPat + ")\"([^(sys)]+)system\":0\\}",
                    RegexOptions.IgnoreCase);
                result = reg.Replace(result, "{\"name\":\"$1\"$2system\":1}");
            }

            return result;
        }


        /// <summary>
        /// 删除文件或目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="file"></param>
        /// <param name="isDir"></param>
        /// <returns></returns>
        internal static bool Delete(string dir, string file, bool isDir)
        {
            dir = Regex.Replace(dir, "^(\\/)*([\\S\\s]+)(\\/)$", "$2");
            var path = EnvUtil.GetBaseDirectory() + dir + "/" + file;
            if (isDir)
            {
                //如果为系统文件,则返回false
                foreach (var s in SysIntArray)
                    if (string.CompareOrdinal(s, file) == 0)
                        return false;
                Directory.Delete(path + "/"); //目录下有文件,不能删除
                CheckReloadTemplate(path, true);
            }
            else
            {
                //如果为系统文件,则返回false
                foreach (var s in SysIntArray)
                    if (string.CompareOrdinal(s, file.Replace("\\", "")) == 0)
                        return false;

                File.Delete(path);
                CheckReloadTemplate(path, false);
            }

            return true;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="file"></param>
        /// <param name="newFile"></param>
        /// <param name="isDir"></param>
        /// <returns></returns>
        internal static bool Rename(string dir, string file, string newFile, bool isDir)
        {
            dir = Regex.Replace(dir, "^(\\/)*([\\S\\s]+)(\\/)$", "$2");

            var path = Path.Combine(EnvUtil.GetBaseDirectory(), dir, file);
            var newPath = Path.Combine(EnvUtil.GetBaseDirectory(), dir, newFile);

            if (isDir)
            {
                Directory.Move(path + "/", newPath + "/");
                CheckReloadTemplate(path, true);
            }
            else
            {
                File.Move(path, newPath);
                CheckReloadTemplate(path, false);
            }

            return true;
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="path"></param>
        /// <param name="isDir"></param>
        /// <returns></returns>
        internal static string Create(string dir, string path, bool isDir)
        {
            var filePath = Cms.PhysicPath + dir + path;
            if (isDir) return CreateDir(filePath);
            return CreateFile(filePath);
        }

        private static string CreateDir(string filePath)
        {
            if (File.Exists(filePath))
            {
                return ReturnError("已存在相同名称的文件");
            }

            if (Directory.Exists(filePath))
            {
                return ReturnError("目录已存在");
            }
            else
            {
                Directory.CreateDirectory(filePath).Create();
                return "{}";
            }
        }

        private static string CreateFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return ReturnError("文件已存在!");
            }

            File.Create(filePath).Dispose();
            CheckReloadTemplate(filePath, false);
            return "{}";
        }

        /// <summary>
        /// 如果新增了模板文件,则重新加载模板
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="b"></param>
        private static void CheckReloadTemplate(string filePath, bool isDir)
        {
            if (filePath.IndexOf("/templates/") == -1) return;
            if (isDir || filePath.EndsWith(".html") || filePath.EndsWith(".part.html"))
            {
                Cms.Template.Reload();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Upload(string dir, ICompatiblePostedFile file)
        {
            var dirPath = Cms.PhysicPath + dir;
            var fileName = file.GetFileName();

            var filePath = dirPath + file.GetFileName();
            if (File.Exists(filePath)) return "{\"error\":\"文件已经存在\"}";
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(fs);
                fs.Flush();
            }

            return "{\"url\":\"" + dir + fileName + "\"}";
        }
    }
}