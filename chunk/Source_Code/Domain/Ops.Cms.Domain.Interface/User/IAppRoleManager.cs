using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtNet.Cms.Domain.Interface.User
{
    public interface CreateAppRoleManager
    {
        /// <summary>
        ///   获取应用的角色列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        IList<IRole> GetAppRoles(int appId);
    }
}
