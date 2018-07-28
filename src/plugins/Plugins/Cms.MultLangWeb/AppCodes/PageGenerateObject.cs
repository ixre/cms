//
// Copyright (C) 2011 OPS,All rights reseved.
// PageGeneratorObject.cs
// Author: newmin
// Date  : 2011/07/29
//


namespace CMS.Web
{
    using System;
    using System.Data;
    //using System.Data.Extensions;
	using J6.Data;
    using System.Text;
    using System.Web;
    using J6.Cms;
    using J6.Cms.BLL;
    using J6.Cms.Models;
    using J6.Template;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 页面生成器对象
    /// </summary>
    public class PageGeneratorObject
    {
        private static ArchiveBLL bll = new ArchiveBLL();
        private static CategoryBLL cbll = new CategoryBLL();
        private static TemplateBindBLL tbbll=new TemplateBindBLL();

        private static string formatTpl(string tplName)
        {
            return String.Format("/{0}/{1}", Settings.TPL_Name, tplName);
        }

        /// <summary>
        /// 根据模板路径获取模板ID
        /// </summary>
        /// <param name="tplPath"></param>
        /// <returns></returns>
        private static string getTpl(string tplPath)
        {
            const string pattern="^templates\\/(?<id>.+?)\\.html$";
            //const string pattern="^templates\\/(?<id>[^\\.]+)\\.[a-z]+$";
            Match m=Regex.Match(tplPath,pattern);
            return m.Groups[1].Value;
        }

        public static GenerateDelegate InitData = () =>
        {
        };

        #region 首页

        /// <summary>
        /// 首页
        /// </summary>
        public static GenerateDelegate Default = () =>
        {
            HttpContext.Current.Response.Write(
                PageUtility.Require(formatTpl("index"), null)
                );
        };
        #endregion

        #region 分类页面
        public static CategoryListPageGenerateDelegate CategoryPage = (category, pageIndex) =>
        {
            string tplID = null;
            ModuleBLL mbll = new ModuleBLL();
            Module m = mbll.GetModule(category.ModuleID);
            if (m != null)
            {
                TemplateBind tb = tbbll.GetCategoryTemplateBind(category.ID);
                if (tb != null)
                {
                    string id = getTpl(tb.TplPath);
                    if (id != null)
                    {
                        tplID = "/" + id;
                    }
                }
            }

            //设置默认的模板
            if (String.IsNullOrEmpty(tplID))
            {
                tplID = formatTpl("list");
            }

            Category parentCategory = cbll.Get(a => a.ID == category.PID);
  
           return PageUtility.Require(tplID,new
            {
                categoryName = category.Name,
                categoryTag=category.Tag,
                parentCategoryName=parentCategory==null?"":parentCategory.Name,
                parentCategoryTag=parentCategory==null?"":parentCategory.Tag,
                pageIndex=pageIndex,
                moduleId=category.ModuleID,
                pageTitle=pageIndex==1?"":"(第"+pageIndex+"页)",
                keywords = category.Keywords,
                description = category.Description
            });
        };

        #endregion

        #region 文档页面

        public static ArchiveBehavior ArchivePage = (category, archive) =>
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
            string tplID =null;
            ModuleBLL mbll = new ModuleBLL();
            Module m = mbll.GetModule(category.ModuleID);
            if (m != null)
            {
                TemplateBind tb = tbbll.GetArchiveTemplateBind(archive.ID, category.ID);
                if (tb != null)
                {
                    string id = getTpl(tb.TplPath);
                    if (id != null)
                    {
                        tplID = "/" + id;
                    }

                }
            }

            //设置默认的模板
            if (String.IsNullOrEmpty(tplID))
            {
                tplID = formatTpl("archive");
            }           

            string description=ArchiveUtility.GetOutline(archive,200);
           string html= PageUtility.Require(tplID,
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

        };

        #endregion 

        #region 搜索
        public static ParameterGenerateDelegate Search = (args) =>
        {
            string cate=args[0] as string;
             string  key=args[1] as string;

            //计算页码
            int pageIndex;
            int.TryParse(HttpContext.Current.Request["p"], out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            string html = PageUtility.Require(formatTpl("search"),
                new
                {
                    key = key.ToString().Replace(",", "+"),
                    cate=cate,
                    escape_key = HttpContext.Current.Server.UrlEncode(key.ToString()),
                    pageIndex = pageIndex,
                    pageTitle = pageIndex == 1 ? String.Empty : "(第" + pageIndex + "页)"              //分页标题
                }
             );

            return html;
        };
        #endregion

        #region 标签
        public static ParameterGenerateDelegate Tag = (args) =>
        {
            string key = args[0] as string;

            //计算页码
            int pageIndex;
            int.TryParse(HttpContext.Current.Request["p"], out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            string html = PageUtility.Require(formatTpl("tag"),
                new
                {
                    key = key.ToString().Replace(",", "+"),
                    escape_key = HttpContext.Current.Server.UrlEncode(key.ToString()),
                    pageIndex = pageIndex,
                    pageTitle = pageIndex == 1 ? String.Empty : "(第" + pageIndex + "页)"              //分页标题
                }
             );

            return html;
        };
        #endregion

    }
}