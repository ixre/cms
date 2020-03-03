//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: OPSoft.JSON
// FileName : JsonAnalyzer.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/11/26 21:14:30
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Utils
{
    /// <summary>
    /// JSON分析器
    /// </summary>
    public class JsonAnalyzer
    {
        private string jsonString;
        private IList<string> keys = new List<string>();

        public JsonAnalyzer(string json)
        {
            if (String.IsNullOrEmpty(json))
            {
                this.jsonString = "{}";
            }
            else
            {
                this.jsonString = json;

                //将字符中的键存到列表中
                const string pattern = "[\\{,]([^:]+):";
                MatchCollection mc = Regex.Matches(this.jsonString, pattern);

                foreach (Match m in mc)
                {
                    keys.Add(m.Groups[1].Value);
                }
            }
        }

        /// <summary>
        /// 键的集合
        /// </summary>
        public string[] Keys
        {
            get
            {
                string[] arr = new string[keys.Count];
                keys.CopyTo(arr, 0);
                return arr;
            }
        }


        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            // Regex reg = new Regex(key+String.Intern(":'*(?<value>[^'|,|\\}]+)"));
            Regex reg = new Regex(key + String.Intern(":(?<value>'[^']+|(?!')[^,|}]+)"));
            if (reg.IsMatch(jsonString))
            {
                return reg.Match(jsonString).Groups["value"].Value.Replace("'", String.Empty);
            }
            return String.Empty;
        }

        /// <summary>
        /// 设置指定键的值并返回新的JSON字符串,如果不存在键，则返回空
        /// </summary>
        public string SetValue(string key, string value)
        {
            string newJsonString = String.Empty;

            Regex reg = new Regex(key + String.Intern(":(?<reff>'*)(?<value>[^'|,|\\}]+)"));
            if (reg.IsMatch(jsonString))
            {
                if (reg.Match(jsonString).Groups["reff"].Value == "'")
                {
                    value = "'" + value;
                }
                newJsonString = reg.Replace(jsonString, String.Format("{0}:{1}", key, value));

                //更新到数据
                jsonString = newJsonString;
            }

            return newJsonString;
        }

        /// <summary>
        /// 添加新的键值，如果存在键返回false
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Append(string key, string value)
        {
            Regex reg = new Regex(key + String.Intern(":(?<reff>'*)(?<value>[^'|,|\\}]+)"));
            if (reg.IsMatch(jsonString)) return false;
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.jsonString.Substring(0, this.jsonString.LastIndexOf('}')));
                if (sb.Length != 1)
                {
                    sb.Append(",");
                }

                sb.Append(key).Append(":'").Append(value).Append("'")
                    .Append("}");

                keys.Add(key); //添加到KEYS集合
                this.jsonString = sb.ToString(); //更新到数据

                return true;
            }
        }

        public IDictionary<string, string> ConvertToDictionary()
        {
            return ConvertToDictionary(jsonString);
        }

        /// <summary>
        /// 将JSON转为字典
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ConvertToDictionary(string jsonString)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            const string pattern = "(?<key>(?!\\{|,|')[^:]+):(?<value>'[^']+|(?!')[^,|}]+)"; //非{和'以及,开头,匹配键和值
            Regex reg = new Regex(pattern);
            if (reg.IsMatch(jsonString))
            {
                MatchCollection mc = reg.Matches(jsonString);
                foreach (Match m in mc)
                {
                    dict.Add(m.Groups["key"].Value, m.Groups["value"].Value.Replace("'", String.Empty));
                }
            }
            return dict;
        }

        /// <summary>
        /// 将Hashtable转为Json字符串
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string ToJson(Hashtable hash)
        {
            StringBuilder sb = new StringBuilder();
            var fieldCount = hash.Count;
            int i = 0;
            sb.Append("{");
            foreach (object key in hash.Keys)
            {
                sb.Append("\"").Append(key).Append("\":");
                if ((hash[key] as string) != null)
                {
                    if (hash[key].ToString().StartsWith("["))
                    {
                        sb.Append(hash[key].ToString());
                    }
                    else
                    {
                        sb.Append("\"").Append(hash[key].ToString()).Append("\"");
                    }
                }
                else
                {
                    if (hash[key].ToString() == "True" || hash[key].ToString() == "False")
                    {
                        sb.Append(hash[key].ToString().ToLower());
                    }
                    else
                    {
                        sb.Append(hash[key].ToString());
                    }
                }
                if (++i < fieldCount)
                {
                    sb.Append(",");
                }
            }

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 返回对象的JSON表示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return jsonString;
        }
    }
}