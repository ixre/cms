using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using RelatedLinkDto = JR.Cms.ServiceDto.RelatedLinkDto;

namespace JR.Cms.ServiceContract
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
        IBaseContent GetContent(int siteId, string typeIndent, int contentId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <param name="relatedLinkId"></param>
        void RemoveRelatedLink(int siteId, string typeIndent, int contentId, int relatedLinkId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="typeIndent"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        IEnumerable<RelatedLinkDto> GetRelatedLinks(int siteId, string typeIndent, int contentId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDictionary<int, RelateIndent> GetRelatedIndents();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relatedIndents"></param>
        void SetRelatedIndents(IDictionary<int, RelateIndent> relatedIndents);

        /// <summary>
        /// 保存关联文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="link"></param>
        int SaveRelatedLink(int siteId, RelatedLinkDto link);
        
        /// <summary>
        /// 获取所有的标签,并排序
        /// </summary>
        /// <returns></returns>
        IList<SiteWord> GetTags();

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Error SaveTag(SiteWord word);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Error DeleteTag(SiteWord word);

        /// <summary>
        /// 替换标签内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="openInBlank">是否在新窗口中打开</param>
        /// <returns></returns>
        String Replace(string content, bool openInBlank);

        /// <summary>
        /// 移出所有的标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        String RemoveTags(string content);
    }
}