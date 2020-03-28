//
// ReviewsDAL   点评(赞同和反对)访问层
// Copryright 2011 @ TO2.NET,All rights reseved !
// Create by newmin @ 2011/03/13
//

using System;
using JR.Cms.Library.DataAccess.IDAL;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    /// <summary>
    /// 友情链接数据访问
    /// </summary>
    public sealed class ReviewsDAL : DalBase, IRenewsDAL
    {
        /// <summary>
        /// 创建点评数据
        /// </summary>
        /// <param name="archive"></param>
        public void CreateReviews(string id)
        {
            ExecuteNonQuery(
                NewQuery(DbSql.Reviews_Create,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", id}
                        }))
            );
        }

        /// <summary>
        /// 获取已经点评的用户
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="agree"></param>
        public string GetReviewsMembers(string id)
        {
            return ExecuteScalar(
                NewQuery(DbSql.Reviews_GetMember,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", id}
                        }))) as string;
        }


        /// <summary>
        /// 更新评价
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="num">正数为同意,负数为反对</param>
        /// <param name="agree"></param>
        public void UpdateEvaluate(string archiveId, bool agree)
        {
            var sql = agree
                ? OptimizeSql(DbSql.Reviews_UpdateEvaluate_Agree)
                : OptimizeSql(DbSql.Reviews_UpdateEvaluate_Disagree);

            ExecuteNonQuery(new SqlQuery(sql, new object[,] {{"@id", archiveId}}));
        }

        /// <summary>
        /// 更新点评
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="agree"></param>
        /// <param name="members"></param>
        public void UpdateReviews(string id, string members)
        {
            ExecuteNonQuery(
                NewQuery(DbSql.Reviews_UpdateReviews,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@Members", members},
                            {"@id", id}
                        })));
        }

        public void DeleteReviews(string id)
        {
            ExecuteNonQuery(
                NewQuery(DbSql.Reviews_Delete,
                    Db.CreateParametersFromArray(
                        new object[,]
                        {
                            {"@id", id}
                        })));
        }
    }
}