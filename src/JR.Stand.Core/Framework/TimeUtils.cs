using System;

namespace JR.Stand.Core.Framework
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    public static class TimeUtils
    {
        /// <summary>
        /// 小时->秒
        /// </summary>
        public static int Hour = 3600;
        /// <summary>
        /// 小时->秒
        /// </summary>
        public static int Day = 24 * Hour;

        private static DateTime unixVar = new DateTime(1970, 1, 1, 0, 0, 0, 0);
       
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static int Unix()
        {
            return Unix(DateTime.Now);
        }
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static int Unix(DateTime d)
        {
            TimeSpan ts = d - unixVar;
            return Convert.ToInt32(ts.TotalSeconds);
        }
        
        /// <summary>
        /// 将时间戳转为时间
        /// </summary>
        /// <param name="unix">时间戳</param>
        /// <param name="zone">时区</param>
        /// <returns></returns>
        public static DateTime UnixTime(int unix,TimeZone zone)
        {
            long l = unix;
            return zone.ToLocalTime(unixVar).Add(new TimeSpan(l * 10000 * 1000));
        }

        /// <summary>
        /// 将时间戳转为时间
        /// </summary>
        /// <param name="unix">时间戳</param>
        /// <returns></returns>
        public static DateTime UnixTime(int unix)
        {
            long l = unix;
            TimeSpan ts = new TimeSpan(l * 10000 * 1000);
            return TimeZone.CurrentTimeZone.ToLocalTime(unixVar).Add(ts);
        }


        /// <summary>
        /// 获取时间戳(微秒)
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long MilliUnix(DateTime d)
        {
            TimeSpan ts = d - unixVar;
            return Convert.ToInt64(ts.Milliseconds);
        }

        /// <summary>
        /// 获取日期00:00:00的时间戳
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DateUnix(DateTime d)
        {
            return Unix(d.Date);
        }
    }
}
