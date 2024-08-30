//
// Copyright 2011 @ TO2.NET,All rights reserved.
// Name: RegexHelper.cs
// author_id: newmin
//

using System;
using System.Text.RegularExpressions;

namespace JR.Cms.Library.Utility
{
    public class RegexHelper
    {
        public static string FilterHtml(string html)
        {
            return Regex.Replace(html, "(<[^>]+>)|(&(\\w)+;)|(\\s)", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是否为邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, "^[A-Za-z0-9_\\-]+@[a-zA-Z0-9\\\\-]+(\\.[a-zA-Z0-9]+)+$");
        }
    }
}