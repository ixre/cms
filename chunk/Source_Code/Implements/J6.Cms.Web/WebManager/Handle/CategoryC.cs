//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : category.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 18:10:55
// Description :
//
// Get infromation of this software,please visit our site http://z3q.net/cms
//
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using J6.Cms.Cache.CacheCompoment;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Core;
using J6.Cms.DataTransfer;
using J6.Cms.Infrastructure.Tree;
using J6.Cms.WebManager;

namespace J6.Cms.Web.WebManager.Handle
{
    public class CategoryC : BasePage
    {


        public void Data_POST()
        {
            IList<CategoryDto> categories = new List<CategoryDto>();

            //根节点
            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (c, level) =>
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
        public void All_GET()
        {
            /*
            string categoryRowsHtml;            //栏目行HTML

            StringBuilder sb = new StringBuilder();
            ModuleBLL mb = new ModuleBLL();
            Module m;

            var allCate = CmsLogic.Category.GetCategories();


            if (allCate != null && allCate.Count != 0)
            {
                //根节点
                Category root = CmsLogic.Category.Root;

                CmsLogic.Category.HandleCategoryTree(root.Name, (c, level) =>
                {
                    m = mb.GetModule(c.ModuleID);
                    sb.Append("<tr><td class=\"hidden\">").Append(c.Lft).Append("</td>")
                          .Append("<td><span class=\"pnode\"></span>");

                    for (var i = 0; i < level; i++)
                    {
                        sb.Append("&nbsp;&nbsp;");
                    }

                    sb.Append(c.Name).Append("</td>")
                    .Append("<td class=\"center\">").Append(c.Tag).Append("</td>")
                    .Append("<td class=\"center\">").Append(m == null ? "-" : m.Name).Append("</td><td class=\"center\">")
                            .Append(c.Keywords)
                            .Append("</td><td class=\"center\"><button class=\"draw\" onclick=\"location.href='?module=archive&action=create&moduleID=").Append(c.ModuleID.ToString())
                    .Append("&categoryID=").Append(c.ID).Append("'\" /></td><td class=\"center\"><button class=\"edit\" /></td><td class=\"center\"><button class=\"delete\" /></td></tr>");

                });
            */
            /*
            foreach (Category parentCategory in CmsLogic.Category.GetCategories(a => a.PID == 0))
            {
                m = mb.GetModule(parentCategory.ModuleID);

                sb.Append("<tr class=\"row\"><td class=\"hidden\">").Append(parentCategory.ID).Append("</td>")
                    .Append("<td style=\"padding-left:10px\"><span class=\"pnode\"></span>").Append(parentCategory.Name).Append("</td>")
                    .Append("<td class=\"center\">").Append(parentnode.tag).Append("</td>")
                    .Append("<td class=\"center\">").Append(m == null ? "-" : m.Name).Append("</td><td class=\"center\">")
                            .Append(parentCategory.Keywords)
                            .Append("</td><td class=\"center\"><button class=\"draw\" onclick=\"location.href='?module=archive&action=create&moduleID=").Append(parentCategory.ModuleID.ToString())
                    .Append("&categoryID=").Append(parentCategory.ID).Append("'\" /></td><td class=\"center\"><button class=\"edit\" /></td><td class=\"center\"><button class=\"delete\" /></td></tr>");

                childCategories = new List<global::Spc.Category>(CmsLogic.Category.GetCategories(a => a.PID == parentCategory.ID));
                if (childCategories.Count != 0)
                {
                    // sb.Append("<tr><td colspan=\"8\"><table class=\"childCategories\" cellspacing=\"0\">");

                    foreach (global::Spc.Category childCategory in childCategories)
                    {
                        m = mb.GetModule(childCategory.ModuleID);
                        sb.Append("<tr><td class=\"hidden\">").Append(childCategory.ID).Append("</td><td style=\"padding-left:10px\"><span class=\"cnode\"></span>")
                            .Append(childCategory.Name).Append("</td>")
                            .Append("<td class=\"center\">").Append(childnode.tag).Append("</td><td class=\"center\">")
                            .Append(m == null ? "-" : m.Name)
                            .Append("</td><td class=\"center\">")
                            .Append(childCategory.Keywords)
                            .Append("</td><td class=\"center\"><button class=\"draw\" onclick=\"location.href='?module=archive&action=create&moduleID=")
                            .Append(childCategory.ModuleID).Append("&categoryID=").Append(childCategory.ID).Append("'\" /></td>")
                            .Append("<td class=\"center\"><button class=\"edit\" /></td>")
                            .Append("<td class=\"center\"><button class=\"delete\" /></td></tr>");
                    }

                    //sb.Append("</table></td></tr>");
                }

            }
             */



            //模块JSON
            //categoryTypeJSON = CmsLogic.Module.GetModuleJson();

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Category_List), new
            {
                //categoryRowsHtml = categoryRowsHtml,
                //categoryTypeJSON = categoryTypeJSON,
                //count=allCate.Count.ToString()
            });
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
            int siteID = base.CurrentSite.SiteId;

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

            //加载栏目
            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (category, level) =>
            {
                sb.Append("<option value=\"").Append(category.Lft.ToString()).Append("\">");
                for (var i = 0; i < level; i++)
                {
                    sb.Append(CmsCharMap.Dot);
                }
                sb.Append(category.Name).Append("</option>");

            });

            categoryOptions = sb.ToString();



            //获取模板视图下拉项
            sb.Remove(0, sb.Length);

            //模板目录
            DirectoryInfo dir = new DirectoryInfo(
                String.Format("{0}templates/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                Settings.TPL_MultMode ? "" : base.CurrentSite.Tpl + "/"
                ));

            EachClass.EachTemplatePage(dir, sb, TemplatePageType.Archive);
            archiveTplOpts = sb.ToString();

            sb.Remove(0, sb.Length);

            EachClass.EachTemplatePage(dir, sb, TemplatePageType.Category);
            categoryTplOpts = sb.ToString();

            object entity = new
            {
                ParentLft = Request["lft"]
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


            string parentLft = form["parentLft"];

            int _parentLft;//, t;
            int.TryParse(parentLft, out _parentLft);
            //int.TryParse(moduleID, out t);

            CategoryDto category = InitCategoryDtoFromHttpPost(form, new CategoryDto());

            //已经存在相同Tag的栏目
            if (!ServiceCall.Instance.SiteService
                .CheckCategoryTagAvailable(this.SiteId, -1, category.Tag))
            {
                return base.ReturnError("标签已经存在");
            }

            int categoryId = ServiceCall.Instance.SiteService
                .SaveCategory(this.SiteId, _parentLft, category);

            int categoryLeft = ServiceCall.Instance.SiteService.
                GetCategory(this.SiteId, categoryId).Lft;

            return base.ReturnSuccess(null, categoryLeft.ToString());
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
                      categoryTpl,
                      archiveTpl,
                      archiveTplOpts,
                      categoryTplOpts;

            StringBuilder sb = new StringBuilder();

            //获取栏目
            int leftID = int.Parse(base.Request["lft"]);
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategoryByLft(this.SiteId, leftID);


            //检验站点
            if (!(category.Id > 0)) return;

            //获取父栏目pleft
            int pleft = 1;
            CategoryDto pc = ServiceCall.Instance.SiteService.GetParentCategory(this.SiteId, category.Lft);

            if (pc.Id > 0) pleft = pc.Lft;

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

            //加载栏目(排除当前栏目)
            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (c, level) =>
            {
                if (c.Lft != leftID)
                {
                    sb.Append("<option value=\"").Append(c.Lft.ToString()).Append("\">");
                    for (var i = 0; i < level; i++)
                    {
                        sb.Append(CmsCharMap.Dot);
                    }
                    sb.Append(c.Name).Append("</option>");
                }
            });

            categoryOptions = sb.ToString();



            //获取模板视图下拉项
            sb.Remove(0, sb.Length);

            //模板目录
            DirectoryInfo dir = new DirectoryInfo(
                String.Format("{0}templates/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                Settings.TPL_MultMode ? "" : base.CurrentSite.Tpl + "/"
                ));

            EachClass.EachTemplatePage(dir, sb, TemplatePageType.Archive);
            archiveTplOpts = sb.ToString();

            sb.Remove(0, sb.Length);

            EachClass.EachTemplatePage(dir, sb, TemplatePageType.Category);
            categoryTplOpts = sb.ToString();


            //获取栏目及文档模板绑定
            categoryTpl = String.IsNullOrEmpty(category.CategoryTemplate) ? "" : category.CategoryTemplate;
            archiveTpl = String.IsNullOrEmpty(category.CategoryArchiveTemplate) ? "" : category.CategoryArchiveTemplate;

            object data = new
            {
                entity = JsonSerializer.Serialize(category),
                url = base.Request.RawUrl,
                categories = categoryOptions,
                //categoryTypes = categoryTypeOptions,
                parentLft = pleft,
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
            string categoryTag;
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategoryByLft(
                this.SiteId,
                int.Parse(form["lft"].ToString()));

            if (!(category.Id > 0)) return base.ReturnError("分类不存在!");
            categoryTag = category.Tag;

            //获取新的栏目信息
            category = InitCategoryDtoFromHttpPost(form, category);

            //更改了categoryTag则检验,区分大小写
            if (String.Compare(categoryTag, category.Tag,false,CultureInfo.InvariantCulture) != 0)
            {
                if (!ServiceCall.Instance.SiteService.CheckCategoryTagAvailable(
                    this.SiteId,
                    category.Id,
                    category.Tag))
                {
                    return base.ReturnError("标签已经存在！");
                }
            }

            //string moduleID = form["moduleId"];
            //category.ModuleID = int.Parse(moduleID);

            int plft = -1;// int.Parse(form["plft"].ToString());

            //设置并保存
            ServiceCall.Instance.SiteService.SaveCategory(this.SiteId, plft, category);

            return base.ReturnSuccess("保存成功!");
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        [MCacheUpdate(CacheSign.Category | CacheSign.Link)]
        public string Delete_POST()
        {
            int lft = int.Parse(base.Request.Form["lft"]);
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategoryByLft(this.SiteId, lft);

            if (!(category.Id > 0))
                return base.ReturnError("栏目不存在");

            try
            {
                //bool result =
                ServiceCall.Instance.SiteService.DeleteCategoryByLft(this.SiteId, lft);
                return base.ReturnSuccess();
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }

        }

        /// <summary>
        /// 左侧栏目树
        /// </summary>
        public void Tree_GET()
        {

            TreeNode node = ServiceCall.Instance.SiteService.GetCategoryTreeNode(this.SiteId, 1);

            BuiltCacheResultHandler<String> bh = () =>
            {
                string _treeHtml = "";
                StringBuilder sb = new StringBuilder();


                //var allCate = CmsLogic.Category.GetCategories();

                sb.Append("<dl><dt class=\"tree-title\"><img src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\" class=\"tree-title\"/>")
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
                string treeHtml = CacheFactory.Sington.GetResult<String>(
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

        //private IList<Category> cates;
        //private Category nextCategory;
        //private Category parentNextCategory;
        //private Category tempCategory;


        //[Obsolete]
        //private void ItrTree(Category cate, StringBuilder sb,int siteID)
        //{

        //    int level = new List<Category>(CmsLogic.Category.GetCategories(cate.Lft, cate.Rgt, CategoryContainerOption.Parents)).Count;

        //    if (level >= 2) return;


        //    if (cate.Lft != 1)
        //    {
        //        if (cate.SiteId != siteID) return;  //判断站点
        //        sb.Append("<dd>");
        //    }

        //    cates=new List<Category>(cate.NextLevelCategories);
        //    nextCategory = CmsLogic.Category.GetNext(cate.Lft, cate.Rgt);
        //    string className="";

        //    //获取父类及父类的下一个类目
        //    if (cate.Lft != 1)
        //    {
        //        tempCategory = CmsLogic.Category.GetParent(cate.Lft, cate.Rgt);
        //        parentNextCategory = CmsLogic.Category.GetNext(tempCategory.Lft, tempCategory.Rgt);
        //    }


        //    if (cates.Count != 0)
        //    {
        //        if (cate.Lft != 1)
        //        {
        //            sb.Append("<dl><dt")
        //                .Append(" lft=\"").Append(cate.Lft.ToString()).Append("\" level=\"")
        //                .Append(level.ToString()).Append("\">");

        //            for (int i = 0; i < level; i++)
        //            {
        //                //最后一个竖线不显示
        //                 if (i!=0 && nextCategory==null && (i!=level-1 || ( i==level-1 && parentNextCategory==null)))
        //                {
        //                    sb.Append("<img src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
        //                }
        //                else
        //                {
        //                    sb.Append("<img class=\"tree-line\" src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
        //                }
        //            }

        //            //tree-expand:已展开
        //            //tree-expand-last:已经展开最后一个

        //            //tree-collage:未开展
        //            //tree-collage:未展开最后一个

        //            if (level == 0)
        //            {
        //                if (nextCategory == null)
        //                {
        //                    className = "tree-expand tree-expand-last";
        //                }
        //                else
        //                {
        //                    className = "tree-expand";
        //                }
        //            }
        //            else
        //            {
        //                if (nextCategory == null)
        //                {
        //                    className = "tree-collage tree-collage-last";
        //                }
        //                else
        //                {
        //                    className = "tree-collage";
        //                }
        //            }


        //            sb.Append("<img class=\"").Append(className)
        //            .Append("\" src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/><span class=\"txt parent\" cid=\"")
        //            .Append(cate.ID.ToString()).Append("\">").Append(cate.Name).Append("</span></dt>");
        //        }

        //        foreach (var c in cates)
        //        {
        //            ItrTree(c, sb,siteID);
        //        }
        //        if (cate.Lft != 1)
        //        {
        //            sb.Append("</dl>");
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i <level; i++)
        //        {
        //            if (i != 0 && i == level - 1 && nextCategory==null)
        //            {
        //                sb.Append("<img src=\"/framework/assets/sys_themes/default/icon_trans.png\"/>");
        //            }
        //            else if (parentNextCategory != null)
        //            {
        //                sb.Append("<img class=\"tree-line\" src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
        //            }
        //            else
        //            {
        //                sb.Append("<img src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/>");
        //            }
        //        }

        //        sb.Append("<img class=\"tree-item\" src=\"/framework/assets/sys_themes/default/icon_trans.png\" width=\"24\" height=\"24\"/><span class=\"txt archvie\" cid=\"")
        //            .Append(cate.ID.ToString()).Append("\">").Append(cate.Name).Append("</span>");
        //    }

        //    if (cate.Lft != 1)
        //    {
        //        sb.Append("</dd>");
        //    }
        //}

        ///// <summary>
        ///// 分类页
        ///// </summary>
        //public void GetTree_GET()
        //{
        //    int lft = int.Parse(base.Request["lft"]);
        //    int lineNum = int.Parse(base.Request["lines"]);
        //    int level = int.Parse(base.Request["level"]);


        //   // string html= ServiceCall.Instance.SiteService.GetChildsTreeHtml(this.SiteId, lft, lineNum, level);

        //   // base.Render(html);
        //}

    }
}
