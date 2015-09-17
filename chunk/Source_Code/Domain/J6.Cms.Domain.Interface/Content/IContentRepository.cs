
using System.Collections.Generic;

namespace J6.Cms.Domain.Interface.Content
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContentRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        IContentContainer GetContent(int siteId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkManager"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        void ReadLinksOfContent(IContentLinkManager linkManager, int contentType, int contentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <param name="list"></param>
        void SaveLinksOfContent(int contentType, int contentId, IList<IContentLink> list);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <param name="ids"></param>
        void RemoveRelatedLinks(int contentType, int contentId, int[] ids);
    }
}
