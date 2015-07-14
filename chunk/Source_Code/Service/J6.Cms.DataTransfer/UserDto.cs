/*
* Copyright(C) 2010-2013 S1N1.COM
* 
* File Name	: Site.cs
* publisher_id	: Newmin (new.min@msn.com)
* Create	: 2011/05/25 19:59:54
* Description	:
*
*/

using System;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Interface.Value;
using J6.DevFw.Framework.Automation;

namespace J6.Cms.DataTransfer
{
    /// <summary>
    /// 账户表
    /// </summary>
    [EntityForm]
    public class UserDto
    {
        public int Id { get; set; }

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
            UserDto usr = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Avatar = user.Avatar,
                CheckCode = user.CheckCode,
                CreateTime = user.CreateTime,
                Email = user.Email,
                LastLoginTime = user.LastLoginTime,
                Phone = user.Phone,
                RoleFlag = user.RoleFlag
            };
            return usr;
        }

        public int RoleFlag { get; set; }

        public string Phone { get; set; }


        public string Email { get; set; }


        public string CheckCode { get; set; }

        public string Avatar { get; set; }

        public RoleValue[] Roles { get; set; }
        
        /// <summary>
        /// 令牌验证
        /// </summary>
        public Credential Credential { get; set; }

        public UserFormObject ToFormObject()
        {
            return new UserFormObject
            {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                Phone = this.Phone,
                Enabled = this.Credential.Enabled,
                UserName = this.Credential.UserName,
                Password = this.Credential.Password,
            };
        }
    }
}