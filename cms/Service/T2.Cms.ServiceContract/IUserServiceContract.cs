using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using T2.Cms.DataTransfer;
using T2.Cms.Domain.Interface.User;
using T2.Cms.Domain.Interface.Value;

namespace T2.Cms.ServiceContract
{
    public interface IUserServiceContract
    {
        LoginResultDto TryLogin(string username, string password);
        UserDto GetUser(int id);
        Credential GetCredentialByUserName(string username);

        /// <summary>
        /// 获取我下属的用户表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        DataTable GetMyUserTable(int appId, int userId);

        int SaveUser(UserDto user);
        DataTable GetAllUsers();
        void SaveUserRole(int userId, int appId, int[] flags);
        Dictionary<int, int[]> GetUserAppRoles(int userId);
        SiteDto[] GetUserRelationSites(int id);
        string GetUserRealName(int publisherId);

        int DeleteUser(int userId);
    }
}
