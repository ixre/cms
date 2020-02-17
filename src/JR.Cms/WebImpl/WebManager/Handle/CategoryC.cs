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
using JR.Cms.CacheService;
using JR.Cms.Core;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Tree;
using JR.Cms.Library.CacheProvider.CacheCompoment;
using JR.Cms.ServiceDto;
using JR.Cms.WebImpl.Json;
using Kvdb = JR.Cms.CacheService.Kvdb;

namespace JR.Cms.WebImpl.WebManager.Handle
{
    public class CategoryC : BasePage
    {

        public void Data1_POST()
        {
            IList<CategoryDto> categories = new List<CategoryDto>();

            //根节点
            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (c, level, isLast) =>
             {
                 CategoryDto dto = CategoryDto.ConvertFrom(c);


                 dto.Name = "<span class=\"level" + level.ToString() + "\">"
                     + dto.Name
                     + "</span>";

                 categories.Add(dto);


             });

            base.PagerJson(categories, String.Format("共{0}条", categories.Count.ToString()));

        }

        /// <summary>
        /// 所有栏目
        /// </summary>
        public string All_GET()
        {
            SiteDto siteDto = base.CurrentSite;
            ViewData["site_id"] = siteDto.SiteId;
            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Category_List));
        }



        /// <summary>
        /// 创建栏目
        /// </summary>
        public void Create_GET()
        {
            string categoryOptions,             //栏目下拉列表
                   categoryTypeOptions,         //栏目类型下拉列表
                      archiveTplOpts,
                      categoryTplOpts;

            StringBuilder sb = new StringBuilder();

            //当前站点
            var site = base.CurrentSite;

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
            int.TryParse(Request["parent_id"], out parentId);

            categoryOptions = Helper.GetCategoryIdSelector(this.SiteId, parentId);


            //获取模板视图下拉项
            sb.Remove(0, sb.Length);

            //模板目录
            DirectoryInfo dir = new DirectoryInfo(String.Format("{0}templates/{1}/",Cms.PyhicPath,base.CurrentSite.Tpl));
            IDictionary<String, String> names = Cms.TemplateManager.Get(base.CurrentSite.Tpl).GetNameDictionary();
            EachClass.EachTemplatePage(dir, dir, sb, names, TemplatePageType.Archive);
            archiveTplOpts = sb.ToString();

            sb.Remove(0, sb.Length);

            EachClass.EachTemplatePage(dir, dir, sb, names, TemplatePageType.Category);
            categoryTplOpts = sb.ToString();

            object entity = new
            {
                ParentId = Request["parent_id"]
            };

            object data = new
            {
                url = base.Request.RawUrl,
                categories = categoryOptions,
                tpls = sb.ToString(),
                category_tpls = categoryTplOpts,
                archive_tpls = archiveTplOpts,
                entity = JsonSerializer.Serialize(entity)
            };
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Category_CreateCategory), data);
        }

        [MCacheUpdate(CacheSign.Category | CacheSign.Link)]
        public string Create_POST()
        {
            var form = HttpContext.Current.Request.Form;

            int parentId = 0;
            int.TryParse(form["ParentId"], out parentId);

            CategoryDto category = InitCategoryDtoFromHttpPost(form, new CategoryDto());

            Result r = ServiceCall.Instance.SiteService
                .SaveCategory(this.SiteId, parentId, category);
            if(r.ErrCode > 0)
            {
                return base.ReturnError(r.ErrMsg);
            }
            Kvdb.Gca.Delete(Consts.NODE_TREE_JSON_KEY);
            int categoryId = Convert.ToInt32(r.Data["CategoryId"]);
            String key = Consts.NODE_TREE_JSON_KEY + ":" + this.SiteId.ToString();
            Kvdb.Gca.Delete(key);
            return base.ReturnSuccess(null, categoryId.ToString());
        }

        /// <summary>
        /// 移动顺序
        /// </summary>
        public void MoveSortNumber_post()
        {
            int id = int.Parse(base.Request["category.id"]);
            int di = int.Parse(base.Request["direction"]);

            try
            {
                ServiceCall.Instance.SiteService.MoveCategorySortNumber(this.SiteId, id, di);
                String key = Consts.NODE_TREE_JSON_KEY + ":" + this.SiteId.ToString();
                Kvdb.Gca.Delete(key);
                base.RenderSuccess();
            }
            catch (Exception exc)
            {
                base.RenderError(exc.Message);
            }
        }

        private static CategoryDto InitCategoryDtoFromHttpPost(NameValueCollection form, CategoryDto category)
        {
            //form.BindToEntity(category);
            category.Keywords = form["Keywords"];
            category.PageTitle = form["PageTitle"];
            category.Tag = form["Tag"];
            category.Name = form["Name"];
            category.SortNumber = int.Parse(form["SortNumber"]);
            category.Description = form["Description"];
            category.Location = form["Location"];
            category.Icon = form["Icon"];

            if (!String.IsNullOrEmpty(category.Keywords))
            {
                category.Keywords = Regex.Replace(category.Keywords, "，|\\s|\\|", ",");
            }

            //设置模板
            string categoryTplPath = form["CategoryTemplate"],
                        archiveTplPath = form["CategoryArchiveTemplate"];


            //如果设置了栏目视图路径，则保存
            category.CategoryTemplate = categoryTplPath;

            //如果设置了文档视图路径，则保存
            category.CategoryArchiveTemplate = archiveTplPath;

            return category;
        }


        /// <summary>
        /// 创建栏目
        /// </summary>
        public void Update_GET()
        {
            string categoryOptions,             //栏目下拉列表
                      categoryTypeOptions,     //栏目类型下拉列表
                archiveTpl,
                      archiveTplOpts,
                      categoryTplOpts;

            StringBuilder sb = new StringBuilder();

            //获取栏目
            int categoryId = int.Parse(base.Request["category_id"]);
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, categoryId);


            //检验站点
            if (!(category.ID > 0)) return;

            //获取父栏目pleft
            int pId = category.ParentId;

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

            categoryOptions = Helper.GetCategoryIdSelector(this.SiteId, 0);




            //获取模板视图下拉项
            sb.Remove(0, sb.Length);

            //模板目录
            DirectoryInfo dir = new DirectoryInfo(String.Format("{0}templates/{1}/",Cms.PyhicPath,base.CurrentSite.Tpl));

            IDictionary<String, String> names = Cms.TemplateManager.Get(base.CurrentSite.Tpl).GetNameDictionary();
            EachClass.EachTemplatePage(dir, dir, sb, names,TemplatePageType.Archive);
            archiveTplOpts = sb.ToString();

            sb.Remove(0, sb.Length);

            EachClass.EachTemplatePage(dir, dir, sb, names,TemplatePageType.Category);
            categoryTplOpts = sb.ToString();


            //获取栏目及文档模板绑定
            var categoryTpl = String.IsNullOrEmpty(category.CategoryTemplate) ? "" : category.CategoryTemplate;
            archiveTpl = String.IsNullOrEmpty(category.CategoryArchiveTemplate) ? "" : category.CategoryArchiveTemplate;

            object data = new
            {
                entity = JsonSerializer.Serialize(category),
                url = base.Request.RawUrl,
                categories = categoryOptions,
                //categoryTypes = categoryTypeOptions,
                parentId = pId,
                categoryTplPath = categoryTpl,
                archiveTplPath = archiveTpl,
                category_tpls = categoryTplOpts,
                archive_tpls = archiveTplOpts
            };

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Category_EditCategory), data);
        }


        [MCacheUpdate(CacheSign.Category | CacheSign.Link)]
        public string Update_POST()
        {
            var form = HttpContext.Current.Request.Form;
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(
                this.SiteId,int.Parse(form["ID"].ToString()));
            if (!(category.ID > 0)) return base.ReturnError("分类不存在!");

            //获取新的栏目信息
            category = InitCategoryDtoFromHttpPost(form, category);

            int parentId = Convert.ToInt32(form["ParentId"]);
            //设置并保存
           Result r =  ServiceCall.Instance.SiteService.SaveCategory(this.SiteId, parentId, category);
            if(r.ErrCode > 0)
            {
                return base.ReturnError(r.ErrMsg);
            }
            String key = Consts.NODE_TREE_JSON_KEY + ":" + this.SiteId.ToString();
            Kvdb.Gca.Delete(key);
            return base.ReturnSuccess("保存成功!");
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        [MCacheUpdate(CacheSign.Category | CacheSign.Link)]
        public string Delete_POST()
        {
            int categoryId = int.Parse(base.Request.Form["category_id"]);
            Error err = ServiceCall.Instance.SiteService.DeleteCategory(this.SiteId, categoryId);
            if (err == null)
            {
                String key = Consts.NODE_TREE_JSON_KEY + ":" + this.SiteId.ToString();
                Kvdb.Gca.Delete(key);
                return base.ReturnSuccess();
            }
            return base.ReturnError(err.Message);

        }

        /// <summary>
        /// 左侧栏目树
        /// </summary>
        public void Tree_GET()
        {

            TreeNode node = ServiceCall.Instance.SiteService.GetCategoryTreeWithRootNode(this.SiteId);

            BuiltCacheResultHandler<String> bh = () =>
            {
                string _treeHtml = "";
                StringBuilder sb = new StringBuilder();


                //var allCate = CmsLogic.Category.GetCategories();

                sb.Append("<dl><dt class=\"tree-title\"><img src=\"/public/mui/css/old/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\" class=\"tree-title\"/>")
                .Append(this.CurrentSite.Name ?? "默认站点").Append("</dt>");



                //从根目录起循环
                ServiceCall.Instance.SiteService.ItrCategoryTree(sb, this.SiteId, 1);

                //ItrTree(CmsLogic.Category.Root, sb,siteID);


                sb.Append("</dl>");

                _treeHtml = sb.ToString();


                _treeHtml = Regex.Replace(_treeHtml, "(<img[^>]+>)+<span([^>]+)>([^<]+)</span></dd></dl>", m =>
                {
                    string returnStr = m.Value.Replace("tree-item", "tree-item-last");

                    MatchCollection mcs = Regex.Matches(m.Value, "<img class=\"tree-line\"[^>]+>");
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
                string json = JsonSerializer.Serialize(node);

                /*
                string treeHtml = CacheFactory.Singleton.GetResult<String>(
                    String.Format("{0}_{1}_admin_tree", CacheSign.Category.ToString(), this.SiteId.ToString()),
                    bh);
                */

                base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Category_LeftBar_Tree), new
                {
                    tree = json,
                    treeFor = base.Request["for"]
                });
            }
            catch (Exception exc)
            {
                this.Response.Write("<script>parent.M.alert('站点栏目异常！这可能是由于数据不正确导致。<br />具体错误信息：");
                this.Response.Write(exc.Message);
                this.Response.Write("')</script>");
            }
        }
    }
}
