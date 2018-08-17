using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T2.Cms.Infrastructure
{
    public class DataChecker
    {
        private static String[] injStrArr =
            "'|and|exec|insert|select|delete|update|count|*|%|chr|mid|master|truncate|char|declare|;|or|-|+|,".Split('|');

        /// <summary>
        /// 判断是否sql注入
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool SqlIsInject(string keyword)
        {
            if (String.IsNullOrEmpty(keyword)) return false;

            foreach (String str in injStrArr)
            {
                if (keyword.IndexOf(str, StringComparison.Ordinal)!=-1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
