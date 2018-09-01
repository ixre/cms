using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Models;

namespace T2.Cms.Domain.Implement.Site.Category
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseCategoryRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryRep"></param>
        /// <param name="extendRep"></param>
        /// <param name="tempRep"></param>
        /// <param name="categoryId"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public ICategory CreateCategory(
            ICategoryRepository categoryRep,
             ISiteRepository siteRepo,
            IExtendFieldRepository extendRep,
            ITemplateRepository tempRep,
            CmsCategoryEntity value)
        {
            return new Category(categoryRep,siteRepo, extendRep, tempRep,value);
        }
    }
}
