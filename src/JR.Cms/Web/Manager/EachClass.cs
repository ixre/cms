/*
* Copyright(C) 2010-2012 TO2.NET
* 
* File Name	: EachClass
* Author	: Administrator
* Create	: 2012/10/28 8:53:32
* Description	:
*
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JR.Cms.Core;

namespace JR.Cms.Web.Manager
{
    internal static class EachClass
    {
        internal class TemplateNames
        {
            public string path;
            public string name;
        }

        private static string[] allowListedExt = new string[] {".html", ".phtml", ".css", ".js"};

        /// <summary>
        /// 查找文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="sb"></param>
        /// <param name="ext"></param>
        /// <param name="isroot"></param>
        public static void EachFiles(DirectoryInfo dir, StringBuilder sb, string dirName, string ext, bool isroot)
        {
            if (!dir.Exists) return;
            foreach (var file in dir.GetFiles())
                if (file.Name != "cms.conf" && (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                                            && (string.IsNullOrEmpty(ext) ||
                                                ext.IndexOf(file.Extension, StringComparison.Ordinal) != -1))
                {
                    sb.Append("<option value=\"").Append(dirName).Append("/");

                    if (isroot)
                    {
                        sb.Append(file.Name).Append("\">").Append(file.Name).Append("</option>");
                    }
                    else
                    {
                        var path = dir.FullName.Substring(dir.FullName.IndexOf(dirName) + dirName.Length + 1)
                            .Replace("\\", "/");
                        sb.Append(path).Append("/").Append(file.Name).Append("\">").Append(path.Replace("/", "\\"))
                            .Append("\\").Append(file.Name).Append("</option>");
                    }
                }

            foreach (var _dir in dir.GetDirectories()) EachFiles(_dir, sb, dirName, ext, false);
        }

        /// <summary>
        /// 模板页面
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="sb"></param>
        public static void WalkTemplateFiles(DirectoryInfo dir, StringBuilder sb, string tplName)
        {
            if (dir.Name == ".backup") return;

            var path = Cms.PhysicPath + "templates/" + tplName + "/";
            IList<FileInfo> fileList = new List<FileInfo>();

            foreach (var file in dir.GetFiles())
                if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    && !file.Name.EndsWith(".bak")
                    && Array.Exists(allowListedExt, a => file.Name.IndexOf(a) != -1))
                    fileList.Add(file);

            if (fileList.Count != 0)
            {
                TemplatePageType pageType;
                if (string.Compare(dir.Name, tplName, true) == 0)
                    pageType = TemplatePageType.Index;
                else
                    pageType = Cms.Template.GetPageType(fileList[0].FullName.Substring(path.Length).Replace("\\", "/"));
                while (true)
                {
                    sb.Append("<optgroup label=\"").Append(Cms.Template.GetPageDescribe(pageType)).Append("\">");

                    for (var i = 0; i < fileList.Count; i++)
                        if (Cms.Template.GetPageType(fileList[i].Name) == pageType)
                        {
                            var filePath = fileList[i].FullName.Substring(path.Length).Replace("\\", "/");

                            sb.Append("<option value=\"templates/").Append(tplName).Append("/");

                            sb.Append(filePath).Append("\">/").Append(filePath).Append("</option>");

                            fileList.Remove(fileList[i]);
                            --i;
                        }

                    sb.Append("</optgroup>");

                    if (fileList.Count == 0)
                        break;
                    else
                        pageType = Cms.Template.GetPageType(fileList[0].Name);
                }
            }

            /*
            foreach (FileInfo file in filelist)
            {
                if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    && !file.Name.EndsWith(".bak")
                    && Array.Exists(allowListedExt, a => file.Name.IndexOf(a) != -1))
                {
                    filePath = file.FullName.Substring(path.Length).Replace("\\", "/");
                    pageType = jr.Template.GetPageType(filePath);
                   

                    sb.Append("<option value=\"templates/").Append(tplName).Append("/");

                    sb.Append(filePath).Append("\">/").Append(filePath);
                    if (pageType != TemplatePageType.Custom)
                    {
                        sb.Append("(").Append( jr.Template.GetPageDescript(pageType) ).Append(")");
                    }

                    sb.Append("</option>");
                }
            }
             */

            foreach (var _dir in dir.GetDirectories()) WalkTemplateFiles(_dir, sb, tplName);
        }

        /// <summary>
        /// 栏目，模块，页面选择模板
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="sb"></param>
        public static void EachTemplatePage(DirectoryInfo dir, StringBuilder sb)
        {
            var rootDirLength = (Cms.PhysicPath + "templates/").Length;
            foreach (var file in dir.GetFiles())
                if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && file.Name.EndsWith(".html"))
                {
                    sb.Append("<option value=\"templates/");

                    var path = file.FullName.Substring(rootDirLength).Replace("\\", "/");
                    sb.Append(path).Append("\">/").Append(path).Append("</option>");

                    /*
                    if (isroot)
                    {
                        sb.Append(file.Name).Append("\">").Append(file.Name).Append("</option>");
                    }
                    else
                    {
                        path = dir.FullName.Replace(EnvUtil.GetBaseDirectory(), String.Empty).Replace("\\", "/");
                        sb.Append(path).Append("/").Append(file.Name).Append("\">").Append(path.Replace("/", "\\")).Append("\\").Append(file.Name).Append("</option>");
                    }
                     */
                }

            foreach (var _dir in dir.GetDirectories()) EachTemplatePage(_dir, sb);
        }

        /// <summary>
        /// 迭代模板(指定类型)
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="dir"></param>
        /// <param name="sb"></param>
        /// <param name="pageType"></param>
        /// <param name="names"></param>
        public static void EachTemplatePage(DirectoryInfo rootDir, DirectoryInfo dir, StringBuilder sb,
            IDictionary<string, string> names, TemplatePageType[] pageType)
        {
            if (!dir.Exists || dir.Name == ".backup") return;
            var rootDirLength = rootDir.FullName.Length;

            foreach (var file in dir.GetFiles())
                if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    && CheckExtension(file, ".html")
                    && file.FullName.Replace("\\", "/").IndexOf("/mobi/", StringComparison.Ordinal) == -1 //手机模版页面排除
                    && Array.Exists(pageType, a => Cms.Template.GetPageType(file.Name) == a)
                    && !Cms.Template.IsSystemTemplate(file.Name) //独享模式不显示系统模板
                )
                {
                    sb.Append("<option value=\"");
                    var path = file.FullName.Substring(rootDirLength).Replace("\\", "/");

                    if (names.TryGetValue(path, out var name) && name.Trim().Length > 0)
                        sb.Append(path).Append("\">").Append(name).Append(" - ( /").Append(path).Append(" )")
                            .Append("</option>");
                    else
                        sb.Append(path).Append("\">/").Append(path).Append("</option>");
                }

            foreach (var _dir in dir.GetDirectories()) EachTemplatePage(rootDir, _dir, sb, names, pageType);
        }

        private static bool CheckExtension(FileInfo file, string extensions)
        {
            return extensions.IndexOf(file.Extension.ToLower(), StringComparison.Ordinal) != -1;
        }

        public static void IterTemplateFiles2(DirectoryInfo dir, int subLen, IList<TemplateNames> list,
            IDictionary<string, string> nameDictionary)
        {
            if (!dir.Exists || dir.Name == ".backup") return;
            foreach (var file in dir.GetFiles())
                if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    && CheckExtension(file, ".html|.html"))
                {
                    var path = file.FullName.Substring(subLen).Replace("\\", "/");
                    nameDictionary.TryGetValue(path, out var value);
                    list.Add(new TemplateNames {path = path, name = value ?? ""});
                }

            foreach (var _dir in dir.GetDirectories()) IterTemplateFiles2(_dir, subLen, list, nameDictionary);
        }
    }
}