using System;
using System.Collections.Generic;
using System.Linq;
using J6.Cms.Domain.Implement.Content.Archive;
using J6.Cms.Domain.Interface.Common;
using J6.Cms.Domain.Interface.Content;

namespace J6.Cms.Domain.Implement.Content
{
    internal class ContentLinkManager : IContentLinkManager
    {
        private int _contentId;
        private int _contentModelIndent;
        private IContentRepository _contentRep;
        private IList<IContentLink> _links;
        private IList<int> _delLinkIds;

        public ContentLinkManager(IContentRepository linkRep, int contentTypet, int contentId)
        {
            this._contentModelIndent = contentTypet;
            this._contentId = contentId;
            this._contentRep = linkRep;

            _links = new List<IContentLink>();
            this.ReadLinks();
        }


        private void ReadLinks()
        {
            this._contentRep.ReadLinksOfContent(this, this._contentModelIndent, this._contentId);
        }


        public void Remove(IContentLink link)
        {
            if (this._delLinkIds == null)
            {
                this._delLinkIds = new List<int>();
            }


            for (int i = 0; i < this._links.Count; i++)
            {
                if (this._links[i].Equal(link))
                {
                    IContentLink _link = this._links[i];
                    this._links.Remove(_link);
                    this._delLinkIds.Add(_link.Id);
                    _link = null;
                }
            }
        }

        public void SaveRelatedLinks()
        {
            this._contentRep.SaveLinksOfContent(this._contentModelIndent, this._contentId, this._links);

            if (this._delLinkIds!= null && this._delLinkIds.Count != 0)
            {
                this._contentRep.RemoveRelatedLinks(
                    this._contentModelIndent,
                    this._contentId,
                    this._delLinkIds.ToArray());
            }

        }



        public IList<IContentLink> GetRelatedLinks()
        {
            return this._links;
        }



        public bool Contain(IContentLink link)
        {
            foreach (IContentLink _link in this._links)
            {
                if (_link.Equal(link))
                    return true;
            }
            return false;
        }


        public IContentLink GetLinkById(int id)
        {
            foreach (IContentLink link in this._links)
            {
                if (link.Id == id)
                    return link;
            }
            return null;
        }

        public bool Contain(int relatedContentType, int relatedContentId)
        {
            throw new NotImplementedException();
        }

        public void Add(int id, int relatedIndent, int relatedId, bool enabled)
        {
            IContentLink link = null;
            switch (this._contentModelIndent)
            {
                case 1:
                    link = new LinkOfArchive(0, this._contentId, this._contentModelIndent, relatedId, relatedIndent,
                        enabled);
                    break;
            }

            if (link != null)
            {
                foreach (IContentLink _link in this._links)
                {
                    if (_link.Equal(link))
                        throw new Exception("已存在重名的链接！");
                }

                this._links.Add(link);
            }
        }

        IList<IContentLink> IContentLinkManager.GetRelatedLinks()
        {
            throw new NotImplementedException();
        }

        IContentLink IContentLinkManager.GetLinkById(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveRelatedLink(int id)
        {
            throw new NotImplementedException();
        }
    }
}
