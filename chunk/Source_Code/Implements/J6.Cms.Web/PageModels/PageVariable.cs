
using J6.Cms.Cache.CacheCompoment;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Core;
using J6.Cms.Domain.Interface.Enum;
using J6.Cms.Infrastructure;

namespace J6.Cms.Web
{
    using J6.DevFw.PluginKernel;
    using J6.Cms.DataTransfer;
    using J6.Cms.Template;
    using J6.DevFw.Template;
    using System;

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
        private string _built;
        private static string _currentBuilt;

        static PageVariable()
        {
            //http://www.cnblogs.com/dayezi/p/4702038.html
            //<!-- HTML5 shiv and Respond.js for IE8 support of HTML5 elements and media queries -->
            //<!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
            IeHtml5ShivTag = @"
    <!--[if lt IE 9]>
        <script src=""//cdn.bootcss.com/html5shiv/3.7.2/html5shiv.min.js""></script>
        <script src=""//cdn.bootcss.com/respond.js/1.4.2/respond.min.js""></script>
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
                return Cms.Version;
            }
        }

        [TemplateVariableField("生成标签")]
        public string Built
        {
            get
            {
                if (String.IsNullOrEmpty(this._built))
                {
                    this._built = _currentBuilt ?? Cms.BuiltTime.ToString().Substring(8);
                }
                return this._built;
            }
        }

        public static void ResetBuilt()
        {
            String built = DateHelper.ToUnix(DateTime.Now).ToString().Substring(8);
            PageVariable._currentBuilt = built;
        }

        [TemplateVariableField("当前地址")]
        public string Url
        {
            get { return _context.Request.RawUrl; }
        }

        public string Spam
        {
            get { return this._spam; }
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
        public string Fpath { get { return this.FrameworkPath; } }

        [TemplateVariableField("模板根路径")]
        public string Tpath { get { return this.TemplatePath; } }

        [TemplateVariableField("资源域名路径")]
        public string Rdomain { get { return this.ResDomain; } }

        [TemplateVariableField("插件根路径")]
        public string Ppath { get { return this.PluginPath; } }


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
                                                          TemplateSetting tSetting = null;

                                                          //缓存=》模板设置
                                                          string settingCacheKey = String.Format("{0}_{1}_settings", CacheSign.Template.ToString(), tpl);
                                                          object settings = Cms.Cache.Get(settingCacheKey);
                                                          if (settings == null)
                                                          {
                                                              tSetting = new TemplateSetting(Cms.Context.CurrentSite.Tpl);
                                                              Cms.Cache.Insert(settingCacheKey, tSetting, String.Format("{0}templates/{1}/tpl.conf", Cms.PyhicPath, tpl));
                                                          }
                                                          else
                                                          {
                                                              tSetting = settings as TemplateSetting;
                                                          }

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
