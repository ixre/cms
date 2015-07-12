using System;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Domain.Interface.Value;

namespace J6.Cms.Domain.Implement.User
{
    public class User:IUser
    {
        private readonly IUserRepository _userRepository;
        private Credential _credential;

        internal User(IUserRepository userRep, int id, int flag)
        {
            this._userRepository = userRep;
            this.Id = id;
            this.RoleFlag = flag;
        }

        public Credential Credential
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



    }
}

