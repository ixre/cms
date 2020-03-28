using System;
using System.Collections.Generic;
using System.Text;
using JR.Cms.Domain.Interface;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Infrastructure.Tree;
using JR.Cms.Infrastructure;
using CategoryDto = JR.Cms.ServiceDto.CategoryDto;
using ExtendFieldDto = JR.Cms.ServiceDto.ExtendFieldDto;
using Result = JR.Cms.ServiceDto.Result;
using SiteDto = JR.Cms.ServiceDto.SiteDto;
using SiteLinkDto = JR.Cms.ServiceDto.SiteLinkDto;

namespace JR.Cms.ServiceContract
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
        /// 获取单个或默认站点
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        SiteDto GetSingleOrDefaultSite(string host, string appPath);

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
        Result SaveExtendField(int siteId, ExtendFieldDto dto);

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
        /// <param name="catPath"></param>
        /// <returns></returns>
        IEnumerable<CategoryDto> GetCategories(int siteId, string catPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <returns></returns>
        CategoryDto GetCategory(int siteId, string catPath);


        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        Error DeleteCategory(int siteId, int catId);

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
        /// <param name="parentId"></param>
        /// <param name="treeHandler"></param>
        void HandleCategoryTree(int siteId, int parentId, CategoryTreeHandler treeHandler);

        /// <summary>
        /// 保存分类，并返回分类的编号
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="parentId">父节点编号</param>
        /// <param name="category"></param>
        /// <returns></returns>
        Result SaveCategory(int siteId, int parentId, CategoryDto category);

        /// <summary>
        /// 获取栏目树，包含根节点
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        TreeNode GetCategoryTreeWithRootNode(int siteId);

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


        IList<RoleValue> GetAppRoles(int siteId);

        /// <summary>
        /// 克隆栏目
        /// </summary>
        /// <param name="fromSiteId"></param>
        /// <param name="toSiteId"></param>
        /// <param name="fromCid"></param>
        /// <param name="toCid"></param>
        /// <param name="includeChild"></param>
        /// <param name="includeExtend"></param>
        /// <param name="includeTemplateBind"></param>
        //todo: 新建extend设计为单独同步
        void CloneCategory(int fromSiteId, int toSiteId, int fromCid, int toCid, bool includeChild, bool includeExtend,
            bool includeTemplateBind);

        /// <summary>
        /// 克隆文档
        /// </summary>
        /// <param name="sourceSiteId"></param>
        /// <param name="targetSiteId"></param>
        /// <param name="toCid"></param>
        /// <param name="archiveIdArray"></param>
        /// <param name="includeExtend"></param>
        /// <param name="includeTempateBind"></param>
        /// <param name="includeRelatedLink"></param>
        IDictionary<int, string> ClonePubArchive(int sourceSiteId, int targetSiteId, int toCid, int[] archiveIdArray,
            bool includeExtend, bool includeTempateBind, bool includeRelatedLink);

        bool CheckSiteExists(int siteId);

        /// <summary>
        /// 移动栏目排序编号
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="direction"></param>
        void MoveCategorySortNumber(int siteId, int id, int direction);
    }
}