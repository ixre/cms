using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Stand.Core.Framework.Security;

namespace JR.Stand.Core.Framework.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 为字符串追加指定长度的随机字符并返回该字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string RandomLetters(this String str, int length)
        {
            length = length < 1 ? 1 : length;
            char[] cs =
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't'
                , 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                str += cs[rd.Next(24)]; //24为cs.Length
            }
            return str;
        }

        /// <summary>
        /// 为字符串追加指定长度的随机字符并返回该字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string RandomUpperLetters(this String str, int length)
        {
            length = length < 1 ? 1 : length;
            char[] cs =
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't'
                , 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random rd = new Random();
            int randomNum = 0;
            for (int i = 0; i < length; i++)
            {
                randomNum = rd.Next(24); //24为cs.Length
                str += randomNum > 12 ? cs[rd.Next(randomNum)].ToString().ToUpper() : cs[rd.Next(randomNum)].ToString();
            }
            return str;
        }

        /// <summary>
        /// 用md5加密
        /// </summary>
        /// <returns></returns>
        public static string Md5(this String s)
        {
            return CryptoUtils.MD5(s).ToLower();
        }

       

        /// <summary>
        /// 转换为16位的md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ShortMd5(this String str)
        {
            return CryptoUtils.MD516(str).ToLower();
        }


        /// <summary>
        /// 替换类似%tag%的标签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Template(this string str, Func<string, string> func)
        {
            Regex regex = new Regex("%([^%]+)%");

            return regex.Replace(str, a => { return func(a.Groups[1].Value); });
        }


        /// <summary>
        /// 替换类似%tag%的标签
        /// </summary>
        /// <param name="str"></param>
        /// <param name="data">传递标签值的数组</param>
        /// <returns></returns>
        public static string Template(this string str, params string[] data)
        {
            Regex regex = new Regex("%([^%]+)%");
            MatchCollection mcs = regex.Matches(str);
            if (mcs.Count != data.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                for (int i = 0; i < mcs.Count; i++)
                {
                    str = Regex.Replace(str, mcs[i].Groups[0].Value, data[i]);
                }
            }
            return str;
        }

        public static string Template(this string str, Hashtable hash)
        {
            if (hash == null || hash.Count == 0) return str;
            Regex regex = new Regex("{([^{]+)}");
            MatchCollection mcs = regex.Matches(str);
            string key;
            for (int i = 0; i < mcs.Count; i++)
            {
                key = mcs[i].Groups[1].Value;
                if (hash.ContainsKey(key) && hash[key] != null)
                {
                    str = Regex.Replace(str, mcs[i].Groups[0].Value, hash[key].ToString());
                }
            }
            return str;
        }

        public static string Template(this string str, IDictionary<string, string> hash)
        {
            if (hash == null || hash.Count == 0) return str;
            Regex regex = new Regex("{([^{]+)}");
            MatchCollection mcs = regex.Matches(str);
            string key;
            for (int i = 0; i < mcs.Count; i++)
            {
                key = mcs[i].Groups[1].Value;
                if (hash.ContainsKey(key) && hash[key] != null)
                {
                    str = Regex.Replace(str, mcs[i].Groups[0].Value, hash[key]);
                }
            }
            return str;
        }

        /// <summary>
        /// 是否匹配文本
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static bool IsMatch(this string str, string pattern, RegexOptions option = RegexOptions.None)
        {
            return Regex.IsMatch(str, pattern, option);
        }
    }
}