using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtNet.Cms.Domain.Interface.User
{
    public interface IUserRepository
    {
        IUser CreateUser(int id,int flag);

        IUser GetUser(int id);

        /// <summary>
        ///   获取应用的角色列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        IList<IRole> GetAppRoles(int appId);

        UserCredential GetUserCredential(int userId);


        int SaveUser(IUser user);
        int SaveRole(IRole role);
        IUser GetUserByUser(string username);
    }
}
