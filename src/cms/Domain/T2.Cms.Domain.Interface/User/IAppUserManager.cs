using System;
using System.Collections.Generic;
using T2.Cms.Domain.Interface.User;

namespace T2.Cms.Domain.Interface.User
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
