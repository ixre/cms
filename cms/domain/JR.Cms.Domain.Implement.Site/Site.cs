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
using JR.Cms.Domain.Implement.Site.Extend;
using JR.Cms.Domain.Implement.Site.Link;
using JR.Cms.Domain.Interface;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Tree;
using JR.Cms.Models;

namespace JR.Cms.Domain.Implement.Site
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
        private ICategory _rootCategory;

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
                            siteAppPath = "/" + this.value.AppPath + "/";
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
            return this.GetAggregaterootId();
        }


        public SiteRunType RunType()
        {
            if (this.runType == SiteRunType.Unknown)
            {
                if (String.IsNullOrEmpty(this.value.AppPath))
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
                    this._categories =  this._categoryRep.GetCategories(this.GetAggregaterootId());
                }
                return this._categories;
            }
        }

        public ICategory RootCategory
        {
            get
            {
                if(this._rootCategory == null)
                {
                    this._rootCategory = this._categoryRep.CreateCategory(new CmsCategoryEntity
                    {
                        SiteId = this.GetAggregaterootId(),
                        Tag = "root",
                        Name = "根栏目",
                    });
                }
                return this._rootCategory;
            }
        }
        

        public ICategory GetCategory(int categoryId)
        {
            foreach (ICategory c in this.Categories)
            {
                if (c.GetDomainId() == categoryId) return c;
            }
            return null;
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

        public Error DeleteCategory(int catId)
        {
            ICategory category = this.GetCategory(catId);
            if (category.Childs.Count() != 0)
            {
                return new Error("栏目包含子栏目!");
            }
            if (this._categoryRep.GetArchiveCount(this.GetAggregaterootId(),category.Get().Path) != 0)
            {
                return new Error("栏目包含文档!");
            }
            foreach (TemplateBind bind in category.GetTemplates())
            {
                this._tempRep.RemoveBind( category.GetDomainId(),bind.BindType);
            }
            this._categoryRep.DeleteCategory(this.GetAggregaterootId(), catId);
            this._categories = null;
            return null;
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
       

        public TreeNode GetCategoryTreeWithRootNode()
        {
            ICategory root = this.RootCategory;
            var node = new TreeNode(this.value.Name, "0", "javascript:void(0);", true, "");
            String nodeValue = this.GetCategoryNodeValue(root);
            var rootNode = new TreeNode(root.Get().Name,nodeValue  ,"javascript:void(0);", true, "");
            node.childs.Add(rootNode);
            ItrNodeTree(rootNode, root);
            return node;
        }

        private String GetCategoryNodeValue(ICategory cat)
        {
            int catId = cat.GetDomainId();
            String path = cat.Get().Path;
            return String.Format("{0}'cid':{1},'path':'{2}'{3}", "{",
                catId.ToString(),path, "}");
        }

        private void ItrNodeTree(TreeNode node, ICategory root)
        {
            IEnumerable<ICategory> list = root.NextLevelChilds.OrderBy(a => a.Get().SortNumber);
            foreach (ICategory c in list)
            {
                String nodeValue = this.GetCategoryNodeValue(c);
                var tNode = new TreeNode(c.Get().Name,nodeValue, "javascript:void(0);", true,"");
                node.childs.Add(tNode);
                ItrNodeTree(tNode, c);
            }
        }

        /// <summary>
        /// 处理站点栏目树形
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="treeHandler"></param>
        public void HandleCategoryTree(int parentId, CategoryTreeHandler treeHandler)
        {
            ICategory root = this.RootCategory;
            int level = 0;
            ItrCategoryTree(root,level,true,treeHandler);
        }

        private void ItrCategoryTree(ICategory root, int level,bool isLast, CategoryTreeHandler handler)
        {
            if (root.GetDomainId() != 0) handler(root, level,isLast);
            IEnumerable<ICategory> list = root.NextLevelChilds.OrderBy(a => a.Get().SortNumber);
            int len = list.Count();
            if (len > 0)
            {
                int i = 0;
                foreach (ICategory c in list)
                {
                    this.ItrCategoryTree(c, level + 1, i++ == len, handler);
                }
            }
        }

        public void ClearSelf()
        {
            this._categories = null;
            this._rootCategory = null;
        }

        public CmsSiteEntity Get()
        {
            return this.value;
        }

        public Error Set(CmsSiteEntity value)
        {
            this.value.AppPath = value.AppPath;
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
