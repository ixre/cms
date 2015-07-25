/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://h3f.net/cms
 * 
 * name : VerifyCodeManager.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.Web;

namespace com.plugin.sso.Core.Utils
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
            if (word != null)
            {
                HttpContext.Current.Session["$manager.login.verifycode"] = word;
            }
        }

        /// <summary>
        /// 比较验证码
        /// </summary>
        /// <param name="inputWord"></param>
        /// <returns></returns>
        public static bool Compare(string inputWord)
        {
            var sess = HttpContext.Current.Session["$manager.login.verifycode"];
            if (sess != null)
            {
                return String.Compare(inputWord, sess.ToString(), true) == 0;
            }
            return true;
        }
    }
}
