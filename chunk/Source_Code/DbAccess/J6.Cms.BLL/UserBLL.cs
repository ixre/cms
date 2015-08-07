//
// Copyright K3F.NET.
// UserBLL.cs newmin (new.min@msn.com)
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using J6.Cms.Dal;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface._old;
using J6.Cms.IDAL;
using J6.DevFw.Data.Extensions;
using J6.DevFw.Framework.Extensions;

namespace J6.Cms.BLL
{
    public class UserBll : IUser
    {
        private readonly IUserDal _dal = new UserDal();
        private static IList<Operation> _operations;
        private static UserGroup[] _groups;

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
            bool result=_dal.CreateOperation(name, path, true);
            if (result) _operations = null;
            return result;
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="id"></param>
        public void DeleteOperation(int id)
        {
            _dal.DeleteOperation(id);
            _operations.Remove(GetOperation(id));
        }

        /// <summary>
        /// 获得操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Operation GetOperation(int id)
        {
            Operation op = null;
            _dal.GetOperation(id, rd =>
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
            if (_operations == null)
                _dal.GetOperations(reader =>
                {
                    if (reader.HasRows)
                    {
                        _operations = reader.ToEntityList<Operation>();
                    }
                });
            return _operations;
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
            _dal.UpdateOperation(id, name, path, available);

            //更新缓存
            if (_operations != null)
            {
                foreach (Operation op in _operations)
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
            _operations = null;
        }

        public DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount, out int pageCount)
        {
            return _dal.GetPagedOperationList(pageSize, currentPageIndex, out recordCount, out pageCount);
        }
        
        public DataTable GetPagedAvailableOperationList(bool available,int pageSize, int currentPageIndex, out int recordCount, out int pageCount)
        {
            return _dal.GetPagedAvailableOperationList(available,pageSize, currentPageIndex, out recordCount, out pageCount);
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
            if (_groups == null)
            {
                //将DataTable转换为UserGroup数组
                //todo:
                DataTable tb  = new DataTable();//= _dal.GetUserRoles();
                _groups = new UserGroup[tb.Rows.Count];

                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    _groups[i] = new UserGroup
                    {
                        Id = Convert.ToInt32(tb.Rows[i][0]),
                        Name = tb.Rows[i][1].ToString(),

                        //todo:权限控制
                        //Permissions = ConvertToPermissionArray(tb.Rows[i][2].ToString())
                    };
                }
            }
            return _groups;
        }

        /// <summary>
        /// 更新用户组的权限
        /// </summary>
        /// <param name="group"></param>
        /// <param name="permissions"></param>
        public void UpdateUserGroupPermissions(UserGroups group, Operation[] permissions)
        {
            _dal.UpdateUserGroupPermissions((int)group, ConvertPermissionArrayToString(permissions));

            //更新组的权限
            if (_groups == null) GetUserGroups();
            Array.ForEach(_groups, a => { if (a.Id == (int)group)a.Permissions = permissions; });
        }

        /// <summary>
        /// 重命名用户组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="groupName"></param>
        public void RenameUserGroup(UserGroups group, string groupName)
        {
           _dal.RenameUserGroup((int)group, groupName);

            //更新用户组的名称
            Array.ForEach(GetUserGroups(), a => { if (a.Id == (int)group)a.Name = groupName; });

        }

        /// <summary>
        /// 获得所有系统用户
        /// </summary>
        /// <returns></returns>
        public User[] GetAllUser()
        {
            return new List<User>(_dal.GetAllUser().ToEntityList<User>()).ToArray();
        }

        /// <summary>
        /// 获取符合条件的系统用户
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public User[] GetUsers(Func<User, bool> func)
        {
            IList<User> users = _dal.GetAllUser().ToEntityList<User>();
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
            _dal.GetUserCredential(username, rd =>
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
            return _dal.GetUserIdByUserName(username) >0;
        }


        /// <summary>
        /// 设置用户最后登录时间
        /// </summary>
        /// <param name="username"></param>
        public void UpdateUserLastLoginDate(string username)
        {
            _dal.UpdateUserLastLoginDate(username, DateTime.Now);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        internal static void Clear()
        {
            _operations = null;
            _groups = null;
        }

    }
}