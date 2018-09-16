//
// Copyright (C) 2011 OPS,All rights reseved.
// PageGeneratorObject.cs
// Author: newmin
// Date  : 2011/07/29
//


using T2.Cms.Domain.Interface.Enum;

namespace T2.Cms.Web
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;
    using T2.Cms.DataTransfer;
    using T2.Cms;
    using T2.Cms.Core;
    using T2.Cms.Core.Interface;
    using T2.Cms.Domain.Interface.Common.Language;
    using T2.Cms.Domain.Interface.Models;
    using T2.Cms.Web.PageModels;

    /// <summary>
    /// 页面生成器对象
    /// </summary>
    public class PageGeneratorObject : ICmsPageGenerator
    {
        private PageSite _site;

        public PageGeneratorObject(CmsContext context)
        {
            //this.context=context;
            this._site = new PageSite(context.CurrentSite);
        }

        public virtual string FormatTemplatePath(string tplPath)
        {
            if (Cms.Context.DeviceType == DeviceType.Mobi &&
                Cms.TemplateManager.Get(this._site.Tpl).CfgEnabledMobiPage)
            {
                return String.Format("/{0}/mobi/{1}", _site.Tpl, tplPath);
            }
            return String.Format("/{0}/{1}", _site.Tpl, tplPath);
        }

        public virtual string GetTemplateId(string tplPath)
        {
            const string pattern = "^\\/*templates\\/(?<id>.+?)\\.h*tml$";
            Match m = Regex.Match(tplPath, pattern);
            return m.Groups[1].Value;
        }

        /// <summary>
        /// 获取栏目的模板Id
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual string GetCategoryTemplateId(CategoryDto category)
        {
            if (category.CategoryTemplate!=null)
            {
                if (category.CategoryTemplate.StartsWith("templates"))
                {
                    return "/" + this.GetTemplateId(category.CategoryTemplate);
                }
                else
                {
                    return this.FormatTemplatePath(category.CategoryTemplate.Replace(".tml", "").Replace(".html", ""));
                }
            }
            //设置默认的模板
            return this.FormatTemplatePath("category");
        }

        /// <summary>
        /// 获取文档的模板ID
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        public virtual string GetArchiveTemplateId(ArchiveDto archive)
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
                //return "/" + this.GetTemplateId(archive.TemplatePath);
                if (archive.TemplatePath.StartsWith("templates"))
                {
                    return "/" + this.GetTemplateId(archive.TemplatePath);
                }
                else
                {
                    return this.FormatTemplatePath(archive.TemplatePath.Replace(".tml", "").Replace(".html", ""));
                }

            }
            //设置默认的模板
            return this.FormatTemplatePath("archive");
        }

        public virtual string GetIndex(params object[] args)
        {
            return PageUtility.Require(this.FormatTemplatePath("index"), page =>
                {
                    page.AddVariable("site", this._site);
                    page.AddVariable("page", new PageVariable { Title = _site.Title, SubTitle = _site.Title, Keywords = _site.Keywords, Description = _site.Description });
                });
        }

        public virtual string GetCategory(CategoryDto category, int pageIndex, params object[] args)
        {
            string tplId = this.GetCategoryTemplateId(category);


            //
            //todo:  /news/news/news/时候仍然能访问
            //

            //用于当前的模板共享数据
            //Cms.Context.Items["category.tag"] = category.Tag;
            Cms.Context.Items["category.path"] = category.Path;
            Cms.Context.Items["module.id"] = category.ModuleId;
            Cms.Context.Items["page.index"] = pageIndex;

            bool isFirstPage = pageIndex == 1;

            string title;
            if (pageIndex == 1)
            {
                if (!String.IsNullOrEmpty(category.PageTitle))
                {
                    title = category.PageTitle;
                }
                else
                {
                    title = String.Format("{0}_{1}", category.Name, _site.Title);
                }
            }
            else
            {
                switch (_site.Language)
                {
                    case Languages.Zh_CN:
                        title = String.Format("- 第" + pageIndex.ToString() + "页");
                        break;
                    default:
                    case Languages.En:
                        title = String.Format("- page " + pageIndex.ToString());
                        break;
                }

                title = String.Format("{0}{1}_{2}", category.Name, title, _site.Title);
            }

            //解析模板
            return PageUtility.Require(tplId, page =>
            {
                page.AddVariable("site", _site);
                page.AddVariable("page", new PageVariable
                            {
                                Title = title,
                                SubTitle = _site.Title,
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
            string tplId = this.GetArchiveTemplateId(archive);
            CategoryDto category = archive.Category;

            //用于当前的模板共享数据
            Cms.Context.Items["archive.id"] = archive.Id;
            Cms.Context.Items["category.path"] = category.Path;
            Cms.Context.Items["module.id"] = category.ModuleId;

            //解析模板
            string html = PageUtility.Require(tplId,

                                              page =>
                                              {

                                                  page.AddVariable("site",this._site);

                                                  PageArchive pageArchive = new PageArchive(archive);

                                                  page.AddVariable("archive", pageArchive);

                                                  page.AddVariable("category", category);
                                                  page.AddVariable("module", new Module { ID = category.ModuleId });


                                                  page.AddVariable("page", new PageVariable
                                                  {
                                                      Title = String.Format("{0}_{1}_{2}", archive.Title, category.Name, _site.Title),
                                                      SubTitle = _site.Title,
                                                      Keywords = archive.Tags,
                                                      Description = pageArchive.Outline.Replace("\"", String.Empty)
                                                  });

                                              });
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
            Cms.Context.Items["search.key"] = key;
            Cms.Context.Items["search.param"] = cate;       //搜索按模块或按栏目
            Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            string html = PageUtility.Require(this.FormatTemplatePath("search"),
                                              page =>
                                              {
                                                  page.AddVariable("site", this._site);

                                                  page.AddVariable("page", new PageVariable
                                                                   {
                                                                       Title = String.Format("\"{0}\"相关的信息{1}_{2}",
                                                                                             key,
                                                                                             pageIndex == 1 ? String.Empty :
                                                                                             String.Format(Cms.Language.Get(LanguagePackageKey.PAGE_PagerTitle),pageIndex.ToString()),
                                                                                             _site.Title),
                                                                       SubTitle = _site.Title,
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
            Cms.Context.Items["tag.key"] = key;
            Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            string html = PageUtility.Require(this.FormatTemplatePath("tag"),
                                              page =>
                                              {

                                                  page.AddVariable("site", this._site);

                                                  page.AddVariable("page", new PageVariable
                                                                   {
                                                                       Title = String.Format("{0}{1}_{2}",
                                                                                             key,
                                                                                             pageIndex == 1 ? String.Empty :
                                                                                             String.Format(Cms.Language.Get(LanguagePackageKey.PAGE_PagerTitle), pageIndex.ToString()),
                                                                                             _site.Title),
                                                                       SubTitle = _site.Title,
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