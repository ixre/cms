using StructureMap;
using System.Collections.Generic;
using AtNet.Cms.Domain.Implement.Content;
using AtNet.Cms.Domain.Interface.Content;
using AtNet.Cms.Domain.Interface.Content.Archive;
using AtNet.Cms.Domain.Interface.Site.Template;

namespace AtNet.Cms.ServiceRepository
{
    public class ContentRepository : BaseContentRepository, IContentRepository
    {
        private IArchiveRepository _archiveRep;
        IDictionary<int, IContentContainer> siteContents;
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
                    this._tempRep ,
                    siteId)
                    );
            }
            return siteContents[siteId];
        }

       
    }
}
