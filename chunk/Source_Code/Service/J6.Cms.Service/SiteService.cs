using J6.Cms.DataTransfer;
using J6.Cms.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using J6.Cms.Domain.Interface;
using J6.Cms.Domain.Interface.Site;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.Cms.Domain.Interface.Site.Link;
using J6.Cms.Domain.Interface.Site.Template;
using J6.Cms.Infrastructure.Tree;
using J6.DevFw.Framework.Extensions;

namespace J6.Cms.Service
{
    public class SiteService : ISiteServiceContract
    {
        private ISiteRepository _resp;
        private IExtendFieldRepository _extendResp;
        private ICategoryRepository _categoryRep;
        private ITemplateRepository _tempRep;

        public SiteService(ISiteRepository resp,
            ICategoryRepository categoryRep,
            IExtendFieldRepository extendFieldPository,
            ITemplateRepository tempRep)
        {
            this._resp = resp;
            this._extendResp = extendFieldPository;
            this._categoryRep = categoryRep;
            this._tempRep = tempRep;
        }

        public int SaveSite(SiteDto site)
        {
            if (site.SiteId != 0 && _resp.GetSites().SingleOrDefault(a => a.Id == site.SiteId) == null)
            {
                site.SiteId = 0;
            }
            ISite _site = _resp.CreateSite(site.SiteId, site.Name);
            _site.CloneData(site);
            return _site.Save();
        }


        public IList<DataTransfer.SiteDto> GetSites()
        {
            IList<SiteDto> siteDtos = new List<SiteDto>();
            IList<ISite> sites = _resp.GetSites();
            foreach (ISite site in sites)
            {
                siteDtos.Add(SiteDto.ConvertFromSite(site));
            }
            return siteDtos;
        }



        public SiteDto GetSiteByUri(Uri uri)
        {
            return getSiteDtoFromISite(_resp.GetSiteByUri(uri));
        }

        private static SiteDto getSiteDtoFromISite(ISite site)
        {
            if (site == null)
                throw new Exception("站点不存在！");
            return SiteDto.ConvertFromSite(site);
        }



        public SiteDto GetSingleOrDefaultSite(Uri uri)
        {
            return getSiteDtoFromISite(
                _resp.GetSingleOrDefaultSite(uri)
                );

        }


        public SiteDto GetSiteById(int siteId)
        {
            return getSiteDtoFromISite(
                    this._resp.GetSiteById(siteId)
                );
        }


        public IEnumerable<ExtendFieldDto> GetExtendFields(int siteId)
        {
            ISite site = this._resp.GetSiteById(siteId);
            ExtendFieldDto dto;
            IEnumerable<IExtendField> extends = site.Extend.GetAllExtends();
            foreach (IExtendField extend in extends)
            {
                dto = new ExtendFieldDto().CloneData(extend);
                dto.Id = extend.Id;
                yield return dto;
            }
        }


        public int SaveExtendField(int siteId, ExtendFieldDto dto)
        {
            ISite site = this._resp.GetSiteById(siteId);
            if (site == null)
                throw new Exception("站点不存在");

            IExtendField field = this._extendResp.CreateExtendField(dto.Id, dto.Name);
            field.CloneData(dto);
            return site.Extend.SaveExtendField(field);
        }


        public CategoryDto GetCategory(int siteId, int categoryId)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return CategoryDto.ConvertFrom(site.GetCategory(categoryId));
        }


        public CategoryDto GetCategoryByName(int siteId, string categoryName)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return CategoryDto.ConvertFrom(site.GetCategoryByName(categoryName));
        }

        public CategoryDto GetCategory(int siteId, string categoryTag)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return CategoryDto.ConvertFrom(site.GetCategoryByTag(categoryTag));
        }



        public IEnumerable<CategoryDto> GetCategories(int siteId)
        {
            IList<SiteDto> siteDtos = new List<SiteDto>();
            ISite site = this._resp.GetSiteById(siteId);
            IList<ICategory> categories = site.Categories;
            foreach (ICategory category in categories)
            {
                yield return CategoryDto.ConvertFrom(category);
            }
        }

        public IEnumerable<CategoryDto> GetCategories(
            int siteId, int lft, int rgt,
            CategoryContainerOption categoryContainerOption)
        {
            ISite site = this._resp.GetSiteById(siteId);
            IEnumerable<ICategory> categories =
                site.GetCategories(lft, rgt, categoryContainerOption);
            CategoryDto dto;
            foreach (ICategory category in categories)
            {
                yield return CategoryDto.ConvertFrom(category);
            }
        }

        public CategoryDto GetCategoryByLft(int siteId, int lft)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return CategoryDto.ConvertFrom(site.GetCategoryByLft(lft));
        }

        public bool DeleteCategoryByLft(int siteId, int lft)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return site.DeleteCategory(lft);
        }

        public void ItrCategoryTree(StringBuilder sb, int siteId, int categoryLft)
        {
            ISite site = this._resp.GetSiteById(siteId);
            site.ItreCategoryTree(sb, categoryLft);
        }



        public void HandleCategoryTree(int siteId, int lft, CategoryTreeHandler treeHandler)
        {
            ISite site = this._resp.GetSiteById(siteId);
            site.HandleCategoryTree(lft, treeHandler);
        }


        public int SaveCategory(int siteId, int parentLft, CategoryDto category)
        {
            ISite site = this._resp.GetSiteById(siteId);
            ICategory ic = site.GetCategoryByLft(category.Lft);
            if (ic == null) ic = _categoryRep.CreateCategory(-1, site);
            //ic.CloneData(category);
            ic.Id = category.Id;
            ic.Keywords = category.Keywords;
            ic.Description = category.Description;
            ic.Tag = category.Tag;
            ic.Icon = category.Icon;
            ic.Name = category.Name;
            ic.PageTitle = category.PageTitle;
            ic.OrderIndex = category.OrderIndex;
            ic.ModuleId = category.ModuleId;
            ic.Location = category.Location;

            bool isExistCategoryBind = false;
            bool isExistArchiveBind = false;

            #region 模板绑定

            foreach (ITemplateBind tempBind in ic.Templates)
            {
                if (tempBind.BindType == TemplateBindType.CategoryTemplate)
                {
                    isExistCategoryBind = true;
                    tempBind.TplPath = category.CategoryTemplate;
                }
                else if (tempBind.BindType == TemplateBindType.CategoryArchiveTemplate)
                {
                    isExistArchiveBind = true;
                    tempBind.TplPath = category.CategoryArchiveTemplate;
                }
            }

            //栏目模板
            if (!isExistCategoryBind && !String.IsNullOrEmpty(category.CategoryTemplate))
            {
                ITemplateBind bind = this._tempRep.CreateTemplateBind(-1,
                        TemplateBindType.CategoryTemplate,
                        category.CategoryTemplate);

                bind.BindRefrenceId = ic.Id;
                ic.Templates.Add(bind);
            }

            //栏目文档模板
            if (!isExistArchiveBind && !String.IsNullOrEmpty(category.CategoryArchiveTemplate))
            {
                ITemplateBind bind = this._tempRep.CreateTemplateBind(-1,
                        TemplateBindType.CategoryArchiveTemplate,
                        category.CategoryArchiveTemplate);

                bind.BindRefrenceId = ic.Id;

                ic.Templates.Add(bind);
            }
            #endregion

            #region 扩展属性

            if (category.ExtendFields != null)
            {
                ic.ExtendFields = category.ExtendFields;
            }

            #endregion

            if (parentLft >= 0) ic.Parent = site.GetCategoryByLft(parentLft);
            return ic.Save();
        }


        public CategoryDto GetParentCategory(int siteId, int lft)
        {
            ICategory category = this._resp.GetSiteById(siteId).GetCategoryByLft(lft);
            if (category == null || category.Parent == null)
                return default(CategoryDto);
            return CategoryDto.ConvertFrom(category.Parent);

        }


        public TreeNode GetCategoryTreeNode(int siteId, int lft)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return site.GetCategoryTree(lft);
        }


        public string GetCategorySitemapHtml(int siteId, string categoryTag, string split, string linkFormat)
        {
            string categoryPath = null;
            int rootLft;
            ICategory parentCategory;
            ISite site = this._resp.GetSiteById(siteId);
            ICategory category = site.GetCategoryByTag(categoryTag);
            if (category == null) throw new Exception("栏目不存在");
            rootLft = site.RootCategory.Lft;

            string html = "";
            StringBuilder sb = new StringBuilder();
            int tmpInt = 0;

            while (category != null && category.Lft != rootLft)
            {
                sb.Remove(0, sb.Length);

                sb.Append("<a href=\"")
                    .Append(String.Format(linkFormat, category.UriPath))
                    .Append("\"");

                //栏目的上一级添加不追踪特性
                if (++tmpInt > 1)
                {
                    sb.Append(" rel=\"nofollow\"");
                }
                sb.Append(">").Append(category.Name).Append("</a>");


                //添加分隔符
                if (tmpInt != 1)
                {
                    sb.Append(split);
                }

                html = sb.ToString() + html;

                category = category.Parent;
            }

            //去掉双斜杠
            // Regex reg = new Regex("\\b//");
            // return reg.Replace(sb.ToString(), "/");

            return html;
        }


        public bool CheckCategoryTagAvailable(int siteId, int categoryId, string categoryTag)
        {
            ISite site = this._resp.GetSiteById(siteId);
            ICategory category = site.GetCategoryByTag(categoryTag);

            if (category == null)
                return true;
            //
            //            if (categoryId <= 0)
            //            {
            //                return false;
            //            }
            return category.Id == categoryId;

        }


        public IEnumerable<SiteLinkDto> GetLinksByType(int siteId, SiteLinkType type, bool ignoreDisabled)
        {
            ISite site = this._resp.GetSiteById(siteId);
            IEnumerable<ISiteLink> links = site.LinkManager.GetLinks(type);
            foreach (ISiteLink link in links)
            {
                if (!ignoreDisabled && !link.Visible) continue;

                yield return SiteLinkDto.ConvertFrom(link);
            }
        }


        public void DeleteLink(int siteId, int linkId)
        {
            ISite site = this._resp.GetSiteById(siteId);

            site.LinkManager.DeleteLink(linkId);
        }


        public int SaveLink(int siteId, SiteLinkDto dto)
        {
            ISite site = this._resp.GetSiteById(siteId);
            ISiteLink link = null;
            if (dto.Id <= 0)
            {
                link = this._resp.CreateLink(site, 0, dto.Text);
            }
            else
            {
                link = site.LinkManager.GetLinkById(dto.Id);
            }

            link.Bind = dto.Bind;
            link.ImgUrl = dto.ImgUrl;
            link.OrderIndex = dto.OrderIndex;
            link.Pid = dto.Pid;
            link.Target = dto.Target;
            link.Text = dto.Text;
            link.Type = dto.Type;
            link.Uri = dto.Uri;
            link.Visible = dto.Visible;

            return link.Save();
        }

        public SiteLinkDto GetLinkById(int siteId, int linkId)
        {
            ISite site = this._resp.GetSiteById(siteId);
            ISiteLink link = site.LinkManager.GetLinkById(linkId);
            if (link == null) return default(SiteLinkDto);

            return SiteLinkDto.ConvertFrom(link);
        }
    }
}
