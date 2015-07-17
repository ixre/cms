//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name: ArchiveUtility.cs
// publisher_id: newmin
// Comments:
// -------------------------------------------
// Modify:
//  2011-06-04  newmin  [+]:添加查找栏目的方法
//  2013-03-11  newmin  [+]:Getpublisher_idName方法
//

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using J6.Cms.BLL;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Content;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface.User;
using IUser = J6.Cms.Domain.Interface._old.IUser;

namespace J6.Cms.Utility
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
        /// <param name="publisherId"></param>
        /// <returns></returns>
        public static bool CanModifyArchive(int siteId,int publisherId)
        {
            UserDto user = UserState.Administrator.Current;
            if (user.Id == publisherId || (user.RoleFlag & Role.Master.Flag) != 0)
            {
                return true;
            }

            AppRoleBind[] u = user.Roles;
            return Role.ContainsApp(siteId,u) && (user.RoleFlag & Role.SiteOwner.Flag) != 0;
        }

        /// <summary>
        /// 获取作者名称
        /// </summary>
        /// <param name="publisherId"></param>
        /// <returns></returns>
        public static string GetPublisherName(int publisherId)
        {
            return "-";

            //todo:
            throw new NotImplementedException();

            return "";
//            if (Regex.IsMatch(publisher_id, "^[a-z0-9_]+$"))
//            {
//                User u = ubll.GetUser(publisher_id);
//                if (u != null)
//                {
//                    return u.Name;
//                }
//            }
//            return publisher_id;
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
