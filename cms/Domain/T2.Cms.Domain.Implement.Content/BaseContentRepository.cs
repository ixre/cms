using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Template;

namespace T2.Cms.Domain.Implement.Content
{
    public abstract class BaseContentRepository
    {
        public IContentContainer CreateSiteContent(
            ICategoryRepo catRepo,
            IArchiveRepository archiveRep,
            ITemplateRepo tempRep,
            int siteId)
        {
            return new ContentContainer(catRepo,archiveRep, tempRep,siteId);
        }

    }
}
