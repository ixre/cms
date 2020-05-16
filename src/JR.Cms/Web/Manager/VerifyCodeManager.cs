//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
// Modify:
//      2012-11-28  20:20   :  修改验证码不能正常显示的问题
//

using System;
using JR.Stand.Core.Web;

namespace JR.Cms.Web.Manager
{
    /// <summary>
    /// 验证码管理器
    /// </summary>
    internal static class VerifyCodeManager
    {
        /// <summary>
        /// 添加词语
        /// </summary>
        /// <param name="word"></param>
        public static void AddWord(string word)
        {
            if (word != null) HttpHosting.Context.Session.SetString("$manager.login.verify_code", word);
        }

        /// <summary>
        /// 比较验证码
        /// </summary>
        /// <param name="inputWord"></param>
        /// <returns></returns>
        public static bool Compare(string inputWord)
        {
            var str = HttpHosting.Context.Session.GetString("$manager.login.verify_code");
            if (!string.IsNullOrEmpty(str))
                return string.Compare(inputWord, str, StringComparison.OrdinalIgnoreCase) == 0;
            return true;
        }
    }
}