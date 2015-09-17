using J6.Cms.DataTransfer;
using System.Collections.Generic;
using J6.Cms.Domain.Interface.Content;

namespace J6.Cms.ServiceContract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContentServiceContract
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        IBaseContent GetContent(int siteId,string typeIndent, int contentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        int SaveOuterRelatedLink(int siteId, string typeIndent, int contentId, LinkDto link);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <param name="relatedLinkId"></param>
        void RemoveOuterRelatedLink(int siteId, string typeIndent, int contentId, int relatedLinkId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        IEnumerable<LinkDto> GetOuterRelatedLinks(int siteId, string typeIndent, int contentId);

        IDictionary<int, string> GetRelatedIndents();

        void SetRelatedIndents(IDictionary<int, string> relatedIndents);

        /// <summary>
        /// 保存关联文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <param name="relatedIndent"></param>
        /// <param name="relatedId"></param>
        int SaveRelatedLink(int siteId, int id, string contentType, int contentId, int relatedIndent, int relatedId);
    }
}
