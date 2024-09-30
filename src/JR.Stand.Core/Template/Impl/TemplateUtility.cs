//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:TemplateUtility.cs
// Author:newmin
// Create:2011/06/05
//

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TemplateUtility
    {
        private static readonly DateTime unixVar = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long Unix(DateTime d)
        {
            TimeSpan ts = d - unixVar;
            return Convert.ToInt64(ts.TotalSeconds);
        }


        internal static string Read(string templateId)
        {
            //从缓存中获取模板内容
            return TemplateCache.GetTemplateContent(templateId.ToLower());
        }


        /// <summary>
        /// 从缓存中读取部分模板内容
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        internal static string ReadPartial(string templateName)
        {
            var partName = templateName.Replace(".html", "");
            if (partName.Length > 0 && partName[0] != '/') partName = "/" + partName;
            if (TemplateCache.Exists(partName))
            {
                return TemplateCache.GetTemplateContent(partName);
            }
            return null;
        }

        internal static string GetTemplateId(string filePath, TemplateNames nameType)
        {
            Match match = Regex.Match(filePath, "templates(/|\\\\)+#*(.+?)$", RegexOptions.IgnoreCase);
            if (String.IsNullOrEmpty(match.Value)) throw new Exception("模版页文件名:" + filePath + "不合法");
            string fileName = match.Groups[2].Value;
            String lowerFileName = fileName.ToLower();
            if (lowerFileName.EndsWith(".html") || nameType == TemplateNames.ID)
            {
                //return MD5.EncodeTo16(Regex.Replace(fileName, "/|\\\\", String.Empty).ToLower());
            }

            string id = $"{match.Groups[1].Value}{match.Groups[2].Value}"
                .Replace('\\', '/');
            return id.Substring(0, id.LastIndexOf('.')).ToLower();
        }

        /// <summary>
        /// 获取部分模板的编号
        /// </summary>
        /// <param name="partPath"></param>
        /// <param name="filePath"></param>
        /// <param name="partialFilePath"></param>
        /// <returns></returns>
        internal static string GetPartialTemplateId(string partPath, string filePath, out string partialFilePath)
        {
            string walkFilePath = filePath;
            string walkFileName = partPath;

            //
            // inc/header.html
            //

            if (!partPath.StartsWith("/"))
            {
                DirectoryInfo dWrap = null;
                DirectoryInfo dPar = null;
                DirectoryInfo dCurr = new FileInfo(walkFilePath).Directory;

                //example path: ../../inc/top.html
                if (Regex.IsMatch(partPath, "^\\.\\./"))
                {
                    Regex pathRegex = new Regex("\\.\\./");
                    int dirLayer = pathRegex.Matches(partPath).Count;
                    walkFileName = pathRegex.Replace(walkFileName, String.Empty);

                    int i = 0;
                    do
                    {
                        if (dPar != null)
                        {
                            dWrap = dPar;
                            dPar = dPar.Parent;
                        }
                        else
                        {
                            dPar = dCurr?.Parent;
                        }

                        if (dWrap != null)
                        {
                            dCurr = dWrap;
                        }
                    } while (++i <= dirLayer);

                    walkFileName = $"{dCurr.Name}/{walkFileName}";
                }
                else
                {
                    //
                    //example path: inc/top.html
                    //

                    do
                    {
                        if (dPar != null)
                        {
                            dWrap = dPar;
                            dPar = dPar.Parent;
                        }
                        else
                        {
                            dPar = dCurr.Parent;
                        }

                        if (dWrap != null)
                        {
                            dCurr = dWrap;
                        }
                        walkFileName = $"{dCurr.Name}/{walkFileName}";
                    } while (String.Compare(dPar.Name, "templates", true) != 0);
                }
            }

            partialFilePath = walkFileName;
            return MD5.EncodeTo16(Regex.Replace(walkFileName, "/|\\\\", String.Empty).ToLower());
        }

        /// <summary>
        /// 显示模板信息到页面
        /// </summary>
        public static string GetTemplatePagesHTML()
        {
            string templateContent = ResourcesReader.Read(typeof(TemplateUtility).Assembly,
                "Template/Resources/SysTemplatePage.html");
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<style type=""text/css"">
                          table{width:100%;background:#eee;margin:0 auto;line-height:25px;color:#222;cursor:pointer}
                          table td{background:white;padding:0 8px;}
                          table th{background:#006699;color:white;}
                          table tr.hover td{background:#222;color:white;}
                          table tr.even td{}
                        </style>
                        <script type=""text/javascript"">
                             function dynamicTable(table) {
                                if (table && table.nodeName === 'TABLE') {
                                    var rows = table.getElementsByTagName('tr');
                                    for (var i = 0; i < rows.length; i++) {
                                    if (i % 2 == 1) if (!rows[i].className) rows[i].className = 'even';
                                    rows[i].onmouseover = function () {
                                        this.className = this.className.indexOf('even') != -1 ? 'hover even' : 'hover';
                                    };
                                    rows[i].onmouseout = function () {
                                        this.className = this.className.indexOf('even') == -1 ? '' : 'even';
                                    };
                                 }
                               }
                            }</script>

                        <table cellspacing=""1"" id=""templates"">
                            <tr>
                                <th style=""width:50px;""></th><th style=""width:150px;"">模板编号</th><th style=""width:80px;"">模板类型</th><th>文件名</th><th>模板注释</th><th>文件路径</th></tr>
				<!--
        		<tr><td colspan=""6"" align=""center"" style=""background:#c20000;color:white"">扩展名为“.phtml”表示为一个部分视图；部分视图只能使用ID命名</td></tr>
           		-->");

            string tplContent; //模板文件名,内容
            int i = 0;

            foreach (string key in TemplateCache.TemplateDictionary.Keys)
            {
                var tpl = TemplateCache.TemplateDictionary[key];
                var tplFileName = new Regex("templates(/|\\\\)+#*(.+?)$", RegexOptions.IgnoreCase).Match(tpl.FilePath).Groups[2].Value
                    .Replace("\\", "/"); //模板文件名,内容
                tplContent = tpl.GetContent();

                sb.Append("<tr><td class=\"center\">").Append((++i).ToString()).Append("</td><td class=\"center\">")
                    .Append(key.ToLower()).Append("</td><td class=\"center\">")
                    .Append(
                        //RegexUtility.partialRegex.IsMatch(tplContent) && tplContent.IndexOf("<title>") != -1 
                        !tpl.FilePath.EndsWith(".phtml")
                            ? "<span style=\"color:#333\">模板页面</span>"
                            : "<span style=\"color:#006699\">部分视图</span>")
                    .Append("</td><td>/").Append(tplFileName).Append("</td><td>").Append(tpl.Comment)
                    .Append("</td><td>").Append(tpl.FilePath).Append("</td></tr>");
            }

            sb.Append(@"<tr><td colspan=""6"" align=""center"" style=""background:#f0f0f0;color:#333"">
						部分视图扩展名为“.phtml”,可允许格式如:
						&nbsp;&nbsp;A:${include ""inc/header.html""}
						&nbsp;&nbsp;A:${include:""inc/header.html""}
						&nbsp;&nbsp;B:${include:""/tmpdir/inc/header.html""}
						&nbsp;&nbsp;C:${include:""../../inc/header.html""}
						</td></tr>");

            sb.Append(
                "</table><script type=\"text/javascript\">dynamicTable(document.getElementsByTagName('table')[0]);</script>");

            templateContent = TemplateRegexUtility.Replace(templateContent, match =>
            {
                switch (match.Groups[1].Value)
                {
                    case "title":
                        return "模板信息";
                    case "content":
                        return sb.ToString();
                    case "year":
                        return DateTime.Now.Year.ToString();
                }

                return String.Empty;
            });
            return TemplateUtils.CompressHtml(templateContent);
        }
    }
}