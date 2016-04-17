using System;
using System.Collections.Generic;
using J6.Cms.Dal;
using J6.Cms.Domain.Implement.User;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Interface.Value;
using J6.Cms.IDAL;

namespace J6.Cms.ServiceRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserCreator _creator;
        private readonly IUserDal _userDal;

        public UserRepository()
        {
            this._creator = new UserCreator();
            this._userDal = new UserDal();
        }

        public IList<RoleValue> GetAppRoles(int appId)
        {
            //todo:  暂时不增加用户组
            return new List<RoleValue>();
        }


        public int SaveUser(IUser user)
        {
            if (user.Id > 0)
            {
                return this._userDal.SaveUser(user, false);
            }
            return this._userDal.SaveUser(user, true);
        }

        public int SaveRole(RoleValue role)
        {
            throw new NotImplementedException();
        }

        public IUser CreateUser(int id, int flag)
        {
            return this._creator.CreateUser(this, id, flag);
        }


        public IUser GetUser(int id)
        {
            IUser user = null;
            this._userDal.GetUserById(id, rd =>
            {
                if (rd.Read())
                {
                    user = this.CreateUser(Convert.ToInt32(rd["id"]), rd["flag"] == DBNull.Value ? 0 : 
                        Convert.ToInt32(rd["flag"]));
                    user.Name = rd["name"].ToString();
                    user.CreateTime = Convert.ToDateTime(rd["create_time"]);
                    user.LastLoginTime = Convert.ToDateTime(rd["last_login_time"]);
                    user.CheckCode =( rd["check_code"]??"").ToString();
                    user.Avatar = (rd["avatar"] ?? "").ToString();
                    user.Phone = (rd["phone"] ?? "").ToString();
                    user.Email = (rd["email"] ?? "").ToString();
                }
            });
            return user;
        }


        public Credential GetUserCredential(int userId)
        {
            Credential cre = null;
            this._userDal.GetUserCredentialById(userId, rd =>
            {
                if (rd.Read())
                {
                    cre = new Credential(rd.GetInt32(0), rd.GetInt32(1), rd.GetString(2), rd.GetString(3), rd.GetInt32(4));
                }
            });
            return cre;
        }

        public Credential GetCredentialByUserName(string username)
        {
            Credential cre = null;
            this._userDal.GetUserCredential(username, rd =>
            {
                if(rd.Read())
                {
                    cre = new Credential(rd.GetInt32(0),rd.GetInt32(1),rd.GetString(2),rd.GetString(3),rd.GetInt32(4));
                }
            });
            return cre;
        }


        public int SaveCredential(Credential credential)
        {
            if (credential.Id > 0)
            {
                return this._userDal.SaveCredential(credential,false);
            }
            return this._userDal.SaveCredential(credential, true);
        }


        public IDictionary<int,AppRolePair> GetUserRoles(int id)
        {
            IDictionary<int,AppRolePair> dict = new Dictionary<int, AppRolePair>();
            int appId;
            int flag;
            this._userDal.ReadUserRoles(id, rd =>
            {
                while (rd.Read())
                {
                    appId = Convert.ToInt32(rd["app_id"]);
                    flag = Convert.ToInt32(rd["flag"]);
                    if (dict.ContainsKey(appId))
                    {
                        dict[appId].Flags.Add(flag);
                    }
                    else
                    {
                        AppRolePair p = new AppRolePair();
                        p.AppId = appId;
                        p.Flags.Add(flag);
                        dict.Add(appId,p);
                    }
                }
            });
            return dict;
        }


        public IAppUserManager GetAppUserManager(int appId)
        {
            return this._creator.CreateAppUserManager(this, appId);
        }


        public void SaveRoleFlags(int userId, int appId, int[] flags)
        {
            this._userDal.CleanUserRoleFlag(userId, appId);
            foreach (int flag in flags)
            {
                this._userDal.SaveUserRole(userId, appId, flag);
            }
        }


        public int GetUserIdByUserName(string userName)
        {
            return this._userDal.GetUserIdByUserName(userName);
        }


        /// <summary>
        /// 删除系统用户并将已删除用户的文档作者改为管理员
        /// </summary>
        /// <returns>返回-1表示未删除成功,0表示未将已删除用户的文档设置超级管理员,否则返回修改数量</returns>
        public int DeleteUser(int userId)
        {
            int result = this._userDal.DeleteUser(userId);
            if (result > 0)
            {
                this._userDal.DeleteRoleBind(userId);
                this._userDal.DeleteCredential(userId);
            }
            return result;
        }
    }
}
