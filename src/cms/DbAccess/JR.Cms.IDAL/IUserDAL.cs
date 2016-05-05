//
//
//  Copyright 2011 (C) Z3Q.NET.All rights reseved.
//
//  @ Project : OSite
//  @ File Name : IUserDAL.cs
//  @ Date : 2011/8/22
//  @ Author : 
// Note: 用户名不可重复
//
//

using System;
using System.Data;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Interface.Value;
using JR.DevFw.Data;

namespace JR.Cms.IDAL
{
    public interface IUserDal
    {
       
        /// <summary>
        /// 更新用户信息
        /// </summary>
        int SaveUser(IUser user,bool isNew);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int DeleteUser(int userId);

        /// <summary>
        /// 根据用户名读取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="func"></param>
        void GetUserCredential(string username, DataReaderFunc func);

        /// <summary>
        /// 设置用户最后登录时间
        /// </summary>
        void UpdateUserLastLoginDate(string username, DateTime date);

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        DataTable GetAllUser();

        /// <summary>
        /// 检测用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        int GetUserIdByUserName(string userName);


      

        /// <summary>
        /// 用户组更名
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        void RenameUserGroup(int groupId, string groupName);

        /// <summary>
        /// 新增操作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        bool CreateOperation(string name, string path, bool available);

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="id"></param>
        void DeleteOperation(int id);

        /// <summary>
        /// 根据ID获得操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        void GetOperation(int id, DataReaderFunc func);

        /// <summary>
        /// 获取所有操作
        /// </summary>
        /// <returns></returns>
        void GetOperations(DataReaderFunc func);

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="available"></param>
        void UpdateOperation(int id, string name, string path, bool available);

        /// <summary>
        /// 获取分页操作列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount, out int pageCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="available"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex, out int recordCount,
            out int pageCount);

        /// <summary>
        /// 更新用户组权限
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="permissions"></param>
        void UpdateUserGroupPermissions(int groupId, string permissions);

        void GetUserById(int id, DataReaderFunc func);
        DataTable GetMyUserTable(int appId, int userId);
        int SaveCredential(Credential credential,bool isNew);
        void GetUserCredentialById(int userId, DataReaderFunc func);
        void ReadUserRoles(int userId, DataReaderFunc func);
        void SaveUserRole(int userId, int appId, int flag);
        void CleanUserRoleFlag(int userId, int appId);
        string GetUserRealName(int userId);
        int GetMinUserId();
        int DeleteRoleBind(int userId);
        int DeleteCredential(int userId);
    }
}