using Ops.Cms.Domain.Interface.Content;
using Ops.Cms.Domain.Interface.Content.Archive;
using Ops.Cms.Domain.Interface.Site.Template;

namespace Ops.Cms.Domain.Implement.Content
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
