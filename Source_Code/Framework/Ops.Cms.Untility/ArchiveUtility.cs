//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name: ArchiveUtility.cs
// Author: newmin
// Comments:
// -------------------------------------------
// Modify:
//  2011-06-04  newmin  [+]:添加查找栏目的方法
//  2013-03-11  newmin  [+]:GetAuthorName方法
//

using System;
using System.Text.RegularExpressions;
using AtNet.Cms.BLL;
using AtNet.Cms.Domain.Interface.Models;
using AtNet.Cms.Domain.Interface._old;

namespace AtNet.Cms.Utility
{
    public static class ArchiveUtility
    {
    	private static IUser _iuser;
        internal static  IUser ubll 
        {
        	get
        	{
        		return _iuser??(_iuser=CmsLogic.User);
        	}
        }

      

        ///// <summary>
        ///// 获取文章的指定长度的描述
        ///// </summary>
        ///// <param name="archive"></param>
        ///// <param name="length"></param>
        ///// <returns></returns>
        //public static string GetDescription(Archive archive, int? length)
        //{
        //    //if (String.IsNullOrEmpty(archive.Content)) return null;
        //    int _length = length ?? 198;
        //    string str = RegexHelper.FilterHtml(archive.Content);
        //    return str.Length > _length ? str.Substring(0, _length) + "..." : str;
        //}
        /// <summary>
        /// 获取文章的大纲,如果未填写大纲将返回文档开头指定数目的文字
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        //public static string GetOutline(Archive archive, int? length)
        //{
        //    if (archive == null) return String.Empty;
        //    if (String.IsNullOrEmpty(archive.Outline))
        //        return GetDescription(archive, length);
        //    return length == null || length >= archive.Outline.Length ?
        //        archive.Outline : archive.Outline.Substring(0, length ?? archive.Outline.Length);
        //}

        public static string GetOutline(string html, int length)
        {
            string str = RegexHelper.FilterHtml(html);
            return str.Length > length ? str.Substring(0, length) + "..." : str;
        }

       
        /// <summary>
        /// 判断是否有权限修改文档
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool CanModifyArchive(string author)
        {
            User user = UserState.Administrator.Current;
            return user.Group == UserGroups.Master||String.Compare(user.UserName,author,true)==0;
        }


        /// <summary>
        /// 更新文档的标签链接
        /// </summary>
        /// <param name="archive"></param>
        //[Obsolete]
        //public static void UpdateArchiveTagLinks(string linkformat,Archive archive,bool singleMode)
        //{
        //    //archive.Content = tags.ReplaceSingleTag(archive.Content);
        //   // archiveBLL.Update(archive);
        //}




        /// <summary>
        /// 获取上一个和下一个链接
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
           [Obsolete]
        public static string GetPreviousAndNextArchiveLinks(string archiveID,string linkFormat)
        {
            /*
            //获取上一篇和下一篇
            //新闻和图文信息才计算上一篇下一篇
            Archive ar = archiveBLL.GetSameCategoryPreviousArchive(archiveID);

            StringBuilder sb = new StringBuilder("<span class=\"prev_archive\">上一篇：");

            if (ar != null)
                sb.Append("<a href=\"").Append(String.Format(linkFormat,String.IsNullOrEmpty(ar.Alias)?ar.ID:ar.Alias)).Append("\">")
                    .Append(ar.Title).Append("</a>");
            else
                sb.Append("没有了");

            sb.Append("</span>&nbsp;&nbsp;");


            //下一篇
            ar = archiveBLL.GetSameCategoryNextArchive(archiveID);


            sb.Append("<span class=\"next_archive\">下一篇：");

            if (ar != null)
                sb.Append("<a href=\"").Append(String.Format(linkFormat, String.IsNullOrEmpty(ar.Alias) ? ar.ID : ar.Alias)).Append("\">")
                    .Append(ar.Title).Append("</a>");
            else
                sb.Append("没有了");

            sb.Append("</span>");


            return sb.ToString();
             */
            return null;
        }




        //==========================  2011-11-29 日以前的方法  ========================================//




        /// <summary>
        /// 获取列表HTML
        /// </summary>
        /// <param name="idOrTag">栏目的ID或Tag</param>
        /// <param name="number">指定条数,默认10条</param>
        /// <returns></returns>
           [Obsolete]
        public static string GetListHtml(string idOrTag, int number,string format)
        {
               /*
            //获取栏目,判断参数为ID还是Tag
            return Regex.IsMatch(idOrTag, "(\\d)+") ?
                BuildListHtml(format,archiveBLL.GetArchives(int.Parse(idOrTag),number).ToEntityList<Archive>()) :
                BuildListHtml(format, archiveBLL.GetArchives(idOrTag, number).ToEntityList<Archive>());
               */
            return null;
        }

        /// <summary>
        /// 获取列表HTML
        /// </summary>
        /// <param name="idOrTag">栏目的ID或Tag</param>
        /// <param name="number">指定条数,默认10条</param>
        /// <returns></returns>
           [Obsolete]
        public static string GetSpecailListHtml(int categoryID, int number, string format)
        {
               /*
            return BuildListHtml(format,archiveBLL.GetSpecialArchives(categoryID, number).ToEntityList<Archive>());
                */
            return null;
        }

        /// <summary>
        /// 获取列表HTML
        /// </summary>
        /// <param name="idOrTag">栏目的ID或Tag</param>
        /// <param name="number">指定条数,默认10条</param>
        /// <returns></returns>
           [Obsolete]
        public static string GetSpecailPicArchiveListHtml(string idOrTag, int number)
        {
            return null;
               /*
            Category category;

            bool isInteger=Regex.IsMatch(idOrTag, "^(\\d)+$");

            category = categoryBLL.Get(a => isInteger && a.ID.ToString() == idOrTag || a.Tag == idOrTag);

            IList<Archive> list= archiveBLL.GetSpecialArchives(category.ID, number).ToEntityList<Archive>();

            return list.Count == 0 ? null : BuildPicArchiveListHtml(list);
                */
        }

        /// <summary>
        /// 获取指定数量特殊标记文档HTML
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
       [Obsolete]
        public static string GetTopSpecialArchiveDetailsHtml(int? number)
        {
           /*
            Archive a;
            StringBuilder sb = new StringBuilder();
            //获取头部第一个特殊显示文档
            DataTable dt = archiveBLL.GetSpecialArchive(number ?? 3);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    a = new Archive
                    {
                        ID = dr["id"].ToString(),
                        Title = dr["title"].ToString(),
                        Content = global::Ops.Web.HtmlHelper.FilterHTMLTag(dr["content"].ToString())
                    };
                    sb.Append("<h1><a href=\"").Append(dr["tag"].ToString())
                        .Append("/").Append(dr["Alias"] is DBNull ? dr["id"].ToString() : dr["Alias"].ToString())
                        .Append("/\" title=\"").Append(a.Title).Append("\">")
                        .Append(a.Title.Length > 20 ? a.Title.Substring(0, 20) + "..." : a.Title).Append("</a></h1><span>")
                        .Append(a.Content.Length > 158 ? a.Content.Remove(158) + "..." : a.Content).Append("</span>");

                }
                return sb.ToString();
            }*/
            return null;
        }


        
        /// <summary>
        /// 获取父栏目指定类型的文档Html,包含子栏目链接
        /// </summary>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        //public static string GetArchivesHtmlByBigCategory(CategoryDto category,SysModuleType type, int number)
        //{
            /*
            DataTable dt =archiveBLL.GetArchives(category.ID,number);
            if (dt.Rows.Count == 0) return null;
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<li>[<a href=\"/").Append(category.Tag).Append("_")
                    .Append(dr["tag"].ToString()).Append("/\" title=\"").Append(dr["name"].ToString())
                    .Append("\">").Append(dr["name"].ToString().Length > 4 ? dr["name"].ToString().Remove(3) + "..." : dr["name"].ToString())
                    .Append("</a>]&nbsp;<a href=\"/").Append(dr["tag"]).Append("/")
                    .Append(dr["Alias"] is DBNull ? dr["id"].ToString() : dr["Alias"].ToString()).Append("/\" title=\"")
                    .Append(dr["title"].ToString()).Append("\">").Append(dr["title"].ToString())
                    .Append("</a></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
            */
         //   return "";
        //}

        /// <summary>
        /// 获取作者名称
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        public static string GetAuthorName(string author)
        {
            if (Regex.IsMatch(author, "^[a-z0-9_]+$"))
            {
                User u = ubll.GetUser(author);
                if (u != null)
                {
                    return u.Name;
                }
            }
            return author;
        }
    }
}
