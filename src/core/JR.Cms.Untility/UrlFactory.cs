//
// Copyright 2011 @ OPS Inc,All right reseved.
// Name: ArchiveUtility.cs
// Author: newmin
// Comments:
// -------------------------------------------
// Modify:
//  2011-06-04  newmin  [+]:添加查找栏目的方法
//  2013-03-11  newmin  [+]:GetAuthorName方法
//
namespace J6.Cms
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    public static class UrlFactory
    {
        internal static readonly ArchiveBLL archiveBLL = new BLL.ArchiveBLL();
        internal static readonly CategoryBLL categoryBLL = new BLL.CategoryBLL();


        /// <summary>
        /// 获取文章中的第一张图片,如果没有则返回NULL(忽略data:image;base64格式的图片地址)
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        public static string GetPicUri(Archive archive)
        {
            return GetFirstPicUri(archive.Content, true);
        }

        /// <summary>
        /// 获取文章中的第一张图片,如果没有则返回NULL
        /// </summary>
        /// <param name="html">包含图片标签的内容</param>
        /// <param name="ignoreBase64">是否忽略data:image;base64格式的图片地址</param>
        /// <returns></returns>
        public static string GetFirstPicUri(string html,bool ignoreBase64)
        {
            const string imgTagRegPattern = "<img[^>]*\\bsrc=\"(?<imguri>[^\"]+)\"[^>]*>";

            Regex reg = new Regex(imgTagRegPattern);

            if (reg.IsMatch(html))
            {
                //忽略base64格式的图片
                if (ignoreBase64)
                {
                    //匹配结果
                    string matchResult;

                    foreach (Match match in reg.Matches(html))
                    {
                        matchResult = reg.Match(html).Groups["imguri"].Value;
                        if (!Regex.IsMatch(matchResult, "^data:image/[a-z]+;base64", RegexOptions.IgnoreCase))
                            return matchResult;
                    }

                }
                else
                {
                    return reg.Match(html).Groups["imguri"].Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取文章中的第一张图片,如果没有则返回NULL
        /// </summary>
        public static string GetFirstPicUri(string html)
        {
            return GetFirstPicUri(html, false);
        }

    
        /// <summary>
        /// 获取栏目文件路径，如news/us
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public static string GetCategoryUrlPath(Category category)
        {
            StringBuilder sb = new StringBuilder();

            IList<Category> categories = new List<Category>(categoryBLL.GetCategories(category.Lft, category.Rgt,CategoryContainerOption.Parents));

            foreach (Category c in categories)
            {
                sb.Append(c.Tag).Append("/");
            }
            sb.Append(category.Tag);
            return sb.ToString();
        }

        /// <summary>
        /// 获取栏目文件路径，如/news/us/
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="useHyperlink">是否加上超链接</param>
        /// <param name="linkFormat">链接格式，如/{0}/</param>
        /// <returns></returns>
        public static string GenerateSitemap(string categoryTag,string split, bool useHyperlink, string linkFormat)
        {
            int i = 0, j = 0;
            string categoryPath=null;
            StringBuilder sb = new StringBuilder();
            Category category = categoryBLL.Get(a => String.Compare(a.Tag, categoryTag, true) == 0);

            //如果栏目不存在
            if (category == null)
            {
                return String.Empty;
            }

            IList<Category> categories =new List<Category>(categoryBLL.GetCategories(category.Lft, category.Rgt,CategoryContainerOption.ParentsAndSelf));
            j = categories.Count;

            foreach (Category c in categories)
            {
                categoryPath = String.Format("{0}/{1}{2}", categoryPath, c.Tag, i < j ? "/" : "");

                //
                //TODO:如果启用多目录,则注释下行
                //
                //categoryPath = c.Tag;

                if (useHyperlink)
                {
                    sb.Append("<a href=\"")
                        .Append(String.Format(linkFormat, categoryPath))
                        .Append("\"");

                    //栏目的上一级添加不追踪特性
                    if (i < j - 2)
                    {
                        sb.Append(" rel=\"nofollow\"");
                    }
                    sb.Append(">").Append(c.Name).Append("</a>");
                }
                else
                {
                    sb.Append(c.Name);
                }

                //添加分隔符
                if (i < j-1)
                {
                    sb.Append(split);
                }
                ++i;
            }

            //去掉双斜杠
            Regex reg = new Regex("\\b//");
            return reg.Replace(sb.ToString(), "/");
        }

    }
}
