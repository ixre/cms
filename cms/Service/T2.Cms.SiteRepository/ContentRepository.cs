using System;
using System.Collections.Generic;
using T2.Cms.Dal;
using T2.Cms.Domain.Implement.Content;
using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Infrastructure.Ioc;

namespace T2.Cms.ServiceRepository
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
            this._catRepo = catRepo;
            this._tempRep = tempRep;
            siteContents = new Dictionary<int, IContentContainer>();
        }

        public IContentContainer GetContent(int siteId)
        {
            if (siteId == 0)
            {
                return base.CreateSiteContent(
                    this._catRepo,
                    this._archiveRep ?? (this._archiveRep = Ioc.GetInstance<IArchiveRepository>()),
                    this._tempRep,
                    siteId);
            }

            if (!siteContents.ContainsKey(siteId))
            {

                siteContents.Add(siteId,
                    base.CreateSiteContent(
                        this._catRepo,
                        this._archiveRep ?? (this._archiveRep = Ioc.GetInstance<IArchiveRepository>()),
                        this._tempRep,
                        siteId)
                    );
            }
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
            this._linkDal.ReadLinksOfContent(contentType, contentId, rd =>
            {
                while (rd.Read())
                {
                    linkManager.Add(
                        int.Parse(rd["id"].ToString()),
                        int.Parse(rd["related_site_id"].ToString()),
                        int.Parse(rd["related_indent"].ToString()),
                        int.Parse(rd["related_content_id"].ToString()),
                        rd["enabled"].ToString() == "1" || rd["enabled"].ToString() == "True"
                        );
                }
            });
        }

        public void SaveLinksOfContent(string contentType, int relatedId, IList<IContentLink> list)
        {
            this._linkDal.SaveLinksOfContent(contentType, relatedId, list);
        }



        public void RemoveRelatedLinks(string contentType, int relatedId, int[] idList)
        {
            String ids = String.Join(",",idList);

            this._linkDal.RemoveRelatedLinks(contentType, relatedId, ids);
        }


    }
}
