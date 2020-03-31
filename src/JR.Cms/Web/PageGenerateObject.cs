//
// Copyright (C) 2011 OPS,All rights reseved.
// PageGeneratorObject.cs
// Author: newmin
// Date  : 2011/07/29
//


using System;
using System.Text.RegularExpressions;
using System.Web;
using JR.Cms.Core;
using JR.Cms.Core.Interface;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.WebImpl.PageModels;
using JR.Stand.Core.Framework.Web;
using JR.Stand.Core.Framework.Web.Utils;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;
using ArchiveDto = JR.Cms.ServiceDto.ArchiveDto;
using CategoryDto = JR.Cms.ServiceDto.CategoryDto;

namespace JR.Cms.WebImpl
{
    /// <summary>
    /// 页面生成器对象
    /// </summary>
    public class PageGeneratorObject : ICmsPageGenerator
    {
        private PageSite _site;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public PageGeneratorObject(CmsContext context)
        {
            //this.context=context;
            _site = new PageSite(context.CurrentSite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tplPath"></param>
        /// <returns></returns>
        public virtual string FormatTemplatePath(string tplPath)
        {
            var ts = Cms.TemplateManager.Get(_site.Tpl);
            if (ts.CfgEnabledMobiPage && Cms.Context.DeviceType == DeviceType.Mobile)
            {
                var path = string.Format("/{0}/_mobile_/{1}", _site.Tpl, tplPath);
                if (Cms.Template.Exists(path)) return path;
            }

            return $"/{_site.Tpl}/{tplPath}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tplPath"></param>
        /// <returns></returns>
        public virtual string GetTemplateId(string tplPath)
        {
            const string pattern = "^\\/*templates\\/(?<id>.+?)\\.h*tml$";
            var m = Regex.Match(tplPath, pattern);
            return m.Groups[1].Value;
        }

        /// <summary>
        /// 获取栏目的模板Id
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual string GetCategoryTemplateId(CategoryDto category)
        {
            if (category.CategoryTemplate != null)
            {
                if (category.CategoryTemplate.StartsWith("templates"))
                    return "/" + GetTemplateId(category.CategoryTemplate);
                else
                    return FormatTemplatePath(category.CategoryTemplate.Replace(".tml", "").Replace(".html", ""));
            }

            //设置默认的模板
            return FormatTemplatePath("category");
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

            if (archive.TemplatePath != null)
            {
                //return "/" + this.GetTemplateId(archive.TemplatePath);
                if (archive.TemplatePath.StartsWith("templates"))
                    return "/" + GetTemplateId(archive.TemplatePath);
                else
                    return FormatTemplatePath(archive.TemplatePath.Replace(".tml", "").Replace(".html", ""));
            }

            //设置默认的模板
            return FormatTemplatePath("archive");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual string GetIndex(params object[] args)
        {
            return PageUtility.Require(FormatTemplatePath("index"), page =>
            {
                page.AddVariable("site", _site);
                page.AddVariable("page",
                    new PageVariable
                    {
                        Title = _site.Title, SubTitle = _site.Title, Keywords = _site.Keywords,
                        Description = _site.Description
                    });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="pageIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual string GetCategory(CategoryDto category, int pageIndex, params object[] args)
        {
            var tplId = GetCategoryTemplateId(category);


            //
            //todo:  /news/news/news/时候仍然能访问
            //

            //用于当前的模板共享数据
            //Cms.Context.Items["category.tag"] = category.Tag;
            Cms.Context.Items["category.path"] = category.Path;
            Cms.Context.Items["module.id"] = category.ModuleId;
            Cms.Context.Items["page.index"] = pageIndex;

            var isFirstPage = pageIndex == 1;

            string title;
            if (pageIndex == 1)
            {
                if (!string.IsNullOrEmpty(category.PageTitle))
                    title = category.PageTitle;
                else
                    title = $"{category.Name}_{_site.Title}";
            }
            else
            {
                switch (_site.Language)
                {
                    case Languages.Zh_CN:
                        title = string.Format("- 第" + pageIndex + "页");
                        break;
                    default:
                    case Languages.En:
                        title = string.Format("- page " + pageIndex);
                        break;
                }

                title = $"{category.Name}{title}_{_site.Title}";
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
                page.AddVariable("module", new Module {ID = category.ModuleId});
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
            var tplId = GetArchiveTemplateId(archive);
            var category = archive.Category;

            //用于当前的模板共享数据
            Cms.Context.Items["archive.id"] = archive.Id;
            Cms.Context.Items["category.path"] = category.Path;
            Cms.Context.Items["module.id"] = category.ModuleId;

            //解析模板
            var html = PageUtility.Require(tplId,
                page =>
                {
                    page.AddVariable("site", _site);

                    var pageArchive = new PageArchive(archive);

                    page.AddVariable("archive", pageArchive);

                    page.AddVariable("category", category);
                    page.AddVariable("module", new Module {ID = category.ModuleId});


                    page.AddVariable("page", new PageVariable
                    {
                        Title = string.Format("{0}_{1}_{2}", archive.Title, category.Name, _site.Title),
                        SubTitle = _site.Title,
                        Keywords = archive.Tags,
                        Description = pageArchive.Outline.Replace("\"", string.Empty)
                    });
                });
            return html;
        }

        public virtual string GetSearch(params object[] args)
        {
            var cate = args[0] as string;
            var key = args[1] as string;

            var ctx = HttpHosting.Context;
            //计算页码
            int pageIndex;
            int.TryParse(ctx.Request.Query("p"), out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            //模板标签共享数据
            Cms.Context.Items["search.key"] = key;
            Cms.Context.Items["search.param"] = cate; //搜索按模块或按栏目
            Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            var html = PageUtility.Require(FormatTemplatePath("search"),
                page =>
                {
                    page.AddVariable("site", _site);

                    page.AddVariable("page", new PageVariable
                    {
                        Title = string.Format("\"{0}\"相关的信息{1}_{2}",
                            key,
                            pageIndex == 1
                                ? string.Empty
                                : string.Format(Cms.Language.Get(LanguagePackageKey.PAGE_PagerTitle),
                                    pageIndex.ToString()),
                            _site.Title),
                        SubTitle = _site.Title,
                        Keywords = key,
                        Description = string.Empty,
                        PageIndex = pageIndex
                    });

                    page.AddVariable("search", new
                    {
                        Key = key.Replace(",", "+"),
                        Param = cate,
                        PageIndex = pageIndex,
                        EscapeKey = HttpUtil.UrlEncode(key.ToString()),
                        Escape_Key = HttpUtil.UrlEncode(key.ToString()), //过期
                        Cate = cate //过期
                    });
                }
            );

            return html;
        }

        public virtual string GetTagArchive(params object[] args)
        {
            var key = args[0] as string;
            var ctx = HttpHosting.Context;
            //计算页码
            int pageIndex;
            int.TryParse(ctx.Request.Query("p"), out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            //模板标签共享数据
            Cms.Context.Items["tag.key"] = key;
            Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            var html = PageUtility.Require(FormatTemplatePath("tag"),
                page =>
                {
                    page.AddVariable("site", _site);

                    page.AddVariable("page", new PageVariable
                    {
                        Title = string.Format("{0}{1}_{2}",
                            key,
                            pageIndex == 1
                                ? string.Empty
                                : string.Format(Cms.Language.Get(LanguagePackageKey.PAGE_PagerTitle),
                                    pageIndex.ToString()),
                            _site.Title),
                        SubTitle = _site.Title,
                        Keywords = key,
                        Description = string.Empty,
                        PageIndex = pageIndex
                    });

                    page.AddVariable("tag", new
                    {
                        Key = key.Replace(",", "+"),
                        Param = key,
                        PageIndex = pageIndex,
                        EscapeKey = HttpUtil.UrlEncode(key.ToString()),
                        Escape_Key = HttpUtil.UrlEncode(key.ToString()),
                        Cate = key
                    });
                }
            );
            return html;
        }
    }
}