///*
// * Copyright 2010 OPS,All rights reseved!
// * name     : ArchiveHelper
// * author   : newmin
// * date     : 2010/12/06
// */
//namespace OPS.OPSite
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Data;
//    using System.Text;
//    using System.Text.RegularExpressions;
//    using OPS.Data;
//    using OPS.OPSite.Models;
//    using OPS.OPSite.DAL;
//    using OPS.OPSite.BLL;

//    public class ArchiveHelper
//    {
//        private static readonly ArchiveDAL dal = new ArchiveDAL();
//        private static readonly ArchiveBLL bll = new ArchiveBLL();

//         <summary>
//         获取文章的指定长度的描述
//         </summary>
//         <param name="archive"></param>
//         <param name="length"></param>
//         <returns></returns>
//        public string GetDescription(Archive archive,int? length)
//        {
//            if (String.IsNullOrEmpty(archive.Content)) return null;
//            int _length=length??198;
//           string str=OPS.Web.HtmlHelper.FilterHTMLTag(archive.Content).Trim();
//            return str.Length>_length?str.Substring(0,_length)+"...":str;
//        }
//         <summary>
//         获取文章的大纲,如果未填写大纲将返回文档开头指定数目的文字
//         </summary>
//         <param name="archive"></param>
//         <param name="length"></param>
//         <returns></returns>
//        public string GetOutline(Archive archive, int? length)
//        {
//            if (String.IsNullOrEmpty(archive.Outline))
//                return GetDescription(archive, length);
//            return length == null || length >= archive.Outline.Length ?
//                archive.Outline : archive.Outline.Substring(0, length??archive.Outline.Length);
//        }

//         <summary>
//         获取文章中的第一张图片
//         </summary>
//         <param name="archive"></param>
//         <returns></returns>
//        public string GetPicUri(Archive archive)
//        {
//            Regex reg = new Regex("\\ssrc=\"([A-Za-z0-9_:\\-\\.\\/]+\\.[jpg|jpeg|gif|bmp|png]+)\"",RegexOptions.IgnoreCase);
//            MatchCollection mc = reg.Matches(archive.Content);
//            if (mc.Count == 0) return "/images/nopic.gif";
//            获取第一组的值
//            string x = reg.Match(archive.Content).Groups[0].Value;
//            x = reg.Replace(x, "$1");
//            return x;
//        }
//         <summary>
//         获取上一个和下一个链接
//         </summary>
//         <param name="archive"></param>
//         <returns></returns>
//        public string GetPreviousAndNextArchiveLinks(Archive archive)
//        {
//            string pattern = AppContext.Config.Uri.ArchiveUriPattern;
//            获取上一篇和下一篇
//            新闻和图文信息才计算上一篇下一篇
//            IList<Archive> ars=dal.GetPreviousArchives(archive.ID, 1);
//            StringBuilder sb=new StringBuilder("<span class=\"pre-archive\">");
//            if (ars != null)
//                sb.Append("<a href=\"").Append(AppContext.Uri.GetArchiveUri(ars[0])).Append("\">")
//                    .Append(ars[0].Title).Append("</a>");
//            else
//                sb.Append("没有了");
//            sb.Append("</span><span class=\"next-archive\">");
//            ars = dal.GetNextArchives(archive.ID, 1);
//            if (ars != null)
//                sb.Append("<a href=\"").Append(AppContext.Uri.GetArchiveUri(ars[0])).Append("\">")
//                    .Append(ars[0].Title).Append("</a>");
//            else sb.Append("没有了");
//            sb.Append("</span>");
//            return sb.ToString();
//        }
//         <summary>
//         生成文章链接列表Html
//         </summary>
//         <param name="list"></param>
//         <returns></returns>
//        internal static string BuildListHtml(IList<Archive> list)
//        {
//            if (list == null || list.Count == 0) return null;
//            StringBuilder sb = new StringBuilder();
//            foreach (Archive a in list)
//            {
//                sb.Append("<a ").Append(a.IsSpecial ? "class=\"special\" " : "").Append("href=\"")
//                    .Append(AppContext.Uri.GetArchiveUri(a))
//                    .Append("\">").Append(a.Title.Length > 18 ? a.Title.Remove(18) + "..." : a.Title)
//                    .Append("</a>");
//            }
//            return sb.ToString();

//            /*
//            if (list == null || list.Count == 0) return null;
//            StringBuilder sb = new StringBuilder("<ul class=\"archivelist\">");
//            foreach (Archive a in list)
//            {
//                sb.Append("<li><a ").Append(a.IsSpecial ? "class=\"special\" " : "").Append("href=\"/")
//                    .Append(String.IsNullOrEmpty(a.alias)?a.ID.ToString():a.alias)
//                    .Append(".html\" title=\"").Append(a.Title).Append("\">").Append(a.Title.Length > 18 ? a.Title.Remove(18) + "..." : a.Title)
//                    .Append("</a></li>");
//            }
//            sb.Append("</ul>");
//            return sb.ToString();
//             */
//        }

//         <summary>
//         生成文章链接列表Html
//         </summary>
//         <param name="list"></param>
//         <returns></returns>
//        internal static string BuildPicArchiveListHtml(ArchiveHelper helper,IList<Archive> list)
//        {
//            if (list == null || list.Count == 0) return null;
//            string pattern = AppContext.Config.Uri.ArchiveUriPattern;
//            StringBuilder sb = new StringBuilder();
//            foreach (Archive a in list)
//            {
//                sb.Append("<a ").Append(a.IsSpecial ? "class=\"special\" " : "").Append("href=\"")
//                    .Append(AppContext.Uri.GetArchiveUri(a))
//                    .Append("\"><img src=\"").Append(helper.GetPicUri(a)).Append("\" alt=\"").Append(a.Title)
//                    .Append("\"/>").Append(a.Title).Append("</a>");
//            }
//            return sb.ToString();
//        }

//         <summary>
//         获取列表HTML
//         </summary>
//         <param name="idOrTag">分类的ID或Tag</param>
//         <param name="number">指定条数,默认10条</param>
//         <returns></returns>
//        public string GetListHtml(string idOrTag, int? number)
//        {
//            获取分类,判断参数为ID还是Tag
//           return Regex.IsMatch(idOrTag, "(\\d)+") ?
//               BuildListHtml(bll.GetArchives(int.Parse(idOrTag),10)):
//               BuildListHtml(bll.GetArchives(idOrTag,10));
//        }
//         <summary>
//         获取列表HTML
//         </summary>
//         <param name="idOrTag">分类的ID或Tag</param>
//         <param name="number">指定条数,默认10条</param>
//         <returns></returns>
//        public string GetSpecailListHtml(string idOrTag, int? number)
//        {
//            获取分类,判断参数为ID还是Tag
//            Catalog catalog = Regex.IsMatch(idOrTag, "(\\d)+") ?
//                new Catalog { ID = int.Parse(idOrTag) } :
//                new Catalog { Tag = idOrTag };

//            return BuildListHtml(new ArchiveDAL().GetSpecialArchives(catalog, number ?? 10));
//        }

//         <summary>
//         获取列表HTML
//         </summary>
//         <param name="idOrTag">分类的ID或Tag</param>
//         <param name="number">指定条数,默认10条</param>
//         <returns></returns>
//        public string GetSpecailPicArchiveListHtml(string idOrTag, int number)
//        {
//            获取分类,判断参数为ID还是Tag
//            Catalog c = Regex.IsMatch(idOrTag, "(\\d)+") ?
//                new Catalog { ID = int.Parse(idOrTag) } :
//                new Catalog { Tag = idOrTag };

//            if (c == null) throw new ArgumentNullException();
//            DataTable dt;
//            if (String.IsNullOrEmpty(c.Tag))
//            {
//                dt = db.GetDataSet(@"select top " + number.ToString() +
//                       @" O_Archives.[ID],alias,Title,[Content] from O_Archives inner join O_Categories on
//                    O_Categories.ID=O_Archives.CID where IsSpecial and CID=@categoryID and Visible Order by CreateDate desc,O_Archives.ID",
//                       CreateParameter("@categoryID", c.ID)).Tables[0];
//            }
//            else
//            {
//                dt = db.GetDataSet(@"select top " + number.ToString()+
//                        @" O_Archives.[ID],alias,Title,[Content] from O_Archives inner join O_Categories on
//                    O_Categories.ID=O_Archives.CID where IsSpecial and tag=@Tag and Visible Order by CreateDate desc,O_Archives.ID",
//                       CreateParameter("@Tag", c.Tag)).Tables[0];
//            }
//            IList<Archive> list=dt.ToEntityList<Archive>();
//            return list.Count == 0? null:BuildPicArchiveListHtml(this,list);
//        }

//         <summary>
//         获取指定数量特殊标记文档HTML
//         </summary>
//         <param name="number"></param>
//         <returns></returns>
//        public string GetTopSpecialArchiveDetailsHtml(int? number)
//        {
//            ArchiveDAL dal = new ArchiveDAL();
//            Archive a;
//            StringBuilder sb = new StringBuilder();
//            获取头部第一个特殊显示文档
//            DataTable dt = dal.GetSpecialArchive(number??3);
//            if (dt.Rows.Count != 0)
//            {
//                foreach (DataRow dr in dt.Rows)
//                {
//                    a = new Archive
//                    {
//                        ID = int.Parse(dr["id"].ToString()),
//                        Title = dr["title"].ToString(),
//                        Content = global::OPS.Web.HtmlHelper.FilterHTMLTag(dr["content"].ToString())
//                    };
//                    sb.Append("<h1><a href=\"").Append(dr["tag"].ToString())
//                        .Append("/").Append(dr["alias"] is DBNull ? dr["id"].ToString() : dr["alias"].ToString())
//                        .Append("/\" title=\"").Append(a.Title).Append("\">")
//                        .Append(a.Title.Length > 20 ? a.Title.Substring(0,20) + "..." : a.Title).Append("</a></h1><span>")
//                        .Append(a.Content.Length > 158 ? a.Content.Remove(158) + "..." : a.Content).Append("</span>");

//                }
//                return sb.ToString();
//            }
//            return null;
//        }
//        public string GetNewArchiveHtml(CatalogType type, int? number)
//        {
//            DataTable dt = new ArchiveDAL().GetArchivesOrderByCreateDate(type, number);
//            if (dt.Rows.Count == 0) return null;
//            StringBuilder sb = new StringBuilder(500);
//            sb.Append("<ul>");
//            foreach (DataRow dr in dt.Rows)
//            {
//                sb.Append("<li><a href=\"/").Append(dr["tag"]).Append("/")
//                    .Append(dr["alias"] is DBNull ? dr["id"].ToString() : dr["alias"].ToString())
//                    .Append("/\" title=\"").Append(dr["title"])
//                    .Append("\">").Append(dr["title"]).Append("</a></li>");
//            }
//            sb.Append("</ul>");
//            return sb.ToString();
//        }
//         <summary>
//         获取父分类指定类型的文档Html,包含子分类链接
//         </summary>
//         <param name="catalog"></param>
//         <param name="type"></param>
//         <param name="number"></param>
//         <returns></returns>
//        public string GetArchivesHtmlByBigCatalog(Catalog catalog,CatalogType type,int? number)
//        {
//            DataTable dt = new ArchiveDAL().GetArchivesOrderByCreateDate(catalog, type, number);
//            if (dt.Rows.Count ==0) return null;
//            StringBuilder sb = new StringBuilder();
//            sb.Append("<ul>");
//            foreach (DataRow dr in dt.Rows)
//            {
//                sb.Append("<li>[<a href=\"/").Append(catalog.Tag).Append("_")
//                    .Append(dr["tag"].ToString()).Append("/\" title=\"").Append(dr["name"].ToString())
//                    .Append("\">").Append(dr["name"].ToString().Length>4?dr["name"].ToString().Remove(3)+"...":dr["name"].ToString())
//                    .Append("</a>]&nbsp;<a href=\"/").Append(dr["tag"]).Append("/")
//                    .Append(dr["alias"] is DBNull?dr["id"].ToString():dr["alias"].ToString()).Append("/\" title=\"")
//                    .Append(dr["title"].ToString()).Append("\">").Append(dr["title"].ToString())
//                    .Append("</a></li>");
//            }
//            sb.Append("</ul>");
//            return sb.ToString();
//        }
//    }
//}