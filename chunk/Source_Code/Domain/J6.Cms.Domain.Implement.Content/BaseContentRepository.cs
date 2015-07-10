using J6.Cms.Domain.Interface.Content;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Site.Template;

namespace J6.Cms.Domain.Implement.Content
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
