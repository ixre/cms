using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Template;

namespace JR.Cms.Domain.Content
{
    public abstract class BaseContentRepository
    {
        public IContentContainer CreateSiteContent(
            ICategoryRepo catRepo,
            IArchiveRepository archiveRep,
            ITemplateRepo tempRep,
            int siteId)
        {
            return new ContentContainer(catRepo, archiveRep, tempRep, siteId);
        }
    }
}