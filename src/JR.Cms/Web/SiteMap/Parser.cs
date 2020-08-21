using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace JR.Cms.Web.SiteMap
{
    /// <summary>
    /// 转换器
    /// </summary>
    public static class Parser
    {
        public static IEnumerable<string> GetAHrefs(string text)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(text);
            var tags = document.DocumentNode.SelectNodes(".//*");
            foreach (var tag in tags)
            {
                if (tag.Name == "a")
                {
                    string href = tag.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(href))
                        yield return href;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="current"></param>
        /// <param name="url"></param>
        public static List<string> Normalize(string domain, string current, List<string> url)
        {
            return url.Select(x =>
            {
                // 带协议的地址, 如果与主机头不一样,排除外站链接
                if (x.IndexOf("://", StringComparison.Ordinal) != -1)
                {
                    // 排除其他协议
                    if (!x.StartsWith("http")) return null;
                    // 排除外站链接
                    if (!x.StartsWith(domain)) return null;
                    return x;
                }
                // 排除javascript:void(0)这样的url
                if (x.Contains(":")) return null;
                // 如:/news/index.html这样的地址
                if (x[0] == '/')return domain + x;
                if (x[0] == '#') return null;
                int last = current.LastIndexOf('/');
                // 如:news/index.html这样的地址
                if (last != -1)return current.Substring(0, last+1) + x;
                // 如:index.html不包含'/'的地址
                return domain + "/" + x;
            }).ToList().Where(x => !string.IsNullOrEmpty(x)).ToList();
        }
    }
}