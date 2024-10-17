using System;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheProvider.CacheComponent;
using JR.Cms.Library.CacheService;
using JR.Stand.Core.PluginKernel;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Framework.Extensions;
using System.Collections.Generic;

namespace JR.Cms.Web.Portal.Template.Model
{
    /// <summary>
    /// 页面变量模型
    /// </summary>
    public class PageVariableModel : ITemplateVariableInstance
    {
        private string _domain;
        private string _frameworkPath;
        private string _templatePath;
        private string _pluginRootPath;
        private string _siteMap;
        private readonly CmsContext _context;
        private string _resDomain;
        private static readonly string IeHtml5ShivTag;
        private string _buildTagString;
        private string _nonce;
        private static string _currentBuilt;
        private string _lang;
        private string _resPath;

        static PageVariableModel()
        {
            //http://www.cnblogs.com/dayezi/p/4702038.html
            //<!-- HTML5 shiv and Respond.js for IE8 support of HTML5 elements and media queries -->
            //<!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
            IeHtml5ShivTag = @"<!--[if lt IE 9]>"
                             + @"<script src=""https://cdn.bootcss.com/html5shiv/r29/html5.min.js""></script>"
                             + @"<script src=""https://cdn.bootcss.com/respond.js/1.4.2/respond.min.js""></script>"
                             + @"<![endif]-->";
        }

        /// <summary>
        /// 
        /// </summary>
        public PageVariableModel()
        {
            _context = Cms.Context;
        }

        /// <summary>
        /// 年
        /// </summary>
        [TemplateVariableField("年")]
        public string Year => DateTime.Now.Year.ToString();

        /// <summary>
        /// 版本
        /// </summary>
        [TemplateVariableField("版本")]
        public string Version
        {
            get
            {
                var i = CmsVariables.VERSION.LastIndexOf(".", StringComparison.Ordinal);
                return CmsVariables.PROD + "(v" + CmsVariables.VERSION.Substring(0, i) + ")";
            }
        }

        /// <summary>
        /// 生成标签
        /// </summary>
        [TemplateVariableField("生成标签")]
        public string Built => BuildTag;

        /// <summary>
        /// 重置生成标签
        /// </summary>
        public static void ResetBuilt()
        {
            var built = DateHelper.ToUnix(DateTime.Now).ToString().Substring(8);
            _currentBuilt = built;
        }

        /// <summary>
        /// 语言
        /// </summary>
        public string Lang
        {
            get
            {
                if (this._lang == null)
                {
                    this._lang = Cms.Context.UserLanguage.ToString();
                }

                return _lang;
            }
        }

        /// <summary>
        /// 当前地址
        /// </summary>
        [TemplateVariableField("当前地址")]
        public string Url
        {
            get
            {
                var url = _context.HttpContext.Request.GetEncodedUrl();
                var proto = this._context.HttpContext.Request.GetProto();
                if (!url.StartsWith(proto))
                {
                    url = "https" + url.Substring(4);
                }

                return url;
            }
        }

        /// <summary>
        /// 构建标签
        /// </summary>
        [TemplateVariableField("构建标签")]
        public string BuildTag
        {
            get
            {
                if (string.IsNullOrEmpty(_buildTagString)) _buildTagString = _currentBuilt ?? Cms.BuiltTime.ToString().Substring(8);
                return _buildTagString;
            }
        }

        /// <summary>
        /// 随机字符串
        /// </summary>
        [TemplateVariableField("随机字符串")]
        public string Nonce
        {
            get
            {
                if (string.IsNullOrEmpty(_nonce)) _nonce = String.Format("page_noance_{0:yyyy:MM:dd HH:mm:ss}", DateTime.Now).Md5().Substring(8, 24);
                return _nonce;
            }
        }

        /// <summary>
        /// 域名
        /// </summary>
        [TemplateVariableField("站点域名")]
        public string Domain => _domain ?? (_domain = _context.SiteDomain);

        /// <summary>
        /// 资源域名
        /// </summary>
        [TemplateVariableField("资源域名")]
        public string ResDomain => _resDomain ?? (_resDomain = _context.ResourceDomain);

        /// <summary>
        /// 静态服务器域名
        /// </summary>
        [TemplateVariableField("静态服务器域名")]
        public string StaticDomain => _context.StaticDomain;

        /// <summary>
        /// 框架路径
        /// </summary>
        [TemplateVariableField("框架资源根路径")]
        private string FrameworkPath
        {
            get
            {
                if (_frameworkPath == null)
                    _frameworkPath = string.Format("{0}/{1}", StaticDomain,
                        CmsVariables.FRAMEWORK_ASSETS_PATH
                            .Substring(0, CmsVariables.FRAMEWORK_ASSETS_PATH.Length - 1)
                    );

                return _frameworkPath;
            }
        }

        /// <summary>
        /// 模板根路径
        /// </summary>
        [TemplateVariableField("模板根路径")]
        private string TemplatePath
        {
            get
            {
                if (_templatePath == null)
                {
                    var site = _context.CurrentSite;

                    _templatePath = $"{ResDomain}/templates/{site.Tpl}";
                }

                return _templatePath;
            }
        }

        /// <summary>
        /// 插件根路径
        /// </summary>
        [TemplateVariableField("插件根路径")]
        public string PluginPath
        {
            get
            {
                if (_pluginRootPath == null)
                {
                    _pluginRootPath = string.Format("{0}/{1}",
                        ResDomain,
                        PluginConfig.PLUGIN_DIRECTORY);

                    _pluginRootPath = _pluginRootPath.Remove(_pluginRootPath.Length - 1);
                }

                return _pluginRootPath;
            }
        }

        /// <summary>
        /// 框架资源根路径
        /// </summary>
        [TemplateVariableField("框架资源根路径")]
        public string FPath => FrameworkPath;

        /// <summary>
        /// 模板根路径
        /// </summary>
        [TemplateVariableField("模板根路径")]
        public string TPath => TemplatePath;

        /// <summary>
        /// 插件根路径
        /// </summary>
        [TemplateVariableField("插件根路径")]
        public string PPath => PluginPath;

        /// <summary>
        /// 资源根路径
        /// </summary>
        [TemplateVariableField("资源根路径")]
        public string RPath => GetResPath();

        /// <summary>
        /// 获取资源根路径
        /// </summary>
        /// <returns></returns>
        private string GetResPath()
        {
            if (_resPath == null)
                _resPath = ResDomain + "/" +
                           CmsVariables.RESOURCE_PATH.Substring(0, CmsVariables.RESOURCE_PATH.Length - 1);
            return _resPath;
        }

        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void AddData(string key, string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="key"></param>
        public void RemoveData(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 资源域名路径
        /// </summary>
        [TemplateVariableField("资源域名路径")]
        public string RDomain => ResDomain;


        /// <summary>
        /// 标题
        /// </summary>
        [TemplateVariableField("网页(当前页)标题")]
        public string Title { get; set; }

        /// <summary>
        /// 子标题
        /// </summary>
        [TemplateVariableField("子标题")]
        public string SubTitle { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [TemplateVariableField("关键词")]
        public string Keywords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [TemplateVariableField("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 站点地图
        /// </summary>
        [TemplateVariableField("站点地图")]
        public string Sitemap
        {
            get
            {
                if (_siteMap == null)
                {
                    var tag = Cms.Context.Items["category.tag"] as string;
                    var tpl = Cms.Context.CurrentSite.Tpl;
                    var siteId = Cms.Context.CurrentSite.SiteId;
                    if (string.IsNullOrEmpty(tag))
                    {
                        var cacheKey = string.Format("{0}_site{1}_site_map_{2}", CacheSign.Category, siteId.ToString(),
                            tag);
                        _siteMap = Cms.Cache.GetCachedResult(cacheKey, () =>
                        {
                            var tSetting = Cms.TemplateManager.Get(tpl);
                            var urlFormat = (Settings.TPL_FULL_URL_PATH
                                                ? Cms.Context.SiteDomain
                                                : Cms.Context.SiteAppPath)
                                            + TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex,
                                                (int)UrlRulePageKeys.Category];
                            return CategoryCacheManager.GetSitemapHtml(siteId, tag, tSetting.CFG_SitemapSplit,
                                urlFormat);
                        }, DateTime.Now.AddHours(Settings.OptiDefaultCacheHours));
                    }
                }

                return _siteMap;
            }
        }

        /// <summary>
        /// 页码
        /// </summary>
        [TemplateVariableField("当前页面（带分页页面)的页码")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 支持HTML5
        /// </summary>
        [TemplateVariableField("支持HTML5")]
        public string Html5 => IeHtml5ShivTag;

        /// <summary>
        /// 数据
        /// </summary>
        public IDictionary<string, string> Data => throw new NotImplementedException();
    }
}