using System;
using System.Collections.Generic;
using System.Linq;
using JR.Cms.Domain.Implement.Content.Archive;
using JR.Cms.Domain.Interface.Common;
using JR.Cms.Domain.Interface.Content;

namespace JR.Cms.Domain.Implement.Content
{
    internal class ContentLinkManager : IContentLinkManager
    {
        private int _contentId;
        private string _contentType;
        private IContentRepository _contentRep;
        private IList<IContentLink> _links;
        private IList<int> _delLinkIds;

        public ContentLinkManager(IContentRepository linkRep, string contentType, int contentId)
        {
            this._contentType = contentType;
            this._contentId = contentId;
            this._contentRep = linkRep;

            _links = new List<IContentLink>();
            this.ReadLinks();
        }


        private void ReadLinks()
        {
            this._contentRep.ReadLinksOfContent(this, this._contentType, this._contentId);
        }


        public void RemoveRelatedLink(int id)
        {
            if (this._delLinkIds == null)
            {
                this._delLinkIds = new List<int>();
            }


            for (int i = 0; i < this._links.Count; i++)
            {
                if (this._links[i].Id == id)
                {
                    IContentLink link = this._links[i];
                    this._links.Remove(link);
                    this._delLinkIds.Add(link.Id);
                    link = null;
                }
            }
        }

        public void SaveRelatedLinks()
        {
            this._contentRep.SaveLinksOfContent(this._contentType, this._contentId, this._links);

            if (this._delLinkIds!= null && this._delLinkIds.Count != 0)
            {
                this._contentRep.RemoveRelatedLinks(
                    this._contentType,
                    this._contentId,
                    this._delLinkIds.ToArray());
            }

        }



        public IList<IContentLink> GetRelatedLinks()
        {
            return this._links;
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
            foreach (IContentLink link in this._links)
            {
                if (link.RelatedContentId == relatedContentId && relatedContentType == link.RelatedIndent)
                    return true;
            }
            return false;
        }

        public void Add(int id, int relatedSiteId,int relatedIndent, int relatedId, bool enabled)
        {
            IContentLink link = null;

            if (this._contentType == ContentTypeIndent.Archive.ToString().ToLower())
            {
                link = new LinkOfArchive(id, this._contentType, this._contentId, relatedSiteId, relatedIndent,relatedId,
                    enabled);
            }

            if (link != null)
            {
                foreach (IContentLink link2 in this._links)
                {
                    if (link2.Equal(link))
                        throw new Exception("已存在重名的链接！");
                }

                this._links.Add(link);
            }
        }

    }
}
