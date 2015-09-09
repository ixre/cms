using System;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using J6.Cms.Conf;
using J6.Cms.Infrastructure;
using SharpCompress.Archive;
using SharpCompress.Common;

namespace J6.Cms
{
    /// <summary>
    /// 更新文件类型
    /// </summary>
    public enum UpgradeFileType{

        /// <summary>
        /// 普通文件
        /// </summary>
        Normal,

        /// <summary>
        /// 库文件
        /// </summary>
        Lib,

        /// <summary>
        /// 压缩文件
        /// </summary>
        Zip
    }

    /// <summary>
    /// 在线更新
    /// </summary>
    public class Updater
    {

        public static string UpgadeDir = "/tmp/update";
        /// <summary>
        /// 升级时发生
        /// </summary>
        public static event CmsConfigureHandler OnUpgrade;

        static Updater()
        {
            Updater.OnUpgrade += () =>
            {
                UpgradePercent = 0.01F;

                if (UpgradePercent < 0.1F) UpgradePercent = 0.1F;

                //升级默认的模版
                InstallTemplate("default", "tpl_default.zip");
                if (UpgradePercent < 0.12F) UpgradePercent = 0.12F;


                string[] verData;
                int result = Updater.CheckUpgrade(out verData);
                if (result == 1)
                {
                    //更新压缩包文件
                    UpgradeFile(verData[1], UpgradeFileType.Zip, "/", false);
                    if (UpgradePercent < 0.3F) UpgradePercent = 0.3F;

                    //最后更新dll
                    //UpgradeFile("Cms.Cms.dll", UpgradeFileType.Lib, "bin/", false);
                    UpgradeFile("bin.zip", UpgradeFileType.Zip,UpgadeDir, false);

                    //v2.1 版本切换至于sponet.dll
                }

                if (UpgradePercent < 0.97F) UpgradePercent = 0.97F;
            };
        }


        /// <summary>
        /// 获取新版本更新日志
        /// </summary>
        /// <returns></returns>
        public static int CheckUpgrade(out string[] data)
        {
            data = new string[3];
            if (!Cms.OfficialEnvironment)
            {
                return -5;
            }
            string remoteVersionDescript = Settings.SERVER_UPGRADE + "upgrade.xml";
            WebRequest   wr = WebRequest.Create(remoteVersionDescript);
            
            HttpWebResponse rsp;
            try
            {
            	rsp = wr.GetResponse() as HttpWebResponse;
            }
            catch{
            	return -3;
            }

            Stream rspStream;
            if (rsp == null || (rspStream = rsp.GetResponseStream()) == null)
            {
                return -3;
            }

            switch (rsp.StatusCode)
            {
                //更新服务器发生内部错误
                case HttpStatusCode.InternalServerError:
                    data = null;
                    return -4;

                //message = "未发现更新版本!";
                default:
                case HttpStatusCode.NotFound:
                    data = null;
                    return -2;

                //有新版本
                case HttpStatusCode.OK: break;// return 1;
            }
            byte[] xmlData = new byte[rsp.ContentLength];
            rspStream.Read(xmlData, 0, xmlData.Length);

            XmlDocument xd = new XmlDocument();
            string result = Encoding.UTF8.GetString(xmlData);
            result = Regex.Match(result, "<upgrade[^>]+>[\\s\\S]+</upgrade>").Value;

            xd.LoadXml(result);
            XmlNode xn = xd.SelectSingleNode("/upgrade");

            data[0] = xn.Attributes["version"].Value;
            data[1] = xn.Attributes["patch"].Value;
            if (!data[1].ToUpper().StartsWith("HTTP://"))
            {
                data[1] = Settings.SERVER_UPGRADE + data[1];
            }
            data[2] = xn.InnerText;

            //版本一致
            if (int.Parse(Cms.Version.Replace(".",""))>=int.Parse(data[0].Replace(".","")))
            {
                return -1;
            }

            return 1;
        }


        /// <summary>
        /// 升级更新
        /// </summary>
        private static int ApplyUpgrade_Core()
        {
            DirectoryInfo libDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "bin\\");

            //初始化设置权限
            // message = "bin目录无法写入更新文件,请修改权限!";  
            //return -1;
            if ((libDir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                libDir.Attributes = libDir.Attributes & ~FileAttributes.ReadOnly;

            }
            try
            {
                //如果升级成功，执行操作
                if (OnUpgrade != null)
                {
                    OnUpgrade();
                }
            }
            catch
            {
            }

            return 1;
        }


        /// <summary>
        /// 升级更新
        /// </summary>
        [Obsolete]
        private static int ApplyUpgrade()
        {

            string[] verData;

            if (CheckUpgrade(out verData) != 1)
            {
                return -2;
            }

            string remoteLib = verData[1];

            DirectoryInfo libDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "bin\\");
            FileInfo libFile = new FileInfo(libDir.FullName + "Cms.Cms.dll");
            FileInfo tempLibFile = new FileInfo(libDir.FullName + "temp.lib");


            //初始化设置权限

            // message = "bin目录无法写入更新文件,请修改权限!";  
            //return -1;
            if ((libDir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                libDir.Attributes = libDir.Attributes & ~FileAttributes.ReadOnly;

            }

            if ((libFile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                libFile.Attributes = libFile.Attributes & ~FileAttributes.ReadOnly;
            }

            //创建临时文件
            if (!tempLibFile.Exists)
            {
                tempLibFile.Create().Dispose();
            }

            const int buffer = 32768;  //100k
            byte[] data = new byte[buffer];
            int cread;
            int cTotal;

            using (FileStream fs = tempLibFile.OpenWrite())
            {
                int fileLength = (int)fs.Length;
                HttpWebRequest wr = WebRequest.Create(remoteLib) as HttpWebRequest;
                if (fileLength != 0)
                {
                    wr.AddRange(fileLength);
                }
                WebResponse rsp = wr.GetResponse();
                Stream st = rsp.GetResponseStream();

                cTotal = (int)rsp.ContentLength;

                while ((cread = st.Read(data, 0, buffer)) != 0)
                {
                    //wr.AddRange(upgrade_process_length,upgrade_process_length+buffer-1);
                    fs.Write(data, 0, cread);
                }
            }

            if (OnUpgrade != null)
            {
                OnUpgrade();
            }

            return 1;
        }


        /// <summary>
        /// 更新
        /// </summary>
        public static void StartUpgrade()
        {
            upgradePercent = 0f;

            if (OnUpgrade != null)
            {
                new Thread(() =>
                {
                    OnUpgrade();

                    //完成100%
                    UpgradePercent = 1F;
                    Thread.CurrentThread.Abort();

                }).Start();
            }
        }


        /// <summary>
        /// 更新文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <param name="upgradePath"></param>
        /// <param name="resume">是否断点续传</param>
        /// <returns></returns>
        public static int UpgradeFile(string fileName, UpgradeFileType type, string upgradePath, bool resume)
        {

            //检查目录是否存在及权限
            DirectoryInfo dir = new DirectoryInfo(String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, upgradePath));
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dir.FullName).Create();
            }
            else
            {
                //设置权限
                if ((dir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                }
            }

            //断点续传
            byte[] bytes = DownloadFile(fileName, null);
            if (bytes == null)
            {
                return -1;
            }

            //压缩文件
            if (type == UpgradeFileType.Zip)
            {
                //IArchive archive = ArchiveFactory.Open(file.FullName);
                MemoryStream ms = new MemoryStream(bytes);
                IArchive archive = ArchiveFactory.Open(ms);

                foreach (IArchiveEntry entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(dir.FullName, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
                archive.Dispose();
                ms.Dispose();
            }
            else
            {
                string dirName = dir.FullName;
                if (type == UpgradeFileType.Lib)
                {
                   // fileName = fileName + ".lib";
                    dirName = Cms.PyhicPath;
                }

                //检查文件是否存在和权限
                FileInfo file = new FileInfo(String.Format("{0}{1}", dirName, fileName));

                if (file.Exists)
                {
                    if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        file.Attributes = file.Attributes & ~FileAttributes.ReadOnly;
                    }
                }
                else
                {
                    file.Create().Dispose();
                }

                //输出到文件
                FileStream fs = file.OpenWrite();
                //  fs.Write(ms.GetBuffer(), 0, (int)ms.Length);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Dispose();
            }

            return 1;
        }

        /// <summary>
        /// 下载文件并存为内存流产
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        private static byte[] DownloadFile(string fileName, byte[] fileBytes)
        {
            const int buffer = 32768;  //32k
            byte[] data = new byte[buffer];
            int cread;
            int cTotal;

            MemoryStream ms = fileBytes == null || fileBytes.Length == 0
                ? new MemoryStream()
                : new MemoryStream(fileBytes);

            string remoteAddr = fileName.IndexOf("http://", StringComparison.Ordinal) == 0 ? fileName : string.Format("{0}/{1}", Settings.SERVER_UPGRADE, fileName);

            int fileLength = (int)ms.Length;

            HttpWebRequest wr = WebRequest.Create(remoteAddr) as HttpWebRequest;
            if (fileLength != 0)
            {
                wr.AddRange(fileLength);
            }

            try
            {
                WebResponse rsp = wr.GetResponse();

                Stream st = rsp.GetResponseStream();

                //cTotal = (int)rsp.ContentLength;

                while (st != null && (cread = st.Read(data, 0, buffer)) != 0)
                {
                    ms.Write(data, 0, cread);
                }

                byte[] streamArray = ms.ToArray();
                ms.Dispose();

                return streamArray;

            }
            catch
            {
                // ignored
            }
            return null;
        }

        /// <summary>
        /// 升级指定的模板包(-1:获取包失败,-2:已经安装,1:安装成功)
        /// </summary>
        public static int InstallTemplate(string tplName, string url)
        {
            string tplRootPath = String.Format("{0}templates/", AppDomain.CurrentDomain.BaseDirectory);
            MemoryStream ms;
            IArchive archive;
            DirectoryInfo dir;
            byte[] zipData = DownloadFile(url, null);

            if (zipData == null){
                return -1;
            }

            dir = new DirectoryInfo(tplRootPath); // + tplName);  模板包括模板文件名

            //模板不存在,则解压
            if (!dir.Exists)
            {
                ms = new MemoryStream(zipData);

                archive = ArchiveFactory.Open(ms);

                foreach (IArchiveEntry entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(dir.FullName, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }

                archive.Dispose();
                ms.Dispose();
            }
            else
            {
                return -2;
            }

            return 1;
        }

        private static float upgradePercent = 0f;

        /// <summary>
        /// 更新进度
        /// </summary>
        public static float UpgradePercent
        {
            get
            {
                return upgradePercent;
             
            }
            set
            {
                upgradePercent = value;

            }
        }

        /// <summary>
        /// 应用更新核心库
        /// </summary>
        public static void ApplyCoreLib()
        {
            if (upgradePercent == 1F)
            {
                //线程沉睡并更新dll
                FileInfo[] files = new DirectoryInfo(Cms.PyhicPath+UpgadeDir).GetFiles();
                string toFile;

                foreach (FileInfo file in files)
                {
                    FileInfo to = new FileInfo(String.Format("{0}bin/{1}", Cms.PyhicPath, file.Name));
                    try
                    {
                        to.Delete();
                        file.MoveTo(to.FullName);
                    }
                    catch
                    {
                        file.Delete();
                    }
                }
            }
        }
    }
}
