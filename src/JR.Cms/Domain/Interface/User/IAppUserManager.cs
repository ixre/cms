using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.User;

namespace JR.Cms.Domain.Interface.User
{
    public interface IAppUserManager
    {
        /// <summary>
        ///   获取应用的角色列表
        /// </summary>
        /// <returns></returns>
        IList<RoleValue> GetAppRoles();
    }
}