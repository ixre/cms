
//
// Copyright 2011 (C) OPSoft INC,All rights reseved.
// Project : OPSite.Plugin
// Name : 采集插件
// Author : newmin
// Date : 2011-08-27
//

namespace Spc.Plugin
{

    using System;
    using System.Web;
    using Ops.Plugin.Tags;


    public class Tags:TagsManage,System.Web.SessionState.IRequiresSessionState
    {
        public override void Begin_Request()
        {
            //
            //UNDONE: Plugin Permission
            //
            PermissionAttribute permission = new PermissionAttribute(HttpContext.Current.Request.Path);
           // permission.Validate(UserState.Administrator.Current);
        }
    }
}