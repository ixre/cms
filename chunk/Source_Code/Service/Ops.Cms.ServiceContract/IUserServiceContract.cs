using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtNet.Cms.DataTransfer;
using AtNet.Cms.Domain.Interface.Value;

namespace AtNet.Cms.ServiceContract
{
    public interface IUserServiceContract
    {
        LoginResultDto TryLogin(string username, string password);
        UserDto GetUser(int id);
        Credential GetCredentialByUserName(string username);
    }
}
