using T2.Cms.DataTransfer;
using System.Collections.Generic;
using System.Data;

namespace T2.Cms.ServiceContract
{
    public interface IArchiveServiceContract
    { 
        /// <summary>
        /// 根据编号获取文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        ArchiveDto GetArchiveById(int siteId, int archiveId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        ArchiveDto GetArchiveByIdOrAlias(int siteId, string id);

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="siteId">站点编号</param>
        /// <param name="catId">栏目编号</param>
        /// <param name="archive">文档</param>
        /// <returns></returns>
        Result SaveArchive(int siteId,int catId, ArchiveDto archive);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        ArchiveDto GetSameCategoryPreviousArchive(int siteId, int archiveId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        ArchiveDto GetSameCategoryNextArchive(int siteId, int archiveId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        ArchiveDto[] GetArchivesByCategoryPath(int siteId, string catPath,bool includeChild, int number, int skipSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        ArchiveDto[] GetArchivesByModuleId(int siteId, int moduleId, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        ArchiveDto[] GetArchivesByViewCount(int siteId, string catPath, bool includeChild, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        ArchiveDto[] GetSpecialArchivesByModuleId(int siteId, int moduleId, int number);
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryTag"></param>
        /// <param name="number"></param>
        /// <param name="skipSize">跳过的条数</param>
        /// <returns></returns>
        ArchiveDto[] GetSpecialArchives(int siteId, string catPath, bool includeChild, int number, int skipSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        ArchiveDto[] GetArchivesByViewCountByModuleId(int siteId, int moduleId, int number);

        /// <summary>
        /// 获取分页文档信息数据表(后台)
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="publisherId"></param>
        /// <param name="includeChild"></param>
        /// <param name="flag"></param>
        /// <param name="keyword"></param>
        /// <param name="orderByField"></param>
        /// <param name="orderAsc"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        DataTable GetPagedArchives(int siteId, int? categoryId,
            int publisherId, bool includeChild, int flag, string keyword,
            string orderByField, bool orderAsc, int pageSize, 
            int currentPageIndex, out int recordCount, out int pages);


        /// <summary>
        /// 获取分页文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <param name="pageSize"></param>
        /// <param name="skipSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="records"></param>
        /// <param name="pages"></param>
        /// <param name="extendValues"></param>
        /// <returns></returns>
        DataTable GetPagedArchives(int siteId,
            string catPath,int pageSize, int skipSize,ref int pageIndex,
            out int records, out int pages,
            out IDictionary<int, IDictionary<string, string>> extendValues);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        void DeleteArchive(int siteId, int archiveId);

        /// <summary>
        /// 刷新文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        void RefreshArchive(int siteId, int archiveId);

        /// <summary>
        /// 根据分类搜索文章
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryLft"></param>
        /// <param name="categoryRgt"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="records"></param>
        /// <param name="pages"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IEnumerable<ArchiveDto> SearchArchivesByCategory(int siteId,
                string catPath,
            string keyword, int pageSize,
            int pageIndex, out int records,
            out int pages, string orderBy);

        /// <summary>
        /// 搜索文档
        /// </summary>
        /// <param name="siteId"></param>
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
        IEnumerable<ArchiveDto> SearchArchives(int siteId,string catPath, bool onlyMatchTitle,
            string keyword, int pageSize, int pageIndex, out int records, out int pages, string orderBy);

        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        bool CheckArchiveAliasAvailable(int siteId, int archiveId, string alias);

        /// <summary>
        /// 为文档添加浏览次数
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="count"></param>
        void AddCountForArchive(int siteId, int id, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        IEnumerable<ArchiveDto> GetRelatedArchives(int siteId, int contentId);

        /// <summary>
        /// 移动排序
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="direction"></param>
        void MoveSortNumber(int siteId, int id, int direction);

        void BatchDelete(int siteId, int[] idArray);
    }
}
