using AtNet.Cms.Domain.Interface.Common;
using AtNet.Cms.Domain.Interface.Content;
using AtNet.Cms.Domain.Interface.Content.Archive;
using AtNet.Cms.Domain.Interface.Site;
using AtNet.Cms.Domain.Interface.Site.Category;
using AtNet.Cms.Domain.Interface.Site.Extend;
using AtNet.Cms.Domain.Interface.Site.Template;
using AtNet.Cms.ServiceContract;
using AtNet.Cms.ServiceRepository;
/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using StructureMap;

namespace AtNet.Cms.Service
{
	/// <summary>
	/// 服务初始化
	/// </summary>
	public static class ServiceInit
	{
        /// <summary>
        /// 
        /// </summary>
		public static void Initialize()
        {

            new ExtendFieldRepository();

            //设置依赖反转
            ObjectFactory.Configure(act =>
            {
                act.For<ISiteServiceContract>().Singleton().Use<SiteService>();
                act.For<IArchiveServiceContract>().Singleton().Use<ArchiveService>();
                act.For<IContentServiceContract>().Singleton().Use<ContentService>();

                act.For<ISiteRepository>().Singleton().Use<SiteRepository>();
                act.For<IContentRepository>().Singleton().Use<ContentRepository>();
                act.For<ICategoryRepository>().Singleton().Use<CategoryRepository>();
                act.For<IExtendFieldRepository>().Singleton().Use<ExtendFieldRepository>();
                act.For<IArchiveRepository>().Singleton().Use<ArchiveRepository>();
                act.For<ITemplateRepository>().Singleton().Use<TemplateRepository>();
                act.For<ILinkRepository>().Singleton().Use<LinkRepository>();
            });
        }
	}
}