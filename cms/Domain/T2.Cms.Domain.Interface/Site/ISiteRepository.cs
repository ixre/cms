using System;
using System.Collections.Generic;
using T2.Cms.Domain.Interface.Site.Link;
using T2.Cms.Models;

namespace T2.Cms.Domain.Interface.Site
{
    public interface ISiteRepository
    {
        ISite CreateSite(CmsSiteEntity site);

        /// <summary>
        /// 创建链接对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        ISiteLink CreateLink(ISite site, int id, string text);


        IList<ISite> GetSites(); 
        int SaveSite(ISite site);

        /// <summary>
        /// 根据Uri获取站点
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        ISite GetSiteByUri(Uri uri);

        /// <summary>
        /// 根据编号获取网站
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        ISite GetSiteById(int siteId);

        /// <summary>
        /// 根据Uri获取站点，如果不存在则返回默认站点
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        ISite GetSingleOrDefaultSite(Uri uri);

        /// <summary>
        /// 保存链接
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="link"></param>
        int SaveSiteLink(int siteId, ISiteLink link);

        bool DeleteSiteLink(int siteId, int linkId);

        ISiteLink GetSiteLinkById(int siteId, int linkId);

        IEnumerable<ISiteLink> GetSiteLinks(int siteId, SiteLinkType type);
    }
}
