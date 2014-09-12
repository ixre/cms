using Ops.Cms.DataTransfer;
using Ops.Cms.Domain.Interface;
using Ops.Cms.Domain.Interface.Site.Category;
using Ops.Cms.Domain.Interface.Site.Link;
using Ops.Cms.Infrastructure.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ops.Cms.ServiceContract
{
    public interface ISiteServiceContract
    {
        /// <summary>
        /// 创建站点
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        int SaveSite(SiteDto site);

        /// <summary>
        /// 获取所有站点
        /// </summary>
        /// <returns></returns>
        IList<SiteDto> GetSites();

        /// <summary>
        /// 根据编号获取网站
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        SiteDto GetSiteById(int siteId);

        /// <summary>
        /// 根据Uri获取站点
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        SiteDto GetSiteByUri(Uri uri);

        /// <summary>
        /// 获取单个或默认站点
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        SiteDto GetSingleOrDefaultSite(Uri uri);

        /// <summary>
        /// 获取站点的扩展字段
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        IEnumerable<ExtendFieldDto> GetExtendFields(int siteId);

        /// <summary>
        /// 保存扩展字段
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        int SaveExtendField(int siteId,ExtendFieldDto dto);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        CategoryDto GetCategory(int siteId, int categoryId);

        /// <summary>
        /// 根据栏目名称获取栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        CategoryDto GetCategoryByName(int siteId, string categoryName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        IEnumerable<CategoryDto> GetCategories(int siteId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="categoryContainerOption"></param>
        /// <returns></returns>
        IEnumerable<CategoryDto> GetCategories(int siteId,int lft, int rgt, 
            CategoryContainerOption categoryContainerOption);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        CategoryDto GetCategory(int siteId, string tag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <returns></returns>
        CategoryDto GetCategoryByLft(int siteId, int lft);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <returns></returns>
        bool DeleteCategoryByLft(int siteId,int lft);

        /// <summary>
        /// 迭代栏目树
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="siteId"></param>
        /// <param name="categoryLft"></param>
        void ItrCategoryTree(StringBuilder sb, int siteId, int categoryLft);

        /// <summary>
        /// 处理树型栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <param name="treeHandler"></param>
        void HandleCategoryTree(int siteId,int lft, CategoryTreeHandler treeHandler);

        /// <summary>
        /// 保存分类，并返回分类的编号
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="parentLft">新的父节点编号(系统默认为-1)</param>
        /// <param name="category"></param>
        /// <returns></returns>
        int SaveCategory(int siteId, int parentLft, CategoryDto category);

        /// <summary>
        /// 获取父栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <returns></returns>
        CategoryDto GetParentCategory(int siteId, int lft);

        /// <summary>
        /// 获取栏目树
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <returns></returns>
        TreeNode GetCategoryTreeNode(int siteId, int lft);

        /// <summary>
        /// 获取站点地图HTML
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryTag"></param>
        /// <param name="split"></param>
        /// <param name="linkFormat"></param>
        /// <returns></returns>
        string GetCategorySitemapHtml(int siteId, string categoryTag, string split, string linkFormat);

        ///// <summary>
        ///// 更新分类的扩展属性
        ///// </summary>
        ///// <param name="siteId"></param>
        ///// <param name="categoryId"></param>
        ///// <param name="extendIds"></param>
        //void UpdateCategoryExtendFields(int siteId, int categoryId, int[] extendIds);

        /// <summary>
        /// 检验栏目Tag是否可用
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId">默认为-1</param>
        /// <param name="categoryTag"></param>
        /// <returns></returns>
        bool CheckCategoryTagAvailable(int siteId, int categoryId, string categoryTag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="type"></param>
        /// <param name="ignoreDisabled"></param>
        /// <returns></returns>
        IEnumerable<SiteLinkDto> GetLinksByType(int siteId, SiteLinkType type, bool ignoreDisabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="linkId"></param>
        void DeleteLink(int siteId, int linkId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        int SaveLink(int siteId, SiteLinkDto link);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="linkId"></param>
        /// <returns></returns>
        SiteLinkDto GetLinkById(int siteId, int linkId);
    }
}
