using System;
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
        public List<SiteWord> GetTags()
        {
            using (IDbConnection db = CmsDataBase.Instance.GetDialect().GetConnection())
            {
                return db.Query<SiteWord>(SqlQueryHelper.SqlFormat("SELECT * FROM $PREFIX_site_word")).AsList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Error SaveTag(SiteWord word)
        {
            try
            {
                using (IDbConnection db = CmsDataBase.Instance.GetDialect().GetConnection())
                {
                    if (word.Id == 0)
                    {
                        db.Execute(SqlQueryHelper.SqlFormat(
                                "INSERT INTO $PREFIX_site_word(word,url,title) VALUES(@Word,@Url,@Title)"),
                            word);
                        return null;
                    }

                    db.Execute(
                        SqlQueryHelper.SqlFormat(
                            "UPDATE $PREFIX_site_word SET word=@word,url=@Url,title=@Title WHERE id=@ID"),
                        word);
                }
            }
            catch (Exception ex)
            {
                return new Error((ex.InnerException ?? ex).Message);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Error DeleteTag(SiteWord word)
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