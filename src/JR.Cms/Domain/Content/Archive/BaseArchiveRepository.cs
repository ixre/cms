using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;

namespace JR.Cms.Domain.Content.Archive
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseArchiveRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentRep"></param>
        /// <param name="archiveRep"></param>
        /// <param name="extendRep"></param>
        /// <param name="categoryRep"></param>
        /// <param name="templateRep"></param>
        /// <param name="linkRep"></param>
        /// <param name="id"></param>
        /// <param name="strId"></param>
        /// <param name="categoryId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public IArchive CreateArchive(
            IContentRepository contentRep,
            IArchiveRepository archiveRep,
            IExtendFieldRepository extendRep,
            ICategoryRepo categoryRep,
            ITemplateRepo templateRep,
            CmsArchiveEntity value)
        {
            return new Archive(contentRep, archiveRep, extendRep,
                categoryRep, templateRep, value);
        }
    }
}