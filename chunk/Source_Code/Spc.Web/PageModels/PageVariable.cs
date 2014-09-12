
using Ops.Cms.Cache.CacheCompoment;
using Ops.Cms.Conf;
using Ops.Cms.Domain.Interface.Enum;

namespace Ops.Cms.Web
{
    using Com.PluginKernel;
    using Ops.Cms;
    using Ops.Cms.Cache;
    using Ops.Cms.CacheService;
    using Ops.Cms.DataTransfer;
    using Ops.Cms.Domain;
    using Ops.Cms.Template;
    using Ops.Template;
    using System;

	public class PageVariable
	{
		private string domain;
		private string frameworkPath;
		private string templatePath;
		private string pluginRootPath;
		private string sitemap;
		private CmsContext context;

		
		public PageVariable()
		{
			context=Cms.Context;
		}
		
		
		[TemplateVariableField("版本")]
		public string Version
		{
			get
			{
				return Cms.Version;
			}
		}
		
		/// <summary>
		/// 域名
		/// </summary>
		[TemplateVariableField("站点域名")]
		public string Domain
		{
			get
			{
				if (domain == null)
				{
					string siteDomain = Cms.Context.SiteDomain;
					domain = siteDomain.Remove(siteDomain.Length - 1);
				}
				return domain;
			}
		}

		[TemplateVariableField("资源域名")]
		public string ResDomain
		{
			get{
                return context.ResourceDomain;
			}
		}
		
		
		[TemplateVariableField("静态服务器域名")]
		public string StaticDomain
		{
			get
			{
                return context.StaticDomain;
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
                if (frameworkPath == null)
                {
                    frameworkPath = String.Format("{0}{1}", this.StaticDomain,
                        CmsVariables.FRAMEWORK_ASSETS_PATH
                        .Substring(0, CmsVariables.FRAMEWORK_ASSETS_PATH.Length - 1)
                        );
                }

				return frameworkPath;
			}
		}

		[TemplateVariableField("模板根路径")]
		private string TemplatePath
		{
			get
			{
				if (templatePath == null)
				{
					CmsContext context=Cms.Context;
					SiteDto site = context.CurrentSite;

					string cacheKey = String.Format("{0}_s{1}_template_path", 
                        CacheSign.Site.ToString(),
                        site.SiteId.ToString()
                        );

					templatePath= Cms.Cache.GetCachedResult(cacheKey, () =>
					{
					    return String.Format(
                            "{0}templates/{1}",
                            this.ResDomain==String.Empty?"/":this.ResDomain,
                            site.Tpl
                            );
					});
					
				}

				return templatePath;
			}
		}
		
		[TemplateVariableField("插件根路径")]
		public string PluginPath 
		{
			get 
			{
                if (pluginRootPath == null)
                {
                    pluginRootPath = String.Format("{0}{1}",
                     this.ResDomain,
                     PluginConfig.PLUGIN_DIRECTORY);

                    pluginRootPath = pluginRootPath.Remove(pluginRootPath.Length - 1);
                }
                return pluginRootPath; 
			} 
		}

		[TemplateVariableField("框架资源根路径")]
		public string Fpath { get { return this.FrameworkPath; } }

		[TemplateVariableField("模板根路径")]
		public string Tpath { get { return this.TemplatePath; } }
		
		
		
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
				if (sitemap == null)
				{

					string tag = Cms.Context.Items["category.tag"] as string;
					string tpl = Cms.Context.CurrentSite.Tpl;
					int siteID = Cms.Context.CurrentSite.SiteId;
					if (string.IsNullOrEmpty(tag))
					{
						string cacheKey = String.Format("{0}_site{1}_sitemap_{2}", CacheSign.Category,siteID.ToString(),tag);
						sitemap = Cms.Cache.GetCachedResult(cacheKey, () =>
						                              {
						                              	TemplateSetting tsetting = null;

						                              	//缓存=》模板设置
						                              	string settingCacheKey = String.Format("{0}_{1}_settings", CacheSign.Template.ToString(), tpl);
						                              	object settings = Cms.Cache.Get(settingCacheKey);
						                              	if (settings == null)
						                              	{
						                              		tsetting = new TemplateSetting(Cms.Context.CurrentSite.Tpl);
						                              		Cms.Cache.Insert(settingCacheKey, tsetting, String.Format("{0}templates/{1}/tpl.conf", Cms.PyhicPath, tpl));
						                              	}
						                              	else
						                              	{
						                              		tsetting = settings as  TemplateSetting;
						                              	}

						                              	string urlFormat = (Settings.TPL_UseFullPath ? Cms.Context.SiteDomain : Cms.Context.SiteAppPath)
						                              		+ TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)UrlRulePageKeys.Category];


                                                        return CategoryCacheManager.GetSitemapHtml(siteID, tag, tsetting.CFG_SitemapSplit, urlFormat);

						                              });
					}
				}
				return sitemap;
			}
		}

		/// <summary>
		/// 页码
		/// </summary>
		[TemplateVariableField("当前页面（带分页页面)的页码")]
		public int PageIndex { get; set; }
	}
}
