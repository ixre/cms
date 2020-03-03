//
// CommentDAL   会员数据访问
// Copryright 2011 @ TO2.NET,All rights reseved !
// Create by newmin @ 2011/03/13
//

using System;
using System.Data;
using JR.Cms.IDAL;
using JR.DevFw.Data;
using JR.Stand.Core.Framework;

namespace JR.Cms.Dal
{
    /// <summary>
    /// 文档评论数据访问
    /// </summary>
    public sealed class CommentDal : DalBase, ICommentDAL
    {
        public void Add(string archiveID, int memberID, string ip, string content, bool recycle)
        {
            var pa = new object[,]{
                {"@ArchiveId", archiveID},
                {"@MemberId",memberID},
                {"@IP",ip},
                {"@Content", content},
                {"@Recycle", recycle},
                {"@create_time",TimeUtils.Unix()}
                };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteNonQuery(base.NewQuery(DbSql.Comment_AddComment, parameters)
              );
        }

        /// <summary>
        /// 查询文档评论数量
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="recycle"></param>
        /// <returns></returns>
        public int GetArchiveCommentsCount(string archiveID)
        {
            var pa = new object[,]{
                 {"@ArchiveId", archiveID}
                    };
            var parameters = base.Db.CreateParametersFromArray(pa);

            return int.Parse(base.ExecuteScalar(
                base.NewQuery(DbSql.Comment_QueryCommentsCountForArchive, parameters)
                 ).ToString());
        }

        public void Delete(int commentID)
        {
            var pa = new object[,]{
                {"@id", commentID}
                    };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Comment_Delete, parameters)

                );
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
        public DataTable GetArchiveComments(string archiveID, bool desc)
        {
            var pa = new object[,]{
                 {"@ArchiveId", archiveID}
                };
            var parameters = base.Db.CreateParametersFromArray(pa);

            return base.GetDataSet(base.NewQuery(DbSql.Comment_GetCommentsForArchive +
                (desc ? " ORDER BY createdate DESC" : ""), parameters)
              ).Tables[0];
        }


        public DataTable GetArchiveCommentsOrderDesc(string archiveID)
        {
            throw new NotImplementedException();
        }

        public DataTable GetCommentsDetailsTable(string archiveID)
        {
            var pa = new object[,]{
                {"@archiveId",archiveID}};
            var parameters = base.Db.CreateParametersFromArray(pa);

            return base.GetDataSet(
                base.NewQuery(DbSql.Comment_GetCommentDetailsListForArchive, parameters)
                   ).Tables[0];
        }

        public void DeleteMemberComments(int id)
        {
            var pa = new object[,]{
                {"@id", id}
                    };
            var parameters = base.Db.CreateParametersFromArray(pa);
            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Comment_DeleteMemberComments, parameters)
            );
        }

        /// <summary>
        /// 删除文档的评论和回复
        /// </summary>
        /// <param name="archiveId"></param>
        public void DeleteArchiveComments(string archiveID)
        {
            var pa = new object[,]{
                {"@ArchiveId", archiveID}
                    };
            var parameters = base.Db.CreateParametersFromArray(pa);
            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Comment_DeleteArchiveComments, parameters)
                    );
        }


    }
}