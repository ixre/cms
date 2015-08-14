//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : BasePage.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 9:33:57
// Description :
//
// Get infromation of this software,please visit our site http://h3f.net/cms
//
//

namespace J6.Cms.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.IO.Compression;

    public class ResourceUtility
    {
        public static string CompressHtml(string html)
        {
            return html;


            html = Regex.Replace(html, ">(\\s)+<", "><");

            //替换 //单行注释
            html = Regex.Replace(html, "[\\s|\\t]*\\/\\/[^\\n]*(?=\\n)", String.Empty);

            //替换多行注释
            //const string multCommentPattern = "";
            html = Regex.Replace(html, "/\\*[^\\*]+\\*/", String.Empty);

            //替换<!-- 注释 -->
            html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->", String.Empty);

            //html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|(^?!=http:|https:)//(.+?)\r\n|\r|\n|\t|(\\s\\s)", String.Empty);
            html = Regex.Replace(html, "\r|\n|\t|(\\s\\s)", String.Empty);

            return html;
        }
    }
}
