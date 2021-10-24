using System.Collections.Generic;
using System.Data;
using JR.Cms.Domain.Interface.Value;
using LoginResultDto = JR.Cms.ServiceDto.LoginResultDto;
using SiteDto = JR.Cms.ServiceDto.SiteDto;
using UserDto = JR.Cms.ServiceDto.UserDto;

namespace JR.Cms.ServiceContract
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