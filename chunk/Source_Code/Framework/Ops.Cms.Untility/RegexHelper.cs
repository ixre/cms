//
// Copyright 2011 @ OPS Inc,All rights reseved.
// Name: RegexHelper.cs
// Author: newmin
//

using System;
using System.Text.RegularExpressions;

namespace Ops.Cms.Utility
{
    public class RegexHelper
    {
        public static string FilterHtml(string html)
        {
            return Regex.Replace(html, "(<[^>]+>)|(&(\\w)+;)|(\\s)", String.Empty, RegexOptions.IgnoreCase);
        }
    }
}
