using System;
using T2.Cms.BLL;
using T2.Cms.DB;
using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Domain.Interface.User;
using T2.Cms.Domain.Interface._old;
using T2.Cms.Infrastructure.Ioc;
using T2.Cms.ServiceContract;
using T2.Cms.ServiceRepository;
using T2.Cms.ServiceRepository.Export;

/*
 * Created by SharpDevelop.
 * UserBll: newmin
 * Date: 2014/2/22
 * Time: 13:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace T2.Cms.Service
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
            Ioc.Configure(_ =>
            {
                _.For<ISiteServiceContract>().Singleton().Use<SiteService>();
                _.For<IArchiveServiceContract>().Singleton().Use<ArchiveService>();
                _.For<IContentServiceContract>().Singleton().Use<ContentService>();
                _.For<IUserServiceContract>().Singleton().Use<UserService>();

                _.For<ISiteRepo>().Singleton().Use<SiteRepository>();
                _.For<IContentRepository>().Singleton().Use<ContentRepository>();
                _.For<ICategoryRepo>().Singleton().Use<CategoryRepository>();
                _.For<IExtendFieldRepository>().Singleton().Use<ExtendFieldRepository>();
                _.For<IArchiveRepository>().Singleton().Use<ArchiveRepository>();
                _.For<ITemplateRepository>().Singleton().Use<TemplateRepository>();

                _.For<IUserRepository>().Singleton().Use<UserRepository>();

                //x.For<IArchiveModel>().Singleton().Use<ArchiveBLL>();
                // x.For<ICategoryModel>().Singleton().Use<CategoryBLL>();
                _.For<IComment>().Singleton().Use<CommentBll>();
                // x.For<ILink>().Singleton().Use<LinkBLL>();
                _.For<Imember>().Singleton().Use<MemberBll>();
                _.For<Imessage>().Singleton().Use<MessageBll>();
                _.For<Imodule>().Singleton().Use<ModuleBLL>();
                //x.For<ISite>().Singleton().Use<SiteBLL>();
                // x.For<ITemplateBind>().Singleton().Use<TemplateBindBLL>();
                _.For<IUserBll>().Singleton().Use<UserBllBll>();
                _.For<ITable>().Singleton().Use<TableBll>();

            });
            ExportManager.Initialize(AppDomain.CurrentDomain.BaseDirectory + "public/query/", CmsDataBase.Instance);

        }
    }
}