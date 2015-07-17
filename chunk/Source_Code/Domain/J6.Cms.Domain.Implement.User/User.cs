using System;
using System.Collections.Generic;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Interface.Value;

namespace J6.Cms.Domain.Implement.User
{
    public class User:IUser
    {
        private readonly IUserRepository _userRepository;
        private Credential _credential;
        private IList<AppRoleBind> _roles;

        internal User(IUserRepository userRep, int id, int flag)
        {
            this._userRepository = userRep;
            this.Id = id;
            this.RoleFlag = flag;
        }

        public Credential GetCredential()
        {
            if (this._credential == null)
            {
                this._credential = this._userRepository.GetUserCredential(this.Id);
            }
            return this._credential;
        }


        /// <summary>
        /// 保存用户凭据
        /// </summary>
        /// <param name="c"></param>
       public int SaveCredential(Credential c)
        {
            this._credential = c;
          return  this._userRepository.SaveCredential(c);
        }

        public string Name { get; set; }



        public string Avatar { get; set; }


        public string CheckCode { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }


        public int RoleFlag { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastLoginTime { get; set; }

        public bool SubOf(int roleFlag)
        {
            return ( this.RoleFlag & roleFlag) ==roleFlag;
        }

        public int Id { 
            get;
            internal set;
        }


        public int Save()
        {
            return this._userRepository.SaveUser(this);
        }



        public IList<AppRoleBind> GetRoles()
        {
            if (this._roles == null)
            {
                //todo:
                this._roles = this._userRepository.GetUserRoles(this.Id);
                foreach (var v in this._roles)
                {
                    if (v.Flag == Role.Master.Flag)
                    {
                        v.Name = Role.Master.Name;
                    }
                    else if (v.Flag == Role.Publisher.Flag)
                    {
                        v.Name = Role.Publisher.Name;
                    }
                    else if (v.Flag == Role.SiteOwner.Flag)
                    {
                        v.Name = Role.SiteOwner.Name;
                    }
                }
            }
            return this._roles;
        }


        public void SetRoleFlags(int appId, int[] flags)
        {
            this._userRepository.SaveRoleFlags(this.Id, appId, flags);
        }
    }
}

