using System.Collections.Generic;
using JR.Cms.Domain.Interface.Content;

namespace JR.Cms.Domain.Interface.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILinkRepository
    {
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
        /// <param name="typeIndent"></param>
        /// <param name="relatedId"></param>
        /// <param name="list"></param>
        void RemoveRelatedLinks(string typeIndent, int relatedId, IList<int> list);
    }
}
