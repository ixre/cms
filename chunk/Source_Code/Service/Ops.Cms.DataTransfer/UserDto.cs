/*
* Copyright(C) 2010-2013 S1N1.COM
* 
* File Name	: Site.cs
* Author	: Newmin (new.min@msn.com)
* Create	: 2011/05/25 19:59:54
* Description	:
*
*/

using System;
using AtNet.Cms.Domain.Interface.Models;
using AtNet.Cms.Domain.Interface.User;
using AtNet.DevFw.Framework.Automation;

namespace AtNet.Cms.DataTransfer
{
    /// <summary>
    /// 账户表
    /// </summary>
    [EntityForm]
    public class UserDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 站点编号
        /// </summary>
        //[FormField("siteid", Text = "站点编号", DisableEdit = true)]
        public int SiteId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [FormField("name", Text = "昵称", Length = "[0,15]", IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }


        public static UserDto Convert(IUser user)
        {
            UserDto usr = new UserDto();
            usr.Id = user.Id;
            usr.SiteId = user.AppId;
            usr.Name = user.Name;
            usr.Avatar = user.Avatar;
            usr.CheckCode = user.CheckCode;
            usr.CreateTime = user.CreateTime;
            usr.Email = user.Email;
            usr.LastLoginTime = user.LastLoginTime;
            usr.Phone = user.Phone;
            usr.RoleFlag = user.RoleFlag;
            return usr;
        }

        public int RoleFlag { get; set; }

        public string Phone { get; set; }


        public string Email { get; set; }


        public string CheckCode { get; set; }

        public string Avatar { get; set; }
    }
}