using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;

namespace JR.Cms.Domain.Site.Category
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
            ICategoryRepo categoryRep,
            ISiteRepo siteRepo,
            IExtendFieldRepository extendRep,
            ITemplateRepo tempRep,
            CmsCategoryEntity value)
        {
            return new Category(categoryRep, siteRepo, extendRep, tempRep, value);
        }
    }
}