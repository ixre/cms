using System.Data;
using JR.Cms.Library.DataAccess.DAL;
using JR.Cms.Library.DataAccess.IDAL;

namespace JR.Cms.Repository.Query
{
    public class UserQuery
    {
        private readonly IUserDal _dal = new UserDal();

        public DataTable GetMyUserTable(int appId, int userId)
        {
            return _dal.GetMyUserTable(appId, userId);
        }

        public DataTable GetAllUser()
        {
            return _dal.GetAllUser();
        }

        public string GetUserRealName(int userId)
        {
            return _dal.GetUserRealName(userId);
        }

        public int GetFirstUserId()
        {
            return _dal.GetMinUserId();
        }
    }
}