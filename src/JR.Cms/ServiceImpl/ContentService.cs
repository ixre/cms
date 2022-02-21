using System;
using System.Collections.Generic;
using JR.Cms.Dao;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Infrastructure;
using JR.Cms.ServiceContract;
using JR.Cms.ServiceDto;

namespace JR.Cms.ServiceImpl
{
    /// <summary>
    /// 
    /// </summary>
    public class ContentService : IContentServiceContract
    {
        private readonly IContentRepository _contentRep;
        private readonly ISiteRepo _siteRep;
        private readonly ISiteTagDao _tagDao;

        private IList<SiteTag> _tags;
        private IDictionary<String, SiteTag> _tagsDict;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentRep"></param>
        /// <param name="siteRep"></param>
        /// <param name="tagDao"></param>
        public ContentService(IContentRepository contentRep, ISiteRepo siteRep,ISiteTagDao tagDao)
        {
            _contentRep = contentRep;
            _siteRep = siteRep;
            this._tagDao = tagDao;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public IBaseContent GetContent(int siteId, string typeIndent, int contentId)
        {
            return _contentRep.GetContent(siteId).GetContent(typeIndent, contentId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="linkDto"></param>
        /// <returns></returns>
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

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <param name="relatedId"></param>
        public void RemoveRelatedLink(int siteId, string contentType, int contentId, int relatedId)
        {
            var content = GetContent(siteId, contentType, contentId);
            content.LinkManager.RemoveRelatedLink(relatedId);
            content.LinkManager.SaveRelatedLinks();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDictionary<int, RelateIndent> GetRelatedIndents()
        {
            return ContentUtil.GetRelatedIndents();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="relatedIndents"></param>
        public void SetRelatedIndents(IDictionary<int, RelateIndent> relatedIndents)
        {
            ContentUtil.SetRelatedIndents(relatedIndents);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<SiteTag> GetTags()
        {
            if (this._tags == null)
            {
                List<SiteTag> list = this._tagDao.GetTags();
                list.Sort((a, b) => b.Tag.Length - a.Tag.Length);
                this._tags = new List<SiteTag>();
                this._tagsDict = new Dictionary<string, SiteTag>();
                foreach (var it in list)
                {
                    this._tags.Add(it);
                    this._tagsDict.Add(it.Tag, it);
                }
            }

            return this._tags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Error SaveTag(SiteTag tag)
        {
            Error err = this._tagDao.SaveTag(tag);
            if (err == null)
            {
                this._tags = null;
                this._tagsDict = null;
            }

            return err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Error DeleteTag(SiteTag tag)
        {
            Error err = this._tagDao.DeleteTag(tag);
            if (err == null)
            {
                this._tags = null;
                this._tagsDict = null;
            }
            return err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="openInBlank"></param>
        /// <returns></returns>
        public string Replace(string content, bool openInBlank)
        {
            return TagUtil.ReplaceSiteTag(content,this._tags, openInBlank);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string RemoveTags(string content)
        {
            return TagUtil.RemoveSiteTag(content,this._tagsDict.Keys);
        }

    }
}