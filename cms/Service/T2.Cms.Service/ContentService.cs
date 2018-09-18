using T2.Cms.DataTransfer;
using T2.Cms.ServiceContract;
using System;
using System.Collections.Generic;
using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site;

namespace T2.Cms.Service
{
    public class ContentService : IContentServiceContract
    {
        private readonly IContentRepository _contentRep;
        private ISiteRepo _siteRep;

        public ContentService(IContentRepository contentRep, ISiteRepo siteRep)
        {
            this._contentRep = contentRep;
            this._siteRep = siteRep;
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
                link.RelatedSiteId = linkDto.RelatedSiteId;
            }
            else
            {
                content.LinkManager.Add(linkDto.Id, linkDto.RelatedSiteId, linkDto.RelatedIndent, linkDto.RelatedContentId, linkDto.Enabled);
            }

            content.LinkManager.SaveRelatedLinks();
            return linkDto.Id;
        }

        public void RemoveRelatedLink(int siteId, string contentType, int contentId, int relatedId)
        {
            IBaseContent content = this.GetContent(siteId, contentType, contentId);
            content.LinkManager.RemoveRelatedLink(relatedId);
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
            ISite site = this._siteRep.GetSiteById(link.RelatedSiteId);
            IBaseContent content = this.GetContent(site.GetAggregaterootId(), ContentTypeIndent.Archive.ToString().ToLower(), link.RelatedContentId);
            String thumbnail = null;
            IArchive archive = content as IArchive;
            if (archive != null)
            {
                thumbnail = archive.Get().Thumbnail;
            }

            return new RelatedLinkDto
            {
                Id = link.Id,
                Enabled = link.Enabled,
                ContentId = link.ContentId,
                ContentType = link.ContentType,
                RelatedSiteId = link.RelatedSiteId,
                RelatedSiteName = site.Get().Name,
                RelatedContentId = link.RelatedContentId,
                RelatedIndent = link.RelatedIndent,
                Title = archive.Get().Title,
                Url = site.FullDomain + archive.Get().Path,
                Thumbnail = thumbnail,
                IndentName = ContentUtil.GetRelatedIndentName(link.RelatedIndent).Name,
            };
        }


        public IDictionary<int, RelateIndent> GetRelatedIndents()
        {
            return ContentUtil.GetRelatedIndents();
        }


        public void SetRelatedIndents(IDictionary<int, RelateIndent> relatedIndents)
        {
            ContentUtil.SetRelatedIndents(relatedIndents);
        }

    }

}
