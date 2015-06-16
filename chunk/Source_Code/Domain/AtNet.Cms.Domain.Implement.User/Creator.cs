using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtNet.Cms.Domain.Interface.User;
using Ops.Cms.Domain.Interface.User;

namespace AtNet.Cms.Domain.Implement.User
{
    public  class UserCreator
    {

        public IUser CreateUser(IUserRepository rep, int id,int flag)
        {
            return new User(rep,id,flag);
        }
        
        public CreateAppRoleManager CreateAppRoleManager(IUserRepository rep, int appId)
        {
            return new AppRoleManager(rep, appId);
        }
    }
}
