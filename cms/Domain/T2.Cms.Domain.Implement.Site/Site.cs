/**
 * Copyright (C) 2007-2015 TO2.NET,All rights reseved.
 * Get more infromation of this software,please visit site http://to2.net/cms
 * 
 * name : Site.cs
 * author : newmin (new.min@msn.com)
 * date : 2014/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using T2.Cms.Domain.Implement.Site.Extend;
using T2.Cms.Domain.Implement.Site.Link;
using T2.Cms.Domain.Interface;
using T2.Cms.Domain.Interface.Common.Language;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Link;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Domain.Interface.User;
using T2.Cms.Infrastructure;
using T2.Cms.Infrastructure.Tree;
using T2.Cms.Models;

namespace T2.Cms.Domain.Implement.Site
{
    internal class Site : ISite
    {
        private readonly ISiteRepo _siteRepository;
        private readonly IExtendFieldRepository _extendRepository;
        private readonly ICategoryRepo _categoryRep;
        private IExtendManager _extendManager;
        private IList<ICategory> _categories;
        private readonly ITemplateRepo _tempRep;
        private ISiteLinkManager _linkManager;
        private string _fullDomain;
        private IAppUserManager _appUserManager;
        private IUserRepository _userRep;
        private CmsSiteEntity value;
        private SiteRunType runType = SiteRunType.Unknown;

        internal Site(ISiteRepo siteRepository,
            IExtendFieldRepository extendRepository,
            ICategoryRepo categoryRep,
            ITemplateRepo tempRep,
            IUserRepository userRep,
            CmsSiteEntity site)
        {
            this.value = site;
            this._siteRepository = siteRepository;
            this._categoryRep = categoryRep;
            this._extendRepository = extendRepository;
            this._tempRep = tempRep;
            this._userRep = userRep;
        }


        /// <summary>
        /// 
        /// </summary>
        public string FullDomain
        {
            get
            {
                if (this._fullDomain == null)
                {
                    string host = String.IsNullOrEmpty(this.value.Domain) ? "#" : this.value.Domain;
                    string appPath = HttpApp.GetApplicationPath();
                    string siteAppPath;

                    switch (this.RunType())
                    {
                        default:
                        case SiteRunType.Stand:
                            siteAppPath = "/";
                            break;

                        case SiteRunType.VirtualDirectory:
                            siteAppPath = "/" + this.value.AppName + "/";
                            break;
                    }


                    this._fullDomain = String.Format("//{0}{1}{2}",
                        host,
                        appPath == "/" ? "" : appPath,
                        siteAppPath
                        );
                }

                return this._fullDomain;
            }
        }

        /// <summary>
        /// 站点使用语言
        /// </summary>
        public Languages Language()
        {
            return (Languages)this.value.Language;
        }

        /// <summary>
        /// 站点状态
        /// </summary>
        public SiteState State()
        {
            return (SiteState)this.value.State;
        }



        public int Save()
        {
            bool create = this.GetAggregaterootId() <= 0;
            int siteId = _siteRepository.SaveSite(this);
            this.value.SiteId = siteId;
            if (create)
            {
                this.initSiteCategories();
            }
            return this.GetAggregaterootId();
        }


        public SiteRunType RunType()
        {
            if (this.runType == SiteRunType.Unknown)
            {
                if (String.IsNullOrEmpty(this.value.AppName))
                {
                    this.runType = SiteRunType.Stand;
                }
                else
                {
                    this.runType = SiteRunType.VirtualDirectory;
                }
            }
            return this.runType;
        }

        public IExtendManager GetExtendManager()
        {
            return _extendManager ?? (_extendManager = new ExtendManager(this._extendRepository, this.GetAggregaterootId()));
        }

        public ISiteLinkManager GetLinkManager()
        {
            return this._linkManager ?? (this._linkManager = new SiteLinkManager(this._siteRepository, this));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IAppUserManager GetUserManager()
        {
            return this._appUserManager ?? (this._appUserManager = this._userRep.GetAppUserManager(this.GetAggregaterootId()));
        }


        public IList<ICategory> Categories
        {
            get
            {
                if (this._categories == null)
                {
                    reload:
                    var categories =  this._categoryRep.GetCategories(this.GetAggregaterootId());
                    if (categories == null || categories.Count == 0)
                    {
                        this.initSiteCategories();
                        goto reload;
                    }
                    this._categories = categories;
                }
                return this._categories;
            }
        }

        private void initSiteCategories()
        {
            ICategory ic = this._categoryRep.CreateCategory(new CmsCategoryEntity());
            CmsCategoryEntity cat = new CmsCategoryEntity();
            cat.Name = "根栏目";
            cat.Tag = "root";
            cat.SortNumber = 1;
            cat.ParentId = 0;
            cat.Description = "";
            cat.Icon = "";
            cat.Keywords = "";
            cat.Location = "";
            cat.Title = "";
            Error err = ic.Set(cat);
            ic.Save();
        }

        public ICategory RootCategory
        {
            get
            {
                //NOTO:应为Lft最小的一个，但分类已按Lft排序，所以获取第一个
                ICategory category = this.Categories.FirstOrDefault();
                if (category == null)
                {
                    throw new Exception("站点栏目信息异常!");
                }
                return category;
            }
        }
        

        public ICategory GetCategory(int categoryId)
        {
            int lft = this._categoryRep.GetCategoryLftById(this.GetAggregaterootId(), categoryId);
            if (lft > 0)
            {
                return BinarySearch.IntSearch(this.Categories, 0, this.Categories.Count, lft, a => a.Lft);
            }

            foreach (ICategory c in this.Categories)
            {
                if (c.GetDomainId() == categoryId) return c;
            }
            return null;
        }



        public IEnumerable<ICategory> GetCategories(int catId, CategoryContainerOption option)
        {
            return this._categoryRep.GetCategories(this.GetAggregaterootId(),catId, option);
        }

        public ICategory GetCategoryByPath(string path)
        {
            return this._categoryRep.GetCategoryByPath(this.GetAggregaterootId(), path);
        }




        public ICategory GetCategoryByName(string categoryName)
        {
            foreach (ICategory category in this.Categories)
            {
                if (String.Compare(category.Get().Name, categoryName, true, CultureInfo.InvariantCulture) == 0) return category;
            }
            return null;
        }
        public ICategory GetCategoryByLft(int lft)
        {
            return BinarySearch.IntSearch(this.Categories, 0, this.Categories.Count, lft, a => a.Lft);
        }

        public bool DeleteCategory(int lft)
        {
            ICategory category = this.GetCategoryByLft(lft);

            if (category.Childs.Count() != 0)
            {
                throw new Exception("栏目包含子栏目!");
            }

            if (this._categoryRep.GetArchiveCount(this.GetAggregaterootId(), lft, category.Rgt) != 0)
            {
                throw new Exception("栏目包含文档!");
            }

            this._categoryRep.DeleteCategory(this.GetAggregaterootId(), lft, category.Rgt);

            foreach (TemplateBind bind in category.GetTemplates())
            {
                this._tempRep.RemoveBind( category.GetDomainId(),bind.BindType);
            }
            this._categories = null;

            return true;
        }


        public void ItreCategoryTree(StringBuilder sb, int catId)
        {
            /*

            //
            //TODO:需要重构
            //

            ICategory category = this.GetCategory(catId);
            int level = this.GetCategories(catId, CategoryContainerOption.Parents).Count();

            //if (level >= 2) return;  //到2级就跳过了
            bool iscollage = level >= 2;

           // if (lft != 1) sb.Append("<dd>");

            IEnumerable<ICategory> cates = this.GetCategories(catId, CategoryContainerOption.NextLevel);
            ICategory nextCategory = category.Next;
            ICategory tempCategory = null;
            ICategory parentNextCategory = null;
            string className = "";

            //获取父类及父类的下一个类目
            if (lft != 1)
            {
                tempCategory = category.Parent;
                parentNextCategory = tempCategory.Next;
            }


            var categories = cates as ICategory[] ?? cates.ToArray();
            if (categories.Count() != 0)
            {
                if (lft != 1)
                {
                    sb.Append("<dl><dt")
                        .Append(" lft=\"").Append(lft.ToString()).Append("\" level=\"")
                        .Append(level.ToString()).Append("\">");

                    for (int i = 0; i < level; i++)
                    {
                        //最后一个竖线不显示
                        if (i != 0 && nextCategory == null
                            && (i != level - 1 || (i == level - 1 && parentNextCategory == null))
                            && category.Childs.Count() == 0
                            )
                        {
                            sb.Append("<img src=\"public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
                        }
                        else
                        {
                            sb.Append("<img class=\"tree-line\" src=\"public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
                        }
                    }

                    //tree-expand:已展开
                    //tree-expand-last:已经展开最后一个

                    //tree-collage:未开展
                    //tree-collage:未展开最后一个

                    if (level == 0)
                    {
                        if (nextCategory == null)
                        {
                            className = "tree-expand tree-expand-last";
                        }
                        else
                        {
                            className = "tree-expand";
                        }
                    }
                    else
                    {
                        if (nextCategory == null)
                        {
                            className = "tree-collage tree-collage-last";
                        }
                        else
                        {
                            className = "tree-collage";
                        }
                    }


                    sb.Append("<img class=\"").Append(className)
                    .Append("\" src=\"public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/><span class=\"txt parent\" cid=\"")
                    .Append(category.GetDomainId().ToString()).Append("\" lft=\"")
                    .Append(category.Lft.ToString()).Append("\">")
                    .Append(category.Get().Name).Append("</span></dt>");
                }

                foreach (var c in categories)
                {
                    this.ItreCategoryTree(sb, c.Lft);
                }
                if (lft != 1)
                {
                    sb.Append("</dl>");
                }
            }
            else
            {
                if (lft != 1)
                {
                    for (int i = 0; i < level; i++)
                    {
                        if (i != 0 && i == level - 1 && nextCategory == null)
                        {
                            sb.Append("<img src=\"public/mui/css/old/sys_themes/default/icon_trans.png\"/>");
                        }
                        else if (parentNextCategory != null)
                        {
                            sb.Append("<img class=\"tree-line\" src=\"public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
                        }
                        else
                        {
                            sb.Append("<img src=\"public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
                        }
                    }

                    sb.Append("<img class=\"tree-item\" src=\"public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/><span class=\"txt archvie\" cid=\"")
                        .Append(category.GetDomainId().ToString()).Append("\">").Append(category.Get().Name).Append("</span>");
                }
            }

            if (lft != 1)
            {
                sb.Append("</dd>");
            }
            */
        }


        public void HandleCategoryTree(int parentId, CategoryTreeHandler treeHandler)
        {
            IList<int> arr = new List<int>();
           
            //获取root节点的所有子节点
            IEnumerable <ICategory> childNodes = this.GetCategories(parentId, CategoryContainerOption.Childs);
            /* SELECT * FROM tree WHERE lft BETWEEN @rootLft AND @rootRgt ORDER BY lft ASC'); */
            int tmpInt = 0;
            var categories = childNodes as ICategory[] ?? childNodes.ToArray();
            int totalInt = categories.Count();

            foreach (ICategory c in categories)
            {
                if (arr.Count > 0)
                {
                    //判断最后一个值是否小于
                    int i;
                    while ((i = arr[arr.Count - 1]) < c.Rgt)
                    {
                        arr.Remove(i);
                        if (arr.Count == 0) break;
                    }
                }
                //树的层级= 列表arr的数量
                treeHandler(c, arr.Count,++tmpInt ==totalInt );

                //把所有栏目的右值,再加入到列表中
                arr.Add(c.Rgt);
            }
            
        }

        /// <summary>
        /// 获取栏目树Json格式
        /// </summary>
        /// <returns></returns>
        public TreeNode GetCategoryTree(int lft)
        {
            ICategory root = lft == 1 ?
                this.RootCategory :
                this.GetCategoryByLft(lft);

            var node = lft == 1 ?
                new TreeNode(this.value.Name, "1", "javascript:;", true, "") :
                new TreeNode(root.Get().Name, String.Format("{0}cid:{1},lft:1{2}", "{", root.GetDomainId().ToString(), "}"),
                    "javascript:;", true, "");

            ItrNodeTree(node, root);

            return node;
        }



        public TreeNode GetCategoryTreeWithRootNode()
        {
            ICategory root = this.RootCategory;
            var node = new TreeNode(this.value.Name, "0", "javascript:;", true, "");
            var rootNode = new TreeNode(root.Get().Name,
                String.Format("{0}cid:{1},lft:{2}{3}", "{", root.GetDomainId().ToString(), root.Lft.ToString(), "}"),
                "javascript:;", true, "");
            node.childs.Add(rootNode);
            ItrNodeTree(rootNode, root);
            return node;
        }

        private void ItrNodeTree(TreeNode node, ICategory root)
        {
            IEnumerable<ICategory> list = root.NextLevelChilds.OrderBy(a => a.Get().SortNumber);
            foreach (ICategory c in list)
            {
                var tNode = new TreeNode(c.Get().Name,
                    String.Format("{0}cid:{1},lft:{2}{3}", "{", c.GetDomainId().ToString(), c.Lft.ToString(), "}"),
                    "javascript:;", true,
                    "");
                node.childs.Add(tNode);
                ItrNodeTree(tNode, c);
            }
        }

        public void ClearSelf()
        {
            this._categories = null;
        }

        public CmsSiteEntity Get()
        {
            return this.value;
        }

        public Error Set(CmsSiteEntity value)
        {
            this.value.AppName = value.AppName;
            return null;
        }

        public void SetRunType(SiteRunType runType)
        {
            this.runType = runType;
        }

        public int GetAggregaterootId()
        {
            return this.value.SiteId;
        }
    }
}
