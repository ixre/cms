using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Interface.Value;

namespace JR.Cms.Domain.Implement.User
{
    public class User:IUser
    {
        private readonly IUserRepository _userRepository;
        private Credential _credential;
        private AppRoleCollection _roleCol;

        internal User(IUserRepository userRep, int id, int flag)
        {
            this._userRepository = userRep;
            this.Id = id;
            this.Flag = flag;
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


        public int Flag { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastLoginTime { get; set; }

        public bool SubOf(int roleFlag)
        {
            return ( this.Flag & roleFlag) ==roleFlag;
        }

        public int Id { 
            get;
            internal set;
        }


        public int Save()
        {
            return this._userRepository.SaveUser(this);
        }




        public AppRoleCollection GetAppRole()
        {
            if (this._roleCol == null)
            {
                
                IDictionary<int,AppRolePair> roles = this._userRepository.GetUserRoles(this.Id);
                this._roleCol = new AppRoleCollection(this.Id,roles);
            }
            return this._roleCol;
        }


        public void SetRoleFlags(int appId, int[] flags)
        {
            this._userRepository.SaveRoleFlags(this.Id, appId, flags);
        }
    }
}

