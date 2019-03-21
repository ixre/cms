//
// CommentBLL.cs   会员数据访问
// Copryright 2011 @ TO2.NET,All rights reseved !
// Create by newmin @ 2011/03/13
//

using System;
using System.Data;
using JR.Cms.Dal;
using JR.Cms.Domain.Interface._old;
using JR.Cms.IDAL;
using JR.DevFw.Framework.Extensions;

namespace JR.Cms.BLL
{
    /// <summary>
    /// 文档评论
    /// </summary>
    public sealed class CommentBll : IComment
    {


        private static ICommentDAL _dal;
        private static IRenewsDAL _renewDal;
        private static ArchiveDal archiveDal = new ArchiveDal();


        
        private static ICommentDAL dal
        {
        	get{return _dal??(_dal=new CommentDal());}
        }
 		private static IRenewsDAL renewDal
        {
 			get{return _renewDal??(_renewDal=new ReviewsDAL());}
        }

 			
        /*
        public static void Add(Comment entity)
        {
            //CommentDAL.Add(entity);
        }
        public static void Delete(Comment entity)
        {
            CommentDAL.Delete(entity);
        }
        public static void Update(Comment entity)
        {
            CommentDAL.Update(entity);
        }*/


        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public DataTable GetArchiveComments(string archiveId,bool desc)
        {
            return dal.GetArchiveComments(archiveId,desc);
        }


        /// <summary>
        /// 创建ID
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="agree"></param>
        /// <returns></returns>
        private string CreateReviewsID(string archiveId, bool agree)
        {
            return (archiveId + (agree ? "agree" : "disagree")).Encode16MD5();
        }


        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="memberId"></param>
        /// <param name="ip"></param>
        /// <param name="content"></param>
        public void InsertComment(string archiveId, int memberId,string ip, string content)
        {
            bool isExists=false;
            archiveDal.GetArchiveByPath(-1,archiveId,rd=>{
                if(rd.HasRows)isExists=true;
            });

            if (!isExists)
            {
                dal.Add(archiveId, memberId, ip, content, false);
            }
        }

        /// <summary>
        /// 获取评论数
        /// </summary>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        public int GetArchiveCommentsCount(string archiveId)
        {
            return dal.GetArchiveCommentsCount(archiveId);
        }


        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId"></param>
        public void DeleteComment(int commentId)
        {
            dal.Delete(commentId);
        }


        /// <summary>
        /// 删除会员的评论
        /// </summary>
        /// <param name="memberId"></param>
        public void DeleteMemberComments(int memberId)
        {
            dal.DeleteMemberComments(memberId);
        }


        /// <summary>
        /// 提交点评
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="member"></param>
        /// <param name="memberId"></param>
        /// <param name="agree"></param>
        /// <param name="archiveId"></param>
        /// <returns>如果已经点评过则返回false</returns>
        public bool SubmitReviews(string archiveId,int memberId, bool agree)
        {
            string id = CreateReviewsID(archiveId, agree);
            string members = renewDal.GetReviewsMembers(id);

            //如果没有数据的话，则创建
            if (members == null)
            {
                renewDal.CreateReviews(id);
            }
            //如果已经评论
            if (members != null && members.IndexOf(memberId.ToString()) != -1) return false;


            //给文章加上点评计数
            renewDal.UpdateEvaluate(archiveId, agree);

            //记录点评过的用户
            renewDal.UpdateReviews(id, String.IsNullOrEmpty(members) ? memberId.ToString() : members + "," + memberId.ToString());

            return true;
        }

        /// <summary>
        /// 删除文档的点评信息
        /// </summary>
        /// <param name="archiveId"></param>
        public void DeleteArchiveReviews(string archiveId)
        {
            renewDal.DeleteReviews(CreateReviewsID(archiveId, true));

            renewDal.DeleteReviews(CreateReviewsID(archiveId, false));
        }
    }
}