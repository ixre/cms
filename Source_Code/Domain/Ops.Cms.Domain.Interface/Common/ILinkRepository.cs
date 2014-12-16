using System.Collections.Generic;

namespace Ops.Cms.Domain.Interface.Common
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
        /// <param name="contentModelIndent"></param>
        /// <param name="contentId"></param>
        void ReadLinksOfContent(ILinkManager linkManager, int contentModelIndent, int contentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentModelIndent"></param>
        /// <param name="contentId"></param>
        /// <param name="list"></param>
        void SaveLinksOfContent(int contentModelIndent, int contentId, IList<ILink> list);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeIndent"></param>
        /// <param name="relatedId"></param>
        /// <param name="list"></param>
        void RemoveRelatedLinks(string typeIndent, int relatedId, IList<int> list);
    }
}
