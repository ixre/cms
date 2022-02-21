using System.Collections.Generic;
using System.Data;
using Dapper;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using JR.Cms.Library.DataAccess.DAL;
using JR.Cms.Library.DataAccess.DB;

namespace JR.Cms.Dao.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteTagDaoImpl:ISiteTagDao
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SiteTag> GetTags()
        {
            using (IDbConnection db = CmsDataBase.Instance.GetDialect().GetConnection())
            {
                return db.Query<SiteTag>(SqlQueryHelper.SqlFormat("SELECT * FROM $PREFIX_site_tag")).AsList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Error SaveTag(SiteTag tag)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Error DeleteTag(SiteTag tag)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public SiteTagDaoImpl()
        {
        }
    }
}