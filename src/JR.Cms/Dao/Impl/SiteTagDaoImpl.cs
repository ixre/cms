using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using JR.Cms.Library.DataAccess.DAL;
using JR.Cms.Library.DataAccess.DB;
using JR.Stand.Core.Data.Provider;

namespace JR.Cms.Dao.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteTagDaoImpl:ISiteTagDao
    {
        private readonly IDbProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        public SiteTagDaoImpl(IDbProvider provider)
        {
            this._provider = provider;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SiteWord> GetTags()
        {
            using (IDbConnection db = _provider.GetConnection())
            {
                return db.Query<SiteWord>(_provider.FormatQuery("SELECT * FROM $PREFIX_site_word")).AsList();
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
                using (IDbConnection db = _provider.GetConnection())
                {
                    Object obj = db.ExecuteScalar(_provider.FormatQuery("SELECT COUNT(1) FROM $PREFIX_site_word WHERE word=@Word AND id<>@Id"), word);
                    if (Convert.ToInt32(obj) > 0)
                    {
                        return new Error("词语已存在");
                    }
                    if (word.Id == 0)
                    {
                        db.Execute(_provider.FormatQuery(
                                "INSERT INTO $PREFIX_site_word(word,url,title) VALUES(@Word,@Url,@Title)"),
                            word);
                        return null;
                    }

                    db.Execute(
                        _provider.FormatQuery(
                            "UPDATE $PREFIX_site_word SET word=@word,url=@Url,title=@Title WHERE id=@Id"),
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
            try
            {
                using (IDbConnection db = _provider.GetConnection())
                {
                    db.Execute(
                        _provider.FormatQuery(
                            "DELETE FROM $PREFIX_site_word WHERE id=@Id"),
                        word);
                }
            }
            catch (Exception ex)
            {
                return new Error((ex.InnerException ?? ex).Message);
            }

            return null;
        }

    }
}