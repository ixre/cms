/*
 * Copyright 2012 OPS,All rights reserved!
 * name     : ArchiveFlag
 * author_id   : newmin
 * date     : 2012/12/22
 */

using System;
using System.Collections.Generic;
using System.Text;
using JR.Stand.Core.Utils;

namespace JR.Cms.Domain.Interface.Content.Archive
{
    /// <summary>
    /// 内置的文档标签
    /// </summary>
    [Flags]
    public enum BuiltInArchiveFlags : int
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = 0,

        /// <summary>
        /// 是否可见
        /// </summary>
        Visible = 1,

        /// <summary>
        /// 是否特殊
        /// </summary>
        IsSpecial = 2,

        /// <summary>
        /// 是否系统
        /// </summary>
        IsSystem = 4,

        /// <summary>
        /// 是否作为单页
        /// </summary>
        AsPage = 8
    }


    /// <summary>
    /// 文档标签
    /// </summary>
    public class ArchiveFlag
    {
        private static string[] internalFlagTexts = new string[] {null, "st", "sc", null, "v", null, null, null, "p"};

        /// <summary>
        /// 获取内置标签的Key
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string GetInternalFlagKey(BuiltInArchiveFlags flag)
        {
            var key = internalFlagTexts[(int) flag];
            return key;
        }

        /// <summary>
        /// 生成标签字符串
        /// </summary>
        /// <param name="isSystem"></param>
        /// <param name="isSpecial"></param>
        /// <param name="visible"></param>
        /// <param name="asSinglePage"></param>
        /// <returns></returns>
        public static string GetFlagString(bool isSystem, bool isSpecial, bool visible, bool asSinglePage)
        {
            var flagArray = new[] {false, isSystem, isSpecial, false, visible, false, false, false, asSinglePage};

            var sb = new StringBuilder();
            sb.Append("{");
            if (flagArray.Length == internalFlagTexts.Length)
                for (var i = 0; i < flagArray.Length; i++)
                    if (internalFlagTexts[i] != null)
                    {
                        sb.Append(internalFlagTexts[i]).Append(":'")
                            .Append(flagArray[i] ? "1" : "0").Append("'");

                        if (i != flagArray.Length - 1) sb.Append(",");
                    }

            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 获取标签值
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="flagKey"></param>
        /// <returns></returns>
        public static bool GetFlag(string flags, string flagKey)
        {
            var json = new JsonAnalyzer(flags);
            return json.GetValue(flagKey) == "1";
        }

        /// <summary>
        /// 获取标签值
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static BuiltInArchiveFlags GetBuiltInFlags(string flags)
        {
            var json = new JsonAnalyzer(flags);
            var flag = BuiltInArchiveFlags.None;


            if (json.GetValue(internalFlagTexts[(int) BuiltInArchiveFlags.AsPage]) == "1")
                flag |= BuiltInArchiveFlags.AsPage;
            if (json.GetValue(internalFlagTexts[(int) BuiltInArchiveFlags.IsSpecial]) == "1")
                flag |= BuiltInArchiveFlags.IsSpecial;

            if (json.GetValue(internalFlagTexts[(int) BuiltInArchiveFlags.IsSystem]) == "1")
                flag |= BuiltInArchiveFlags.IsSystem;
            if (json.GetValue(internalFlagTexts[(int) BuiltInArchiveFlags.Visible]) == "1")
                flag |= BuiltInArchiveFlags.Visible;

            return flag;
        }


        /// <summary>
        /// 获取内置的标签值
        /// </summary>
        public static bool GetFlag(string flags, BuiltInArchiveFlags flag)
        {
            return GetFlag(flags, internalFlagTexts[(int) flag]);
        }

        /// <summary>
        /// 获取标签的字典形式
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Dictionary<string, bool> GetFlagsDict(string flags)
        {
            var dict = new JsonAnalyzer(flags).ConvertToDictionary();
            var flagDict = new Dictionary<string, bool>();
            foreach (var pair in dict) flagDict.Add(pair.Key, pair.Value == "1");
            return flagDict;
        }

        /// <summary>
        /// 生成标签SQL查询条件
        /// </summary>
        /// <param name="flags">示例：new String[,]{{"v","1"},{"sp","1"}};如值为空则不作为查询条件</param>
        /// <returns></returns>
        public static string GetSQLString(string[,] flags)
        {
            var sb = new StringBuilder();
            if (flags.GetLength(1) != 2) throw new ArgumentException();
            for (var i = 0; i < flags.GetLength(0); i++)
                if (!string.IsNullOrEmpty(flags[i, 1]) && flags[i, 1] != "-1")
                {
                    if (sb.Length != 0) sb.Append("AND");
                    sb.Append(" flags LIKE '%").Append(flags[i, 0])
                        .Append(":''").Append(flags[i, 1])
                        .Append("''%'");
                }

            return sb.ToString();
        }

        //public static bool ContainsFlagKey(IDictionary<string, bool> dict, string key)
        //{
        //    foreach (KeyValuePair<string, bool> pair in dict)
        //    {
        //        if (String.Compare(pair.Key, key, true) == 0) return true;
        //    }
        //    return false;
        //}
    }
}