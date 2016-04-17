
using J6.Cms.Domain.Interface.User;

namespace J6.Cms.Domain.Implement.User
{
    public  class UserCreator
    {

        public IUser CreateUser(IUserRepository rep, int id,int flag)
        {
            return new User(rep,id,flag);
        }
        
        public IAppUserManager CreateAppUserManager(IUserRepository rep, int appId)
        {
            return new AppUserManager(rep, appId);
        }
    }
}

