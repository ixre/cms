using System;
using JR.DevFw.PluginKernel;
using JR.DevFw.Template;
using JR.DevFw.Web.Cache.Compoment;

namespace JR.DevFw.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplatePageVariable
	{
		private string _domain;
		private string _frameworkPath;
		private string _templatePath;
		private string _pluginRootPath;
		private string _sitemap;
        private string _resDomain;
		
		
		[TemplateVariableField("版本")]
		public string Version
		{
			get { return FwCtx.Version.GetVersion(); }
		}
		
		/// <summary>
		/// 域名
		/// </summary>
		[TemplateVariableField("站点域名")]
		public string Domain
		{
			get
			{
				if (_domain == null)
				{
				    _domain = WebCtx.Current.Domain;
				}
				return _domain;
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
                    _frameworkPath = String.Format("{0}{1}", this.Domain,
                        FwCtx.Variables.AssetsPath.Substring(0, FwCtx.Variables.AssetsPath.Length - 1)
                        );
                }

				return _frameworkPath;
			}
		}

//		[TemplateVariableField("模板根路径")]
//		private string TemplatePath
//		{
//			get
//			{
//				if (_templatePath == null)
//				{
//					string cacheKey = String.Format("{0}_s{1}_template_path", 
//                        CacheSign.Site.ToString(),
//                        site.SiteId.ToString()
//                        );
//
//					_templatePath= Ops.Cms.Cache.GetCachedResult(cacheKey, () =>
//					{
//					    return String.Format(
//                            "{0}/templates/{1}",
//                            this.ResDomain,
//                            site.Tpl
//                            );
//					});
//					
//				}
//
//				return _templatePath;
//			}
//		}
//		


		[TemplateVariableField("插件根路径")]
		public string PluginPath 
		{
			get 
			{
                if (_pluginRootPath == null)
                {
                    _pluginRootPath = String.Format("{0}/{1}",
                     this.Domain,
                     PluginConfig.PLUGIN_DIRECTORY);

                    _pluginRootPath = _pluginRootPath.Remove(_pluginRootPath.Length - 1);
                }
                return _pluginRootPath; 
			} 
		}

		[TemplateVariableField("框架资源根路径")]
		public string Fpath { get { return this.FrameworkPath; } }

//		[TemplateVariableField("模板根路径")]
//		public string Tpath { get { return this.TemplatePath; } }
//
//        [TemplateVariableField("资源域名路径")]
//        public string Rdomain { get { return this.ResDomain; } }
		
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
		/// 页码
		/// </summary>
		[TemplateVariableField("当前页面（带分页页面)的页码")]
		public int PageIndex { get; set; }
	}
}
