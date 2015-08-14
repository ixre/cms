//
// ReviewsDAL   点评(赞同和反对)访问层
// Copryright 2011 @ Z3Q.NET,All rights reseved !
// Create by newmin @ 2011/03/13
//

using System;
using J6.Cms.IDAL;
using J6.DevFw.Data;

namespace J6.Cms.Dal
{
    /// <summary>
    /// 友情链接数据访问
    /// </summary>
    public sealed class ReviewsDAL:DalBase,IRenewsDAL
    {
        /// <summary>
        /// 创建点评数据
        /// </summary>
        /// <param name="archive"></param>
        public void CreateReviews(string id)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Reviews_Create),
                    new object[,]{
                {"@id",id}
                    })
                );
        }

        /// <summary>
        /// 获取已经点评的用户
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="agree"></param>
        public string GetReviewsMembers(string id)
        {
            return base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.Reviews_GetMember),
                    new object[,]{
                {"@id", id}
                    })) as String;
        }


        /// <summary>
        /// 更新评价
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="num">正数为同意,负数为反对</param>
        /// <param name="agree"></param>
        public void UpdateEvaluate(string archiveId, bool agree)
        {
            string sql = agree ?
                base.OptimizeSql(DbSql.Reviews_UpdateEvaluate_Agree) :
               base.OptimizeSql(DbSql.Reviews_UpdateEvaluate_Disagree);

            base.ExecuteNonQuery( new SqlQuery(sql,new object[,]{{"@id",archiveId}}));
        }

        /// <summary>
        /// 更新点评
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="agree"></param>
        /// <param name="members"></param>
        public void UpdateReviews(string id,string members)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Reviews_UpdateReviews),
                   new object[,]{
                {"@Members", members},
                {"@id",id}
                   }));
        }

        public void DeleteReviews(string id)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Reviews_Delete),
                    new object[,]{
                {"@id", id}
                    }));
        }

    }
}