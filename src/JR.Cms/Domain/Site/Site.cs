using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JR.Cms.Domain.Interface;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Site.Extend;
using JR.Cms.Domain.Site.Link;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Tree;

namespace JR.Cms.Domain.Site
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
        private readonly IUserRepository _userRep;
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
            value = site;
            _siteRepository = siteRepository;
            _categoryRep = categoryRep;
            _extendRepository = extendRepository;
            _tempRep = tempRep;
            _userRep = userRep;
        }


        /// <summary>
        /// 
        /// </summary>
        public string FullDomain
        {
            get
            {
                if (_fullDomain == null)
                {
                    var host = string.IsNullOrEmpty(value.Domain) ? "#" : value.Domain;
                    var appPath = HttpApp.GetApplicationPath();
                    string siteAppPath;

                    switch (RunType())
                    {
                        default:
                        case SiteRunType.Stand:
                            siteAppPath = "/";
                            break;

                        case SiteRunType.VirtualDirectory:
                            siteAppPath = "/" + value.AppPath + "/";
                            break;
                    }


                    _fullDomain = $"//{host}{(appPath == "/" ? "" : appPath)}{siteAppPath}";
                }

                return _fullDomain;
            }
        }

        /// <summary>
        /// 站点使用语言
        /// </summary>
        public Languages Language()
        {
            return (Languages) value.Language;
        }

        /// <summary>
        /// 站点状态
        /// </summary>
        public SiteState State()
        {
            return (SiteState) value.State;
        }


        public int Save()
        {
            var create = GetAggregateRootId() <= 0;
            var siteId = _siteRepository.SaveSite(this);
            value.SiteId = siteId;
            if (create)
            {
                var cat = new CmsCategoryEntity
                {
                    Code = "",
                    Tag = "default",
                    ParentId = 0,
                    SiteId = GetAggregateRootId(),
                    Flag = (int) CategoryFlag.Enabled,
                    Name = "默认栏目",
                    Path = "default",
                    Description = "",
                    Icon = "",
                    Keywords = "",
                    Location = "",
                    SortNumber = 0,
                    Title = ""
                };
                var ic = _categoryRep.CreateCategory(cat);
                var err = ic.Set(cat);
                if (err == null) err = ic.Save();
                if (err != null) throw new Exception("初始化站点目录失败:" + err.Message);
            }

            return GetAggregateRootId();
        }


        public SiteRunType RunType()
        {
            if (runType == SiteRunType.Unknown)
            {
                if (string.IsNullOrEmpty(value.AppPath))
                    runType = SiteRunType.Stand;
                else
                    runType = SiteRunType.VirtualDirectory;
            }

            return runType;
        }

        public IExtendManager GetExtendManager()
        {
            if (this._extendManager == null)
            {
                this._extendManager =  new ExtendManager(_extendRepository, GetAggregateRootId());
            }

            return _extendManager;
        }

        public ISiteLinkManager GetLinkManager()
        {
            if (this._linkManager == null)
            {
                this._linkManager =  new SiteLinkManager(_siteRepository, this);
            }

            return _linkManager;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IAppUserManager GetUserManager()
        {
            if (this._appUserManager == null)
            {
                this._appUserManager = _userRep.GetAppUserManager(GetAggregateRootId());
            }

            return _appUserManager;
        }


        public IList<ICategory> Categories
        {
            get
            {
                if (_categories == null) _categories = _categoryRep.GetCategories(GetAggregateRootId());
                return _categories;
            }
        }

        public ICategory RootCategory
        {
            get
            {
                if (_rootCategory == null)
                    _rootCategory = _categoryRep.CreateCategory(new CmsCategoryEntity
                    {
                        SiteId = GetAggregateRootId(),
                        Tag = "root",
                        Name = "根栏目",
                    });
                return _rootCategory;
            }
        }


        public ICategory GetCategory(int categoryId)
        {
            foreach (var c in Categories)
                if (c.GetDomainId() == categoryId)
                    return c;
            return null;
        }


        public ICategory GetCategoryByPath(string path)
        {
            return _categoryRep.GetCategoryByPath(GetAggregateRootId(), path);
        }


        public ICategory GetCategoryByName(string categoryName)
        {
            foreach (var category in Categories)
                if (string.Compare(category.Get().Name, categoryName, true, CultureInfo.InvariantCulture) == 0)
                    return category;
            return null;
        }

        public Error DeleteCategory(int catId)
        {
            var category = GetCategory(catId);
            if (category == null) return new Error("栏目不存在!");
            if (category.Childs.Count() != 0) return new Error("栏目包含子栏目!");
            if (_categoryRep.GetArchiveCount(GetAggregateRootId(), category.Get().Path) != 0)
                return new Error("栏目包含文档!");
            foreach (var bind in category.GetTemplates()) _tempRep.RemoveBind(category.GetDomainId(), bind.BindType);
            _categoryRep.DeleteCategory(GetAggregateRootId(), catId);
            ClearSelf();
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
            var root = RootCategory;
            var node = new TreeNode(value.Name, "0", "javascript:void(0);", true, "");
            var nodeValue = GetCategoryNodeValue(root);
            var rootNode = new TreeNode(root.Get().Name, nodeValue, "javascript:void(0);", true, "");
            node.childs.Add(rootNode);
            ItrNodeTree(rootNode, root);
            return node;
        }

        private string GetCategoryNodeValue(ICategory cat)
        {
            var catId = cat.GetDomainId();
            var path = cat.Get().Path;
            return string.Format("{0}'cid':{1},'path':'{2}'{3}", "{",
                catId.ToString(), path, "}");
        }

        private void ItrNodeTree(TreeNode node, ICategory root)
        {
            IEnumerable<ICategory> list = root.NextLevelChilds.OrderBy(a => a.Get().SortNumber);
            foreach (var c in list)
            {
                var nodeValue = GetCategoryNodeValue(c);
                var tNode = new TreeNode(c.Get().Name, nodeValue, "javascript:void(0);", true, "");
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
            var root = RootCategory;
            var level = 0;
            ItrCategoryTree(root, level, true, treeHandler);
        }

        private void ItrCategoryTree(ICategory root, int level, bool isLast, CategoryTreeHandler handler)
        {
            if (root.GetDomainId() != 0) handler(root, level, isLast);
            IEnumerable<ICategory> list = root.NextLevelChilds.OrderBy(a => a.Get().SortNumber);
            var len = list.Count();
            if (len > 0)
            {
                var i = 0;
                foreach (var c in list) ItrCategoryTree(c, level + 1, i++ == len, handler);
            }
        }

        public void ClearSelf()
        {
            _categories = null;
            _rootCategory = null;
        }

        public CmsSiteEntity Get()
        {
            return value;
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

        public int GetAggregateRootId()
        {
            return value.SiteId;
        }
    }
}