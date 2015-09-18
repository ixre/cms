using System.Collections.Generic;

namespace J6.Cms.Domain.Interface.Content
{
    public interface IContentLinkManager
    {
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="relatedContentType"></param>
        /// <param name="relatedContentId"></param>
        /// <returns></returns>
        bool Contain(int relatedContentType,int relatedContentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relatedSiteId"></param>
        /// <param name="relatedIndent"></param>
        /// <param name="relatedContentId"></param>
        /// <param name="enabled"></param>
        void Add(int id,int relatedSiteId,int relatedIndent, int relatedContentId,bool enabled);

        /// <summary>
        /// 获取关联的链接
        /// </summary>
        /// <returns></returns>
        IList<IContentLink> GetRelatedLinks();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IContentLink GetLinkById(int id);

        ///// <summary>
        ///// 获取被关链的链接
        ///// </summary>
        ///// <returns></returns>
        //IList<ILink> GetRevertRelatedLinks();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void RemoveRelatedLink(int id);

        /// <summary>
        /// 
        /// </summary>
        void SaveRelatedLinks();
    }
}