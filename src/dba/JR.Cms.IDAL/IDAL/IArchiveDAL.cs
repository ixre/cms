//
//
//  Copyright 2011 (C) OPSoft INC.All rights reseved.
//
//  Project : OPSite
//  File Name : category.cs
//  Date : 2011/8/21
//  Author : 
//
//

namespace Spc.IDAL
{

    using System.Data;
    using J6.Data;

    public interface IArchiveModelDAL
    {
        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryID"></param>
        /// <param name="author"></param>
        /// <param name="title"></param>
        /// <param name="source"></param>
        /// <param name="outline"></param>
        /// <param name="content"></param>
        /// <param name="tags"></param>
        /// <param name="isSpecial"></param>
        /// <param name="isSystem"></param>
        /// <returns></returns>
        bool Add(string id,string alias,int categoryID, string author, string title,string source,string thumbnail, string outline, string content, string tags,string flags);

        /// <summary>
        /// 更新文档
        /// </summary>
        void Update(string id, int categoryID, string title, string alias, string source,string thumbnail, string outline, string content, string tags,string flags);

        /// <summary>
        /// 重新发布文档
        /// </summary>
        /// <param name="archiveID"></param>
        void RePublish(string archiveID);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="id"></param>
        void Delete(string id);

        /// <summary>
        /// 检查别名是否存在
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        bool CheckAliasIsExist(int siteID,string alias);


        /// <summary>
        /// 检查ID是否存在
        /// </summary>
        /// <returns></returns>
        bool CheckIdIsExist(string id);

        /// <summary>
        /// 添加查看次数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number">添加数目：默认1</param>
        void AddViewCount(string id, int number);


        #region 获取文档相关

        /// <summary>
        /// 获取文档
        /// </summary>
        /// <param name="archiveIDOrAlias"></param>
        /// <param name="func"></param>
        void GetArchive(string archiveIDOrAlias, DataReaderFunc func);

        /// <summary>
        /// 获取站点的文档
        /// </summary>
        /// <param name="siteID"></param>
        /// <param name="archiveIDOrAlias"></param>
        /// <param name="func"></param>
        void GetArchive(int siteID, string archiveIDOrAlias, DataReaderFunc func);

        /// <summary>
        /// 根据编号获取文档
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        void GetArchiveByID(string id, DataReaderFunc func);

        /// <summary>
        /// 获取所有文档
        /// </summary>
        /// <returns></returns>
        DataTable GetAllArchives();


        /// <summary>
        /// 获取文档
        /// </summary>
        /// <param name="sqlcondition">SQL条件</param>
        /// <returns></returns>
        DataTable GetArchives(string sqlcondition);

        /// <summary>
        /// 获取指定栏目的最新文档
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetArchives(int lft,int rgt, int number);
        
        /// <summary>
        /// 获取指定栏目和数量的文档
        /// </summary>
        DataTable GetArchives(string categoryAlias, int number);

        /// <summary>
        /// 获取指定模块类型的最新文档
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetArchivesByModuleID(int siteID,int moduleID, int number);

        /// <summary>
        /// 获取指定栏目浏览次数最多的档案(包含子类)
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetArchivesByViewCount(int categoryID, int lft,int rgt,int number);

        /// <summary>
        /// 获取指定栏目浏览次数最多的档案
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetArchivesByViewCount(string categoryTag,int number);
        /// <summary>
        /// 获取指定模块浏览次数最多的档案
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetArchivesByViewCountAndModuleID(int moduleID, int number);

        /// <summary>
        /// 获取包括子栏目的文档
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetArchivesContainerChild(int categoryID, int number);

        /// <summary>
        ///  获取指定数量的特殊文档
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetSpecialArchives(int number);

        /// <summary>
        /// 获取指定数量和栏目的特殊文档（包含子类）
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetSpecialArchives(int categoryID, int lft, int rgt, int number);

        /// <summary>
        /// 获取指定数量和栏目的特殊文档
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetSpecialArchives(string categoryTag, int number);

        /// <summary>
        /// 获取指定数量和模块的特殊文章
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        DataTable GetSpecialArchivesByModuleID(int moduleID, int number);

        /// <summary>
        /// 获取指定栏目的第一篇特殊文档
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="func"></param>
        void GetFirstSpecialArchive(int categoryID, DataReaderFunc func);


        /// <summary>
        ///  获取相同栏目的上一篇文档
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        void GetSameCategoryPreviousArchive(string id, DataReaderFunc func);

        /// <summary>
        /// 获取相同栏目的下一篇文档
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        void GetSameCategoryNextArchive(string id, DataReaderFunc func);

        /// <summary>
        /// 获取文档的前几篇
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        /// <param name="func"></param>
       DataTable GetPreviousArchives(string id, int number);

        /// <summary>
        /// 获取文档的后几篇
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        /// <param name="func"></param>
        DataTable GetNextArchives(string id, int number);


        /************* 栏目 *****************/

        /// <summary>
        /// 获取栏目分页文档
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        DataTable GetPagedArchives(int categoryID,int lft,int rgt,int pageSize, ref int currentPageIndex, out int recordCount, out int pages);

        /// <summary>
        /// 获取栏目分页文档
        /// </summary>
        /// <param name="lft">栏目的左值,可为空</param>
        /// <param name="categoryID">指定栏目ID,NULL可以显示所有栏目档案</param>
        /// <param name="author">指定作者，NULL表示不限，可显示所有作者发布的文档</param>
        ///<param name="flags">标签</param>
        /// <returns></returns>
        DataTable GetPagedArchives(int? siteID,int? moduleID,int? lft, int? rgt, string author, string[,] flags, string orderByField, bool orderASC, int pageSize, int currentPageIndex, out int recordCount, out int pages);


        #endregion 
        
        #region 搜索

        /// <summary>
        /// 搜索关键词相关的内容
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        DataTable Search(int siteID,string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby);

         /// <summary>
        /// 在指定栏目下搜索关键词相关的内容
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="categoryID"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        DataTable Search(int siteID,int categoryID,string keyword,int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby);

         /// <summary>
        /// 在指定模块下搜索关键词相关的内容
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        DataTable SearchByModule(int ModuleID, string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby);

        #endregion


        /// <summary>
        /// 删除会员发布的文档
        /// </summary>
        /// <param name="id"></param>
        void DeleteMemberArchives(int id);

        /// <summary>
        /// 将作者设为新作者
        /// </summary>
        /// <param name="username"></param>
        /// <param name="anotherUsername"></param>
        /// <returns>已修改行数</returns>
        int TransferAuthor(string username, string anotherUsername);

        /// <summary>
        /// 获取栏目下的文档数量,多个id用,隔开
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetCategoryArchivesCount(string id);


        int GetArchiveAutoID(string id);
    }
}