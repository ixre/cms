//
// Copyright (C) 2011 OPS,All rights reseved.
// PageGeneratorObject.cs
// Author: newmin
// Date  : 2011/07/29
//


using AtNet.Cms.Core;
using AtNet.Cms.Core.Interface;
using AtNet.Cms.Domain.Interface.Common.Language;
using AtNet.Cms.Domain.Interface.Models;

namespace AtNet.Cms.Web
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;
    using AtNet.Cms.DataTransfer;
    using AtNet.Cms;

    /// <summary>
    /// 页面生成器对象
    /// </summary>
    public class PageGeneratorObject : ICmsPageGenerator
    {
        private SiteDto site;

        public PageGeneratorObject(CmsContext context)
        {
            //this.context=context;
            this.site = context.CurrentSite;
        }

        public virtual string FormatTemplatePath(string tplPath)
        {
            return String.Format("/{0}/{1}", site.Tpl, tplPath);
        }

        public virtual string GetTemplateId(string tplPath)
        {
            const string pattern = "^\\/*templates\\/(?<id>.+?)\\.html$";
            //const string pattern="^templates\\/(?<id>[^\\.]+)\\.[a-z]+$";
            Match m = Regex.Match(tplPath, pattern);
            return m.Groups[1].Value;
        }

        /// <summary>
        /// 获取栏目的模板ID
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual string GetCategoryTemplateId(CategoryDto category)
        {
            //Module m = CmsLogic.Module.GetModule(category.ModuleID);
            //if (m != null)
            //{ //}

            if (category.CategoryTemplate!=null)
            {
                return "/"+this.GetTemplateId(category.CategoryTemplate);
            }
            //设置默认的模板
            return this.FormatTemplatePath("category");
        }

        /// <summary>
        /// 获取文档的模板ID
        /// </summary>
        /// <param name="category"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
        public virtual string GetArchiveTemplateID(ArchiveDto archive)
        {
            ////获取模板ID
            //string tplID = null;
            //Module m = CmsLogic.Module.GetModule(archive.Category.ModuleID);
            //if (m != null)
            //{
            //    TemplateBind tb = CmsLogic.TemplateBind.GetArchiveTemplateBind(archive.ID, archive.Category.ID);
            //    if (tb != null)
            //    {
            //        string id = this.GetTemplateID(tb.TplPath);
            //        if (id != null)
            //        {
            //            tplID = "/" + id;
            //        }
            //    }
            //}

            ////设置默认的模板
            //if (String.IsNullOrEmpty(tplID))
            //{
            //    tplID = this.FormatTemplatePath("archive");
            //}
            //return tplID;

            if (archive.TemplatePath!=null)
            {
                return "/" + this.GetTemplateId(archive.TemplatePath);
            }
            //设置默认的模板
            return this.FormatTemplatePath("archive");
        }

        public virtual string GetIndex(params object[] args)
        {
            return PageUtility.Require(this.FormatTemplatePath("index"), page =>
                {
                    page.AddVariable("site", this.site);
                    page.AddVariable("page", new PageVariable { Title = site.SeoTitle, SubTitle = site.SeoTitle, Keywords = site.SeoKeywords, Description = site.SeoDescription });
                });
        }

        public virtual string GetCategory(CategoryDto category, int pageIndex, params object[] args)
        {
            string tplID = this.GetCategoryTemplateId(category);


            //
            //todo:  /news/news/news/时候仍然能访问
            //

            //用于当前的模板共享数据
            AtNet.Cms.Cms.Context.Items["category.tag"] = category.Tag;
            AtNet.Cms.Cms.Context.Items["module.id"] = category.ModuleId;
            AtNet.Cms.Cms.Context.Items["page.index"] = pageIndex;

            bool _isFirstPage = pageIndex == 1;

            string title;
            if (pageIndex == 1)
            {
                if (!String.IsNullOrEmpty(category.PageTitle))
                {
                    title = category.PageTitle;
                }
                else
                {
                    title = String.Format("{0}_{1}", category.Name, site.SeoTitle);
                }
            }
            else
            {
                switch (site.Language)
                {
                    case Languages.Zh_CN:
                        title = String.Format("- 第" + pageIndex.ToString() + "页");
                        break;
                    default:
                    case Languages.En_US:
                        title = String.Format("- page " + pageIndex.ToString());
                        break;
                }

                title = String.Format("{0}{1}_{2}", category.Name, title, site.SeoTitle);
            }

            //解析模板
            return PageUtility.Require(tplID, page =>
            {
                page.AddVariable("site", site);
                page.AddVariable("page", new PageVariable
                            {
                                Title = title,
                                SubTitle = site.SeoTitle,
                                Keywords = category.Keywords,
                                Description = category.Description,
                                PageIndex = pageIndex
                            });
                page.AddVariable("category", category);
                page.AddVariable("module", new Module { ID = category.ModuleId });
            });
        }

        public virtual string GetArchive(ArchiveDto archive, params object[] args)
        {
            #region 属性
            /*
            //验证密码
            PropertyBLL pbll = new PropertyBLL();
            var a = pbll.GetProperty(category.ModuleID, "pwd");
            if (a != null)
            {
                var json = archive.GetPropertiesJson();
                string key = json.GetValue("pwd");
                if (!String.IsNullOrEmpty(key))
                {
                    string requireKey = HttpContext.Current.Request.QueryString["pwd"];
                    if (requireKey != key)
                    {
                        HttpContext.Current.Response.Write("<div style='margin:0 auto;padding:30px 0'>请输入密码：<input type='text'/><input type='button' value='提交' onclick=\"location.replace(location.href+'?pwd='+this.previousSibling.value)\"/></div>");
                        HttpContext.Current.Response.End();
                    }

                }
            }*/
            #endregion

            //获取模板ID
            string tplID = this.GetArchiveTemplateID(archive);
            CategoryDto category = archive.Category;

            //用于当前的模板共享数据
            AtNet.Cms.Cms.Context.Items["archive.id"] = archive.Id;
            AtNet.Cms.Cms.Context.Items["category.tag"] = category.Tag;
            AtNet.Cms.Cms.Context.Items["module.id"] = category.ModuleId;

            //解析模板
            string html = PageUtility.Require(tplID,

                                              page =>
                                              {

                                                  page.AddVariable("site",this.site);

                                                  PageArchive pageArchive = new PageArchive(archive);

                                                  page.AddVariable("archive", pageArchive);

                                                  page.AddVariable("category", category);
                                                  page.AddVariable("module", new Module { ID = category.ModuleId });


                                                  page.AddVariable("page", new PageVariable
                                                  {
                                                      Title = String.Format("{0}_{1}_{2}", archive.Title, category.Name, site.SeoTitle),
                                                      SubTitle = site.SeoTitle,
                                                      Keywords = archive.Tags,
                                                      Description = pageArchive.Outline.Replace("\"", String.Empty)
                                                  });

                                              });

            /*
            HttpRequest request = HttpContext.Current.Request;

            //如果包含查询，则加入标签
            if (!String.IsNullOrEmpty(request.Url.Query))
            {
                //将查询参数作为标签
                html = global::AtNet.DevFw.Template.TemplateRegexUtility.Replace(html, a =>
                {
                    if (request[a.Groups[1].Value] != null)
                    {
                        return request[a.Groups[1].Value];
                    }
                    return a.Value;
                });
            }

            //
            //TODO:模板被替换成空白
            //
             */

            return html;
        }

        public virtual string GetSearch(params object[] args)
        {
            string cate = args[0] as string;
            string key = args[1] as string;

            //计算页码
            int pageIndex;
            int.TryParse(HttpContext.Current.Request["p"], out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            //模板标签共享数据
            AtNet.Cms.Cms.Context.Items["search.key"] = key;
            AtNet.Cms.Cms.Context.Items["search.param"] = cate;       //搜索按模块或按栏目
            AtNet.Cms.Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            string html = PageUtility.Require(this.FormatTemplatePath("search"),
                                              page =>
                                              {
                                                  page.AddVariable("site", this.site);

                                                  page.AddVariable("page", new PageVariable
                                                                   {
                                                                       Title = String.Format("\"{0}\"相关的信息{1}_{2}",
                                                                                             key,
                                                                                             pageIndex == 1 ? String.Empty :
                                                                                             String.Format(AtNet.Cms.Cms.Language.Get(LanguagePackageKey.PAGE_PagerTitle),pageIndex.ToString()),
                                                                                             site.SeoTitle),
                                                                       SubTitle = site.SeoTitle,
                                                                       Keywords = key,
                                                                       Description = String.Empty,
                                                                       PageIndex = pageIndex
                                                                   });

                                                  page.AddVariable("search", new
                                                                   {
                                                                       Key = key.ToString().Replace(",", "+"),
                                                                       Param = cate,
                                                                       PageIndex = pageIndex,
                                                                       EscapeKey = HttpContext.Current.Server.UrlEncode(key.ToString()),
                                                                       Escape_Key = HttpContext.Current.Server.UrlEncode(key.ToString()),  //过期
                                                                       Cate = cate                                                           //过期
                                                                   });
                                              }
                                             );

            return html;
        }

        public virtual string GetTagArchive(params object[] args)
        {
            string key = args[0] as string;

            //计算页码
            int pageIndex;
            int.TryParse(HttpContext.Current.Request["p"], out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            //模板标签共享数据
            AtNet.Cms.Cms.Context.Items["tag.key"] = key;
            AtNet.Cms.Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            string html = PageUtility.Require(this.FormatTemplatePath("tag"),
                                              page =>
                                              {

                                                  page.AddVariable("site", this.site);

                                                  page.AddVariable("page", new PageVariable
                                                                   {
                                                                       Title = String.Format("{0}{1}_{2}",
                                                                                             key,
                                                                                             pageIndex == 1 ? String.Empty :
                                                                                             String.Format(AtNet.Cms.Cms.Language.Get(LanguagePackageKey.PAGE_PagerTitle), pageIndex.ToString()),
                                                                                             site.SeoTitle),
                                                                       SubTitle = site.SeoTitle,
                                                                       Keywords = key,
                                                                       Description = String.Empty,
                                                                       PageIndex = pageIndex
                                                                   });

                                                  page.AddVariable("tag", new
                                                                   {
                                                                       Key = key.ToString().Replace(",", "+"),
                                                                       Param = key,
                                                                       PageIndex = pageIndex,
                                                                       EscapeKey = HttpContext.Current.Server.UrlEncode(key.ToString()),
                                                                       Escape_Key = HttpContext.Current.Server.UrlEncode(key.ToString()),  //过期
                                                                       Cate = key                                                         //过期
                                                                   });
                                              }
                                             );

            return html;
        }

    }
}