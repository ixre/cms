/*
* Copyright(C) 2010-2012 OPSoft Inc
* 
* File Name	: PagerLinkHelper
* Author	: Newmin (new.min@msn.com)
* Create	: 2012/10/10 22:30:43
* Description	:
*
*/


using Ops.Web.UI.Pager;

namespace Ops.Cms.WebManager
{
    using Ops.Cms;
    using Ops.Cms.CacheService;
    using Ops.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class Helper
    {
        internal static string BuildPagerInfo(string format, int pageIndex, int recordCount, int pages)
        {
           return UrlPaging.PagerHtml(format, format, pageIndex, recordCount, pages);
            /*
            UrlPager p = new UrlPager(pageIndex, pages);
            p.LinkCount = 5;
            p.RecordCount = recordCount;
            p.FirstPageLink = format;
            p.LinkFormat = format;
            p.SelectPageText = "&nbsp;跳页：";
            p.EnableSelect = true;
            p.PagerTotal = String.Empty;
            p.NextPagerLinkText = "..";
            p.PreviousPagerLinkText = "..";
            p.NextPageText = " > ";
            p.PreviousPageText = " < ";
            p.Style = PagerStyle.Blue;
            return p.ToString();
             */
        }

        internal static string BuildJsonPagerInfo(string firstLinkFormat,string linkFormat, int pageIndex, int recordCount, int pages)
        {
            var pagingGetter = new CustomPagingGetter(firstLinkFormat, linkFormat, "", "", "<<", ">>");
            UrlPager pg = UrlPaging.NewPager(pageIndex,pages,pagingGetter);
            pg.RecordCount = recordCount;
            pg.LinkCount = 10;
            pg.PagerTotal="共{2}条";
            return pg.Pager();
        }


       /// <summary>
       /// 单层树结构
       /// </summary>
       /// <param name="title"></param>
       /// <param name="array"></param>
       /// <returns></returns>
        public static string SingleTree(string title, TreeItem[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"tree\" id=\"single_tree\"><dl><dt class=\"tree-title\"><img src=\"/framework/assets/sys_themes/default/icon_trans.png\" class=\"tree-title\" width=\"24\" height=\"24\"/>").Append(title).Append("</dt>");
            int i=0;
            foreach (TreeItem t in array)
            {
                sb.Append("<dd treeid=\"").Append(t.ID.ToString()).Append("\"><img class=\"")
                    .Append(++i == array.Length ? "tree-item-last" : "tree-item")
                    .Append("\" src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/><span class=\"txt\"><a class=\"namelink\" href=\"javascript:;\">")
                    .Append(t.Name)
                    .Append("</a></span></dd>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetTemplates()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            string[] tpls;
            string tplRootPath = String.Format("{0}templates/", AppDomain.CurrentDomain.BaseDirectory);
            DirectoryInfo dir = new DirectoryInfo(tplRootPath);

            DirectoryInfo[] dirs=dir.GetDirectories();
            tpls = new string[dirs.Length];
            if (dir.Exists)
            {
                int i = -1;
                foreach (DirectoryInfo d in dirs)
                {
                    if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        tpls[++i] = d.Name;
                    }
                }
            }

            SettingFile sf;
            string tplConfigFile,
                   tplName;

            foreach (string key in tpls)
            {

                tplName = key;

                tplConfigFile = String.Format("{0}{1}/tpl.conf", tplRootPath, key);
                if (global::System.IO.File.Exists(tplConfigFile))
                {
                    sf = new SettingFile(tplConfigFile);
                    if (sf.Contains("name"))
                    {
                        tplName = sf["name"];
                    }
                    //if (sf.Contains("thumbnail"))
                    //{
                    //    tplThumbnail = sf["thumbnail"];
                    //}
                }
                if (!String.IsNullOrEmpty(key))
                {
                    dict.Add(key, tplName);
                }
            }
            return dict;
        }


        /// <summary>
        /// 获取下拉选框
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static string GetOptions(IDictionary<string, string> dict, string selectValue)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> p in dict)
            {
                sb.Append("<option value=\"").Append(p.Key).Append("\"")
                    .Append(String.Compare(p.Key, selectValue, true) == 0 ? " selected=\"selected\"" : "")
                    .Append(">").Append(p.Value).Append("</option>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取模板选项
        /// </summary>
        /// <param name="tpl"></param>
        /// <returns></returns>
        public static string GetTemplateOptions(string tpl)
        {
            IDictionary<string, string> tpls = Helper.GetTemplates();
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> p in tpls)
            {
                sb.Append("<option value=\"").Append(p.Key).Append("\"")
                    .Append(String.Compare(p.Key, tpl, true) == 0 ?
                    " selected=\"selected\"" : "")
                    .Append(">")
                    .Append(p.Value==String.Empty?p.Key:p.Value)
                    .Append(p.Key == p.Value ? "" : "(" + p.Key + ")")
                    .Append("</option>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取用户组选项
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static string GetUserGroupOptions(int groupId)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach(var u in CmsLogic.User.GetUserGroups()){
                if (u.Id == 1) continue;
                dict.Add(u.Id.ToString(), u.Name);
            }
            return GetOptions(dict, groupId.ToString());
        }

        /// <summary>
        /// 获取站点选项
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static string GetSiteOptions(int siteId)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var e in SiteCacheManager.GetAllSites())
            {
                dict.Add(e.SiteId.ToString(), e.Name);
            }
            return GetOptions(dict, siteId.ToString());
        }
        
        /// <summary>
        /// 生成备份目录路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetBackupFilePath(string filePath)
        {
        	const string pattern="^(.+)(/|\\\\)([^/\\\\]+)$";
        	if(Regex.IsMatch(filePath,pattern))
        	{
        		Match match=Regex.Match(filePath,pattern);
        		return String.Concat(match.Groups[1].Value,"/.backup/",match.Groups[3].Value);
        	}
        	return filePath;
        }
    }
}
