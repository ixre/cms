//
// Copyright S1N1.COM.
// UserBLL.cs newmin (new.min@msn.com)
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using J6.Cms.DAL;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface._old;
using J6.Cms.IDAL;
using J6.DevFw.Data.Extensions;
using J6.DevFw.Framework.Extensions;

namespace J6.Cms.BLL
{
    public class UserBLL : IUser
    {
        private IUserDal dal = new UserDAL();
        private static IList<Operation> operations;
        private static UserGroup[] groups;

        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string EncodePassword(string password)
        {
            return password.Encode16MD5().EncodeMD5();
        }

        /// <summary>
        /// 创建新操作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CreateNewOperation(string name, string path)
        {
            bool result=dal.CreateOperation(name, path, true);
            if (result) operations = null;
            return result;
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="id"></param>
        public void DeleteOperation(int id)
        {
            dal.DeleteOperation(id);
            operations.Remove(GetOperation(id));
        }

        /// <summary>
        /// 获得操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Operation GetOperation(int id)
        {
            Operation op = null;
            dal.GetOperation(id, rd =>
            {
                if (rd.Read())
                {
                    op = new Operation
                    {
                        Available = rd.GetString(3) == "True",
                        ID = rd.GetInt32(0),
                        Name = rd.GetString(1),
                        Path = rd.GetString(2)
                    };
                }
            });
            return op;
        }

        /// <summary>
        /// 获取符合条件的操作
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Operation GetOperation(Func<Operation, bool> func)
        {
            foreach (Operation o in GetOperationList())
            {
                if (func(o)) return o;
            }
            return null;
        }

        /// <summary>
        /// 获取所有操作列表
        /// </summary>
        /// <returns></returns>
        public IList<Operation> GetOperationList()
        {
            if (operations == null)
                dal.GetOperations(reader =>
                {
                    if (reader.HasRows)
                    {
                        operations = reader.ToEntityList<Operation>();
                    }
                });
            return operations;
        }

        /// <summary>
        /// 获取符合条件的操作列表
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public IEnumerable<Operation> GetOperationList(Func<Operation, bool> func)
        {
            foreach (Operation o in GetOperationList())
            {
                if (func(o)) yield return o;
            }
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="available"></param>
        public void UpdateOperation(int id, string name, string path, bool available)
        {
            //更新到数据库
            dal.UpdateOperation(id, name, path, available);

            //更新缓存
            if (operations != null)
            {
                foreach (Operation op in operations)
                {
                    if (op.ID == id)
                    {
                        op.Name = name;
                        op.Path = path;
                        op.Available = available;
                    }
                }
            }
        }

        /// <summary>
        /// 刷新操作缓存
        /// </summary>
        public void RenewOperations()
        {
            operations = null;
        }

        public DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount, out int pageCount)
        {
            return dal.GetPagedOperationList(pageSize, currentPageIndex, out recordCount, out pageCount);
        }
        
        public DataTable GetPagedAvailableOperationList(bool available,int pageSize, int currentPageIndex, out int recordCount, out int pageCount)
        {
            return dal.GetPagedAvailableOperationList(available,pageSize, currentPageIndex, out recordCount, out pageCount);
        }

        /// <summary>
        /// 将权限数组转为字符串
        /// </summary>
        /// <param name="operations"></param>
        /// <returns></returns>
        public string ConvertPermissionArrayToString(Operation[] operations)
        {
            if (operations == null) return null;

            StringBuilder sb = new StringBuilder();

            Array.ForEach(operations, a => { sb.Append(a.ID).Append(","); });
            return Regex.Replace(sb.ToString(), ",$", "");
        }

        /// <summary>
        /// 将字符串转为权限数组
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public Operation[] ConvertToPermissionArray(string permissions)
        {
            string[] permissionArray =permissions.Split(',');

            //如果为空
            if (permissionArray.Length == 0) return null;

            Operation[] opeartes = null;


            IList<Operation> _list = new List<Operation>(
                 GetOperationList(a => Array.Exists(permissionArray, p => p == a.ID.ToString()))
            );
            opeartes = new Operation[_list.Count];
            _list.CopyTo(opeartes, 0);


            //下注释行替换上面

            //ops = new List<Operation>(
            //     GetOperationList().Where(a =>
            //{
            //    return Array.Exists(permissionArray, p => p == a.ID.ToString());
            //})
            // ).ToArray();


            return opeartes;
        }

        /// <summary>
        /// 获取用户组
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public UserGroup GetUserGroup(UserGroups group)
        {
            UserGroup[] groups = GetUserGroups();
            if (groups != null)
            {
                foreach (UserGroup u in groups)
                {
                    if (u != null && u.Id == (int) group)
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有用户组
        /// </summary>
        /// <returns></returns>
        public UserGroup[] GetUserGroups()
        {
            if (groups == null)
            {
                //将DataTable转换为UserGroup数组

                DataTable tb = dal.GetUserGroups();
                groups = new UserGroup[tb.Rows.Count];

                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    groups[i] = new UserGroup
                    {
                        Id = Convert.ToInt32(tb.Rows[i][0]),
                        Name = tb.Rows[i][1].ToString(),

                        //todo:权限控制
                        //Permissions = ConvertToPermissionArray(tb.Rows[i][2].ToString())
                    };
                }
            }
            return groups;
        }

        /// <summary>
        /// 更新用户组的权限
        /// </summary>
        /// <param name="group"></param>
        /// <param name="permissions"></param>
        public void UpdateUserGroupPermissions(UserGroups group, Operation[] permissions)
        {
            dal.UpdateUserGroupPermissions((int)group, ConvertPermissionArrayToString(permissions));

            //更新组的权限
            if (groups == null) GetUserGroups();
            Array.ForEach(groups, a => { if (a.Id == (int)group)a.Permissions = permissions; });
        }

        /// <summary>
        /// 重命名用户组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="groupName"></param>
        public void RenameUserGroup(UserGroups group, string groupName)
        {
           dal.RenameUserGroup((int)group, groupName);

            //更新用户组的名称
            Array.ForEach(GetUserGroups(), a => { if (a.Id == (int)group)a.Name = groupName; });

        }

        /// <summary>
        /// 获得所有系统用户
        /// </summary>
        /// <returns></returns>
        public User[] GetAllUser()
        {
            return new List<User>(dal.GetAllUser().ToEntityList<User>()).ToArray();
        }

        /// <summary>
        /// 获取符合条件的系统用户
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public User[] GetUsers(Func<User, bool> func)
        {
            IList<User> users = dal.GetAllUser().ToEntityList<User>();
            List<User> _users = new List<User>();
            foreach (User u in users)
            {
                if (func(u)) _users.Add(u);
            }
            return _users.ToArray();
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUser(string username)
        {
            User user = null;
            dal.GetUserCredential(username, rd =>
            {
                if (rd.HasRows)
                {
                    user = rd.ToEntity<User>();
                }

            });

            return user;
        }

        public User GetUser(Func<User, bool> func)
        {
            foreach (User u in GetAllUser())
            {
                if (func(u)) return u;
            }
            return null;
        }


        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool UserIsExist(string username)
        {
            return dal.UserIsExist(username);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user"></param>
        public void CreateUser(User user)
        {
            dal.CreateUser(user.SiteId,user.UserName, EncodePassword(user.Password), user.Name, user.GroupId,user.Available);
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ModifyUserPassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
//            if (this.GetUser(username, oldPassword) != null)
//            {
//                dal.ModifyPassword(username, EncodePassword(newPassword));
//                return true;
//            }
//            return false;
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void ResetUserPassword(string username,string password)
        {
            dal.ModifyPassword(username, EncodePassword(password));
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <param name="username"></param>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <param name="available"></param>
        public void UpdateUser(string username, int siteid, string name, UserGroups group, bool available)
        {
            dal.UpdateUser(username, siteid, name, (int)group, available);
        }

        /// <summary>
        /// 删除系统用户并将已删除用户的文档作者改为管理员
        /// </summary>
        /// <returns>返回-1表示未删除成功,0表示未将已删除用户的文档设置超级管理员,否则返回修改数量</returns>
        public int DeleteUser(string username)
        {
            User usr = this.GetUser(username);
            bool result = dal.DeleteUser(username);
           if (result)
           {
               //获取超级管理员用户
               User user = GetUser(a => a.Group == UserGroups.Master);

               //如果超级管理员用户不为空，则转换作者
               if (user != null)
               {
                   //
                   //TODO:
                   //
                   //return new ArchiveBLL().TransferAuthor(usr.UserName, user.UserName);
               }
               else
               {
                   return 0;
               }
           }
           return -1;
        }

        /// <summary>
        /// 设置用户最后登录时间
        /// </summary>
        /// <param name="username"></param>
        public void UpdateUserLastLoginDate(string username)
        {
            dal.UpdateUserLastLoginDate(username, DateTime.Now);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        internal static void Clear()
        {
            operations = null;
            groups = null;
        }

    }
}