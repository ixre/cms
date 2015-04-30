using AtNet.Cms.Domain.Interface.Content;
using AtNet.Cms.Domain.Interface.Content.Archive;
using AtNet.Cms.Domain.Interface.Site.Template;

namespace AtNet.Cms.Domain.Implement.Content
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
