using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ops.Cms.Domain.Interface.User;

namespace AtNet.Cms.Domain.Interface.User
{
    public interface IUserRepository
    {
        /// <summary>
        ///   获取应用的角色列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        IList<IRole> GetAppRoles(int appId);

        IUserCredential GetUserCredential(int userId);
        int SaveUser(IUser user);
        int SaveRole(IRole role);
    }
}
