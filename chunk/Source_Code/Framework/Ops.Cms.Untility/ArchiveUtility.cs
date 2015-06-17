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
using System.Web.UI.WebControls;
using AtNet.Cms.BLL;
using AtNet.Cms.DataTransfer;
using AtNet.Cms.Domain.Interface.Models;
using AtNet.Cms.Domain.Interface.User;
using IUser = AtNet.Cms.Domain.Interface._old.IUser;

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

        public static string GetOutline(string html, int length)
        {
            string str = RegexHelper.FilterHtml(html);
            return str.Length > length ? str.Substring(0, length) + "..." : str;
        }


        /// <summary>
        /// 判断是否有权限修改文档
        /// </summary>
        /// <param name="p"></param>
        /// <param name="siteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CanModifyArchive(int siteId,int userId)
        {
            UserDto user = UserState.Administrator.Current;

            if (user.Id == userId || (user.RoleFlag & (int)RoleTag.Master) != 0)
            {
                return true;
            }

            return user.SiteId == siteId && (user.RoleFlag & (int) RoleTag.SiteOwner) != 0;
        }

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

        public static string GetFormatedOutline(string outline, string content, int contentLenLimit)
        {
            if (!String.IsNullOrEmpty(outline))
            {
                return outline.Replace("\n", "<br />");
            }

            string str = RegexHelper.FilterHtml(content);
            return str.Length > contentLenLimit ? str.Substring(0, contentLenLimit) + "..." : str;
        }
    }
}
