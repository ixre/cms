
/*
* Copyright(C) 2010-2013 S1N1.COM
* 
* File Name	: Site.cs
* publisher_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using System.Collections.Generic;
using System.Text;
using J6.Cms.Domain.Interface.Common.Language;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.Cms.Domain.Interface.Site.Link;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Infrastructure.Tree;

namespace J6.Cms.Domain.Interface.Site
{
    /// <summary>
    /// 站点
    /// </summary>
    public interface ISite:IDomain<int>
    {
        /// <summary>
        /// 站点名称
        /// </summary>
        string Name { get; }


        /// <summary>
        /// 目录名称
        /// </summary>
        string DirName { get; set; }

        /// <summary>
        /// 域名绑定
        /// </summary>
        string Domain { get; set; }

        /// <summary>
        /// 重定向地址
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// 获取基础URL,如:http://z3q.net/cms/.
        /// 如果未绑定域名，则用#代替Host部分,如：
        /// http://#/sub
        /// </summary>
        string FullDomain { get;}

        /// <summary>
        /// 是否为虚拟目录
        /// </summary>
        SiteRunType RunType { get; set; }

        /// <summary>
        /// 站点使用语言
        /// </summary>
        Languages Language { get; set; }


        /// <summary>
        /// 模板
        /// </summary>
        string Tpl { get; set; }

        /// <summary>
        /// 站点备注
        /// </summary>
        string Note { get; set; }

        /// <summary>
        /// 站点状态
        /// </summary>
        SiteState State { get; set; }

        /// <summary>
        /// SEO标题
        /// </summary>
        string SeoTitle { get; set; }

        /// <summary>
        /// SEO关键字
        /// </summary>
        string SeoKeywords { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        string SeoDescription { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        string Tel { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        string Phone { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        string Fax { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// 邮编号码
        /// </summary>
        string PostCode { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// QQ号码
        /// </summary>
        string Im{get;set;}

        /// <summary>
        /// 网站公告
        /// </summary>
        string Notice { get; set; }

        /// <summary>
        /// 网站标语
        /// </summary>
        string Slogan { get; set; }

        /// <summary>
        /// 保存站点并返回编号
        /// </summary>
        int Save();

        /// <summary>
        /// 扩展管理器
        /// </summary>
        IExtendManager GetExtendManager();

        /// <summary>
        /// 链接管理器
        /// </summary>
        ISiteLinkManager GetLinkManager();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IAppUserManager GetUserManager();

        /// <summary>
        /// 分类
        /// </summary>
        IList<ICategory> Categories { get; }

        /// <summary>
        /// 分类根节点
        /// </summary>
        ICategory RootCategory { get; }


        ICategory GetCategory(int categoryId);

        ICategory GetCategoryByTag(string categoryTag);

        /// <summary>
        /// 根据栏目名称获取栏目
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        ICategory GetCategoryByName(string categoryName);

        IEnumerable<ICategory> GetCategories(int lft, int rgt, CategoryContainerOption option);

        ICategory GetCategoryByLft(int lft);

        bool DeleteCategory(int lft);

        /// <summary>
        /// 迭代栏目树
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="categoryLft"></param>
        void ItreCategoryTree(StringBuilder sb, int categoryLft);

        /// <summary>
        /// 处理分类树
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="treeHandler"></param>
        void HandleCategoryTree(int lft, CategoryTreeHandler treeHandler);

        /// <summary>
        /// 获取栏目树
        /// </summary>
        /// <returns></returns>
        TreeNode GetCategoryTree(int lft);

        /// <summary>
        /// 重新加载数据
        /// </summary>
        void ClearSelf();

    }
}
