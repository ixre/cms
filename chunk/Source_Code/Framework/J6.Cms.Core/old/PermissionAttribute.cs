//
// Copyright 2011 @ S1N1.COM.
// Permission.cs
// publisher_id:
//      newmin(new.min@msn.com)
// 

using System;
using System.Text.RegularExpressions;
using System.Web;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Models;

namespace J6.Cms.old
{
    //using BLL;

    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true,Inherited=false)]
    public class PermissionAttribute : Attribute    
    {
        private string path;
        public PermissionAttribute()
        {
        }
        public PermissionAttribute(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// 请求的路径
        /// </summary>
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(path)) return HttpContext.Current.Request.Path ;
                if (!Regex.IsMatch(path, string.Intern("{([a-z]+)}"),RegexOptions.IgnoreCase)) return path;
                return Regex.Replace(path, string.Intern("{([a-z]+)}"), match =>
                {
                    return HttpContext.Current.Request[match.Groups[1].Value];
                });
            }
            set { path = value; }
        }

        /// <summary>
        /// 验证用户是否有执行此操作的权限
        /// </summary>
        /// <param name="user"></param>
        public bool Validate(UserDto user)
        {
            //
            // UNDONE: 未完成验证
            //
            /*
            string _path = Path;
            UserBLL ubll = new UserBLL();

            Operation op = ubll.GetOperation(a => String.Compare(a.Path, _path, true) == 0);

            if (op != null)
            {
                if (!op.Available)
                {
                    OutputPermissionTipHtml("该操作不可用!");
                    return false;
                }
                else
                {
                    UserGroup group = ubll.GetUserGroup((UserGroups)user.GroupID);

                    if (group != null && !Array.Exists(group.Permissions, a => String.Compare(a.Path, _path, true) == 0))
                    {
                        OutputPermissionTipHtml("你所在的用户组无权限执行此操作!如有疑问请联系管理员!");
                        return false;
                    }
                }
            }
             * 
             */
            return true;

        }

        static void OutputPermissionTipHtml(string html)
        {
            const string tipTemplate = @" <!DOCTYPE html><html><head><title>提示</title><link rel=""StyleSheet"" type=""text/css"" href=""/style/admin/style.css""/></head>
                <body><style type=""text/css"">body{background:#eef7fe;}</style><div class=""tipborder""><div class=""top""></div>
                <div class=""title"">操作提示</div><div class=""content"">{html}</div><div class=""bottom""></div></div></body></html>";


            HttpContext.Current.Response.Write(tipTemplate.Replace("{html}",html));
        }
    }
}