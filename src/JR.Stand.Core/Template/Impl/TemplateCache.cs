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
        internal static  IDictionary<string, Template> TemplateDictionary = new Dictionary<string, Template>();

        /// <summary>
        /// 标签词典
        /// </summary>
        public static readonly TagCollection Tags = new TagCollection();


        /// <summary>
        /// 标签
        /// </summary>
        public class TagCollection
        {
            private static readonly IDictionary<string, string> TagDictionary = new Dictionary<string, string>();

            public string this[string key]
            {
                get
                {
                    if (!TagDictionary.ContainsKey(key)) return "${" + key + "}";
                    return TagDictionary[key];
                }
                set
                {
                    if (TagDictionary.ContainsKey(key)) TagDictionary[key] = value;
                    else TagDictionary.Add(key, value);
                }
            }

            public void Add(string key, string value)
            {
                if (TagDictionary.ContainsKey(key))
                    throw new ArgumentException("键:" + key + "已经存在!");
                else TagDictionary.Add(key, value);
            }
        }

        /// <summary>
        /// 注册模板
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="filePath"></param>
        /// <param name="options"></param>
        internal static void RegisterTemplate(string templateId, string filePath, Options options)
        {
            templateId = templateId.ToLower();
            if (!TemplateDictionary.ContainsKey(templateId))
            {
                TemplateDictionary.Add(templateId, new Template(filePath, options));
            }
        }

        /// <summary>
        /// 是否存在模板
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        internal static bool Exists(String templatePath)
        {
            return TemplateDictionary.ContainsKey(templatePath);
        }

        internal static void Reset()
        {
            TemplateDictionary = new Dictionary<string, Template>();
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
                Template t = TemplateDictionary[templatePath];
                return t.FilePath;
            }
            return null;
        }

        /// <summary>
        /// 如果模板字典包含改模板则获取缓存
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        internal static string GetTemplateContent(string templateId)
        {
            if (TemplateDictionary.ContainsKey(templateId))
            {
                return TemplateDictionary[templateId].GetContent();
            }
            throw new TemplateException(templateId+".html", "模板不存在");
        }
    }
}