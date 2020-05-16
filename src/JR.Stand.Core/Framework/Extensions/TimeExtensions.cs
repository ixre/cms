using System;

namespace JR.Stand.Core.Framework.Extensions
{
    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class TimeExtensions
    {
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long Unix(this DateTime d)
        {
            return TimeUtils.Unix(d);
        }
        /// <summary>
        /// 获取时间戳(微秒)
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long MilliUnix(DateTime d)
        {
            return TimeUtils.MilliUnix(d);
        }

        /// <summary>
        /// 获取日期00:00:00的时间戳
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DateUnix(DateTime d)
        {
            return TimeUtils.DateUnix(d);
        }
    }
}