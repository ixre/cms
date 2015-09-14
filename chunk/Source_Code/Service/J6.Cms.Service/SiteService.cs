using J6.Cms.DataTransfer;
using J6.Cms.ServiceContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using J6.Cms.Domain.Interface;
using J6.Cms.Domain.Interface.Site;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.Cms.Domain.Interface.Site.Link;
using J6.Cms.Domain.Interface.Site.Template;
using J6.Cms.Domain.Interface.User;
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

        public int SaveSite(SiteDto siteDto)
        {
            ISite site;
            if (siteDto.SiteId != 0)
            {
                site = _resp.GetSiteById(siteDto.SiteId);
                if (site == null)
                {
                    throw new ArgumentException("No such site");
                }
            }
            else
            {
                site = _resp.CreateSite(siteDto.SiteId, siteDto.Name);
            }

            SiteDto.CopyTo(siteDto,site);
            return site.Save();
        }


        public IList<SiteDto> GetSites()
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
            return GetSiteDtoFromISite(_resp.GetSiteByUri(uri));
        }

        private static SiteDto GetSiteDtoFromISite(ISite site)
        {
            if (site != null)
            {
                return SiteDto.ConvertFromSite(site);
            }
            return default(SiteDto);
        }



        public SiteDto GetSingleOrDefaultSite(Uri uri)
        {
            return GetSiteDtoFromISite(
                _resp.GetSingleOrDefaultSite(uri)
                );

        }


        public SiteDto GetSiteById(int siteId)
        {
            return GetSiteDtoFromISite(
                    this._resp.GetSiteById(siteId)
                );
        }


        public IEnumerable<ExtendFieldDto> GetExtendFields(int siteId)
        {
            ISite site = this._resp.GetSiteById(siteId);
            ExtendFieldDto dto;
            IEnumerable<IExtendField> extends = site.GetExtendManager().GetAllExtends();
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
            return site.GetExtendManager().SaveExtendField(field);
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
            ICategory ic = null;
            if (category.Lft > 0)
            {
                ic = site.GetCategoryByLft(category.Lft);
            }
            if (ic == null) ic = _categoryRep.CreateCategory(-1, site);
            ic.Id = category.Id;
            ic.Keywords = category.Keywords;
            ic.Description = category.Description;
            ic.Tag = category.Tag;
            ic.Icon = category.Icon;
            ic.Name = category.Name;
            ic.PageTitle = category.PageTitle;
            ic.SortNumber = category.SortNumber;
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

        public TreeNode GetCategoryTreeWithRootNode(int siteId)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return site.GetCategoryTreeWithRootNode();
        }


        public string GetCategorySitemapHtml(int siteId, string categoryTag, string split, string linkFormat)
        {
            ISite site = this._resp.GetSiteById(siteId);
            int rootLft =  site.RootCategory.Lft;
            ICategory category = site.GetCategoryByTag(categoryTag);

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
            IEnumerable<ISiteLink> links = site.GetLinkManager().GetLinks(type);
            foreach (ISiteLink link in links)
            {
                if (!ignoreDisabled && !link.Visible) continue;

                yield return SiteLinkDto.ConvertFrom(link);
            }
        }


        public void DeleteLink(int siteId, int linkId)
        {
            ISite site = this._resp.GetSiteById(siteId);

            site.GetLinkManager().DeleteLink(linkId);
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
                link = site.GetLinkManager().GetLinkById(dto.Id);
            }

            link.Bind = dto.Bind;
            link.ImgUrl = dto.ImgUrl;
            link.SortNumber = dto.SortNumber;
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
            ISiteLink link = site.GetLinkManager().GetLinkById(linkId);
            if (link == null) return default(SiteLinkDto);

            return SiteLinkDto.ConvertFrom(link);
        }

        public IList<RoleValue> GetAppRoles(int siteId)
        {
            ISite site = this._resp.GetSiteById(siteId);
            return site.GetUserManager().GetAppRoles();
        }


        public void CloneCategory(int sourceSiteId, int targetSiteId, int fromCid, int toCid,
            bool includeChild,bool includeExtend,bool includeTemplateBind)
        {
            ISite fromSite = this._resp.GetSiteById(sourceSiteId);
            ISite toSite = this._resp.GetSiteById(targetSiteId);
            if (fromSite == null || toSite == null)
            {
                throw new ArgumentException("no such site");
            }

            ICategory toCate = toSite.GetCategory(toCid);
            ICategory fromCate = fromSite.GetCategory(fromCid);


           int newCateId =  CloneCategoryDetails(targetSiteId, fromCate, toCate.Lft, includeExtend, includeTemplateBind);

            if (includeChild)
            {
                ItrCloneCate(toSite,fromCate,newCateId,includeExtend,includeTemplateBind);
            }
        }

        private void ItrCloneCate(ISite toSite,ICategory fromCate,int parentCateId,bool includeExtend, bool includeTemplateBind)
        {
            ICategory newCategory = toSite.GetCategory(parentCateId);
            foreach (var cate in fromCate.NextLevelChilds)
            {
                int id = CloneCategoryDetails(toSite.Id, cate, newCategory.Lft, includeExtend, includeTemplateBind);
                ItrCloneCate(toSite,cate,id,includeExtend,includeTemplateBind);
            }
        }

        private int CloneCategoryDetails(int toSiteId, ICategory fromCate, int toCateLft, bool includeExtend, bool includeTemplateBind)
        {
            CategoryDto dto = CategoryDto.ConvertFrom(fromCate);
            dto.Lft = 0;
            dto.Id = 0;

            // 包含扩展
            if (!includeExtend)
            {
                dto.ExtendFields = new IExtendField[0];
            }
            else
            {
                dto.ExtendFields = new List<IExtendField>(fromCate.ExtendFields.Count);
                foreach (var extendField in fromCate.ExtendFields)
                {
                    var toField = GetCloneNewExtendField(toSiteId, extendField);
                    dto.ExtendFields.Add(toField);
                }
            }

            // 不包含模版
            if (!includeTemplateBind)
            {
                dto.CategoryTemplate = null;
                dto.CategoryArchiveTemplate = null;
            }
            
            return this.SaveCategory(toSiteId, toCateLft, dto);
        }

        private IExtendField GetCloneNewExtendField(int toSiteId, IExtendField extendField)
        {
            IExtendField toField = this.GetExtendFieldByName(toSiteId, extendField.Name, extendField.Type);
            if (toField == null)
            {
                int id = this.SaveExtendField(toSiteId, new ExtendFieldDto
                {
                    DefaultValue = extendField.DefaultValue,
                    Message = extendField.Message,
                    Regex = extendField.Regex,
                    Name = extendField.Name,
                    Type = extendField.Type,
                });
                toField = new ExtendField(id, extendField.Name);
            }
            return toField;
        }

        private IExtendField GetExtendFieldByName(int siteId, string name, string type)
        {
            return this._extendResp.GetExtendByName(siteId, name, type);
        }
    }
}
