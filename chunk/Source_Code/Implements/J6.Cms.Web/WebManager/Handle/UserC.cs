//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : User.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/18 14:41:34
// Description :
//
// Get infromation of this software,please visit our site http://z3q.net/cms
//
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using J6.Cms.BLL;
using J6.Cms.CacheService;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Interface.Value;
using J6.Cms.Infrastructure.Domain;
using J6.Cms.ServiceContract;
using J6.Cms.Utility;
using J6.Cms.WebManager;
using J6.DevFw.Framework.Extensions;

namespace J6.Cms.Web.WebManager.Handle
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserC:BasePage
    {
        /// <summary>
        /// 修改资料
        /// </summary>
        public void SaveProfile_GET()
        {
            UserDto user = UserState.Administrator.Current;
            base.RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.User_SaveProfile),
                new
                {
                    entity = JsonSerializer.Serialize(new
                    {
                        Name = user.Name,
                        Phone = user.Phone,
                        Email = user.Email,
                    }),
                });
        }

        public void SaveProfile_POST()
        {
            bool opResult = true;

            string oldPassword = base.Request.Form["opwd"],
            newPassword = base.Request.Form["pwd"],
            name = base.Request.Form["name"];

            UserDto curr = UserState.Administrator.Current;
            UserDto user = ServiceCall.Instance.UserService.GetUser(curr.Id);
            user.Name = name;
            user.Phone = Request["Phone"];
            user.Email = Request["Email"];
            if (!String.IsNullOrEmpty(newPassword))
            {
                if (Generator.Md5Pwd(oldPassword, null) != user.Credential.Password)
                {
                    base.RenderError("原密码不正确!");
                    return;

                }
                user.Credential.Password = Generator.Md5Pwd(newPassword, null);
            }

            ServiceCall.Instance.UserService.SaveUser(user);

            UserState.Administrator.Clear();
            base.RenderSuccess("修改成功!");
        }

        /// <summary>
        /// 系统用户列表
        /// </summary>
        public void UserIndex_GET()
        {
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.User_Index),null);
        }

        public void UserRole_GET()
        {
            IUserServiceContract usrService = ServiceCall.Instance.UserService;
            UserDto user =usrService.GetUser(int.Parse(Request["id"]));
            String roleOpts = this.GetRoleOptions(null);
            Dictionary<int, int[]> appRoles = usrService.GetUserAppRoles(user.Id);
            Dictionary<string,int[]> jsonDict = new Dictionary<string, int[]>(appRoles.Count);
            foreach (int key in appRoles.Keys)
            {
                jsonDict.Add("Role_s"+key.ToString(),appRoles[key]);
            }
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.User_Role), new
            {
                role_opts = roleOpts,
                user_id = user.Id,
                data = JsonSerializer.Serialize(jsonDict),
            });
        }


        /// <summary>
        /// 创建会员
        /// </summary>
        public void NewUser_GET()
        {
            UserDto user =new UserDto();
            user.Credential = new Credential(0,0,"","",1);
            String json = JsonSerializer.Serialize(user.ToFormObject());
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.User_Edit), new
            {
                entity = json,
            });
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public void UpdateUser_GET()
        {
            UserDto user = ServiceCall.Instance.UserService.GetUser(int.Parse(Request["id"]));
            if (user.Credential != null)
            {
                user.Credential.Password = "*********";
            }
            String json = JsonSerializer.Serialize(user.ToFormObject());
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.User_Edit), new
            {
                entity=json,
            });
        }

        public delegate void SiteRoleAppend(SiteDto site);

        private string GetRoleOptions(RoleValue[] roles1)
        {
            StringBuilder sb = new StringBuilder();
            SiteRoleAppend ap = (s) =>
            {
                IList<RoleValue> roles = ServiceCall.Instance.SiteService.GetAppRoles(s.SiteId);
                sb.Append("<div class=\"item\"><div class=\"tit\"><strong>[编号:").Append(s.SiteId.ToString()).Append("] - ")
                    .Append(s.Name).Append("</strong></div>");
                sb.Append("<div class=\"role_p con\"><ul>");
                int i = 0;

                String siteId = s.SiteId.ToString();
                foreach (var r in roles)
                {
                    sb.Append("<li><input type=\"checkbox\" class=\"ck\" field=\"Role_s").Append(siteId).Append("[")
                        .Append(i.ToString()).Append("]\" name=\"Role_s").Append(siteId)
                        .Append("\" value=\"").Append(r.Flag).Append("\"/><label for=\"role_s").Append(siteId).Append("_").Append(r.Flag.ToString())
                        .Append("\">").Append(r.Name).Append("</label></li>");
                    i++;
                }
                sb.Append("</ul></div><div class=\"clear-fix\"></div></div>");
            };

            IList<SiteDto> sites = ServiceCall.Instance.SiteService.GetSites();
            foreach (var site in sites)
            {
                ap(site);
            }

            return sb.ToString();
        }


        /// <summary>
        /// 更新用户
        /// </summary>
        public void SaveUser_POST()
        {
            UserFormObject obj = Request.Form.ConvertToEntity<UserFormObject>();

            UserDto user ;

            if (obj.Id > 0)
            {
                user = ServiceCall.Instance.UserService.GetUser(obj.Id);
            }
            else
            {
                user = new UserDto();
            }

            user.Name = obj.Name;
            user.Email = obj.Email;
            user.Phone = obj.Phone;
            if (user.Credential == null)
            {
                user.Credential = new Credential(0, user.Id, obj.UserName, "", 1);
            }

            if (!Regex.IsMatch(obj.Password, "^\\*+$"))
            {
                user.Credential.Password = Generator.Md5Pwd(obj.Password, "");
            }
            user.Credential.Enabled = obj.Enabled;


//            //不允许修改当前用户
//            if ((curUsr.SiteId>0 && user.SiteId!=curUsr.SiteId) || String.Compare(UserState.Administrator.Current.UserName, user.UserName, true) == 0)
//            {
//                base.RenderError("不允许修改当前用户!");
//                return;
//            }
//            else

//            if (user.RoleFlag > curCus.RoleFlag)
//            {
//                base.RenderError("无权限修改用户!");
//                return;
//            }

            try
            {
                int id = ServiceCall.Instance.UserService.SaveUser(user);
                base.RenderSuccess("修改成功!");
            }
            catch (Exception exc)
            {
                base.RenderError(exc.Message);
            }
        }

        public void SaveUserRole_POST()
        {
            int userId = int.Parse(Request["UserId"]);
            const string prefix = "Role_s";
            //todo: master
            try
            {
                foreach (String k in Request.Form.Keys)
                {
                    if (k.StartsWith(prefix))
                    {
                        int siteId = int.Parse(k.Substring(prefix.Length));
                        this.saveUserSiteRole(userId, siteId, Request.Form.Get(k));
                    }
                }
                base.RenderSuccess();
            }
            catch (Exception exc)
            {
                base.RenderError(exc.Message);
            }
        }

        private void saveUserSiteRole(int userId, int siteId, string value)
        {
            int[] flags;
            if (String.IsNullOrEmpty(value))
            {
                flags = new int[0];
            }
            else
            {
                string[] strArr = value.Split(',');
                flags = new int[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    flags[i] = int.Parse(strArr[i]);
                }
            }

            ServiceCall.Instance.UserService.SaveUserRole(userId, siteId, flags);

        }

        /// <summary>
        /// 设置用户状态
        /// </summary>
        public void SetUserState_POST()
        {
            User user = CmsLogic.User.GetUser(base.Request["username"]);
            UserDto curUsr = UserState.Administrator.Current;

            throw new NotImplementedException();
//            //不允许修改当前用户
//            if ((curUsr.SiteId > 0 && user.SiteId != curUsr.SiteId) || String.Compare(UserState.Administrator.Current.UserName, user.UserName, true) == 0)
//            {
//                base.RenderError("不允许修改当前用户!");
//                return;
//            }
//            else if (user.Group == UserGroups.Master)
//            {
//                base.RenderError("不允许修改超级管理员!");
//                return;
//            }
//            else if (curUsr.GroupId >= user.GroupId)
//            {
//                base.RenderError("无权限修改用户!");
//                return;
//            }
//
//            CmsLogic.User.UpdateUser(user.UserName, this.CurrentSite.SiteId, user.Name, (UserGroups)user.GroupId,!user.Available);
//            base.RenderSuccess();

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public void DeleteUser_POST()
        {
            User user;
            string username = base.Request["username"];

            user = CmsLogic.User.GetUser(username);
//
            throw new NotImplementedException();
//            if (String.Compare(UserState.Administrator.Current.UserName, username, true) == 0)
//            {
//                base.RenderError("不允许修改当前用户!");
//                return;
//            }
//
//            if (user.Group == UserGroups.Master)
//            {
//                base.RenderError("不能删除超级管理员!");
//                return;
//            }
//
//            int i = CmsLogic.User.DeleteUser(username);
//
//            if (i!=-1)
//            {
//                base.RenderSuccess("删除成功!<br /><span style=\"font-size:12px\">共修改<span style=\"color:red;font-weight:bold\">" + i.ToString() + "</span>篇文档信息</span>");
//            }
//            else
//            {
//                base.RenderError("删除失败，请重试!");
//            }

        }

        /// <summary>
        /// 用户JSON数据
        /// </summary>
        public void GetUsers_POST()
        {
            //StringBuilder sb = new StringBuilder();

            //用户列
            // string filter = "site";

            //filter 筛选用户的状态
//            switch (filter)
//            {
//                case "disabled":
//                    users = CmsLogic.User.GetUsers(a => !a.Available);
//                    break;
//                case "available":
//                    users = CmsLogic.User.GetUsers(a => a.Available);
//                    break;
//                case "site":
//                    users = CmsLogic.User.GetUsers(a => a.SiteId == this.CurrentSite.SiteId);
//                    break;
//                default:
//                    users = CmsLogic.User.GetAllUser();
//                    break;
//            }

            DataTable dt;
            UserDto user = UserState.Administrator.Current;
            if (user.IsMaster)
            {
                dt = ServiceCall.Instance.UserService.GetAllUsers();
            }
            else
            {
                dt = ServiceCall.Instance.UserService.GetMyUserTable(base.SiteId, user.Id);
            }
            base.PagerJson(dt, String.Format("共{0}个用户", dt.Rows.Count.ToString()));
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
          // UserDto
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

            int groupId;
            int.TryParse(HttpContext.Current.Request["groupid"], out groupId);
            if (groupId == 0) groupId = 1;
            UserGroup usergroup=CmsLogic.User.GetUserGroup((UserGroups)groupId);


            StringBuilder sb=new StringBuilder();
            //用户组下拉列表
            foreach(UserGroup group in CmsLogic.User.GetUserGroups())
            {
                sb.Append("<option value=\"").Append(group.Id).Append(group.Id==groupId?"\" selected=\"selected\"":"\"").Append(">").Append(group.Name).Append("</option>");
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
                groupID=groupId,
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
            string groupId = HttpContext.Current.Request.Form["groupid"],
                   permissionStr = HttpContext.Current.Request.Form["permissions"];

            CmsLogic.User.UpdateUserGroupPermissions((UserGroups)(int.Parse(groupId)),
                CmsLogic.User.ConvertToPermissionArray(permissionStr.Replace("|", ",")));
        }

    }
}
