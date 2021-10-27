using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Cms.Infrastructure
{
    public class DataChecker
    {
        private static string[] injStrArr =
            "'|and|exec|insert|select|delete|update|count|*|%|chr|mid|master|truncate|char|declare|;|or|-|+|,"
                .Split('|');

        /// <summary>
        /// 判断是否sql注入
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool SqlIsInject(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return false;

            foreach (var str in injStrArr)
                if (keyword.IndexOf(str, StringComparison.Ordinal) != -1)
                    return true;
            return false;
        }
    }
}