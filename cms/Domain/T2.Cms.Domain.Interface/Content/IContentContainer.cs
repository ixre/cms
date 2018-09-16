using System.Collections.Generic;
using T2.Cms.Domain.Interface.Content.Archive;

namespace T2.Cms.Domain.Interface.Content
{
    /// <summary>
    /// 内容聚合根
    /// </summary>
    public interface IContentContainer:IAggregateroot
    {
        /// <summary>
        /// 聚合根编号，等于站点编号
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 站点编号
        /// </summary>
        int SiteId { get; }

        IArchive CreateArchive(int id, string strId, int categoryId, string title);

        /// <summary>
        /// 获取文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IArchive GetArchive(string id);


        /// <summary>
        /// 根据文档编号获取文档
        /// </summary>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        IArchive GetArchiveById(int archiveId);

        /// <summary>
        /// 获取栏目下的文档
        /// </summary>
        /// <param name="catPath"></param>
        /// <param name="includeChild">是否包括子栏目</param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByCategoryPath(string catPath, bool includeChild, int number, int skipSize);

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByModuleId(int moduleId, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByViewCount(int lft,int rgt, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByViewCount(string categoryTag, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetSpecialArchivesByModuleId(int moduleId, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetSpecialArchives(int lft, int rgt, int number, int skipSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetSpecialArchives(string categoryTag, int number, int skipSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByViewCountByModuleId(int moduleId, int number);

        /// <summary>
        /// 获取同属的上一篇文章
        /// </summary>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        IArchive GetPreviousSiblingArchive(int archiveId);

        /// <summary>
        /// 获取同属的下一篇文章
        /// </summary>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        IArchive GetNextSiblingArchive(int archiveId);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        bool DeleteArchive(int archiveId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archiveId"></param>
        void RefreshArchive(int archiveId);

        /// <summary>
        /// 根据分类搜索文章
        /// </summary>
        /// <param name="categoryLft"></param>
        /// <param name="categoryRgt"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="records"></param>
        /// <param name="pages"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IEnumerable<IArchive> SearchArchivesByCategory(
            int categoryLft, int categoryRgt,
            string keyword, int pageSize,int pageIndex,
            out int records, out int pages, string orderBy);

        /// <summary>
        /// 搜索文档
        /// </summary>
        /// <param name="categoryRgt"></param>
        /// <param name="onlyMatchTitle"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="records"></param>
        /// <param name="pages"></param>
        /// <param name="orderBy"></param>
        /// <param name="categoryLft"></param>
        /// <returns></returns>
        IEnumerable<IArchive> SearchArchives(
            int categoryLft,int categoryRgt,
             bool onlyMatchTitle,
            string keyword, int pageSize, 
            int pageIndex, out int records,
            out int pages, string orderBy);

        /// <summary>
        /// 为文档添加浏览次数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        void AddCountForArchive(int id, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        IBaseContent GetContent(string typeIndent, int contentId);
    }
}
