using System;
using AtNet.Cms.Domain.Interface.User;
using Ops.Cms.Domain.Interface.User;

namespace AtNet.Cms.Domain.Implement.User
{
    public class User:IUser
    {
        private readonly IUserRepository _userRepository;
        private IUserCredential _credential;

        internal User(IUserRepository userRep, int id,String name, int flag)
        {
            this._userRepository = userRep;
            this.Id = id;
            this.Name = name;
            this.RoleFlag = flag;
        }

        public IUserCredential Credential
        {
            get
            {
                if (this._credential == null)
                {
                    this._credential = this._userRepository.GetUserCredential(this.Id);
                }
                return this._credential;
            }
            set { this._credential = value; }
        }

        public string Name { get; set; }

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
    }
}
