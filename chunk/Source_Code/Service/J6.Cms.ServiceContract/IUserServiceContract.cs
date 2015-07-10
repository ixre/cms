using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Value;

namespace J6.Cms.ServiceContract
{
    public interface IUserServiceContract
    {
        LoginResultDto TryLogin(string username, string password);
        UserDto GetUser(int id);
        Credential GetCredentialByUserName(string username);
    }
}
