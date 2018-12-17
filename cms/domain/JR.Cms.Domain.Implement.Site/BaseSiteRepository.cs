using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Models;

namespace JR.Cms.Domain.Implement.Site
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
            ISiteRepo resp,
            IExtendFieldRepository extendRepository,
            ICategoryRepo categoryRep,
            ITemplateRepo tempRep,
            IUserRepository userRep,
            CmsSiteEntity site
            )
        {
            return new Site(resp,
                extendRepository,
                categoryRep,
                tempRep,
                userRep,
                site);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="site"></param>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ISiteLink CreateLink(ISiteRepo resp, ISite site, int id, string text)
        {
            return new Link.SiteLink(resp, site, id, text);
        }

    }
}
