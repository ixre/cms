using System;
using System.Collections;
using System.IO;
using System.Text;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.Extensions;

namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 
    /// </summary>
    public static class Logger
    {
        private static FileLogger fileLogger;

        private static FileLogger GetFile()
        {
            DateTime dt = DateTime.Now;
            int unixInt = dt.Date.DayOfYear;

            if (fileLogger == null || unixInt != fileLogger.Seed)
            {
                string logDir = EnvUtil.GetBaseDirectory()+ PluginConfig.PLUGIN_TMP_DIRECTORY;

                //创建日志目录
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir).Create();
                }

                fileLogger = new FileLogger(String.Format("{0}/p{1:yyyyMMdd}.log", logDir, dt), false);
                fileLogger.Seed = unixInt;
                fileLogger.FileEncoding = Encoding.UTF8;
            }
            return fileLogger;
        }

        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="addr">引起异常的地址，比如网址</param>
        /// <param name="except"></param>
        public static void PrintException(string addr, Exception except)
        {
            if (!PluginConfig.PLUGIN_LOG_OPENED) return;

            FileLogger log = GetFile();
            DateTime dt = DateTime.Now;

            Exception exc = except;

            if (exc.InnerException != null)
            {
                exc = except.InnerException;
            }

            Hashtable hash = new Hashtable();
            hash.Add("addr", addr ?? "application");
            hash.Add("message", exc.Message);
            hash.Add("stack", exc.StackTrace);
            hash.Add("time", String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt));
            hash.Add("source", exc.Source);

            //附加记录
            log.Println(LoggerLevel.Normal,PluginConfig.PLUGIN_LOG_EXCEPT_FORMAT.Template(hash));

            throw except; //继续抛出异常
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        public static void Println(string line)
        {
            if (!PluginConfig.PLUGIN_LOG_OPENED) return;

            FileLogger log = GetFile();
            log.Println(LoggerLevel.Normal,String.Format("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, line));
        }
    }
}