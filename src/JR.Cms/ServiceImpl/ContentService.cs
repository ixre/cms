using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.ServiceContract;
using JR.Cms.ServiceDto;

namespace JR.Cms.ServiceImpl
{
    public class ContentService : IContentServiceContract
    {
        private readonly IContentRepository _contentRep;
        private ISiteRepo _siteRep;

        public ContentService(IContentRepository contentRep, ISiteRepo siteRep)
        {
            _contentRep = contentRep;
            _siteRep = siteRep;
        }

        public IBaseContent GetContent(int siteId, string typeIndent, int contentId)
        {
            return _contentRep.GetContent(siteId).GetContent(typeIndent, contentId);
        }

        public int SaveRelatedLink(int siteId, RelatedLinkDto linkDto)
        {
            var content = GetContent(siteId, linkDto.ContentType, linkDto.ContentId);

            if (linkDto.Id > 0)
            {
                var link = content.LinkManager.GetLinkById(linkDto.Id);
                link.RelatedIndent = linkDto.RelatedIndent;
                link.RelatedContentId = linkDto.RelatedContentId;
                link.Enabled = linkDto.Enabled;
                link.RelatedSiteId = linkDto.RelatedSiteId;
            }
            else
            {
                content.LinkManager.Add(linkDto.Id, linkDto.RelatedSiteId, linkDto.RelatedIndent,
                    linkDto.RelatedContentId, linkDto.Enabled);
            }

            content.LinkManager.SaveRelatedLinks();
            return linkDto.Id;
        }

        public void RemoveRelatedLink(int siteId, string contentType, int contentId, int relatedId)
        {
            var content = GetContent(siteId, contentType, contentId);
            content.LinkManager.RemoveRelatedLink(relatedId);
            content.LinkManager.SaveRelatedLinks();
        }

        public IEnumerable<RelatedLinkDto> GetRelatedLinks(int siteId, string contentType, int contentId)
        {
            var content = GetContent(siteId, contentType, contentId);
            var linkList = content.LinkManager.GetRelatedLinks();
            foreach (var link in linkList) yield return ConvertToLinkDto(siteId, link);
        }

        private RelatedLinkDto ConvertToLinkDto(int siteId, IContentLink link)
        {
            var site = _siteRep.GetSiteById(link.RelatedSiteId);
            var content = GetContent(site.GetAggregateRootId(), ContentTypeIndent.Archive.ToString().ToLower(),
                link.RelatedContentId);
            string thumbnail = null;
            var archive = content as IArchive;
            if (archive != null) thumbnail = archive.Get().Thumbnail;

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