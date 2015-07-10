//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name: RSSUtility.cs
// publisher_id: newmin
// Comments:
//

namespace J6.Cms.Utility
{
    /// <summary>
    /// 订阅工具
    /// </summary>
    public static class RssUtility
    {
        //internal static readonly ArchiveBLL archiveBLL = new BLL.ArchiveBLL();
        //internal static readonly CategoryBLL categoryBLL = new BLL.CategoryBLL();
        #region RSS Format

        //<?xml version="1.0" encoding="UTF-8"?>  
        //<rss version="2.0">  
        //<channel>  
        //    <title>DotNetBips.com Latest Articles</title>  
        //    <link>www.dotnetbips.com</link>  
        //    <description>DotNetBips.com Latest Articles</description>  
        //    <language>zh-cn</language>  
        //    <copyright>Copyright (C) DotNetBips.com. All rights reserved.</copyright>  
        //    <generator>www.dotnetbips.com RSS Generator</generator>  

        //    <item>  
        //    <title>Using WebRequest and WebResponse</title>  
        //    <link>http://www.dotnetbips.com/displayarticle.aspx?id=239</link>  
        //    <description>Description here</description>  
        //    <category></category>  
        //    <publisher_id>Bipin Joshi</publisher_id>  
        //    <copyright></copyright>  
        //    <pubDate>Sun, 25 Jan 2004 12:00:00 AM GMT</pubDate>  
        //    </item>  
        //</channel>  
        //</rss>
        #endregion


        public static string GetRssAsXml()
        {
            //
            //TODO:重构
            //

            return string.Empty;

            /*
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><rss version=\"2.0\">");
            sb.Append("<channel><title>").Append(Settings.SYS_NAME).Append(" Latest Archives").Append("</title>")
                .Append("<link>").Append(Settings.SYS_NAME).Append("</link>")
                .Append("<description>").Append(Settings.SYS_NAME).Append("-最近发布的文档</description>")
                .Append("<language>zh-cn</language>")
                .Append("<copyright>Copyright (C) OPS.CC. All rights reserved.</copyright>")
                .Append("<generator>www.j6.cc RSS Generator</generator>");

            IList<Archive> archives = archiveBLL.GetAllArchives().ToEntityList<Archive>();
            foreach (Archive a in archives)
            {
                Category category = categoryBLL.Get(c => c.ID == a.Cid);
                sb.Append("<item><title>").Append(a.Title).Append("</title><link>http://").Append(Settings.SYS_DOMAIN).Append(AppContext.Uri.GetArchiveUri(a)).Append("</link><description>")
                    .Append(ArchiveUtility.GetOutline(a, 500)).Append("</description><category>")
                    .Append(category == null ? "" : category.Name).Append("</category><publisher_id>").Append(a.publisher_id).Append("</publisher_id><copyright>")
                    .Append(Settings.SYS_ALIAS).Append("</copyright><pubDate>")
                    .Append(a.CreateDate.ToString()).Append("</pubDate></item>");

            }
            sb.Append("</channel></rss>");
            return sb.ToString();
           */
        }
    }
}