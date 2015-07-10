using J6.Cms.Domain.Interface.Common;
using J6.Cms.Domain.Interface.Content;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Site;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.Cms.Domain.Interface.Site.Template;
using J6.Cms.Domain.Interface.User;
using J6.Cms.ServiceContract;
using J6.Cms.ServiceRepository;
/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using StructureMap;

namespace J6.Cms.Service
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
                act.For<IUserServiceContract>().Singleton().Use<UserService>();

                act.For<ISiteRepository>().Singleton().Use<SiteRepository>();
                act.For<IContentRepository>().Singleton().Use<ContentRepository>();
                act.For<ICategoryRepository>().Singleton().Use<CategoryRepository>();
                act.For<IExtendFieldRepository>().Singleton().Use<ExtendFieldRepository>();
                act.For<IArchiveRepository>().Singleton().Use<ArchiveRepository>();
                act.For<ITemplateRepository>().Singleton().Use<TemplateRepository>();
                act.For<ILinkRepository>().Singleton().Use<LinkRepository>();

                act.For<IUserRepository>().Singleton().Use<UserRepository>();

            });
        }
	}
}