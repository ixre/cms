using JR.Stand.Core.Framework;

namespace JR.Cms
{
    /// <summary>
    /// 
    /// </summary>
    public class CmsLogger
    {
        private static FileLogger loggerFileLogger;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="text"></param>
        public static void Println(LoggerLevel level,string text)
        {
            CheckLazyLogger();
            loggerFileLogger.Println(level,text);
        }

        private static void CheckLazyLogger()
        {
            if (loggerFileLogger == null)
            {
                var path = Cms.PhysicPath + "/tmp/error.log";
                loggerFileLogger = new FileLogger(path,true);
            }
        }
    }
}