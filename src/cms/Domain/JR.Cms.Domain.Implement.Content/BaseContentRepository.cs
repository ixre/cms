using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Template;

namespace JR.Cms.Domain.Implement.Content
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
