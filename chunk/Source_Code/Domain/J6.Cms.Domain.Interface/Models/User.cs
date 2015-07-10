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
using J6.DevFw.Framework.Automation;

namespace J6.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 账户表
    /// </summary>
    [EntityForm]
    public class User
    {
        /// <summary>
        /// 站点编号
        /// </summary>
        //[FormField("siteid", Text = "站点编号", DisableEdit = true)]
        public int SiteId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [FormField("username",Text="用户名",Length="[4,20]")]
        public string UserName { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [FormField("password",Text="密码",Length="[6,20]",IsPassword=true)]
        public string Password { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [FormField("name", Text = "昵称", Length = "[0,15]", IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// 组ID
        /// </summary>
        [FormField("groupid", Text = "用户组", DisableEdit = true)] 
        public int GroupId { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        [FormField("available", Text = "账号状态")]
        [SelectField(Data="正常=True;停用=False",UseDrop=true)]
        public bool Available { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserGroups Group { get { return (UserGroups)GroupId; } }
    }
}