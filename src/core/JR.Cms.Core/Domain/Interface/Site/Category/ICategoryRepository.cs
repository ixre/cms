using System.Collections.Generic;
using JR.Cms.Infrastructure;
using JR.Cms.Models;

namespace JR.Cms.Domain.Interface.Site.Category
{
    public interface ICategoryRepo
    {
        ICategory CreateCategory(CmsCategoryEntity value);

        Error SaveCategory(CmsCategoryEntity category);

        //ICategory GetCategoryById(int siteId,int categoryId);

        ICategory GetCategory(int siteId,int catId);

        /// <summary>
        /// 获取最大的栏目编号
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        int GetNewCategoryId(int siteId);

        /// <summary>
        /// 根据SiteId和tag获取栏目Left
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        ICategory GetCategoryByPath(int siteId, string path);

        IList<ICategory> GetCategories(int siteId);

        /// <summary>
        /// 获取栏目下所有子栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <returns></returns>
        IEnumerable<ICategory> GetChilds(int siteId, string catPath);

        ICategory GetNext(ICategory category);

        ICategory GetPrevious(ICategory category);
        

        /// <summary>
        /// 获取分类的文档数量
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <returns></returns>
        int GetArchiveCount(int siteId,string catPath);

        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catId"></param>
        /// <param name=""></param>
        void DeleteCategory(int siteId,int catId);


        /// <summary>
        /// 获取子栏目
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<ICategory> GetNextLevelChilds(ICategory category);

        void SaveCategorySortNumber(int id, int sortNumber);
        int GetMaxSortNumber(int siteId);

        /// <summary>
        /// 检查tag是否存在
        /// </summary>
        /// <returns></returns>
        bool CheckTagMatch(int siteId, int parentCatId,string tag,int catId);

        /// <summary>
        /// 更新文档路径前缀
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="oldPath"></param>
        /// <param name="path"></param>
        void ReplaceArchivePath(int siteId, string oldPath, string path);
    }
}
