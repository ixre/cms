

namespace JR.Cms.Web
{
    using JR.DevFw.PluginKernel;
    using JR.Cms.DataTransfer;
    using JR.DevFw.Template;
    using System;
    using JR.Cms.Cache.CacheCompoment;
    using JR.Cms.CacheService;
    using JR.Cms.Conf;
    using JR.Cms.Core;
    using JR.Cms.Domain.Interface.Enum;
    using JR.Cms.Infrastructure;

    public class PageVariable
    {
        private string _domain;
        private string _frameworkPath;
        private string _templatePath;
        private string _pluginRootPath;
        private string _sitemap;
        private readonly CmsContext _context;
        private string _resDomain;
        static readonly String IeHtml5ShivTag;
        private string _spam;
        private static string _currentBuilt;
        private string _lang;
        private string _resPath;

        static PageVariable()
        {
            //http://www.cnblogs.com/dayezi/p/4702038.html
            //<!-- HTML5 shiv and Respond.js for IE8 support of HTML5 elements and media queries -->
            //<!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
            IeHtml5ShivTag = @"
    <!--[if lt IE 9]>
        <script src=""https://cdn.bootcss.com/html5shiv/r29/html5.min.js""></script>
        <script src=""https://cdn.bootcss.com/respond.js/1.4.2/respond.min.js""></script>
    <![endif]-->";
        }

        public PageVariable()
        {
            _context = Cms.Context;
        }


        [TemplateVariableField("版本")]
        public string Version
        {
            get
            {
                int i = CmsVariables.VERSION.LastIndexOf(".");
                return CmsVariables.PROD + " " + CmsVariables.VERSION.Substring(0, i);
            }
        }

        [TemplateVariableField("生成标签")]
        public string Built
        {
            get { return this.Spam; }
        }

        public static void ResetBuilt()
        {
            String built = DateHelper.ToUnix(DateTime.Now).ToString().Substring(8);
            PageVariable._currentBuilt = built;
        }

        public string Lang
        {
            get { return this._lang ?? (this._lang = Cms.Context.UserLanguage.ToString()); }
        }

        [TemplateVariableField("当前地址")]
        public string Url
        {
            get { return _context.Request.RawUrl; }
        }

        public string Spam
        {
            get
            {
                if (String.IsNullOrEmpty(this._spam))
                {
                    this._spam = _currentBuilt ?? Cms.BuiltTime.ToString().Substring(8);
                }
                return this._spam;
            }
        }

        /// <summary>
        /// 域名
        /// </summary>
        [TemplateVariableField("站点域名")]
        public string Domain
        {
            get { return this._domain ?? (this._domain = _context.SiteDomain); }
        }

        [TemplateVariableField("资源域名")]
        public string ResDomain
        {
            get { return this._resDomain ?? (this._resDomain = _context.ResourceDomain); }
        }


        [TemplateVariableField("静态服务器域名")]
        public string StaticDomain
        {
            get
            {
                return _context.StaticDomain;
            }
        }

        /// <summary>
        /// 框架路径
        /// </summary>
        [TemplateVariableField("框架资源根路径")]
        private string FrameworkPath
        {
            get
            {
                if (_frameworkPath == null)
                {
                    _frameworkPath = String.Format("{0}/{1}", this.StaticDomain,
                        CmsVariables.FRAMEWORK_ASSETS_PATH
                        .Substring(0, CmsVariables.FRAMEWORK_ASSETS_PATH.Length - 1)
                        );
                }

                return _frameworkPath;
            }
        }

        [TemplateVariableField("模板根路径")]
        private string TemplatePath
        {
            get
            {
                if (_templatePath == null)
                {
                    SiteDto site = _context.CurrentSite;

                    _templatePath = String.Format(
                        "{0}/templates/{1}",
                        this.ResDomain,
                        site.Tpl
                        );
                }
                return _templatePath;
            }
        }

        [TemplateVariableField("插件根路径")]
        public string PluginPath
        {
            get
            {
                if (_pluginRootPath == null)
                {
                    _pluginRootPath = String.Format("{0}/{1}",
                     this.ResDomain,
                     PluginConfig.PLUGIN_DIRECTORY);

                    _pluginRootPath = _pluginRootPath.Remove(_pluginRootPath.Length - 1);
                }
                return _pluginRootPath;
            }
        }

        [TemplateVariableField("框架资源根路径")]
        public string FPath { get { return this.FrameworkPath; } }

        [TemplateVariableField("模板根路径")]
        public string TPath { get { return this.TemplatePath; } }

        [TemplateVariableField("插件根路径")]
        public string PPath { get { return this.PluginPath; } }

        [TemplateVariableField("资源根路径")]
        public string RPath
        {
            get
            {
                return this.getResPath();
            }
        }

        private string getResPath()
        {
            if (_resPath == null)
            {
                _resPath = this.ResDomain +"/"+ CmsVariables.RESOURCE_PATH.Substring(0,CmsVariables.RESOURCE_PATH.Length-1);
            }
            return _resPath;
        }

        [TemplateVariableField("资源域名路径")]
        public string RDomain { get { return this.ResDomain; } }



        /// <summary>
        ///标题
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
                if (_sitemap == null)
                {

                    string tag = Cms.Context.Items["category.tag"] as string;
                    string tpl = Cms.Context.CurrentSite.Tpl;
                    int siteId = Cms.Context.CurrentSite.SiteId;
                    if (string.IsNullOrEmpty(tag))
                    {
                        string cacheKey = String.Format("{0}_site{1}_sitemap_{2}", CacheSign.Category, siteId.ToString(), tag);
                        _sitemap = Cms.Cache.GetCachedResult(cacheKey, () =>
                        {
                            TemplateSetting tSetting = Cms.TemplateManager.Get(tpl);
                            string urlFormat = (Settings.TPL_UseFullPath ? Cms.Context.SiteDomain : Cms.Context.SiteAppPath)
                                + TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)UrlRulePageKeys.Category];
                            return CategoryCacheManager.GetSitemapHtml(siteId, tag, tSetting.CFG_SitemapSplit, urlFormat);
                        }, DateTime.Now.AddHours(Settings.OptiDefaultCacheHours));
                    }
                }
                return _sitemap;
            }
        }

        /// <summary>
        /// 页码
        /// </summary>
        [TemplateVariableField("当前页面（带分页页面)的页码")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        [TemplateVariableField("支持HTML5")]
        public String Html5
        {
            get
            {
                return IeHtml5ShivTag;
            }
        }
    }
}
