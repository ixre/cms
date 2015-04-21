//
// CommentDAL   会员数据访问
// Copryright 2011 @ S1N1.COM,All rights reseved !
// Create by newmin @ 2011/03/13
//

using System;
using System.Data;
using AtNet.Cms.IDAL;
using AtNet.DevFw.Data;

namespace AtNet.Cms.DAL
{
    /// <summary>
    /// 文档评论数据访问
    /// </summary>
    public sealed class CommentDAL:DALBase,ICommentDAL
    {
        public void Add(string archiveID,int memberID,string ip,string content,bool recycle)
        {
            base.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(DbSql.Comment_AddComment),
                new object[,]{
                {"@ArchiveId", archiveID},
                {"@MemberId",memberID},
                {"@IP",ip},
                {"@Content", content},
                {"@Recycle", recycle},
                {"@CreateDate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
                }));
        }

        /// <summary>
        /// 查询文档评论数量
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="recycle"></param>
        /// <returns></returns>
        public int GetArchiveCommentsCount(string archiveID)
        {
            return int.Parse(base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.Comment_QueryCommentsCountForArchive),
                    new object[,]{
                 {"@ArchiveId", archiveID}
                    })
                 ).ToString());
        }

        public void Delete(int commentID)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Comment_Delete),
                    new object[,]{
                {"@id", commentID}
                    })
                );
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
        public DataTable GetArchiveComments(string archiveID,bool desc)
        {
            return base.GetDataSet(new SqlQuery(base.OptimizeSql(DbSql.Comment_GetCommentsForArchive) + (desc ? " ORDER BY createdate DESC" : ""),
                new object[,]{
                 {"@ArchiveId", archiveID}
                })).Tables[0];
        }


        public DataTable GetArchiveCommentsOrderDesc(string archiveID)
        {
            throw new NotImplementedException();
        }

        public DataTable GetCommentsDetailsTable(string archiveID)
        {
            return base.GetDataSet(
                new SqlQuery(base.OptimizeSql(DbSql.Comment_GetCommentDetailsListForArchive),
                    new object[,]{
                {"@archiveId",archiveID}})).Tables[0];
        }

        public void DeleteMemberComments(int id)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Comment_DeleteMemberComments),
                    new object[,]{
                {"@id", id}
                    })
            );
        }

        /// <summary>
        /// 删除文档的评论和回复
        /// </summary>
        /// <param name="archiveId"></param>
        public void DeleteArchiveComments(string archiveID)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Comment_DeleteArchiveComments), 
                    new object[,]{
                {"@ArchiveId", archiveID}
                    }));
        }


    }
}