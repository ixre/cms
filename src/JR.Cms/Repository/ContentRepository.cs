using System;
using System.Collections.Generic;
using JR.Cms.Domain.Content;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure.Ioc;
using JR.Cms.Library.DataAccess.DAL;

namespace JR.Cms.Repository
{
    public class ContentRepository : BaseContentRepository, IContentRepository
    {
        private IArchiveRepository _archiveRep;
        private IDictionary<int, IContentContainer> siteContents;
        private ICategoryRepo _catRepo;
        private ITemplateRepo _tempRep;

        public ContentRepository(
            ICategoryRepo catRepo,
            ITemplateRepo tempRep)
        {
            _catRepo = catRepo;
            _tempRep = tempRep;
            siteContents = new Dictionary<int, IContentContainer>();
        }

        public IContentContainer GetContent(int siteId)
        {
            if (siteId == 0)
                return CreateSiteContent(
                    _catRepo,
                    _archiveRep ?? (_archiveRep = Ioc.GetInstance<IArchiveRepository>()),
                    _tempRep,
                    siteId);

            if (!siteContents.ContainsKey(siteId))
                siteContents.Add(siteId,
                    CreateSiteContent(
                        _catRepo,
                        _archiveRep ?? (_archiveRep = Ioc.GetInstance<IArchiveRepository>()),
                        _tempRep,
                        siteId)
                );
            return siteContents[siteId];
        }

        private LinkDal _linkDal = new LinkDal();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkManager"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        public void ReadLinksOfContent(IContentLinkManager linkManager, string contentType, int contentId)
        {
            _linkDal.ReadLinksOfContent(contentType, contentId, rd =>
            {
                while (rd.Read())
                    linkManager.Add(
                        int.Parse(rd["id"].ToString()),
                        int.Parse(rd["related_site_id"].ToString()),
                        int.Parse(rd["related_indent"].ToString()),
                        int.Parse(rd["related_content_id"].ToString()),
                        rd["enabled"].ToString() == "1" || rd["enabled"].ToString() == "True"
                    );
            });
        }

        public void SaveLinksOfContent(string contentType, int relatedId, IList<IContentLink> list)
        {
            _linkDal.SaveLinksOfContent(contentType, relatedId, list);
        }


        public void RemoveRelatedLinks(string contentType, int relatedId, int[] idList)
        {
            var ids = string.Join(",", idList);

            _linkDal.RemoveRelatedLinks(contentType, relatedId, ids);
        }
    }
}