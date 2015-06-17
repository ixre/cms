 using System;
using System.Collections.Generic;
using System.Linq;
 using System.Runtime.Remoting.Messaging;
 using System.Text;
 using AtNet.Cms.DataTransfer;
 using AtNet.Cms.Domain.Interface.User;
 using AtNet.Cms.ServiceContract;

namespace AtNet.Cms.Service
{
    public class UserService:IUserServiceContract
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository rep)
        {
            this._userRepository = rep;
        }
        public LoginResultDto TryLogin(string username, string password)
        {
            int uid, tag;

            IUser user = this._userRepository.GetUserByUser(username);
            if (user == null)
            {
                return new LoginResultDto {Tag = -1};
            }

            UserCredential cred;
            if (( cred = user.Credential) != null)
            {
                if (cred.Enabled == 0)
                {
                    return new LoginResultDto {Tag = -2};
                }
                if (password != cred.Password)
                {
                    return new LoginResultDto { Tag = -1 };
                }

                //todo: 有无系统的角色和权限
            }
            return new LoginResultDto
            {
                Uid = user.Id,
                Tag = 1,
            };
        }
     
    }
}
