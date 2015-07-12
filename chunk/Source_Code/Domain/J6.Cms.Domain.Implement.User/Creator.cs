
using J6.Cms.Domain.Interface.User;

namespace J6.Cms.Domain.Implement.User
{
    public  class UserCreator
    {

        public IUser CreateUser(IUserRepository rep, int id,int flag)
        {
            return new J6.Cms.Domain.Implement.User.User(rep,id,flag);
        }
        
        public IAppRoleManager CreateAppRoleManager(IUserRepository rep, int appId)
        {
            return new AppRoleManager(rep, appId);
        }
    }
}

