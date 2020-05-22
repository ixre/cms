using System;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template
{
    public static class TemplateUtils
    {
        private static readonly Regex TrimTagSpaceRegex = new Regex(">(\\s)+<");
        private static readonly Regex SingleRowCommentRegex = new Regex("[\\s|\\t]+\\/\\/[^\\n]*(?=\\n)");
        private static readonly Regex MultiRowCommentRegex = new Regex("/\\*[^\\*]+\\*/");
        private static readonly Regex HtmlCommentRegex = new Regex("<!--[^\\[][\\s\\S]*?-->");
        private static readonly Regex TrimBreakRegex = new Regex("\r|\n|\t|(\\s\\s)");

        /// <summary>
        /// 使用模板引擎自带的压缩程序压缩代码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string CompressHtml(string html)
        {
            //html = Regex.Replace(html, ">(\\s)+<", "><");
            ////html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|(^?!=http:|https:)//(.+?)\r\n|\r|\n|\t|(\\s\\s)", String.Empty);
            //html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|\r|\n|\t|(\\s\\s)", String.Empty);
            //return html;
            html = TrimTagSpaceRegex.Replace(html, "><");
            // 替换 //单行注释
            html = SingleRowCommentRegex.Replace(html, String.Empty);
            // 替换多行注释
            html = MultiRowCommentRegex.Replace(html, String.Empty);
            //替换<!-- 注释 -->
            html = HtmlCommentRegex.Replace(html, String.Empty);
            //html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|(^?!=http:|https:)//(.+?)\r\n|\r|\n|\t|(\\s\\s)", String.Empty);
            html = TrimBreakRegex.Replace(html, String.Empty);
            return html;
        }

        public static string GetFunctionParamValue(string value)
        {
            var len = value.Length;
            if (len == 0) return value;
            if (value[0] == '\'')
            {
                if (len == 1 || value[len - 1] != '\'') throw new TemplateException("参数末尾应包含\"'\"");
                return value.Substring(1, len - 2);
            }

            if (value[0] == '\"')
            {
                if (len == 1 || value[len - 1] != '\"') throw new TemplateException("参数末尾应包含\"");
                return value.Substring(1, len - 2);
            }

            if (value[0] == '{')
            {
                if (len == 1 || value[len - 1] != '}') throw new TemplateException("参数末尾应包含\"}\"");
                return value.Substring(1, len - 2);
            }

            return value;
        }
    }
}