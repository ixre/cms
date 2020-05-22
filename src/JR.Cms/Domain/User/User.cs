using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Interface.Value;

namespace JR.Cms.Domain.User
{
    public class User : IUser
    {
        private readonly IUserRepository _userRepository;
        private Credential _credential;
        private AppRoleCollection _roleCol;

        internal User(IUserRepository userRep, int id, int flag)
        {
            _userRepository = userRep;
            Id = id;
            Flag = flag;
        }

        public Credential GetCredential()
        {
            if (_credential == null) _credential = _userRepository.GetUserCredential(Id);
            return _credential;
        }


        /// <summary>
        /// 保存用户凭据
        /// </summary>
        /// <param name="c"></param>
        public int SaveCredential(Credential c)
        {
            _credential = c;
            return _userRepository.SaveCredential(c);
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
            return (Flag & roleFlag) == roleFlag;
        }

        public int Id { get; internal set; }


        public int Save()
        {
            return _userRepository.SaveUser(this);
        }


        public AppRoleCollection GetAppRole()
        {
            if (_roleCol == null)
            {
                var roles = _userRepository.GetUserRoles(Id);
                _roleCol = new AppRoleCollection(Id, roles);
            }

            return _roleCol;
        }


        public void SetRoleFlags(int appId, int[] flags)
        {
            _userRepository.SaveRoleFlags(Id, appId, flags);
        }

        public int GetAggregateRootId()
        {
            return Id;
        }
    }
}