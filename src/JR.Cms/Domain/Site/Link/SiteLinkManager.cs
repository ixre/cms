using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Link;

namespace JR.Cms.Domain.Site.Link
{
    internal class SiteLinkManager : ISiteLinkManager
    {
        private ISiteRepo _siteRep;
        private ISite _site;

        public SiteLinkManager(ISiteRepo siteRep, ISite site)
        {
            _siteRep = siteRep;
            _site = site;
        }

        public bool DeleteLink(int linkId)
        {
            return _siteRep.DeleteSiteLink(_site.GetAggregateRootId(), linkId);
        }


        public ISiteLink GetLinkById(int linkId)
        {
            return _siteRep.GetSiteLinkById(_site.GetAggregateRootId(), linkId);
        }

        public IEnumerable<ISiteLink> GetLinks(SiteLinkType type)
        {
            return _siteRep.GetSiteLinks(_site.GetAggregateRootId(), type);
        }
    }
}