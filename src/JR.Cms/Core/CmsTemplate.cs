//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
// 
// Project: Cms.Cms
// FileName : CmsContext.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2013/06/23 14:53:11
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/jr-cms
//
//

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using JR.Cms.Conf;
using JR.DevFw.Template;

namespace JR.Cms.Core
{
    public enum TemplatePageType
    {
        /// <summary>
        /// 自定义页
        /// </summary>
        Custom = 0,

        /// <summary>
        /// 部分页
        /// </summary>
        Partial = 1,

        /// <summary>
        /// 首页
        /// </summary>
        Index = 2,

        /// <summary>
        /// 栏目页
        /// </summary>
        Category = 3,

        /// <summary>
        /// 文档页
        /// </summary>
        Archive = 4,

        /// <summary>
        /// 搜索页
        /// </summary>
        Search = 5,

        /// <summary>
        /// 404页
        /// </summary>
        Notfound = 6,

        /// <summary>
        /// 标签页
        /// </summary>
        Tag = 7
    }

    /// <summary>
    /// Cms模板
    /// </summary>
    public class CmsTemplate
    {
        /// <summary>
        /// 模板
        /// </summary>
        private static TemplateRegistry tpl;

        /// <summary>
        /// 模板页描述
        /// </summary>
        private static readonly IDictionary<TemplatePageType, string> pageDescripts;

        /// <summary>
        /// 系统模板页
        /// </summary>
        private static readonly string[] systemTemplatePages;

        static CmsTemplate()
        {
            pageDescripts = new Dictionary<TemplatePageType, string>();
            pageDescripts.Add(TemplatePageType.Index, "首页文件");
            pageDescripts.Add(TemplatePageType.Category, "栏目页面");
            pageDescripts.Add(TemplatePageType.Tag, "标签页面");
            pageDescripts.Add(TemplatePageType.Notfound, "404错误页面");
            pageDescripts.Add(TemplatePageType.Archive, "文档页面");
            pageDescripts.Add(TemplatePageType.Search, "搜索页面");
            pageDescripts.Add(TemplatePageType.Partial, "通用部分页");
            pageDescripts.Add(TemplatePageType.Custom, "自定义页");

            //系统模板页
            systemTemplatePages = new string[]{
                "index.html",
                "category.html",
                "archive.html",
                "search.html",
                "tag.html",
                "notfound.html"
            };
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tplPath"></param>
        /// <param name="tplNameAsTemplateId"></param>
        public void Register(string tplPath, bool tplNameAsTemplateId)
        {
            if (Settings.loaded)
            {
                try
                {
                    //初始化模板
                    global::JR.DevFw.Template.Config.EnabledCache = true;
                    //模板设置是否缓存
                    global::JR.DevFw.Template.Config.EnabledCompress = Settings.TPL_USE_COMPRESS;
                    tpl = new TemplateRegistry(tplPath,  tplNameAsTemplateId ?
                        TemplateNames.FileName : TemplateNames.ID);
                    //将配置写入模板缓存
                    RegisterTemplate();
                }
                catch (Exception ex)
                {
                    HttpRuntime.UnloadAppDomain();
                    throw ex;
                }
            }
            else
            {
                HttpRuntime.UnloadAppDomain();
                throw new ApplicationException("请在系统加载配置后再进行初始化!");
            }

        }


        /// <summary>
        /// 重新加载配置
        /// </summary>
        public void Register()
        {
            //  Configuration.Configure();

            //模板设置是否缓存
            Config.EnabledCompress = Settings.TPL_USE_COMPRESS;
          

            //将配置写入模板缓存
            RegisterTemplate();
        }


        /// <summary>
        /// 注册模板
        /// </summary>
        private static void RegisterTemplate()
        {
            //将文件中的加载到模板缓存中
            //AddTagsFromSettingsFile(new SettingFile(Configuration.cmsConfigFile), true);

            //注册模板
            tpl.Register();
        }

        /*
        /// <summary>
        /// 从配置文件中加载到模板缓存
        /// </summary>
        /// <param name="file"></param>
        private static void AddTagsFromSettingsFile(SettingFile file, bool upper)
        {
            foreach (KeyValuePair<string, string> p in file.SearchKey(""))
            {
                TemplateCache.Tags[upper ? p.Key.ToUpper() : p.Key] = p.Value;
            }
        }
        */


        /// <summary>
        /// 获取页面类型
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public TemplatePageType GetPageType(string templatePath)
        {
            RegexOptions regopt = RegexOptions.IgnoreCase;

            if (Regex.IsMatch(templatePath, "(^category|/category)\\.[^\\.]*\\.*html$", regopt))
            {
                return TemplatePageType.Category;
            }
            else if (Regex.IsMatch(templatePath, "[^\\.]+\\.(part\\.html||phtml)$", regopt))
            {
                return TemplatePageType.Partial;
            }
            else if (Regex.IsMatch(templatePath, "(^archive|/archive)\\.[^\\.]*\\.*html$", regopt))
            {
                return TemplatePageType.Archive;
            }
            else if (Regex.IsMatch(templatePath, "(^index|/index)\\.[^\\.]*\\.*html$", regopt))
            {
                return TemplatePageType.Index;
            }
            else if (Regex.IsMatch(templatePath, "(^search|/search)\\.[^\\.]*\\.*html$", regopt))
            {
                return TemplatePageType.Search;
            }
            else if (Regex.IsMatch(templatePath, "(^tag|/tag)\\.[^\\.]*\\.*html$", regopt))
            {
                return TemplatePageType.Tag;
            }
            else if (Regex.IsMatch(templatePath, "(^notfound|/notfound)\\.[^\\.]*\\.*html$", regopt))
            {
                return TemplatePageType.Notfound;
            }
            return TemplatePageType.Custom;
        }

        /// <summary>
        /// 获取页面描述
        /// </summary>
        /// <param name="pageType"></param>
        /// <returns></returns>
        public string GetPageDescript(TemplatePageType pageType)
        {
            if (pageDescripts.Keys.Contains(pageType))
            {
                return pageDescripts[pageType];
            }
            return null;
        }

        /// <summary>
        /// 是否是系统页面
        /// </summary>
        /// <param name="templateFileName"></param>
        /// <returns></returns>
        public bool IsSystemTemplate(string templateFileName)
        {
            return Array.Exists(systemTemplatePages, a => String.Compare(a, templateFileName, true) == 0);
        }

        /// <summary>
        /// 模板是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path)
        {
            return TemplateRegistry.Exists(path);
        }
    }
}