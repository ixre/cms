using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JR.Cms.Domain.Interface.Models;
using JR.Stand.Toolkit.HttpTag;

namespace JR.Cms.ServiceImpl
{
    /// <summary>
    /// 
    /// </summary>
    public static class TagUtil
    {
        /// <summary>
        /// 替换站点标签
        /// </summary>
        /// <param name="content"></param>
        /// <param name="siteTags"></param>
        /// <param name="openInBlank"></param>
        /// <returns></returns>
        public static string ReplaceSiteWord(string content, IList<SiteWord> siteTags, bool openInBlank)
        {
            //if (!defaultTagLinkFormat.Contains("{0}")) throw new ArgumentException("������{0}��ʾ�������ñ�ǩID����");
            // 链接格式
            string tagLinkFormat =openInBlank?
                "<a href=\"{0}\" title=\"{1}\" class=\"site-word\" target=\"_blank\">{2}</a>" :
                "<a href=\"{0}\" title=\"{1}\" class=\"site-word\">{2}</a>";


            foreach (SiteWord it in siteTags)
            {
                var reg = new Regex(String.Format("<a[^>]+>(?<key>{0})</a>|(?!<a[^>]*)(?<key>{0})(?![^<]*</a>)", Regex.Escape(it.Word)), RegexOptions.IgnoreCase);
                content = reg.Replace(content, match => String.Format(tagLinkFormat,
                    String.IsNullOrEmpty(it.Url)?"javascript:;":it.Url,
                    it.Title,
                    it.Word));
            }

            return content; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tagsDictKeys"></param>
        /// <returns></returns>
        public static string RemoveSiteWord(string content)
        {
            string linkText;
            return Regex.Replace(content, "<a(.+?)class=\"site-word\"[^>]+>(.+?)</a>", match => match.Groups[2].Value,RegexOptions.Multiline);
        }
    }
}