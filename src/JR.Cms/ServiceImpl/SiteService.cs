using System;
using System.Collections.Generic;
using System.Text;
using JR.Cms.Domain.Interface;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Tree;
using JR.Cms.Models;
using JR.Cms.ServiceContract;
using JR.DevFw.Framework.Extensions;
using CategoryDto = JR.Cms.ServiceDto.CategoryDto;
using ExtendFieldDto = JR.Cms.ServiceDto.ExtendFieldDto;
using Result = JR.Cms.ServiceDto.Result;
using SiteDto = JR.Cms.ServiceDto.SiteDto;
using SiteLinkDto = JR.Cms.ServiceDto.SiteLinkDto;

namespace JR.Cms.ServiceImpl
{
    public class SiteService : ISiteServiceContract
    {
        private readonly ISiteRepo repo;
        private readonly IExtendFieldRepository _extendRep;
        private readonly ICategoryRepo _categoryRep;
        private readonly ITemplateRepo _tempRep;
        private readonly IArchiveRepository _archiveRep;
        private readonly IContentRepository _contentRep;

        public SiteService(ISiteRepo resp,
            ICategoryRepo categoryRep,
            IArchiveRepository archiveRep,
            IExtendFieldRepository extendFieldPository,
            IContentRepository contentRep,
            ITemplateRepo tempRep)
        {
            this.repo = resp;
            this._extendRep = extendFieldPository;
            this._categoryRep = categoryRep;
            this._archiveRep = archiveRep;
            this._contentRep = contentRep;
            this._tempRep = tempRep;
        }

        public int SaveSite(SiteDto siteDto)
        {
            ISite site;
            if (siteDto.SiteId != 0)
            {
                site = repo.GetSiteById(siteDto.SiteId);
                if (site == null)
                {
                    throw new ArgumentException("No such site");
                }
            }
            else
            {
                site = repo.CreateSite(new Models.CmsSiteEntity());
            }

            SiteDto.CopyTo(siteDto, site);
            return site.Save();
        }


        public IList<SiteDto> GetSites()
        {
            IList<SiteDto> siteDtos = new List<SiteDto>();
            IList<ISite> sites = repo.GetSites();
            foreach (ISite site in sites)
            {
                siteDtos.Add(SiteDto.ConvertFromSite(site));
            }
            return siteDtos;
        }
        
        private static SiteDto GetSiteDtoFromISite(ISite site)
        {
            if (site != null)
            {
                return SiteDto.ConvertFromSite(site);
            }
            return default(SiteDto);
        }



        public SiteDto GetSingleOrDefaultSite(String host, String appPath)
        {
            ISite ist = this.repo.GetSingleOrDefaultSite(host,appPath);
            return GetSiteDtoFromISite(ist);
        }


        public SiteDto GetSiteById(int siteId)
        {
            return GetSiteDtoFromISite(
                    this.repo.GetSiteById(siteId)
                );
        }


        public IEnumerable<ExtendFieldDto> GetExtendFields(int siteId)
        {
            ISite site = this.repo.GetSiteById(siteId);
            ExtendFieldDto dto;
            IEnumerable<IExtendField> extends = site.GetExtendManager().GetAllExtends();
            foreach (IExtendField extend in extends)
            {
                dto = new ExtendFieldDto().CloneData(extend);
                dto.Id = extend.GetDomainId();
                yield return dto;
            }
        }


        public Result SaveExtendField(int siteId, ExtendFieldDto dto)
        {
            ISite site = this.repo.GetSiteById(siteId);
            if (site == null)
                throw new Exception("站点不存在");

            IExtendField field = this._extendRep.CreateExtendField(dto.Id, dto.Name);
            field.CloneData(dto);
            Error err =  site.GetExtendManager().SaveExtendField(field);
            Result r = new Result();
            if(err != null)
            {
                r.ErrCode = 1;
                r.ErrMsg = err.Message;
            }
            return r;
        }


        public CategoryDto GetCategory(int siteId, int categoryId)
        {
            ISite site = this.repo.GetSiteById(siteId);
            return CategoryDto.ConvertFrom(site.GetCategory(categoryId));
        }


        public CategoryDto GetCategoryByName(int siteId, string categoryName)
        {
            ISite site = this.repo.GetSiteById(siteId);
            return CategoryDto.ConvertFrom(site.GetCategoryByName(categoryName));
        }

        public CategoryDto GetCategory(int siteId, string catPath)
        {
            // 如果以"/"开头，则去掉
            if (!String.IsNullOrEmpty(catPath) && catPath[0] == '/')
            {
                catPath = catPath.Substring(1);
            }
            ISite site = this.repo.GetSiteById(siteId);
            return CategoryDto.ConvertFrom(site.GetCategoryByPath(catPath));
        }



        public IEnumerable<CategoryDto> GetCategories(int siteId)
        {
            ISite site = this.repo.GetSiteById(siteId);
            IList<ICategory> categories = site.Categories;
            foreach (ICategory category in categories)
            {
                yield return CategoryDto.ConvertFrom(category);
            }
        }

        public IEnumerable<CategoryDto> GetCategories(
            int siteId,String catPath)
        {
            ISite site = this.repo.GetSiteById(siteId);
            ICategory ic = this._categoryRep.GetCategoryByPath(siteId, catPath);
            if (ic == null) yield break;
            foreach (ICategory category in ic.NextLevelChilds)
            {
                yield return CategoryDto.ConvertFrom(category);
            }
        }
        
        public Error DeleteCategory(int siteId, int catId)
        {
            ISite site = this.repo.GetSiteById(siteId);
            return site.DeleteCategory(catId);
        }

        public void ItrCategoryTree(StringBuilder sb, int siteId, int categoryId)
        {
            ISite site = this.repo.GetSiteById(siteId);
            site.ItreCategoryTree(sb, categoryId);
        }



        public void HandleCategoryTree(int siteId, int parentId, CategoryTreeHandler treeHandler)
        {
            ISite site = this.repo.GetSiteById(siteId);
            site.HandleCategoryTree(parentId, treeHandler);
        }


        public Result SaveCategory(int siteId, int parentId, CategoryDto category)
        {
            ISite site = this.repo.GetSiteById(siteId);
            ICategory ic = null;
            if (category.ID > 0)
            {
                ic = site.GetCategory(category.ID);
            }
            if (ic == null) ic = _categoryRep.CreateCategory(new CmsCategoryEntity());
            CmsCategoryEntity cat = new CmsCategoryEntity();
            cat.SiteId = siteId;
            cat.Keywords = category.Keywords;
            cat.Description = category.Description;
            cat.ParentId = parentId;
            cat.Tag = category.Tag;
            cat.Icon = category.Icon;
            cat.Name = category.Name;
            cat.Title = category.PageTitle;
            cat.SortNumber = category.SortNumber;
            cat.ModuleId = category.ModuleId;
            cat.Location = category.Location;
            Error err = ic.Set(cat);
            if(err == null)
            {
                // 保存模板
                List<TemplateBind> binds = new List<TemplateBind>();
                if (!String.IsNullOrEmpty(category.CategoryTemplate))
                {
                    binds.Add(new TemplateBind(0, TemplateBindType.CategoryTemplate, category.CategoryTemplate));
                }
                if (!String.IsNullOrEmpty(category.CategoryArchiveTemplate))
                {
                    binds.Add(new TemplateBind(0, TemplateBindType.CategoryArchiveTemplate, category.CategoryArchiveTemplate));
                }
                err = ic.SetTemplates(binds.ToArray());
                if(err == null)
                {
                    // 扩展属性
                    if (category.ExtendFields != null)
                    {
                        ic.ExtendFields = category.ExtendFields;
                    }
                    if(err == null)
                    {
                        err = ic.Save();
                    }
                }
            }

            #region 扩展属性

            

            #endregion

            Result r = new Result();
            if (err == null)
            {
                r.Data = new Dictionary<String, String>();
                r.Data["CategoryId"] = ic.GetDomainId().ToString();
            }
            else
            {
                r.ErrCode = 1;
                r.ErrMsg = err.Message;
            }
            return r;
        }

        
        public TreeNode GetCategoryTreeWithRootNode(int siteId)
        {
            ISite site = this.repo.GetSiteById(siteId);
            return site.GetCategoryTreeWithRootNode();
        }


        public string GetCategorySitemapHtml(int siteId, string catPath, string split, string linkFormat)
        {
            ISite site = this.repo.GetSiteById(siteId);
            ICategory category = site.GetCategoryByPath(catPath);

            if (linkFormat.EndsWith("/")) linkFormat = linkFormat.Substring(0, linkFormat.Length - 1);
            StringBuilder sb = new StringBuilder();
            int tmpInt = 0;
            String html = "";

            while (category != null)
            {
                sb.Append("<a class=\"l").Append((tmpInt+1).ToString()).Append("\" href=\"");
                if (!String.IsNullOrEmpty(category.Get().Location))
                {
                    if (category.Get().Location.IndexOf("//", StringComparison.Ordinal) != -1)
                    {
                        sb.Append(category.Get().Location);
                    }
                    else
                    {
                        if (category.Get().Location.StartsWith("/"))
                        {
                            category.Get().Location = category.Get().Location.Substring(1);
                        }
                        string path = String.Format(linkFormat, category.Get().Location);
                        sb.Append(path);
                    }
                }
                else
                {
                    sb.Append(String.Format(linkFormat, category.Get().Path)).Append("/");
                }
                sb.Append("\"");

                //栏目的上一级添加不追踪特性
                if (++tmpInt > 1)
                {
                    sb.Append(" rel=\"nofollow\"");
                }
                sb.Append(">").Append(category.Get().Name).Append("</a>");

                //添加分隔符
                if (tmpInt > 1)
                {
                    sb.Append(split);
                }

                html = sb.ToString() + html;
                sb.Remove(0, sb.Length);

                category = category.Parent;
            }
            return html;
        }


        public bool CheckCategoryTagAvailable(int siteId, int categoryId, string categoryTag)
        {
            ISite site = this.repo.GetSiteById(siteId);
            ICategory category = site.GetCategoryByPath(categoryTag);

            if (category == null)
                return true;
            //
            //            if (categoryId <= 0)
            //            {
            //                return false;
            //            }
            return category.GetDomainId() == categoryId;

        }


        public IEnumerable<SiteLinkDto> GetLinksByType(int siteId, SiteLinkType type, bool ignoreDisabled)
        {
            ISite site = this.repo.GetSiteById(siteId);
            IEnumerable<ISiteLink> links = site.GetLinkManager().GetLinks(type);
            foreach (ISiteLink link in links)
            {
                if (!ignoreDisabled && !link.Visible) continue;

                yield return SiteLinkDto.ConvertFrom(link);
            }
        }


        public void DeleteLink(int siteId, int linkId)
        {
            ISite site = this.repo.GetSiteById(siteId);

            site.GetLinkManager().DeleteLink(linkId);
        }


        public int SaveLink(int siteId, SiteLinkDto dto)
        {
            ISite site = this.repo.GetSiteById(siteId);
            ISiteLink link = null;
            if (dto.Id <= 0)
            {
                link = this.repo.CreateLink(site, 0, dto.Text);
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
            ISite site = this.repo.GetSiteById(siteId);
            ISiteLink link = site.GetLinkManager().GetLinkById(linkId);
            if (link == null) return default(SiteLinkDto);

            return SiteLinkDto.ConvertFrom(link);
        }

        public IList<RoleValue> GetAppRoles(int siteId)
        {
            ISite site = this.repo.GetSiteById(siteId);
            return site.GetUserManager().GetAppRoles();
        }


        public void CloneCategory(int sourceSiteId, int targetSiteId, int fromCid, int toCid,
            bool includeChild, bool includeExtend, bool includeTemplateBind)
        {
            ISite fromSite = this.repo.GetSiteById(sourceSiteId);
            ISite toSite = this.repo.GetSiteById(targetSiteId);
            if (fromSite == null || toSite == null)
            {
                throw new ArgumentException("no such site");
            }
            ICategory toCate = toSite.GetCategory(toCid);
            ICategory fromCate = fromSite.GetCategory(fromCid);
            Result r = CloneCategoryDetails(targetSiteId, fromCate, toCate.GetDomainId(), includeExtend, includeTemplateBind);
            int newCateId = Convert.ToInt32(r.Data["CategoryId"]);
            if (includeChild)
            {
                ItrCloneCate(toSite, fromCate, newCateId, includeExtend, includeTemplateBind);
            }
        }

        private void ItrCloneCate(ISite toSite, ICategory fromCate, int parentCateId, bool includeExtend, bool includeTemplateBind)
        {
            ICategory newCategory = toSite.GetCategory(parentCateId);
            foreach (var cate in fromCate.NextLevelChilds)
            {
                Result r = CloneCategoryDetails(toSite.GetAggregaterootId(), cate, newCategory.GetDomainId(), includeExtend, includeTemplateBind);
                int catId = Convert.ToInt32(r.Data["CategoryId"]);
                ItrCloneCate(toSite, cate, catId, includeExtend, includeTemplateBind);
            }
        }

        private Result CloneCategoryDetails(int toSiteId, ICategory fromCate, int toCateId, bool includeExtend, bool includeTemplateBind)
        {
            CategoryDto dto = CategoryDto.ConvertFrom(fromCate);
            dto.ID = 0;

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

            return this.SaveCategory(toSiteId, toCateId, dto);
        }

        private IExtendField GetCloneNewExtendField(int toSiteId, IExtendField extendField)
        {
            IExtendField toField = this.GetExtendFieldByName(toSiteId, extendField.Name, extendField.Type);
            if (toField == null)
            {
                Result r = this.SaveExtendField(toSiteId, new ExtendFieldDto
                {
                    DefaultValue = extendField.DefaultValue,
                    Message = extendField.Message,
                    Regex = extendField.Regex,
                    Name = extendField.Name,
                    Type = extendField.Type,
                });
                if (r.ErrCode <= 0)
                {
                    toField = new ExtendField(0, extendField.Name);
                }
            }
            return toField;
        }

        private IExtendField GetExtendFieldByName(int siteId, string name, string type)
        {
            return this._extendRep.GetExtendByName(siteId, name, type);
        }


        public IDictionary<int, string> ClonePubArchive(int sourceSiteId, int targetSiteId, int toCid, int[] archiveIdArray, bool includeExtend, bool includeTempateBind, bool includeRelatedLink)
        {
            int totalFailed = 0;
            int totalSuccess = 0;

            IDictionary<int, string> errDict = new Dictionary<int, string>();
            bool isFailed;
            bool shouldReSave;

            IContentContainer srcContent = this._contentRep.GetContent(sourceSiteId);
            IContentContainer tarContent = this._contentRep.GetContent(targetSiteId);
            foreach (int archiveId in archiveIdArray)
            {
                IArchive srcArchive = srcContent.GetArchiveById(archiveId);
                IArchive tarArchive = this.CopyArchive(targetSiteId, toCid, srcArchive);

                isFailed = false;
                shouldReSave = false;

                if (includeTempateBind && srcArchive.Template != null &&
                    srcArchive.Template.BindType == TemplateBindType.ArchiveTemplate &&
                    !string.IsNullOrEmpty(srcArchive.Template.TplPath))
                {
                    tarArchive.SetTemplatePath(srcArchive.Template.TplPath);
                    shouldReSave = true;
                }

                //克隆扩展
                if (includeExtend)
                {
                    this.CloneArchiveExtendValue(srcArchive, tarArchive, errDict, ref isFailed,
                        ref shouldReSave);
                }
                //包含关联链接
                if (includeRelatedLink)
                {
                    this.CloneArchiveRelatedLink(srcArchive, tarArchive);
                }

                if (isFailed)
                {
                    this._archiveRep.DeleteArchive(targetSiteId, tarArchive.GetAggregaterootId());
                    totalFailed += 1;
                }
                else if (shouldReSave)
                {
                    tarArchive.Save();
                    totalSuccess += 1;
                }
            }

            return errDict;

        }

        private IArchive CopyArchive(int targetSiteId, int toCid, IArchive srcArchive)
        {
            throw new NotImplementedException();
        }

        public bool CheckSiteExists(int siteId)
        {
            IList<ISite> sites = this.repo.GetSites();
            foreach (ISite site in sites)
            {
                if (site.GetAggregaterootId() == siteId)
                {
                    return true;
                }
            }
            return false;
        }

        private void CloneArchiveRelatedLink(IArchive srcArchive, IArchive tarArchive)
        {
            IList<IContentLink> links = srcArchive.LinkManager.GetRelatedLinks();
            if (links.Count > 0)
            {
                foreach (var contentLink in links)
                {
                    tarArchive.LinkManager.Add(0, contentLink.RelatedSiteId,
                        contentLink.RelatedIndent, contentLink.RelatedContentId, contentLink.Enabled);
                }
                tarArchive.LinkManager.SaveRelatedLinks();
            }
        }

        private void CloneArchiveExtendValue(IArchive srcArchive, IArchive tarArchive,
            IDictionary<int, string> errDict, ref bool isFailed, ref bool shouldReSave)
        {
            IList<IExtendValue> extends = srcArchive.GetExtendValues();
            // category extend
            IList<IExtendField> cflist = tarArchive.Category.ExtendFields;
            IDictionary<string, IExtendField> cateFields = new Dictionary<string, IExtendField>();
            foreach (IExtendField f in cflist)
            {
                cateFields.Add(f.Name, f);
            }

            if (extends.Count > 0 && cateFields.Count > 0)
            {
                IExtendField field;
                IExtendField tarField;

                IList<IExtendValue> list = new List<IExtendValue>(extends.Count);

                foreach (IExtendValue extendValue in extends)
                {
                    if (String.IsNullOrEmpty(extendValue.Value)) continue;
                    field = extendValue.Field;
                    if (cateFields.ContainsKey(field.Name))
                    {
                        // tarField = this._extendRep.GetExtendByName(targetSiteId, field.Name, field.Type);
                        tarField = cateFields[field.Name];
                        list.Add(this._extendRep.CreateExtendValue(tarField, -1, extendValue.Value));
                        shouldReSave = true;
                    }
                    else
                    {
                        string message = " <break>[1001]：扩展字段\"" + field.Name + "\"不存在";
                        if (errDict.ContainsKey(tarArchive.GetAggregaterootId()))
                        {
                            errDict[tarArchive.GetAggregaterootId()] += message;
                        }
                        else
                        {
                            errDict[tarArchive.GetAggregaterootId()] = "发布文档\"" + tarArchive.Get().Title + "\"不成功！原因：" + message;
                        }
                        isFailed = true;
                    }
                }
                Error err = tarArchive.SetExtendValue(list);
            }
        }


        /// <summary>
        /// 移动栏目排序编号
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="direction"></param>
        public void MoveCategorySortNumber(int siteId, int id, int direction)
        {
            ISite site = this.repo.GetSiteById(siteId);
            if(site == null)throw  new ArgumentException("no such site");
            ICategory c = site.GetCategory(id);
            if (c == null) throw new ArgumentException("no such category");

            if (direction == 1)
            {
                c.MoveSortUp();
            }
            else if (direction == 2)
            {
                c.MoveSortDown();
            }
        }
    }
}
