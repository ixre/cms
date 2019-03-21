using System.Data;
using JR.Cms.Dal;
using JR.Cms.IDAL;

namespace JR.Cms.ServiceRepository.Query
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

       public int GetFirstUserId()
       {
           return this._dal.GetMinUserId();
       }
    }
}
