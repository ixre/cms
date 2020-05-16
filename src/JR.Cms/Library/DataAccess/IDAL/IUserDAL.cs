//
//
//  Copyright 2011 (C) TO2.NET.All rights reserved.
//
//  @ Project : OSite
//  @ File Name : IUserDAL.cs
//  @ Date : 2011/8/22
//  @ Author : 
// Note: �û��������ظ�
//
//

using System;
using System.Data;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Interface.Value;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.IDAL
{
    public interface IUserDal
    {
        /// <summary>
        /// �����û���Ϣ
        /// </summary>
        int SaveUser(IUser user, bool isNew);

        /// <summary>
        /// ɾ���û�
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int DeleteUser(int userId);

        /// <summary>
        /// �����û�����ȡ�û���Ϣ
        /// </summary>
        /// <param name="username"></param>
        /// <param name="func"></param>
        void GetUserCredential(string username, DataReaderFunc func);

        /// <summary>
        /// �����û�����¼ʱ��
        /// </summary>
        void UpdateUserLastLoginDate(string username, DateTime date);

        /// <summary>
        /// ��ȡ�����û�
        /// </summary>
        /// <returns></returns>
        DataTable GetAllUser();

        /// <summary>
        /// ����û����Ƿ����
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        int GetUserIdByUserName(string userName);


        /// <summary>
        /// �û������
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        void RenameUserGroup(int groupId, string groupName);

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        bool CreateOperation(string name, string path, bool available);

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="id"></param>
        void DeleteOperation(int id);

        /// <summary>
        /// ����ID��ò���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        void GetOperation(int id, DataReaderFunc func);

        /// <summary>
        /// ��ȡ���в���
        /// </summary>
        /// <returns></returns>
        void GetOperations(DataReaderFunc func);

        /// <summary>
        /// ���²���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="available"></param>
        void UpdateOperation(int id, string name, string path, bool available);

        /// <summary>
        /// ��ȡ��ҳ�����б�
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
        DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex,
            out int recordCount,
            out int pageCount);

        /// <summary>
        /// �����û���Ȩ��
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="permissions"></param>
        void UpdateUserGroupPermissions(int groupId, string permissions);

        void GetUserById(int id, DataReaderFunc func);
        DataTable GetMyUserTable(int appId, int userId);
        int SaveCredential(Credential credential, bool isNew);
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