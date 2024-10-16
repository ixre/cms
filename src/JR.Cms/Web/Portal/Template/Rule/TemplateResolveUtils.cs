/**
 * Copyright (C) 2007-2024 fze.NET, All rights reserved.
 *
 * name: TemplateResolveUtils.cs
 * author: jarrysix (jarrysix@gmail.com)
 * date: 2024-10-16 20:43:13
 * description: 模板解析工具类
 * history:
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Cms.Core;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;

namespace JR.Cms.Web.Portal.Template.Rule
{
    /// <summary>
    /// 模板解析工具类
    /// </summary>
    public static class TemplateResolveUtils
    {
        private static readonly Dictionary<String, Regex> _extraHtmlTagCache = new Dictionary<string, Regex>();
        public static string GetExtraHtml(string html)
        {
            return html;
        }

        /// <summary>
        /// 获取文档占位标签参数
        /// </summary>
        /// <param name="_ctx">上下文</param>
        /// <param name="archiveDto">文档</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        internal static string GetArchiveExtraTagValue(CmsContext _ctx, ArchiveDto archiveDto, string field)
        {
            // 格式化时间
            var p = GetArchiveHolderTagParam(field, "time_fmt[");
            if (p != null)
            {
                return string.Format("{0:" + p + "}", archiveDto.CreateTime);
            }

            //读取自定义长度大纲
            p = GetArchiveHolderTagParam(field, "summary[");
            if (p != null)
            {
                return ArchiveUtility.GetOutline(
                    string.IsNullOrEmpty(archiveDto.Outline)
                        ? archiveDto.Content
                        : archiveDto.Outline, int.Parse(p));
            }
            // 返回语言项
            p = GetArchiveHolderTagParam(field, "lang[");
            if (p != null)
            {
                return Cms.Language.Get(_ctx.UserLanguage, p) ?? "{" + p + "}";
            }
            // 返回释放html元素内容
            p = GetArchiveHolderTagParam(field, "extra_html[");
            if (p != null)
            {
                return ExtraHtmlTag(archiveDto.Content, p);
            }
            return null;
        }

        /// <summary>
        /// 匹配内容中第一个html标签的内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="htmlTag">html标签</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static string ExtraHtmlTag(string content, string htmlTag)
        {
            // 获取正则，如果不存在，则创建并放入到缓存中
            _extraHtmlTagCache.TryGetValue(htmlTag, out var regex);
            if (regex == null)
            {
                string pattern = $@"<{htmlTag}[^>]*>(.*?)</{htmlTag}>";
                regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                _extraHtmlTagCache[htmlTag] = regex;
            }
            // 匹配内容
            var match = regex.Match(content);
            if (match.Success)
            {
                return match.Value;
            }
            return null;
        }


        /// <summary>
        /// 获取文档占位标签参数
        /// </summary>
        /// <param name="field"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        internal static string GetArchiveHolderTagParam(string field, string prefix)
        {
            if (field.StartsWith(prefix))
            {
                var len = prefix.Length;
                return field.Substring(len, field.Length - len - 1);
            }

            return null;
        }
    }
}