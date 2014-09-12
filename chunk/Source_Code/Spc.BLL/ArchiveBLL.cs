//
// ArchiveBLLL.cs   会员逻辑层
// Copryright 2011 @ OPS Inc,All rights reseved !
// Create by newmin @ 2011/03/18
//
namespace Spc.BLL
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Text;
    using System.Data;
    using System.Data.Extensions;
    using Spc.DAL;
    using Spc.Models;
    using Spc.IDAL;
    using Spc;
    using Spc.Logic;
    using Ops.Cms.Domain.Interface.Archive;
    using Ops.Cms;
    using Ops.Cms.Infrastructure;
    using Ops.Cms.Domain.Interface.Site.Template;


    /// <summary>
    /// 文档逻辑
    /// </summary>
    public sealed class ArchiveBLL:IArchiveModel
    {

        private static ArchiveDAL _dal;


        private static ArchiveDAL dal
        {
        	get{return _dal??(_dal=new ArchiveDAL());}
        }
        

        /// <summary>
        /// 创建ID
        /// </summary>
        /// <returns></returns>
        private string GenerateID(int length)
        {
            try
            {
                System.Threading.Thread.Sleep(100);
            }
            catch
            {
            }
            char[] words = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            Random rd = new Random();
            StringBuilder sb = new StringBuilder(length);

            int max = words.Length;

            for (int i = 0; i < length; i++)
            {
                sb.Append(words[rd.Next(0, max)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 创建新的文档
        /// </summary>
        /// <param name="archive">返回文档的ID</param>
        /// <returns></returns>
        //public string Create(Archive archive)
        //{
        //    string id;
        //    do
        //    {
        //        id = GenerateID(5);               //创建5位ID
        //    } while (dal.CheckIdIsExist(id));


        //    if (archive.Thumbnail == null)
        //    {
        //        archive.Thumbnail = "";
        //    }

        //    dal.Add(id, archive.Alias, archive.Cid, archive.Author, archive.Title,
        //        archive.Source, archive.Thumbnail, archive.Outline, archive.Content, archive.Tags, archive.Flags);

        //    return id;
        //}


        ///// <summary>
        ///// 更新文档
        ///// </summary>
        ///// <param name="archive"></param>
        //public void Update(Archive archive)
        //{
        //    if (archive.Thumbnail == null)// || archive.Thumbnail.IndexOf(CmsVariables.Archive_NoPhoto) != -1)
        //    {
        //        archive.Thumbnail = "";
        //    }
        //    dal.Update(archive.ID, archive.Cid, archive.Title,archive.Alias,
        //        archive.Source,archive.Thumbnail, archive.Outline, archive.Content ?? "", archive.Tags,archive.Flags);
        //}

        /// <summary>
        /// 添加浏览次数
        /// </summary>
        /// <param name="archiveID"></param>
        /// <param name="count"></param>
        public void AddViewCount(string archiveID, int count)
        {
            dal.AddViewCount(archiveID, count);
        }

        /// <summary>
        /// 别名是否存在
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public bool AliasIsExist(int siteID,string alias)
        {
            return dal.CheckAliasIsExist(siteID,alias);
        }

        /// <summary>
        /// 获取指定栏目第一篇特殊文档
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        //public Archive GetFirstSpecialArchive(int categoryID)
        //{
        //    Archive a = null;
        //    dal.GetFirstSpecialArchive(categoryID, rd =>
        //    {
        //        a = rd.ToEntity<Archive>();
        //    });
        //    return a;
        //}

        /// <summary>
        /// 获取指定栏目的第一篇文档
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <returns></returns>
        //public Archive GetFirstSpecialArchive(string categoryTag)
        //{
        //    Archive archive = null;
        //    //Category category = cbll.Get(a => a.Tag == categoryTag);
        //    //if (category == null) throw new ArgumentNullException("不存在标签为" + categoryTag + "的栏目");

        //    dal.GetFirstSpecialArchive(category.ID, rd =>
        //    {
        //        archive = rd.ToEntity<Archive>();
        //    });
        //    return archive;
        //}

        /// <summary>
        /// 根据ID或别名获取文档(多站点)
        /// </summary>
        /// <returns></returns>
        //public Archive Get(int siteID,string idOrAlias)
        //{
        //    Archive a = null;
        //    dal.GetArchive(siteID,idOrAlias, rd =>
        //    {
        //        if (rd.HasRows)
        //        {
        //            a = rd.ToEntity<Archive>();
        //        }
        //    });
        //    return a;
        //}

        

        #region  获取文档

        /*
        /// <summary>
        /// 获取文档，不包括子类
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public DataTable GetArchives(string categoryTag, int number)
        {
           return dal.GetArchives(categoryTag, number);
        }
         */


        /*
        public DataTable GetArchives(int categoryID, int number, bool containerChild)
        {
            return containerChild ? this.GetArchives(categoryID, number) 
                : this.GetArchives(cbll.Get(a => a.ID == categoryID).Tag, number);

            //
            //TODO:下面方法
            //
               // dal.GetArchivesContainerChild(categoryID, number) :
        }*/

        #endregion


        #region 搜索相关
        //public DataTable Search(int siteID,string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount,string orderby)
        //{
        //    return dal.SearchArchives(siteID,keyword, pageSize, currentPageIndex, out recordCount, out pageCount, orderby);
        //}
        //public DataTable SearchByCategory(int siteId,int categoryLft,int categoryRgt,string keyword,int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby)
        //{
        //    return dal.SearchArchives(siteId,categoryLft,categoryRgt,keyword, pageSize, currentPageIndex, out recordCount, out pageCount, orderby);
        //}

        public DataTable SearchByModule(string keyword, int moduleID, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby)
        {
            return dal.SearchByModule(moduleID,keyword, pageSize, currentPageIndex, out recordCount, out pageCount, orderby);
        }
        #endregion


        /// <summary>
        /// 删除会员的文章
        /// </summary>
        /// <param name="id"></param>
        public void DeleteMemberArchive(int id)
        {
            dal.DeleteMemberArchives(id);
        }

        /// <summary>
        /// 将作者设为新作者,并返回更新多少行
        /// </summary>
        /// <param name="username"></param>
        /// <param name="anotherUsername"></param>
        public int TransferAuthor(string username, string anotherUsername)
        {
           return dal.TransferAuthor(username, anotherUsername);
        }

        /// <summary>
        /// 获取文档评论及评论会员信息的JSON格式数据
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        public string GetCommentDetailsJSON(string archiveID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataTable dt = new CommentDAL().GetCommentsDetailsTable(archiveID);
            int i=dt.Rows.Count;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("{member:{id:").Append(dr["uid"]).Append(",avatar:'")
                    .Append(String.IsNullOrEmpty(dr["avatar"].ToString()) ? "/images/noavatar.gif" : dr["avatar"].ToString())
                    .Append("',nick:'").Append(dr["nickname"].ToString())
                    .Append("'},comment:{id:").Append(dr["cid"].ToString()).Append(",txt:'").Append(dr["content"].ToString())
                    .Append("',time:'").Append(dr["createdate"].ToString()).Append("'}}");
                if (dr != dt.Rows[i - 1]) sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }




        /*

        /// <summary>
        /// 获取特殊文章,包括子类
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public DataTable GetSpecialArchives(int categoryID, int number)
        {

            Category c = cbll.Get(a => a.ID == categoryID);
            return dal.GetSpecialArchives(categoryID, c.Lft, c.Rgt, number);
        }

        /// <summary>
        /// 获取特殊文章,不包括子类
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public DataTable GetSpecialArchives(string categoryTag, int number)
        {
            return dal.GetSpecialArchives(categoryTag, number);
        }

        public DataTable GetSpecialArchivesByModuleID(int moduleID, int number)
        {
            return dal.GetSpecialArchivesByModuleID(moduleID, number);
        }
        */



        /*
        public DataTable GetArchivesByModuleID(int siteID,int moduleID, int number)
        {
            return dal.GetArchivesByModuleID(siteID,moduleID, number);
        }
        */

        public DataTable GetAllArchives()
        {
            return dal.GetAllArchives();
        }

        /// <summary>
        /// 获取文档
        /// </summary>
        /// <param name="sqlcondition">sql条件语句如：[categoryid]=5</param>
        /// <returns></returns>
        public DataTable GetArchives(string sqlcondition)
        {
            return dal.GetArchives(sqlcondition);
        }

        /*
        public DataTable GetArchivesByViewCountByModuleID(int moduleID, int number)
        {
            return dal.GetArchivesByViewCountAndModuleID(moduleID, number);
        }

        public DataTable GetArchivesByViewCount(int categoryID, int number)
        {
            Category c = cbll.Get(a => a.ID == categoryID);
            return dal.GetArchivesByViewCount(categoryID, c.Lft, c.Rgt, number);
        }

        public DataTable GetArchivesByViewCount(string categoryTag, int number)
        {
            return dal.GetArchivesByViewCount(categoryTag, number);
        }
         */

        //public bool Delete(string archiveID)
        //{
        //    Archive a = Get(archiveID);
        //    if (a != null && !ArchiveFlag.GetFlag(a.Flags,BuiltInArchiveFlags.IsSystem))
        //    {
        //        //删除文章
        //        new ArchiveDAL().Delete(archiveID);

        //        //删除评论
        //        new CommentDAL().DeleteArchiveComments(archiveID);

        //        //删除点评
        //        new CommentBLL().DeleteArchiveReviews(archiveID);

        //        //删除视图设置
        //        new TemplateBindBLL().RemoveBind(TemplateBindType.ArchiveTemplate, archiveID);

        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// 获取栏目文档列表
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        //public DataTable GetPagedArchives(int categoryID, int pageSize, ref int currentPageIndex, out int recordCount, out int pages)
        //{
        //    Category c = cbll.Get(a => a.ID == categoryID);
        //    return dal.GetPagedArchives(categoryID, c.Lft, c.Rgt, pageSize, ref currentPageIndex, out recordCount, out pages);
        //}


        //public DataTable GetPagedArchives(int? siteid, int? moduleID, int? categoryID, string author, string[,] flags, string orderByField, bool orderASC, int pageSize, int currentPageIndex, out int recordCount, out int pages)
        //{
        //    int _lft = -1, _rgt = -1;
        //    if (categoryID.HasValue && categoryID.Value > 0)
        //    {
        //        //siteid =null;
        //        moduleID = null;

        //        Category c = cbll.Get(a => a.ID == categoryID.Value);

        //        //栏目不存在
        //        if (c == null)
        //        {
        //            recordCount = 0;
        //            pages = 0;
        //            return new DataTable();
        //        }
        //        _lft = c.Lft;
        //        _rgt = c.Rgt;
        //    }

        //    return dal.GetPagedArchives(siteid, moduleID, _lft, _rgt, author, flags, orderByField, orderASC, pageSize, currentPageIndex, out recordCount, out pages);
        //}



        #region TEST

        //public string __TESTID__()
        //{
        //    StringBuilder sb = new StringBuilder();

            
        //    string[] ids = new string[30];
        //    for (var i = 0; i < ids.Length; i++)
        //    {
        //        ids[i] = Create(null);
        //    }

        //    for (int i = 0; i < ids.Length; i++)
        //    {
        //        sb.Append(ids[i])
        //            .Append(Array.Exists(ids, a =>String.Compare(a,ids[i],true)==0)? "[repeat]" : "")
        //            .Append("<br />");
        //    }/*
        //    sb.Append(GenerateID(5)).Append("<br />"); sb.Append(GenerateID(5)).Append("<br />");
        //    sb.Append(GenerateID(5)).Append("<br />");*/

        //    return sb.ToString();
        //}

        #endregion




        public int GetArchiveAutoID(string id)
        {
            return dal.GetArchiveAutoID(id);
        }


    }
}