using System.Collections.Generic;
using T2.Cms.Models;

namespace T2.Cms.Domain.Interface.Site.Category
{
    public interface ICategoryRepository
    {
        ICategory CreateCategory(CmsCategoryEntity value);

        int SaveCategory(ICategory category);

        //ICategory GetCategoryById(int siteId,int categoryId);

        ICategory GetCategoryById(int categoryId);

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
        /// <param name="tag"></param>
        /// <returns></returns>
        int GetCategoryLftByTag(int siteId, string tag);

        int GetCategoryLftById(int siteId, int id);

        IList<ICategory> GetCategories(int siteId);

        IEnumerable<ICategory> GetCategories(int siteId, int lft, int rgt, CategoryContainerOption option);

        IEnumerable<ICategory> GetChilds(ICategory category);

        ICategory GetParent(ICategory category);

        ICategory GetNext(ICategory category);

        ICategory GetPrevious(ICategory category);

        ICategory GetCategoryByLft(int siteId, int lft);

        /// <summary>
        /// 获取分类的文档数量
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <returns></returns>
        int GetArchiveCount(int siteId, int lft, int rgt);

        void DeleteCategory(int siteId, int lft,int rgt);


        IEnumerable<ICategory> GetNextLevelChilds(ICategory category);
        void SaveCategorySortNumber(int id, int sortNumber);
        int GetMaxSortNumber(int siteId);
        bool CheckTagMatch(string tag, int siteId, int iD);
    }
}
