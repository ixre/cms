using System.Data;
using J6.Cms.Dal;
using J6.Cms.IDAL;

namespace J6.Cms.ServiceRepository.Query
{
   public class UserQuery
    {
       private readonly  IUserDal _dal  = new UserDal();
       public DataTable GetMyUserTable(int appId, int userId)
       {
           return this._dal.GetMyUserTable(appId, userId);
       }

       public DataTable GetAllUser()
       {
           return this._dal.GetAllUser();
       }

       public string GetUserRealName(int userId)
       {
           return this._dal.GetUserRealName(userId);
       }
    }
}
