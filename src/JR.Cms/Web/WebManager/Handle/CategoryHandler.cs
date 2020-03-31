//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
// 
// Project: jr.Cms.Manager
// FileName : category.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 18:10:55
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using JR.Cms.Core;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Tree;
using JR.Cms.Library.CacheProvider.CacheCompoment;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Cms.WebImpl.Json;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Kvdb = JR.Cms.Library.CacheService.Kvdb;

namespace JR.Cms.WebImpl.WebManager.Handle
{
    public class CategoryHandler : BasePage
    {
        public void Data1_POST()
        {
            IList<CategoryDto> categories = new List<CategoryDto>();

            //根节点
            ServiceCall.Instance.SiteService.HandleCategoryTree(SiteId, 1, (c, level, isLast) =>
            {
                var dto = CategoryDto.ConvertFrom(c);


                dto.Name = "<span class=\"level" + level.ToString() + "\">"
                           + dto.Name
                           + "</span>";

                categories.Add(dto);
            });

            PagerJson(categories, string.Format("共{0}条", categories.Count.ToString()));
        }

        /// <summary>
        /// 所有栏目
        /// </summary>
        public string All()
        {
            var siteDto = CurrentSite;
            ViewData["site_id"] = siteDto.SiteId;
            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Category_List));
        }


        /// <summary>
        /// 创建栏目
        /// </summary>
        public void Create()
        {
            string categoryOptions, //栏目下拉列表
                categoryTypeOptions, //栏目类型下拉列表
                archiveTplOpts,
                categoryTplOpts;

            var sb = new StringBuilder();

            //当前站点
            var site = CurrentSite;

            //加载模块
            /*
            foreach (var m in CmsLogic.Module.GetSiteAvailableModules(siteID))
            {
                if (!m.IsDelete)
                {
                    sb.Append("<option value=\"").Append(m.ID.ToString()).Append("\">").Append(m.Name).Append("</option>");
                }
            }
            categoryTypeOptions = sb.ToString();
            sb.Remove(0, sb.Length);
            */
            int parentId;
            int.TryParse(Request.Query("parent_id"), out parentId);

            categoryOptions = Helper.GetCategoryIdSelector(SiteId, parentId);


            //获取模板视图下拉项
            sb.Remove(0, sb.Length);

            //模板目录
            var dir = new DirectoryInfo(string.Format("{0}templates/{1}/", Cms.PhysicPath, CurrentSite.Tpl));
            var names = Cms.TemplateManager.Get(CurrentSite.Tpl).GetNameDictionary();
            EachClass.EachTemplatePage(dir, dir, sb, names, TemplatePageType.Archive);
            archiveTplOpts = sb.ToString();

            sb.Remove(0, sb.Length);

            EachClass.EachTemplatePage(dir, dir, sb, names, TemplatePageType.Category);
            categoryTplOpts = sb.ToString();

            object entity = new
            {
                ParentId = Request.Query("parent_id")
            };

            object data = new
            {
                url = Request.GetEncodedUrl(),
                categories = categoryOptions,
                tpls = sb.ToString(),
                category_tpls = categoryTplOpts,
                archive_tpls = archiveTplOpts,
                entity = JsonSerializer.Serialize(entity)
            };
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Category_CreateCategory), data);
        }

        [MCacheUpdate(CacheSign.Category | CacheSign.Link)]
        public string Create_POST()
        {

            var parentId = 0;
            int.TryParse(Request.Form("ParentId"), out parentId);

            var category = InitCategoryDtoFromHttpPost(Request, new CategoryDto());

            var r = ServiceCall.Instance.SiteService
                .SaveCategory(SiteId, parentId, category);
            if (r.ErrCode > 0) return ReturnError(r.ErrMsg);
            Kvdb.Gca.Delete(Consts.NODE_TREE_JSON_KEY);
            var categoryId = Convert.ToInt32(r.Data["CategoryId"]);
            var key = Consts.NODE_TREE_JSON_KEY + ":" + SiteId.ToString();
            Kvdb.Gca.Delete(key);
            return ReturnSuccess(null, categoryId.ToString());
        }

        /// <summary>
        /// 移动顺序
        /// </summary>
        public void MoveSortNumber_post()
        {
            var id = int.Parse(Request.Query("category.id"));
            var di = int.Parse(Request.Query("direction"));

            try
            {
                ServiceCall.Instance.SiteService.MoveCategorySortNumber(SiteId, id, di);
                var key = Consts.NODE_TREE_JSON_KEY + ":" + SiteId.ToString();
                Kvdb.Gca.Delete(key);
                RenderSuccess();
            }
            catch (Exception exc)
            {
                RenderError(exc.Message);
            }
        }

        private static CategoryDto InitCategoryDtoFromHttpPost(ICompatibleRequest form, CategoryDto category)
        {
            //form.BindToEntity(category);
            category.Keywords = form.Form("Keywords");
            category.PageTitle =form.Form("PageTitle");
            category.Tag = form.Form("Tag");
            category.Name =form.Form("Name");
            category.SortNumber = int.Parse(form.Form("SortNumber"));
            category.Description = form.Form("Description");
            category.Location = form.Form("Location");
            category.Icon = form.Form("Icon");

            if (!string.IsNullOrEmpty(category.Keywords))
                category.Keywords = Regex.Replace(category.Keywords, "，|\\s|\\|", ",");

            //设置模板
            string categoryTplPath = form.Form("CategoryTemplate"),
                archiveTplPath = form.Form("CategoryArchiveTemplate");


            //如果设置了栏目视图路径，则保存
            category.CategoryTemplate = categoryTplPath;

            //如果设置了文档视图路径，则保存
            category.CategoryArchiveTemplate = archiveTplPath;

            return category;
        }


        /// <summary>
        /// 创建栏目
        /// </summary>
        public void Update()
        {
            string categoryOptions, //栏目下拉列表
                categoryTypeOptions, //栏目类型下拉列表
                archiveTpl,
                archiveTplOpts,
                categoryTplOpts;

            var sb = new StringBuilder();

            //获取栏目
            var categoryId = int.Parse(Request.Query("category_id"));
            var category = ServiceCall.Instance.SiteService.GetCategory(SiteId, categoryId);


            //检验站点
            if (!(category.ID > 0)) return;

            //获取父栏目pleft
            var pId = category.ParentId;

            /*
            //加载模块
            foreach (var m in CmsLogic.Module.GetSiteAvailableModules(this.SiteId))
            {
                if (!m.IsDelete)
                {
                    sb.Append("<option value=\"").Append(m.ID.ToString()).Append("\">").Append(m.Name).Append("</option>");
                }
            }
            categoryTypeOptions = sb.ToString();
            sb.Remove(0, sb.Length);

            */

            categoryOptions = Helper.GetCategoryIdSelector(SiteId, 0);


            //获取模板视图下拉项
            sb.Remove(0, sb.Length);

            //模板目录
            var dir = new DirectoryInfo(string.Format("{0}templates/{1}/", Cms.PhysicPath, CurrentSite.Tpl));

            var names = Cms.TemplateManager.Get(CurrentSite.Tpl).GetNameDictionary();
            EachClass.EachTemplatePage(dir, dir, sb, names, TemplatePageType.Archive);
            archiveTplOpts = sb.ToString();

            sb.Remove(0, sb.Length);

            EachClass.EachTemplatePage(dir, dir, sb, names, TemplatePageType.Category);
            categoryTplOpts = sb.ToString();


            //获取栏目及文档模板绑定
            var categoryTpl = string.IsNullOrEmpty(category.CategoryTemplate) ? "" : category.CategoryTemplate;
            archiveTpl = string.IsNullOrEmpty(category.CategoryArchiveTemplate) ? "" : category.CategoryArchiveTemplate;

            object data = new
            {
                entity = JsonSerializer.Serialize(category),
                url = Request.GetEncodedUrl(),
                categories = categoryOptions,
                //categoryTypes = categoryTypeOptions,
                parentId = pId,
                categoryTplPath = categoryTpl,
                archiveTplPath = archiveTpl,
                category_tpls = categoryTplOpts,
                archive_tpls = archiveTplOpts
            };

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Category_EditCategory), data);
        }


        [MCacheUpdate(CacheSign.Category | CacheSign.Link)]
        public string Update_POST()
        {
            var category = ServiceCall.Instance.SiteService.GetCategory(
                SiteId, int.Parse(Request.Form("ID").ToString()));
            if (!(category.ID > 0)) return ReturnError("分类不存在!");

            //获取新的栏目信息
            category = InitCategoryDtoFromHttpPost(Request, category);

            var parentId = Convert.ToInt32(Request.Form("ParentId"));
            //设置并保存
            var r = ServiceCall.Instance.SiteService.SaveCategory(SiteId, parentId, category);
            if (r.ErrCode > 0) return ReturnError(r.ErrMsg);
            var key = Consts.NODE_TREE_JSON_KEY + ":" + SiteId.ToString();
            Kvdb.Gca.Delete(key);
            return ReturnSuccess("保存成功!");
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        [MCacheUpdate(CacheSign.Category | CacheSign.Link)]
        public string Delete_POST()
        {
            var categoryId = int.Parse(Request.Form("category_id"));
            var err = ServiceCall.Instance.SiteService.DeleteCategory(SiteId, categoryId);
            if (err == null)
            {
                var key = Consts.NODE_TREE_JSON_KEY + ":" + SiteId.ToString();
                Kvdb.Gca.Delete(key);
                return ReturnSuccess();
            }

            return ReturnError(err.Message);
        }

        /// <summary>
        /// 左侧栏目树
        /// </summary>
        public void Tree()
        {
            var node = ServiceCall.Instance.SiteService.GetCategoryTreeWithRootNode(SiteId);

            BuiltCacheResultHandler<string> bh = () =>
            {
                var _treeHtml = "";
                var sb = new StringBuilder();


                //var allCate = CmsLogic.Category.GetCategories();

                sb.Append(
                        "<dl><dt class=\"tree-title\"><img src=\"/public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\" class=\"tree-title\"/>")
                    .Append(CurrentSite.Name ?? "默认站点").Append("</dt>");


                //从根目录起循环
                ServiceCall.Instance.SiteService.ItrCategoryTree(sb, SiteId, 1);

                //ItrTree(CmsLogic.Category.Root, sb,siteID);


                sb.Append("</dl>");

                _treeHtml = sb.ToString();


                _treeHtml = Regex.Replace(_treeHtml, "(<img[^>]+>)+<span([^>]+)>([^<]+)</span></dd></dl>", m =>
                {
                    var returnStr = m.Value.Replace("tree-item", "tree-item-last");

                    var mcs = Regex.Matches(m.Value, "<img class=\"tree-line\"[^>]+>");
                    if (mcs.Count > 0)
                    {
                        //returnStr = returnStr.Replace(mcs[mcs.Count - 1].Value, mcs[mcs.Count - 1].Value.Replace("tree-line", "tree-line-last"));
                    }

                    return returnStr;
                });

                return _treeHtml;
            };


            try
            {
                var json = JsonSerializer.Serialize(node);

                /*
                string treeHtml = CacheFactory.Singleton.GetResult<String>(
                    String.Format("{0}_{1}_admin_tree", CacheSign.Category.ToString(), this.SiteId.ToString()),
                    bh);
                */

                RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Category_LeftBar_Tree), new
                {
                    tree = json,
                    treeFor = Request.Query("for")
                });
            }
            catch (Exception exc)
            {
                Response.WriteAsync("<script>parent.M.alert('站点栏目异常！这可能是由于数据不正确导致。<br />具体错误信息：");
                Response.WriteAsync(exc.Message);
                Response.WriteAsync("')</script>");
            }
        }
    }
}