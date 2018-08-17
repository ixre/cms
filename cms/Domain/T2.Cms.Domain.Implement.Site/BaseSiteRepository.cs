using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Link;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Domain.Interface.User;

namespace T2.Cms.Domain.Implement.Site
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
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISite CreateSite(
            ISiteRepository resp,
            IExtendFieldRepository extendRepository,
            ICategoryRepository categoryRep,
            ITemplateRepository tempRep,
            IUserRepository userRep,
            int siteId,
            string name
            )
        {
            return new Site(resp,
                extendRepository,
                categoryRep,
                tempRep,
                userRep,
                siteId,
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
