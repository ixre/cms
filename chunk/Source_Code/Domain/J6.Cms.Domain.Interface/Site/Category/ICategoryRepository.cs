using System.Collections.Generic;

namespace J6.Cms.Domain.Interface.Site.Category
{
    public interface ICategoryRepository
    {
        ICategory CreateCategory(int categoryId,ISite site);

        int SaveCategory(ICategory category);

        //ICategory GetCategoryById(int siteId,int categoryId);

        ICategory GetCategoryById(int categoryId);

        /// <summary>
        /// 根据SiteId和tag获取栏目编号
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        int GetCategoryIdByTag(int siteId, string tag);

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
    }
}
