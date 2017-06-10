using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site.Template;

namespace T2.Cms.Domain.Implement.Content
{
    public abstract class BaseContentRepository
    {
        public IContentContainer CreateSiteContent(
            IArchiveRepository archiveRep,
            ITemplateRepository tempRep,
            int siteId)
        {
            return new ContentContainer(archiveRep,tempRep,siteId);
        }

    }
}
