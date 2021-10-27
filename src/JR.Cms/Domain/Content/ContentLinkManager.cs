using System;
using System.Collections.Generic;
using System.Linq;
using JR.Cms.Domain.Content.Archive;
using JR.Cms.Domain.Interface.Content;

namespace JR.Cms.Domain.Content
{
    internal class ContentLinkManager : IContentLinkManager
    {
        private readonly int _contentId;
        private readonly string _contentType;
        private readonly IContentRepository _contentRep;
        private readonly IList<IContentLink> _links;
        private IList<int> _delLinkIds;

        public ContentLinkManager(IContentRepository linkRep, string contentType, int contentId)
        {
            _contentType = contentType;
            _contentId = contentId;
            _contentRep = linkRep;

            _links = new List<IContentLink>();
            ReadLinks();
        }


        private void ReadLinks()
        {
            _contentRep.ReadLinksOfContent(this, _contentType, _contentId);
        }


        public void RemoveRelatedLink(int id)
        {
            if (_delLinkIds == null) _delLinkIds = new List<int>();

            for (var i = 0; i < _links.Count; i++)
                if (_links[i].Id == id)
                {
                    var link = _links[i];
                    _links.Remove(link);
                    _delLinkIds.Add(link.Id);
                }
        }

        public void SaveRelatedLinks()
        {
            _contentRep.SaveLinksOfContent(_contentType, _contentId, _links);

            if (_delLinkIds != null && _delLinkIds.Count != 0)
                _contentRep.RemoveRelatedLinks(
                    _contentType,
                    _contentId,
                    _delLinkIds.ToArray());
        }


        public IList<IContentLink> GetRelatedLinks()
        {
            return _links;
        }


        public IContentLink GetLinkById(int id)
        {
            foreach (var link in _links)
                if (link.Id == id)
                    return link;
            return null;
        }

        public bool Contain(int relatedContentType, int relatedContentId)
        {
            foreach (var link in _links)
                if (link.RelatedContentId == relatedContentId && relatedContentType == link.RelatedIndent)
                    return true;
            return false;
        }

        public void Add(int id, int relatedSiteId, int relatedIndent, int relatedId, bool enabled)
        {
            IContentLink link = null;

            if (_contentType == ContentTypeIndent.Archive.ToString().ToLower())
                link = new LinkOfArchive(id, _contentType, _contentId, relatedSiteId, relatedIndent, relatedId,
                    enabled);

            if (link != null)
            {
                foreach (var link2 in _links)
                    if (link2.Equal(link))
                        throw new Exception("已存在重名的链接！");

                _links.Add(link);
            }
        }
    }
}