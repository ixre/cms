using System;
using System.Collections.Generic;
using J6.Cms.Domain.Implement.Content.Archive;
using J6.Cms.Domain.Interface.Common;

namespace J6.Cms.Domain.Implement.Content
{
    internal class ContentLinkManager : ILinkManager
    {
        private int _contentId;
        private int _contentModelIndent;
        private ILinkRepository _linkRep;
        private IList<ILink> _links;
        private IList<int> _delLinkIds;

        public ContentLinkManager(ILinkRepository linkRep, int contentModelIndent, int contentId)
        {
            this._contentModelIndent = contentModelIndent;
            this._contentId = contentId;
            this._linkRep = linkRep;

            _links = new List<ILink>();
            this.ReadLinks();
        }

        public string TypeIndent
        {
            get { return this._contentModelIndent.ToString(); }
        }

        private void ReadLinks()
        {
            this._linkRep.ReadLinksOfContent(this, this._contentModelIndent, this._contentId);
        }

        public void Add(int linkId, string name, string title, string uri, bool enabled)
        {
            ILink link = null;
            switch (this.TypeIndent)
            {
                case "1":
                    link = new LinkOfArchive(linkId, name, title, uri, enabled);
                    break;
            }

            if (link != null)
            {
                foreach (ILink _link in this._links)
                {
                    if (_link.Equal(link))
                        throw new Exception("已存在重名的链接！");
                }

                this._links.Add(link);
            }

        }

        public void Remove(ILink link)
        {
            if (this._delLinkIds == null)
            {
                this._delLinkIds = new List<int>();
            }


            for (int i = 0; i < this._links.Count; i++)
            {
                if (this._links[i].Equal(link))
                {
                    ILink _link = this._links[i];
                    this._links.Remove(_link);
                    this._delLinkIds.Add(_link.LinkId);
                    _link = null;
                }
            }
        }

        public void SaveLinks()
        {
            this._linkRep.SaveLinksOfContent(this._contentModelIndent, this._contentId, this._links);

            if (this._delLinkIds!= null && this._delLinkIds.Count != 0)
            {
                this._linkRep.RemoveRelatedLinks(
                    this._contentModelIndent.ToString(),
                    this._contentId,
                    this._delLinkIds);
            }

        }



        public IList<ILink> GetRelatedLinks()
        {
            return this._links;
        }



        public bool Contain(ILink link)
        {
            foreach (ILink _link in this._links)
            {
                if (_link.Equal(link))
                    return true;
            }
            return false;
        }


        public ILink GetLinkById(int id)
        {
            foreach (ILink link in this._links)
            {
                if (link.LinkId == id)
                    return link;
            }
            return null;
        }
    }
}
