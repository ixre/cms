 using System;
using System.Collections.Generic;
 using System.Data;
 using System.Linq;
 using System.Runtime.Remoting.Messaging;
 using System.Text;
 using J6.Cms.DataTransfer;
 using J6.Cms.Domain.Interface.User;
 using J6.Cms.Domain.Interface.Value;
 using J6.Cms.ServiceContract;
 using J6.Cms.ServiceRepository.Query;

namespace J6.Cms.Service
{
    public class UserService:IUserServiceContract
    {
        private readonly IUserRepository _userRepository;
        private readonly UserQuery _userQuery;

        public UserService(IUserRepository rep)
        {
            this._userRepository = rep;
            this._userQuery = new UserQuery();
        }

        public LoginResultDto TryLogin(string username, string password)
        {
            Credential cre = this._userRepository.GetCredentialByUserName(username);
            if (cre == null)
            {
                return new LoginResultDto {Tag = -1};
            }

            if (cre.Enabled == 0)
            {
                return new LoginResultDto {Tag = -2};
            }
            if (password != cre.Password)
            {
                return new LoginResultDto {Tag = -1};
            }

            //todo: 有无系统的角色和权限

            return new LoginResultDto
            {
                Uid = cre.UserId,
                Tag = 1,
            };
        }


        public UserDto GetUser(int id)
        {
            IUser user = this._userRepository.GetUser(id);
            if (user != null)
            {
                return UserDto.Convert(user);
            }
            return null;
        }


        public Credential GetCredentialByUserName(string username)
        {
            return this._userRepository.GetCredentialByUserName(username);
        }



        public IList<UserDto> GetMyUserTableable(int appId, int id)
        {
            throw new NotImplementedException();
        }


        public DataTable GetMyUserTable(int appId, int userId)
        {
            return this._userQuery.GetMyUserTable(appId, userId);
        }
    }
}
