using System;
using System.Collections.Generic;
using JR.Cms.Infrastructure;

namespace JR.Cms.Domain.Interface.Content.Archive
{
    public interface IArchiveRepository
    {
        /// <summary>
        /// 创建文档
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IArchive CreateArchive(CmsArchiveEntity value);

        /// <summary>
        /// 根据文档编号获取文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        IArchive GetArchiveById(int siteId, int archiveId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IArchive GetArchive(int siteId, string alias);

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        Error SaveArchive(CmsArchiveEntity archive);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesContainChildCategories(int siteId, int[] catIdArray, int number, int skipSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByModuleId(int siteId, int moduleId, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByViewCountByModuleId(int siteId, int moduleId, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetSpecialArchives(int siteId, int[] catIdArray, int number, int skipSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetSpecialArchivesByModuleId(int siteId, int moduleId, int number);

        /// <summary>
        /// 根据点击量获取文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catIdArray"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<IArchive> GetArchivesByViewCount(int siteId, int[] catIdArray, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <param name="sameCategory"></param>
        /// <param name="ingoreSpecial"></param>
        /// <returns></returns>
        IArchive GetNextArchive(int siteId, int archiveId, bool sameCategory, bool ingoreSpecial);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <param name="sameCategory"></param>
        /// <param name="ingoreSpecial"></param>
        /// <returns></returns>
        IArchive GetPreviousArchive(int siteId, int archiveId, bool sameCategory, bool ingoreSpecial);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        bool DeleteArchive(int siteId, int archiveId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        void RepublishArchive(int siteId, int archiveId);

        /// <summary>
        /// 
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
        IEnumerable<IArchive> SearchArchivesByCategory(int siteId,
            string catPath, string keyword, int pageSize,
            int pageIndex, out int records, out int pages, string orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="records"></param>
        /// <param name="pages"></param>
        /// <param name="orderBy"></param>
        /// <param name="categoryLft"></param>
        /// <param name="categoryRgt"></param>
        /// <param name="onlyMatchTitle"></param>
        /// <returns></returns>
        IEnumerable<IArchive> SearchArchives(int siteId, string catPath, bool onlyMatchTitle, string keyword,
            int pageSize, int pageIndex, out int records, out int pages, string orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="count"></param>
        void AddArchiveViewCount(int siteId, int id, int count);


        /// <summary>
        /// 获取最大的排序号码
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        int GetMaxSortNumber(int siteId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archiveId"></param>
        /// <param name="sortNumber"></param>
        void SaveSortNumber(int archiveId, int sortNumber);

        int TransferArchives(int userId, int toUserId);

        /// <summary>
        /// 检查文档字符ID是否存在
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="strId"></param>
        /// <returns></returns>
        bool CheckSidIsExist(int siteId, string strId);

        /// <summary>
        /// 检查文档路径是否存在
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="path"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        bool CheckPathMatch(int siteId, string path, int archiveId);

        /// <summary>
        /// 根据路径获取文档
        /// </summary>
        /// <param name="siteId">站点编号</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        IArchive GetArchiveByPath(int siteId, string path);
    }
}