using System;

namespace JR.Stand.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 如果字符串为空或长度为零，则返回默认值
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="destination"></param>
        public static String EmptyElse(this String s, String e)
        {
            if (String.IsNullOrEmpty(s)) return e;
            return s;
        }


    }
}