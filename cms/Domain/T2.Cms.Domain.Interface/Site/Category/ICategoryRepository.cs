using System.Collections.Generic;
using T2.Cms.Infrastructure;
using T2.Cms.Models;

namespace T2.Cms.Domain.Interface.Site.Category
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

        int GetCategoryLftById(int siteId, int id);

        IList<ICategory> GetCategories(int siteId);

        IEnumerable<ICategory> GetCategories(int siteId, int catId, CategoryContainerOption option);

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


        /// <summary>
        /// 获取子栏目
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<ICategory> GetNextLevelChilds(ICategory category);

        void SaveCategorySortNumber(int id, int sortNumber);
        int GetMaxSortNumber(int siteId);
        bool CheckTagMatch(string tag, int siteId, int iD);
    }
}
