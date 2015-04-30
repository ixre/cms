
using AtNet.Cms;
using AtNet.Cms.Cache.CacheCompoment;
using AtNet.Cms.CacheService;
using AtNet.Cms.Conf;
using AtNet.Cms.Core;
using AtNet.Cms.Domain.Interface.Enum;

namespace AtNet.Cms.Web
{
    using AtNet.DevFw.PluginKernel;
    using AtNet.Cms.DataTransfer;
    using AtNet.Cms.Template;
    using AtNet.DevFw.Template;
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

		
		public PageVariable()
		{
			_context=AtNet.Cms.Cms.Context;
		}
		
		
		[TemplateVariableField("版本")]
		public string Version
		{
			get
			{
				return AtNet.Cms.Cms.Version;
			}
		}

        [TemplateVariableField("当前地址")]
	    public string Url
	    {
	        get { return _context.Request.RawUrl; }
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

					string cacheKey = String.Format("{0}_s{1}_template_path", 
                        CacheSign.Site.ToString(),
                        site.SiteId.ToString()
                        );

					_templatePath= AtNet.Cms.Cms.Cache.GetCachedResult(cacheKey, () =>
					{
					    return String.Format(
                            "{0}/templates/{1}",
                            this.ResDomain,
                            site.Tpl
                            );
					});
					
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

					string tag = AtNet.Cms.Cms.Context.Items["category.tag"] as string;
					string tpl = AtNet.Cms.Cms.Context.CurrentSite.Tpl;
					int siteID = AtNet.Cms.Cms.Context.CurrentSite.SiteId;
					if (string.IsNullOrEmpty(tag))
					{
						string cacheKey = String.Format("{0}_site{1}_sitemap_{2}", CacheSign.Category,siteID.ToString(),tag);
						_sitemap = AtNet.Cms.Cms.Cache.GetCachedResult(cacheKey, () =>
						                              {
						                              	TemplateSetting tsetting = null;

						                              	//缓存=》模板设置
						                              	string settingCacheKey = String.Format("{0}_{1}_settings", CacheSign.Template.ToString(), tpl);
						                              	object settings = AtNet.Cms.Cms.Cache.Get(settingCacheKey);
						                              	if (settings == null)
						                              	{
						                              		tsetting = new TemplateSetting(AtNet.Cms.Cms.Context.CurrentSite.Tpl);
						                              		AtNet.Cms.Cms.Cache.Insert(settingCacheKey, tsetting, String.Format("{0}templates/{1}/tpl.conf", AtNet.Cms.Cms.PyhicPath, tpl));
						                              	}
						                              	else
						                              	{
						                              		tsetting = settings as  TemplateSetting;
						                              	}

						                              	string urlFormat = (Settings.TPL_UseFullPath ? AtNet.Cms.Cms.Context.SiteDomain : AtNet.Cms.Cms.Context.SiteAppPath)
						                              		+ TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)UrlRulePageKeys.Category];


                                                        return CategoryCacheManager.GetSitemapHtml(siteID, tag, tsetting.CFG_SitemapSplit, urlFormat);

						                              });
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
	}
}
