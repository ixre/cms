/*
 * 用户： newmin
 * 日期: 2013/12/4
 * 时间: 6:26
 * 
 * 修改说明：
 */

using System;

namespace JR.Stand.Core.Data
{
    /// <summary>
    /// 提供将字符串格式化的函数
    /// </summary>
    public delegate string ISqlFormatHandler(string source);


    public interface ISqlFormat
    {
        string Format(string source, params string[] objs);
    }

    public class SqlFormat : ISqlFormat
    {
        public string Format(string source, params string[] objs)
        {
            if (objs.Length == 0) return source;
            return String.Format(source, objs);
        }
    }
}