//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
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
using JR.Cms.Conf;
using JR.Stand.Abstracts;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Web;
using JR.Stand.Core.Template;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Utils;
using JR.Stand.Core.Web;

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
        private readonly TemplateRegistry registry;

        /// <summary>
        /// 模板页描述
        /// </summary>
        private static readonly IDictionary<TemplatePageType, string> pageDescripts;

        /// <summary>
        /// 系统模板页
        /// </summary>
        private static readonly string[] systemTemplatePages;

        private readonly IMemoryCacheWrapper cache;

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
            systemTemplatePages = new[]
            {
                "index.html",
                "category.html",
                "archive.html",
                "search.html",
                "tag.html",
                "notfound.html"
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="names"></param>
        public CmsTemplate(IMemoryCacheWrapper cache, TemplateNames names)
        {
            this.cache = cache;
            var opt = new Options
            {
                EnabledCompress = Settings.TPL_USE_COMPRESS,
                // 非正式环境关闭模板缓存
                EnabledCache = Cms.OfficialEnvironment && Settings.TPL_USE_CACHE,
                UrlQueryShared = true,
                HttpItemShared = true,
                Names = names
            };
            registry = new TemplateRegistry(CreateContainer(), opt);
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tplPath"></param>
        public void Register(string tplPath)
        {
            //将配置写入模板缓存
            if (Settings.loaded) registry.Register(tplPath);
            //将文件中的加载到模板缓存中
            //AddTagsFromSettingsFile(new SettingFile(Configuration.cmsConfigFile), true);
            else
            {
                throw new ApplicationException("请在系统加载配置后再进行初始化!");
            }
        }

        private IDataContainer CreateContainer()
        {
            var ctx = HttpHosting.Context;
            var adapter = new TemplateDataAdapter(ctx, cache);
            return new BasicDataContainer(adapter);
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
            var opt = RegexOptions.IgnoreCase;
            if (Regex.IsMatch(templatePath, "(^category|/category)[^\\.]*\\.html*$", opt))
            {
                return TemplatePageType.Category;
            }
            if (Regex.IsMatch(templatePath, "[^\\.]+\\.(part\\.html||phtml)$", opt))
            {
                return TemplatePageType.Partial;
            }
            if (Regex.IsMatch(templatePath, "(^archive|/archive)[^\\.]*\\.*html*$", opt))
            {
                return TemplatePageType.Archive;
            }
            if (Regex.IsMatch(templatePath, "(^index|/index)[^\\.]*\\.*html*$", opt))
            {
                return TemplatePageType.Index;
            }
            if (Regex.IsMatch(templatePath, "(^search|/search)[^\\.]*\\.*html*$", opt))
            {
                return TemplatePageType.Search;
            }
            if (Regex.IsMatch(templatePath, "(^tag|/tag)[^\\.]*\\.*html*$", opt))
            {
                return TemplatePageType.Tag;
            }
            if (Regex.IsMatch(templatePath, "(^notfound|/notfound)\\.[^\\.]*\\.*html$", opt))
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
        public string GetPageDescribe(TemplatePageType pageType)
        {
            if (pageDescripts.Keys.Contains(pageType)) return pageDescripts[pageType];
            return null;
        }

        /// <summary>
        /// 是否是系统页面
        /// </summary>
        /// <param name="templateFileName"></param>
        /// <returns></returns>
        public bool IsSystemTemplate(string templateFileName)
        {
            return Array.Exists(systemTemplatePages, a => string.Compare(a, templateFileName, true) == 0);
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

        /// <summary>
        /// 
        /// </summary>
        public void Reload()
        {
            registry.Reload();
        }

        /// <summary>
        /// 清理页面缓存
        /// </summary>
        public void CleanPageCache()
        {
            Cms.Cache.RemoveKeys("site:page");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public TemplatePage GetTemplate(string pageName)
        {
            return registry.GetPage(pageName);
        }
    }

    internal class TemplateDataAdapter : IDataAdapter
    {
        private readonly ICompatibleHttpContext _context;
        private readonly IMemoryCacheWrapper _cache;

        public TemplateDataAdapter(ICompatibleHttpContext context, IMemoryCacheWrapper cache)
        {
            this._context = context;
            this._cache = cache;
        }

        public object GetItem(string key)
        {
            return _context.TryGetItem<object>(key, out var v) ? v : null;
        }

        public void SetItem(string key, object value)
        {
            _context.SaveItem(key, value);
        }

        public object GetCache(string key)
        {
            return _cache.Get(key);
        }

        public void InsertCache(string key, object value, int duration, string dependFileName)
        {
            _cache.Set(key, value, TimeSpan.Zero, new TimeSpan(0, 0, duration));
        }

        public string GetQueryParam(string varKey)
        {
            return this._context.Request.Query(varKey);
        }

        public string GetFormParam(string varKey)
        {
            return _context.Request.Form(varKey);
        }
    }
}