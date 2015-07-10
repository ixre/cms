
/*
* 文件名：  DaysConverter.cs
* 文件说明：星期转换
* 创建人:   cwliu
* 创建日期：2014-03-25  10:48
* 修改说明：
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace J6.Cms.Infrastructure
{
    public static class WeekConverter
    {
        private static readonly string[] _chDayNames =
            new string[] { "日", "一", "二", "三", "四", "五", "六" };

        public static DayOfWeek[] ConvertToDaysFromString(string days)
        {
            if (String.IsNullOrEmpty(days)) return new DayOfWeek[0];

            Regex regex = new Regex(",*(\\d),*");
            if (!regex.IsMatch(days))
                throw new FormatException("星期格式不正确，应为: ,0,1,2,3,4,5, 。左右均需加,");

            List<DayOfWeek> enumDays = new List<DayOfWeek>();

            MatchCollection matachs = regex.Matches(days);
            foreach (Match match in matachs)
            {
                enumDays.Add((DayOfWeek)int.Parse(match.Groups[1].Value));
            }
            return enumDays.ToArray();
        }

        /// <summary>
        /// 转换为星期的分割的字符格式
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static string ConvertToString(DayOfWeek[] days)
        {
            StringBuilder strBuilder = new StringBuilder();
            int arrayIndex = 0;
            Array.ForEach(days, day =>
            {
                if (arrayIndex++ != 0)
                    strBuilder.Append(",");
                strBuilder.Append("星期").Append(_chDayNames[(int)day]);
            });

            return strBuilder.ToString();
        }

        /// <summary>
        /// 转换为分割的字符格式
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static string ConvertToSplitString(DayOfWeek[] days)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (DayOfWeek d in days)
                strBuilder.Append(",").Append(((int)d).ToString());
            if (days.Length != 0)
                strBuilder.Append(",");

            return strBuilder.ToString();
        }
    }
}

