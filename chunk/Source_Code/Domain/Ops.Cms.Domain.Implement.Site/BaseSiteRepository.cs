using AtNet.Cms.Domain.Interface.Site;
using AtNet.Cms.Domain.Interface.Site.Category;
using AtNet.Cms.Domain.Interface.Site.Extend;
using AtNet.Cms.Domain.Interface.Site.Link;
using AtNet.Cms.Domain.Interface.Site.Template;

namespace AtNet.Cms.Domain.Implement.Site
{
    public abstract class BaseSiteRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="extendRepository"></param>
        /// <param name="categoryRep"></param>
        /// <param name="tempRep"></param>
        /// <param name="siteid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISite CreateSite(
            ISiteRepository resp,
            IExtendFieldRepository extendRepository,
            ICategoryRepository categoryRep,
            ITemplateRepository tempRep,
            int siteid,
            string name
            )
        {
            return new Site(resp,
                extendRepository,
                categoryRep,
                tempRep,
                siteid,
                name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="site"></param>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ISiteLink CreateLink(ISiteRepository resp, ISite site, int id, string text)
        {
            return new Link.SiteLink(resp, site, id, text);
        }

    }
}
