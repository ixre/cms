﻿/*
 * Copyright 2010 OPS.CC,All rights reserved !
 * 
 * name     : UserGroup
 * author_id   : newmin
 * date     : 2010/11/06 23:45
 * 
 */

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 用户组
    /// </summary>
    public class UserGroup
    {
        /// <summary>
        /// 用户组编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 可以执行操作的权限列表
        /// </summary>
        public Operation[] Permissions { get; set; }
    }
}