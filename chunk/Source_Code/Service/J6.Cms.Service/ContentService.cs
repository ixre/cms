using J6.Cms.DataTransfer;
using J6.Cms.ServiceContract;
using System;
using System.Collections.Generic;
using J6.Cms.Domain.Interface.Content;
using J6.Cms.Domain.Interface.Content.Archive;

namespace J6.Cms.Service
{
    public class ContentService : IContentServiceContract
    {
        private readonly IContentRepository _contentRep;

        public ContentService(IContentRepository contentRep)
        {
            this._contentRep = contentRep;
        }

        public IBaseContent GetContent(int siteId, string typeIndent, int contentId)
        {
            return this._contentRep.GetContent(siteId).GetContent(typeIndent, contentId);
        }

        public int SaveRelatedLink(int siteId, RelatedLinkDto linkDto)
        {
            IBaseContent content = this.GetContent(siteId, linkDto.ContentType, linkDto.ContentId);

            if (linkDto.Id > 0)
            {
                IContentLink link = content.LinkManager.GetLinkById(linkDto.Id);
                link.RelatedIndent = linkDto.RelatedIndent;
                link.RelatedContentId = linkDto.RelatedContentId;
                link.Enabled = linkDto.Enabled;
            }
            else
            {
                content.LinkManager.Add(linkDto.Id, linkDto.RelatedContentId, linkDto.RelatedIndent, linkDto.Enabled);
            }

            content.LinkManager.SaveRelatedLinks();
            return linkDto.Id;
        }

        public void RemoveOuterRelatedLink(int siteId, string typeIndent, int contentId, int relatedLinkId)
        {
            IBaseContent content = this.GetContent(siteId, typeIndent, contentId);

            if (relatedLinkId > 0)
            {
                IContentLink link = content.LinkManager.GetLinkById(relatedLinkId);
                content.LinkManager.RemoveRelatedLink(link.Id);
            }
            else
            {
                throw new Exception("relatedLinkId无效");
            }

            content.LinkManager.SaveRelatedLinks();

        }

        public IEnumerable<RelatedLinkDto> GetRelatedLinks(int siteId, string contentType, int contentId)
        {
            IBaseContent content = this.GetContent(siteId, contentType, contentId);
            IList<IContentLink> linkList = content.LinkManager.GetRelatedLinks();
            foreach (IContentLink link in linkList)
            {
                yield return this.ConvertToLinkDto(siteId, link);
            }
        }

        private RelatedLinkDto ConvertToLinkDto(int siteId, IContentLink link)
        {
            IBaseContent content = this.GetContent(siteId, ContentTypeIndent.Archive.ToString().ToLower(), link.RelatedContentId);
            String thumbnail = null;
            IArchive archive = content as IArchive;
            if (archive != null)
            {
                thumbnail = archive.Thumbnail;
            }

            return new RelatedLinkDto
            {
                Id = link.Id,
                Enabled = link.Enabled,
                ContentId = link.ContentId,
                ContentType = link.ContentType,
                RelatedContentId = link.RelatedContentId,
                RelatedIndent = link.RelatedIndent,
                Title = content.Title,
                Url = content.Uri,
                Thumbnail = thumbnail,
                IndentName = ContentUtil.GetRelatedIndentName(link.RelatedIndent),
            };
        }


        public IDictionary<int, string> GetRelatedIndents()
        {
            return ContentUtil.GetRelatedIndents();
        }


        public void SetRelatedIndents(IDictionary<int, string> relatedIndents)
        {
            ContentUtil.SetRelatedIndents(relatedIndents);
        }

    }

}
