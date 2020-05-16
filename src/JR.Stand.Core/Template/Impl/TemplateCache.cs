//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:TemplateCache.cs
// Author:newmin
// Create:2011/06/05
//

using System;
using System.Collections.Generic;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 模板缓存
    /// </summary>
     static class TemplateCache
    {
        /// <summary>
        /// 模板编号列表
        /// </summary>
        internal static readonly IDictionary<string, Template> templateDictionary = new Dictionary<string, Template>();

        /// <summary>
        /// 标签词典
        /// </summary>
        public static readonly TagCollection Tags = new TagCollection();


        /// <summary>
        /// 标签
        /// </summary>
        public class TagCollection
        {
            private static readonly IDictionary<string, string> tagDictionary = new Dictionary<string, string>();

            public string this[string key]
            {
                get
                {
                    if (!tagDictionary.ContainsKey(key)) return "${" + key + "}";
                    return tagDictionary[key];
                }
                set
                {
                    if (tagDictionary.ContainsKey(key)) tagDictionary[key] = value;
                    else tagDictionary.Add(key, value);
                }
            }

            public void Add(string key, string value)
            {
                if (tagDictionary.ContainsKey(key))
                    throw new ArgumentException("键:" + key + "已经存在!");
                else tagDictionary.Add(key, value);
            }
        }

        /// <summary>
        /// 注册模板
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="filePath"></param>
        internal static void RegisterTemplate(string templateID, string filePath)
        {
            templateID = templateID.ToLower();
            if (!templateDictionary.ContainsKey(templateID))
            {
                templateDictionary.Add(templateID, new Template
                {
                    Id = templateID,
                    FilePath = filePath
                });
            }
        }

        /// <summary>
        /// 是否存在模板
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        internal static bool Exists(String templatePath)
        {
            return templateDictionary.ContainsKey(templatePath);
        }

        /// <summary>
        /// 获取模板的实际路径
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        internal static String GetTemplateFilePath(String templatePath)
        {
            if (Exists(templatePath))
            {
                Template t = templateDictionary[templatePath];
                return t.FilePath;
            }
            return null;
        }

        /// <summary>
        /// 如果模板字典包含改模板则获取缓存
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        internal static string GetTemplateContent(string templateID)
        {
            if (templateDictionary.ContainsKey(templateID))
            {
                return templateDictionary[templateID].Content;
            }
            throw new TemplateException(templateID+".html", "模板不存在");
        }
    }
}