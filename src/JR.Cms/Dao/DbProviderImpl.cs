using System.Data;
using JR.Cms.Library.DataAccess.DAL;
using JR.Cms.Library.DataAccess.DB;
using JR.Stand.Core.Data.Provider;

namespace JR.Cms.Dao
{
    /// <summary>
    /// 
    /// </summary>
    public class DbProviderImpl :IDbProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            return CmsDataBase.Instance.GetDialect().GetConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string FormatQuery(string query)
        {
            return SqlQueryHelper.SqlFormat(query);
        }
    }
}