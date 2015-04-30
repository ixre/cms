using System.Collections.Generic;

namespace AtNet.Cms.Domain.Interface.Site.Link
{
    public interface ISiteLinkManager
    {
        /// <summary>
        /// 删除链接
        /// </summary>
        /// <param name="linkId"></param>
        /// <returns></returns>
       bool DeleteLink(int linkId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkId"></param>
        /// <returns></returns>
       ISiteLink GetLinkById(int linkId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
       IEnumerable<ISiteLink> GetLinks(SiteLinkType type);
    }
}

