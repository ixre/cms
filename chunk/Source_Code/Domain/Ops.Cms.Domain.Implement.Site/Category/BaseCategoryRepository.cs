using AtNet.Cms.Domain.Interface.Site;
using AtNet.Cms.Domain.Interface.Site.Category;
using AtNet.Cms.Domain.Interface.Site.Extend;
using AtNet.Cms.Domain.Interface.Site.Template;

namespace AtNet.Cms.Domain.Implement.Site.Category
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
            IExtendFieldRepository extendRep,
            ITemplateRepository tempRep,
            int categoryId,
            ISite site)
        {
            return new Category(categoryRep, extendRep, tempRep,categoryId, site);
        }
    }
}
