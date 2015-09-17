using System;
using StructureMap;
using System.Collections.Generic;
using System.Globalization;
using J6.Cms.Dal;
using J6.Cms.Domain.Implement.Content;
using J6.Cms.Domain.Interface.Content;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Site.Template;

namespace J6.Cms.ServiceRepository
{
    public class ContentRepository : BaseContentRepository, IContentRepository
    {
        private IArchiveRepository _archiveRep;
        private IDictionary<int, IContentContainer> siteContents;
        private ITemplateRepository _tempRep;

        public ContentRepository(
            ITemplateRepository tempRep)
        {

            this._tempRep = tempRep;

            siteContents = new Dictionary<int, IContentContainer>();
        }

        public IContentContainer GetContent(int siteId)
        {
            if (!siteContents.ContainsKey(siteId))
            {

                siteContents.Add(siteId,
                    base.CreateSiteContent(
                        this._archiveRep ?? (this._archiveRep = ObjectFactory.GetInstance<IArchiveRepository>()),
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
        private void ReadLinks(IContentLinkManager linkManager, int contentType, int contentId)
        {
            this._linkDal.ReadLinksOfContent(contentType.ToString(), contentId, rd =>
            {
                while (rd.Read())
                {
                    linkManager.Add(
                        int.Parse(rd["id"].ToString()),
                        int.Parse(rd["related_indent"].ToString()),
                        int.Parse(rd["related_content_id"].ToString()),
                        rd["enabled"].ToString() == "1" || rd["enabled"].ToString() == "True"
                        );
                }
            });
        }

        private void SaveLinksOfContent(string typeIndent, int relatedId, IList<IContentLink> list)
        {
            this._linkDal.SaveLinksOfContent(typeIndent, relatedId, list);
        }



        public void RemoveRelatedLinks(string typeIndent, int relatedId, IList<int> list)
        {
            String ids = "";

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                {
                    ids += ",";
                }
                ids += list[i].ToString(CultureInfo.InvariantCulture);
            }

            this._linkDal.RemoveRelatedLinks(typeIndent, relatedId, ids);
        }
    }
}
