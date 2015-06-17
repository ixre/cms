using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtNet.Cms.DataTransfer;

namespace AtNet.Cms.ServiceContract
{
    public interface IUserServiceContract
    {
        LoginResultDto TryLogin(string username, string password);
    }
}
