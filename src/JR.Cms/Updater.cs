using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using JR.Cms.Conf;
using JR.Cms.Infrastructure;
using JR.Stand.Core;
using SharpCompress.Archive;
using SharpCompress.Common;

namespace JR.Cms
{
    /// <summary>
    /// 更新文件类型
    /// </summary>
    public enum UpgradeFileType
    {

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

        public static string UpgadeDir = "/tmp/";
        /// <summary>
        /// 升级时发生
        /// </summary>
        public static event CmsConfigureHandler OnUpgrade;

        /// <summary>
        /// 版本信息
        /// </summary>
        public class VersionInfo
        {
            /// <summary>
            /// 获取版本代码
            /// </summary>
            public int FetchCode { get; set; }
            /// <summary>
            /// 获取版本消息
            /// </summary>
            public string FetchMsg { get; set; }
            /// <summary>
            /// 产品版本号
            /// </summary>
            public string Version { get; set; }
            /// <summary>
            /// 生成版本号
            /// </summary>
            public string Build { get; set; }
            /// <summary>
            /// 发布描述
            /// </summary>
            public string ChangeLog { get; set; }
            /// <summary>
            /// 更新包地址
            /// </summary>
            public string PatchPackUrl { get; internal set; }
        }

        static Updater()
        {
            Updater.OnUpgrade += () =>
            {
                UpgradePercent = 0.01F;

                if (UpgradePercent < 0.1F) UpgradePercent = 0.1F;
                //升级默认的模版
                InstallTemplate("examples", "tpl_default.zip");
                if (UpgradePercent < 0.12F) UpgradePercent = 0.12F;
                VersionInfo result = Updater.CheckUpgrade();
                if (result.FetchCode == 1)
                {
                    //更新压缩包文件
                    Console.WriteLine("[ CMS][ Update]: extras update.zip ..");
                    UpgradeFile(result.PatchPackUrl, UpgradeFileType.Zip, "/", false);
                    if (UpgradePercent < 0.3F) UpgradePercent = 0.3F;

                    //最后更新dll
                    Console.WriteLine("[ CMS][ Update]: extras boot.zip ..");
                    UpgradeFile("boot.zip", UpgradeFileType.Zip, UpgadeDir, false);
                    Console.WriteLine("[ CMS][ Update]: cms update to v" + result.Version);
                }
                else
                {
                    Console.WriteLine("[ CMS][ Update]: fetch version error code" + result.ToString());
                }
                if (UpgradePercent < 0.97F) UpgradePercent = 0.97F;
            };
        }


        /// <summary>
        /// 获取新版本更新日志
        /// </summary>
        /// <returns></returns>
        public static VersionInfo CheckUpgrade()
        {
            if (!Cms.OfficialEnvironment)
            {
                return new VersionInfo { FetchCode = -5, FetchMsg = "程序当前运行在生产环境下，无法升级" };
            }
            string updateMetaFile = GetUpdateUrl("upgrade.xml");
            WebRequest wr = WebRequest.Create(updateMetaFile);

            HttpWebResponse rsp;
            try
            {
                rsp = wr.GetResponse() as HttpWebResponse;
            }
            catch
            {
                return new VersionInfo { FetchCode = -3, FetchMsg = "无法连接到更新服务器" };
            }

            Stream rspStream;
            if (rsp == null || rsp.ContentLength <= 0 || (rspStream = rsp.GetResponseStream()) == null)
            {
                return new VersionInfo { FetchCode = -3, FetchMsg = "无法连接到更新服务器" };
            }

            switch (rsp.StatusCode)
            {
                //更新服务器发生内部错误
                case HttpStatusCode.InternalServerError:
                    return new VersionInfo { FetchCode = -4, FetchMsg = "更新服务器未返回正确的版本信息" };

                //message = "未发现更新版本!";
                default:
                case HttpStatusCode.NotFound:
                    return new VersionInfo { FetchCode = -2, FetchMsg = "无法获取版本信息" };
                //有新版本
                case HttpStatusCode.OK: break;// return 1;
            }
            //读取内容
            StreamReader sr = new StreamReader(rspStream);
            String result = sr.ReadToEnd();
            sr.Dispose();
            //加载数据
            XmlDocument xd = new XmlDocument();
            result = Regex.Match(result, "<upgrade[^>]+>[\\s\\S]+</upgrade>").Value;
            try
            {
                xd.LoadXml(result);
            }
            catch (XmlException exc)
            {
                throw new Exception("更新描述文件错误：" + result);
            }
            VersionInfo vi = new VersionInfo();
            vi.FetchCode = 1;
            vi.FetchMsg = "发现新版本";
            XmlNode xn = xd.SelectSingleNode("/upgrade");
            vi.Version = xn.Attributes["version"].Value;
            //版本一致
            if (int.Parse(Cms.Version.Replace(".", "")) >= int.Parse(vi.Version.Replace(".", "")))
            {
                return new VersionInfo { FetchCode = -1, FetchMsg = "已经是最新版本" };
            }
            vi.Build = xn.Attributes["build"].Value;
            vi.PatchPackUrl = xn.Attributes["patch"].Value;
            if (!vi.PatchPackUrl.ToUpper().StartsWith("HTTP://"))
            {
                vi.PatchPackUrl = GetUpdateUrl(vi.PatchPackUrl);
            }
            vi.ChangeLog = xn.InnerText;
            return vi;
        }

        /// <summary>
        /// 获取更新路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetUpdateUrl(string fileName)
        {
            if (Settings.SERVER_UPGRADE.EndsWith("/"))
            {
                return Settings.SERVER_UPGRADE + fileName;
            }
            return Settings.SERVER_UPGRADE + "/" + fileName;
        }


        /// <summary>
        /// 升级更新
        /// </summary>
        private static int ApplyUpgrade_Core()
        {
            DirectoryInfo libDir = new DirectoryInfo(EnvUtil.GetBaseDirectory() + "/bin/");

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
        /// 更新
        /// </summary>
        public static void StartUpgrade()
        {
            UpgradePercent = 0f;

            if (OnUpgrade != null)
            {
                new Thread(() =>
                {
                    OnUpgrade();
                    //完成100%
                    UpgradePercent = 1F;
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
            DirectoryInfo dir = new DirectoryInfo(EnvUtil.GetBaseDirectory()+ upgradePath);
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
                        try
                        {
                            entry.WriteToDirectory(dir.FullName, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("[ Upgrade][ Err]:" + exc.Message);
                        }
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
                    dirName = Cms.PhysicPath;
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
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Dispose();
            }

            return 1;
        }

        /// <summary>
        /// 下载文件并存储到内存
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

            string remoteAddr = fileName.IndexOf("http://", StringComparison.Ordinal) == 0 ? fileName : GetUpdateUrl(fileName);

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
            catch (Exception ex)
            {
                Console.WriteLine("[ Upgrade][ DownFile]: " + fileName + "-" + ex.Message);
                // ignored
            }
            return null;
        }

        /// <summary>
        /// 升级指定的模板包(-1:获取包失败,-2:已经安装,1:安装成功)
        /// </summary>
        public static int InstallTemplate(string tplName, string url)
        {
            string tplRootPath = EnvUtil.GetBaseDirectory() + "/templates/";
            MemoryStream ms;
            IArchive archive;
            DirectoryInfo dir;
            if (!url.ToUpper().StartsWith("HTTP://"))
            {
                url = GetUpdateUrl(url);
            }
            byte[] zipData = DownloadFile(url, null);

            if (zipData == null)
            {
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

        /// <summary>
        /// 更新进度
        /// </summary>
        public static float UpgradePercent { get; set; } = 0f;

        /// <summary>
        /// 应用更新核心库
        /// </summary>
        public static void ApplyBinFolder()
        {
            if (UpgradePercent == 1F)
            {
                //线程沉睡并更新dll
                FileInfo[] files = new DirectoryInfo(Cms.PhysicPath + UpgadeDir+"/bin").GetFiles();
                Console.WriteLine("[ CMS][ Update]: apply " + files.Length.ToString() + " files in bin.zip");
                foreach (FileInfo file in files)
                {
                    FileInfo to = new FileInfo(String.Format("{0}bin/{1}", Cms.PhysicPath, file.Name));
                    try
                    {
                        if (to.Exists)
                        {
                            to.Delete();
                        }
                        file.MoveTo(to.FullName);
                        Console.WriteLine("[ CMS][ Update]: update file " + file.Name + " success");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ CMS][ Update]: update file " + file.Name + " failed!"+ex.Message);
                        file.Delete();
                    }
                }
            }
        }
    }
}
