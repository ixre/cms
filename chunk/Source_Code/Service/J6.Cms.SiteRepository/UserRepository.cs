using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using J6.Cms.DAL;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Implement.User;
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
            this._userDal = new UserDAL();
        }

        public IList<IRole> GetAppRoles(int appId)
        {
            //todo:  暂时不增加用户组
            return new List<IRole>();
        }

        public Credential GetUserCredential(int userId)
        {
            throw new NotImplementedException();
        }

        public int SaveUser(IUser user)
        {
            return -1;
        }

        public int SaveRole(IRole role)
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
                    user = this.CreateUser(Convert.ToInt32(rd["id"]), rd["role_flag"] == DBNull.Value ? 1 : 
                        Convert.ToInt32(rd["role_flag"]));
                    user.Name = rd["name"].ToString();
                    user.AppId = rd["app_id"] == DBNull.Value ? 0 : Convert.ToInt32(rd["app_id"]);
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


        public Credential GetCredentialByUserName(string username)
        {
            Credential cre = null;
            this._userDal.GetUserCredential(username, rd =>
            {
                if(rd.Read())
                {
                    cre = new Credential(rd.GetInt32(0),rd.GetInt32(1),username,rd.GetString(2),rd.GetInt32(3));
                }
            });
            return cre;
        }
    }
}
