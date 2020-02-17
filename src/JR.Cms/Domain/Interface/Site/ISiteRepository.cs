using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Models;

namespace JR.Cms.Domain.Interface.Site
{
    public interface ISiteRepo
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
        ISite GetSiteByUri(String host, String appPath);

        /// <summary>
        /// 根据编号获取网站
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        ISite GetSiteById(int siteId);

        /// <summary>
        /// 根据Uri获取站点，如果不存在则返回默认站点
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        ISite GetSingleOrDefaultSite(String host, String appPath);

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
