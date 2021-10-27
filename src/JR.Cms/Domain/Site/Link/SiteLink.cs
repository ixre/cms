using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Link;

namespace JR.Cms.Domain.Site.Link
{
    public class SiteLink : ISiteLink
    {
        private ISiteRepo _siteRep;
        private ISite _site;

        internal SiteLink(ISiteRepo siteRep, ISite site, int id, string text)
        {
            _siteRep = siteRep;
            _site = site;
            Id = id;
            Text = text;
        }

        public int Pid { get; set; }

        public SiteLinkType Type { get; set; }

        public string Text { get; set; }

        public string Uri { get; set; }

        public string ImgUrl { get; set; }

        public string Target { get; set; }

        public int SortNumber { get; set; }

        public bool Visible { get; set; }

        public string Bind { get; set; }

        public int Id { get; set; }

        public int GetDomainId()
        {
            return Id;
        }

        public int Save()
        {
            return _siteRep.SaveSiteLink(_site.GetAggregateRootId(), this);
        }
    }
}