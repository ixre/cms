using Ops.Cms.Domain.Interface.Site;
using Ops.Cms.Domain.Interface.Site.Link;

namespace Ops.Cms.Domain.Implement.Site.Link
{
    public class SiteLink:ISiteLink
    {
        private ISiteRepository _siteRep;
        private ISite _site;

        internal SiteLink(ISiteRepository siteRep, ISite site, int id,string text)
        {
            this._siteRep = siteRep;
            this._site = site;
            this.ID = id;
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

        public int OrderIndex
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

        public int ID
        {
            get;
            set;
        }


        public int Save()
        {
           return  this._siteRep.SaveSiteLink(this._site.ID, this);
        }

    }
}
