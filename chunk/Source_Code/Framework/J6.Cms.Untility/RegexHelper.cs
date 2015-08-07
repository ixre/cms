//
// Copyright 2011 @ K3F.NET,All rights reseved.
// Name: RegexHelper.cs
// publisher_id: newmin
//

using System;
using System.Text.RegularExpressions;

namespace J6.Cms.Utility
{
    public class RegexHelper
    {
        public static string FilterHtml(string html)
        {
            return Regex.Replace(html, "(<[^>]+>)|(&(\\w)+;)|(\\s)", String.Empty, RegexOptions.IgnoreCase);
        }
    }
}
