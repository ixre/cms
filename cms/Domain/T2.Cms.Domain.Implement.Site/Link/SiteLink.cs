using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Link;

namespace T2.Cms.Domain.Implement.Site.Link
{
    public class SiteLink:ISiteLink
    {
        private ISiteRepository _siteRep;
        private ISite _site;

        internal SiteLink(ISiteRepository siteRep, ISite site, int id,string text)
        {
            this._siteRep = siteRep;
            this._site = site;
            this.Id = id;
            this.Text = text;
        }

        public int Pid
        {
            get;
            set;
        }

        public SiteLinkType Type
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string Uri
        {
            get;
            set;
        }

        public string ImgUrl
        {
            get;
            set;
        }

        public string Target
        {
            get;
            set;
        }

        public int SortNumber
        {
            get;
            set;
        }

        public bool Visible
        {
            get;
            set;
        }

        public string Bind
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }


        public int Save()
        {
           return  this._siteRep.SaveSiteLink(this._site.GetAggregaterootId(), this);
        }

    }
}
