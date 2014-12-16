//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: Ops.Cms.Manager
// FileName : User.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/18 14:41:34
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
//
//

using Ops.Cms.Utility;

namespace Ops.Cms.WebManager
{
    using Ops.Cms;
    using Ops.Framework.Automation;
    using Spc;
    using Spc.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using model = Spc.Models;

    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserC:BasePage
    {

        public void Groups_GET()
        {
        }



        /// <summary>
        /// 修改资料
        /// </summary>
        public void ModifyUserBasicProfile_GET()
        {
            User user = UserState.Administrator.Current;
            base.RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.User_ModifyBasicProfile),
                new
            {
                name = user.Name
            });
        }

        public void ModifyUserBasicProfile_POST()
        {
            bool opResult = true;

            string oldPassword = base.Request.Form["opwd"],
            newPassword = base.Request.Form["pwd"],
            name = base.Request.Form["name"];


            User user = UserState.Administrator.Current;

            //更新名称
            if (String.Compare(user.Name, name, true) != 0)
            {
                CmsLogic.User.UpdateUser(user.UserName, user.SiteId, name, user.Group, user.Available);
                UserState.Administrator.Clear();
            }

            //修改密码

            if (!String.IsNullOrEmpty(newPassword))
            {
                bool result = CmsLogic.User.ModifyUserPassword(user.UserName, oldPassword, newPassword);
                if (!result)
                {
                    opResult = false;
                    base.RenderError("原密码不正确!");
                    return;
                }
            }

            base.RenderSuccess("修改成功!");
        }

        /// <summary>
        /// 系统用户列表
        /// </summary>
        public void UserIndex_GET()
        {
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.User_Index), new
            {
            });
        }

        /// <summary>
        /// 创建会员
        /// </summary>
        public void CreateUser_GET()
        {
            Cms.Context.Items["ajax"] = "1";

            string html = EntityForm.Build<model.User>(new model.User {Available=true}, true, "添加用户");
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.User_Edit), new
           {
               entity=html,
               sites=Helper.GetSiteOptions(-1),
               groups=Helper.GetUserGroupOptions(-1)
           });
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        public void CreateUser_POST()
        {
            var form = HttpContext.Current.Request.Form;
            model.User usr = EntityForm.GetEntity<model.User>();
            usr.SiteId = base.CurrentSite.SiteId;
            model.User cusUsr = UserState.Administrator.Current;

            if (CmsLogic.User.UserIsExist(usr.UserName))
            {
                base.RenderError("用户名不可用!");
                return;
            }
            else if ((int)UserGroups.Master == usr.GroupId)
            {
                base.RenderError("系统只允许一个超级管理员!");
                return;
            }
            else if (cusUsr.GroupId > (int)UserGroups.Administrator)
            {
                base.RenderError("无权限创建用户!");
            }
            else if (cusUsr.SiteId > 0 && usr.GroupId <= (int)UserGroups.Administrator)
            {
                base.RenderError("站点只允许一个管理员!");
            }
            else
            {
                CmsLogic.User.CreateUser(usr);
                base.RenderSuccess("用户创建成功!");
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public void UpdateUser_GET()
        {
           Cms.Context.Items["ajax"] = "1";

            model.User usr = CmsLogic.User.GetUser(base.Request["username"]);
            usr.Password = "*********";
            string html = EntityForm.Build<model.User>(usr, true, "保存");
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.User_Edit), new
            {
                entity=html,
                groups=Helper.GetUserGroupOptions(usr.GroupId)
            });
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public void UpdateUser_POST()
        {
            model.User user = EntityForm.GetEntity<model.User>();
            model.User curUsr = UserState.Administrator.Current;

            //不允许修改当前用户
            if ((curUsr.SiteId>0 && user.SiteId!=curUsr.SiteId) || String.Compare(UserState.Administrator.Current.UserName, user.UserName, true) == 0)
            {
                base.RenderError("不允许修改当前用户!");
                return;
            }else if(user.Group == UserGroups.Master)
            {
                base.RenderError("不允许修改超级管理员!");
                return;
            }
            else if (curUsr.GroupId >= user.GroupId)
            {
                base.RenderError("无权限修改用户!");
                return;
            }

            CmsLogic.User.UpdateUser(user.UserName,this.CurrentSite.SiteId,user.Name, (UserGroups)user.GroupId,user.Available);

            if (!Regex.IsMatch(user.Password, "^\\*+$"))
            {
                CmsLogic.User.ResetUserPassword(user.UserName, user.Password);
                base.RenderSuccess("修改成功,请妥善保管密码!");
            }
            else
            {
                base.RenderSuccess("修改成功!");
            }
        }

        /// <summary>
        /// 设置用户状态
        /// </summary>
        public void SetUserState_POST()
        {
            model.User user = CmsLogic.User.GetUser(base.Request["username"]);
            model.User curUsr = UserState.Administrator.Current;

            //不允许修改当前用户
            if ((curUsr.SiteId > 0 && user.SiteId != curUsr.SiteId) || String.Compare(UserState.Administrator.Current.UserName, user.UserName, true) == 0)
            {
                base.RenderError("不允许修改当前用户!");
                return;
            }
            else if (user.Group == UserGroups.Master)
            {
                base.RenderError("不允许修改超级管理员!");
                return;
            }
            else if (curUsr.GroupId >= user.GroupId)
            {
                base.RenderError("无权限修改用户!");
                return;
            }

            CmsLogic.User.UpdateUser(user.UserName, this.CurrentSite.SiteId, user.Name, (UserGroups)user.GroupId,!user.Available);
            base.RenderSuccess();

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public void DeleteUser_POST()
        {
            model.User user;
            string username = base.Request["username"];

            user = CmsLogic.User.GetUser(username);

            if (String.Compare(UserState.Administrator.Current.UserName, username, true) == 0)
            {
                base.RenderError("不允许修改当前用户!");
                return;
            }

            if (user.Group == UserGroups.Master)
            {
                base.RenderError("不能删除超级管理员!");
                return;
            }

            int i = CmsLogic.User.DeleteUser(username);

            if (i!=-1)
            {
                base.RenderSuccess("删除成功!<br /><span style=\"font-size:12px\">共修改<span style=\"color:red;font-weight:bold\">" + i.ToString() + "</span>篇文档信息</span>");
            }
            else
            {
                base.RenderError("删除失败，请重试!");
            }

        }

        /// <summary>
        /// 用户JSON数据
        /// </summary>
        public void GetUsers_POST()
        {
            StringBuilder sb = new StringBuilder();

            //用户列表
            User[] users;
            string filter = "site";

            //filter 筛选用户的状态
            switch (filter)
            {
                case "disabled":
                    users = CmsLogic.User.GetUsers(a => !a.Available);
                    break;
                case "available":
                    users = CmsLogic.User.GetUsers(a => a.Available);
                    break;
                case "site":
                    users = CmsLogic.User.GetUsers(a => a.SiteId == this.CurrentSite.SiteId);
                    break;
                default:
                    users = CmsLogic.User.GetAllUser();
                    break;
            }

            int i = 0;
            User usr = UserState.Administrator.Current;
            IEnumerable<User> _users = users.Where(a => a.GroupId > (int)usr.GroupId);

            /*
            foreach (User user in )
            {
                sb.Append("<tr avaiable=\"").Append(user.Available?"1":"0").Append("\" indent=\"").Append(user.UserName).Append("\">")
                    .Append("<td>").Append((++i).ToString()).Append("</td>")
                    .Append("<td>").Append(user.Available?user.UserName:"<span style=\"color:#d0d0d0\">"+user.UserName+"</span>").Append("</td><td>")
                    .Append(user.Name).Append("</td></tr>");
            }*/

            base.PagerJson(_users,String.Format("共{0}个用户",users.Length.ToString()));
        }

        /// <summary>
        /// 会员列表
        /// </summary>
        public void MemberList_GET()
        {
            int pageSize = 10;
            int pageCount, recordCount;
            int currentPage;

            //计算页码
            int.TryParse(HttpContext.Current.Request["page"], out currentPage);
            if (currentPage < 1) currentPage = 1;

            string pagerHtml,
                   memberRowsHtml;


            //会员列表
            DataTable dt = CmsLogic.Member.GetPagedMembers(pageSize, ref currentPage, out recordCount, out pageCount);
            if (dt.Rows.Count == 0)
            {
                pagerHtml = String.Empty;
                memberRowsHtml = "暂无注册会员!";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<p><img alt=\"会员头像\" src=\"").Append(String.IsNullOrEmpty(dr["avatar"].ToString()) ? "?module=file&action=guestavatar" : dr["avatar"].ToString())
                        .Append("\" /><span class=\"profile\">").Append(dr["nickname"].ToString()).Append("(")
                        .Append(dr["username"].ToString()).Append(") <span class=\"regip\">注册IP:")
                        .Append(dr["regip"].ToString()).Append("</span><br />注册时间：")
                        .Append(String.Format("{0:yyyy/MM/dd HH:mm:s}", dr["regTime"])).Append("&nbsp;&nbsp;&nbsp;&nbsp;最后登录：")
                        .Append(String.Format("{0:yyyy/MM/dd HH:mm:s}", dr["LastloginTime"])).Append("</span><span class=\"control\"><a href=\"javascript:showProfile('")
                        .Append(dr["id"].ToString()).Append("')\">详细</a> | <a href=\"javascript:;\" onclick=\"deleteMember(this,'")
                        .Append(dr["id"].ToString()).Append("')\">删除</a></span></p>");
                }
                memberRowsHtml = sb.ToString();
                pagerHtml = Helper.BuildPagerInfo("?module=user&action=memberlist&page={0}", currentPage, recordCount, pageCount);

            }
            base.RenderTemplate(ResourceMap.MemberList,
                new
                {
                    memberRowsHtml=memberRowsHtml,
                    pagerHtml=pagerHtml
                });
        }

        /// <summary>
        /// 操作列表
        /// </summary>
        public void OperationList_GET()
        {

            string pagerHtml,
                operationRowsHtml;

            //page:当前页
            //filter:筛选
            int pageSize = 10;
            int currentPageIndex;
            int pageCount = 1, recordCount = 0;

            int.TryParse(HttpContext.Current.Request["page"], out currentPageIndex);
            if (currentPageIndex < 1) currentPageIndex = 1;

            string filter = HttpContext.Current.Request["filter"];


            DataTable dt;
            StringBuilder sb = new StringBuilder();
            switch (filter)
            {
                case "disabled":

                    dt = CmsLogic.User.GetPagedAvailableOperationList(false, pageSize, currentPageIndex, out recordCount, out pageCount);
                    break;
                case "available":
                    dt = CmsLogic.User.GetPagedAvailableOperationList(true, pageSize, currentPageIndex, out recordCount, out pageCount);
                    break;
                default:
                    dt = CmsLogic.User.GetPagedOperationList(pageSize, currentPageIndex, out recordCount, out pageCount);
                    break;
            }

            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr><td>").Append(dr["id"].ToString()).Append("</td>")
                    .Append("<td><input type=\"text\" class=\"tb_normal\" value=\"").Append(dr["name"].ToString()).Append("\"/></td>")
                    .Append("<td><input type=\"text\" class=\"tb_normal\" value=\"").Append(dr["Path"].ToString()).Append("\"/></td>")
                    .Append("<td align=\"center\"><input type=\"checkbox\"")
                    .Append(String.Compare("true",dr["available"].ToString(),true)==0?" checked=\"checked\"":"")
                    .Append("/></td><td><button class=\"save\"/></td></tr>");
            }

            operationRowsHtml = sb.ToString();


            pagerHtml= Helper.BuildPagerInfo("?module=user&action=operationlist&page={0}&filter=" + filter, currentPageIndex, recordCount, pageCount);


            base.RenderTemplate(ResourceMap.OperationList, new
            {
                operationRowsHtml=operationRowsHtml,
                pagerHtml=pagerHtml
            });
        }

        /// <summary>
        /// 创建新操作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        public void CreateOperation_POST()
        {
            var form = HttpContext.Current.Request.Form;
            string name = form["name"],
                path = form["path"];

            CmsLogic.User.CreateNewOperation(name, path);
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        public void UpdateOperation_POST()
        {
            var form = HttpContext.Current.Request.Form;
            string id = form["id"],
                name = form["name"],
                path = form["path"],
                available = form["available"];

            CmsLogic.User.UpdateOperation(int.Parse(id), name, path,available == "true");
        }

        /// <summary>
        /// 设置权限
        /// </summary>
        public void SetPermissions_GET()
        {
            string usergroupOptions,            //用户组下拉列表
                usergroupPermissionOptions,     //用户组权限下拉列表
                otherPermissionOptions;         //其他权限下拉列表

            int groupID;
            int.TryParse(HttpContext.Current.Request["groupid"], out groupID);
            if (groupID == 0) groupID = 1;
            UserGroup usergroup=CmsLogic.User.GetUserGroup((UserGroups)groupID);


            StringBuilder sb=new StringBuilder();
            //用户组下拉列表
            foreach(UserGroup group in CmsLogic.User.GetUserGroups())
            {
                sb.Append("<option value=\"").Append(group.Id).Append(group.Id==groupID?"\" selected=\"selected\"":"\"").Append(">").Append(group.Name).Append("</option>");
            }
            usergroupOptions=sb.ToString();
            sb.Remove(0,sb.Length);

            //用户组权限下拉列表
            foreach (Operation op in usergroup.Permissions)
            {
                sb.Append("<option value=\"").Append(op.ID).Append("\">").Append(op.Name).Append("</option>");
            }
            usergroupPermissionOptions = sb.ToString();
            sb.Remove(0, sb.Length);


            //其他的权限下拉列表
            foreach (Operation op in CmsLogic.User.GetOperationList())
            {
                if (Array.Find(usergroup.Permissions, a => a.ID == op.ID) == null)
                {
                    sb.Append("<option value=\"").Append(op.ID).Append("\">").Append(op.Name).Append("</option>");
                }
            }
            otherPermissionOptions = sb.ToString();
            sb.Remove(0, sb.Length);

            //显示页面
            base.RenderTemplate(ResourceMap.SetPermissions, new
            {
                groupID=groupID,
                usergroups=usergroupOptions,
                usergroupPermissions = usergroupPermissionOptions,
                otherPermissions=otherPermissionOptions,
                usergroupPermissionCount=usergroup.Permissions==null?0:usergroup.Permissions.Length
            });
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        public void UpdatePermission_POST()
        {
            string groupID = HttpContext.Current.Request.Form["groupid"],
                   permissionStr = HttpContext.Current.Request.Form["permissions"];

            CmsLogic.User.UpdateUserGroupPermissions((UserGroups)(int.Parse(groupID)),
                CmsLogic.User.ConvertToPermissionArray(permissionStr.Replace("|", ",")));
        }

    }
}
