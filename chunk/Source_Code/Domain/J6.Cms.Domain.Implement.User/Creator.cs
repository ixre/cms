using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J6.Cms.Domain.Interface.User;

namespace J6.Cms.Domain.Implement.User
{
    public  class UserCreator
    {

        public IUser CreateUser(IUserRepository rep, int id,int flag)
        {
            return new User(rep,id,flag);
        }
        
        public IAppRoleManager CreateAppRoleManager(IUserRepository rep, int appId)
        {
            return new AppRoleManager(rep, appId);
        }
    }
}
