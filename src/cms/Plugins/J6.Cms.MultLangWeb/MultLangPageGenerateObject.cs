//
// Copyright (C) 2011 OPS,All rights reseved.
// PageGeneratorObject.cs
// Author: newmin
// Date  : 2011/07/29
//


namespace Spc.Web
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;
    using Spc;
    using Spc.BLL;
    using Spc.Models;
    using Spc.Web;

    /// <summary>
    /// 页面生成器对象
    /// </summary>
    public class MultLangPageGeneratorObject:PageGeneratorObject // ICmsPageGenerator
    {
        private static ArchiveBLL bll = new ArchiveBLL();
        private static CategoryBLL cbll = new CategoryBLL();
        private static TemplateBindBLL tbbll=new TemplateBindBLL();

        public override string FormatTemplatePath(string tplPath)
        {
            string lang = HttpContext.Current.Items["lang"].ToString();
            string pathFormat = tplPath.IndexOf(lang + "/") != -1
                                            ? "/{0}/{1}"
                                            : "/{0}/" + lang + "/{1}";

            return String.Format(pathFormat, Cms.Context.CurrentSite.Tpl, tplPath);
        }

        public override string GetTemplateID(string tplPath)
        {
            const string pattern = "^templates\\/(?<id>.+?)\\.html$";
            //const string pattern="^templates\\/(?<id>[^\\.]+)\\.[a-z]+$";
            Match m = Regex.Match(tplPath, pattern);
            return m.Groups[1].Value;
        }

        private bool IsDefaultLang(string lang)
        {
            return String.Compare(lang, Mvc.Lang.defaultLang, true) == 0;
        }

        public override string GetIndex(params object[] args)
        {
            return MultLangPageUtility.Require(
                this.FormatTemplatePath("index")
                , null);
        }

        public override string GetCategory(Category category, int pageIndex, params object[] args)
        {
            string tplID = null;
            ModuleBLL mbll = new ModuleBLL();
            Module m = mbll.GetModule(category.ModuleID);
            if (m != null)
            {
                TemplateBind tb = tbbll.GetCategoryTemplateBind(category.ID);
                if (tb != null)
                {
                    string id = this.GetTemplateID(tb.TplPath);
                    if (id != null)
                    {
                        tplID = "/" + id;
                    }
                }
            }

            //设置默认的模板
            if (String.IsNullOrEmpty(tplID))
            {
                tplID = this.FormatTemplatePath("category");
            }

            Category parentCategory = cbll.GetParent(category.Lft, category.Rgt);

            //用于当前的模板共享数据
            Cms.Context.Items["category.tag"] = category.Tag;
            Cms.Context.Items["module.id"] = category.ModuleID;
            Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            return MultLangPageUtility.Require(tplID, new
            {
                categoryName = category.Name,
                categoryTag = category.Tag,
                parentCategoryName = parentCategory == null ? "" : parentCategory.Name,
                parentCategoryTag = parentCategory == null ? "" : parentCategory.Tag,
                pageIndex = pageIndex,
                moduleId = category.ModuleID,
                pageTitle = pageIndex == 1 ? "" : "(第" + pageIndex + "页)",
                keywords = category.Keywords,
                description = category.Description
            });
        }

        public override string GetArchive(Category category, Archive archive, params object[] args)
        {

           // return base.GetArchive(category, archive, args);

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
            string tplID =null;
            ModuleBLL mbll = new ModuleBLL();
            Module m = mbll.GetModule(category.ModuleID);
            if (m != null)
            {
                TemplateBind tb = tbbll.GetArchiveTemplateBind(archive.ID, category.ID);
                if (tb != null)
                {
                    string id = this.GetTemplateID(tb.TplPath);
                    if (id != null)
                    {
                        tplID = "/" + id;
                    }
                }
            }

            //设置默认的模板
            if (String.IsNullOrEmpty(tplID))
            {
                tplID = this.FormatTemplatePath("archive");
            }           

            string description=ArchiveUtility.GetOutline(archive,200);



            //用于当前的模板共享数据
            Cms.Context.Items["archive.id"] = archive.ID;
            Cms.Context.Items["category.tag"] = category.Tag;
            Cms.Context.Items["module.id"] = category.ModuleID;


            //解析模板
           string html= MultLangPageUtility.Require(tplID,
                 new
                 {
                     id2=string.IsNullOrEmpty(archive.Alias)?archive.ID:archive.Alias,
                     id=archive.ID,
                     title = archive.Title,
                     categoryName = category.Name,
                     categoryTag = category.Tag,
                     moduleId = category.ModuleID,
                     author = archive.Author,
                     content = archive.Content,
                     tags=archive.Tags,
                     keywords = archive.Tags,
                     description=description.Replace("\"",String.Empty),
                     outline=description,
                     count = archive.ViewCount,
                     source = String.IsNullOrEmpty(archive.Source) ? "原创" : archive.Source,
                     publishdate = string.Format("{0:yyyy-MM-dd}", archive.CreateDate)
                 });


            HttpRequest request=HttpContext.Current.Request;

            //如果包含查询，则加入标签
            if (!String.IsNullOrEmpty(request.Url.Query))
            {
                //将查询参数作为标签
                html = global::J6.Template.TemplateRegexUtility.Replace(html, a =>
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

            return html;
        }

        public override string GetSearch(params object[] args)
        {
            string cate = args[0] as string;
            string key = args[1] as string;

            //计算页码
            int pageIndex;
            int.TryParse(HttpContext.Current.Request["p"], out pageIndex);
            if (pageIndex < 1) pageIndex = 1;


            //模板标签共享数据
            Cms.Context.Items["search.key"] = key;
            Cms.Context.Items["search.param"] =cate;       //搜索按模块或按栏目
            Cms.Context.Items["page.index"] = pageIndex;

            //解析模板
            string html = MultLangPageUtility.Require(this.FormatTemplatePath("search"),
                new
                {
                    key = key.ToString().Replace(",", "+"),
                    cate = cate,
                    escape_key = HttpContext.Current.Server.UrlEncode(key.ToString()),
                    pageIndex = pageIndex,
                    pageTitle = pageIndex == 1 ? String.Empty : "(第" + pageIndex + "页)"              //分页标题
                }
             );

            return html;
        }

        public override string GetTagArchive(params object[] args)
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
            string html = MultLangPageUtility.Require(this.FormatTemplatePath("tag"),
                new
                {
                    key = key.ToString().Replace(",", "+"),
                    escape_key = HttpContext.Current.Server.UrlEncode(key.ToString()),
                    pageIndex = pageIndex,
                    pageTitle = pageIndex == 1 ? String.Empty : "(第" + pageIndex + "页)"              //分页标题
                }
             );

            return html;
        }
    }
}