using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template
{
    public static class TemplateUtils
    {
        private static readonly Regex TrimTagSpaceRegex = new Regex(">(\\s)+<");
        private static readonly Regex SingleRowCommentRegex = new Regex("[\\s|\\t]+\\/\\/[^\\n]*(?=\\n)");
        private static readonly Regex MultiRowCommentRegex = new Regex("/\\*[^\\*]+\\*/");
        private static readonly Regex HtmlCommentRegex = new Regex("<!--[^\\[][\\s\\S]*?-->");
        private static readonly Regex TrimBreakRegex = new Regex("(\r|\n|\t|\\s\\s)+");

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
            //替换 <!-- 注释 -->
            html = HtmlCommentRegex.Replace(html, String.Empty);
            // 替换空白换行
            html = TrimBreakRegex.Replace(html, " ");
            return html;
        }

        public static string GetFunctionParamValue(string value)
        {
            var len = value.Length;
            if (len == 0) return value;
            if (value[0] == '\'')
            {
                if (len == 1 || value[len - 1] != '\'')
                {
                    Console.WriteLine("value=" + value);
                    throw new TemplateException("参数末尾应包含\"'\"");
                }
                return value.Substring(1, len - 2);
            }

            if (value[0] == '\"')
            {
                if (len == 1 || value[len - 1] != '\"')
                {
                    Console.WriteLine("value=" + value);
                    throw new TemplateException("参数末尾应包含\"");
                }
                return value.Substring(1, len - 2);
            }

            if (value[0] == '{')
            {
                if (len == 1 || value[len - 1] != '}')
                {
                    Console.WriteLine("value=" + value);
                    throw new TemplateException("参数末尾应包含\"}\"");
                }
                return value.Substring(1, len - 2);
            }

            return value;
        }


        /// <summary>
        /// 使用指定编码保存成为本地文件
        /// </summary>
        /// <param name="dstFile">包含路径的文件名称,如：C:/html/default.html</param>
        /// <param name="html"></param>
        /// <param name="encoder"></param>
        public static void SaveFile(string html, string dstFile, Encoding encoder)
        {
            //FileShare.None  独占方式打开
            FileStream fs = new FileStream(dstFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            StreamWriter sr = new StreamWriter(fs, encoder);
            sr.Write(html);
            sr.Flush();
            fs.Flush();
            sr.Dispose();
            fs.Dispose();
        }
    }
}