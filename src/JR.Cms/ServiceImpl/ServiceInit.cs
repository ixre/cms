using JR.Cms.Dao;
using JR.Cms.Dao.Impl;
using JR.Cms.Domain.Interface._old;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Infrastructure.Ioc;
using JR.Cms.Library.DataAccess.BLL;
using JR.Cms.Repository;
using JR.Cms.ServiceContract;
using JR.Stand.Core.Data.Provider;

/*
 * Created by SharpDevelop.
 * UserBll: newmin
 * Date: 2014/2/22
 * Time: 13:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace JR.Cms.ServiceImpl
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
                _.For<ISiteServiceContract>().Singleton().Use<SiteServiceImpl>();
                _.For<IArchiveServiceContract>().Singleton().Use<ArchiveService>();
                _.For<IContentServiceContract>().Singleton().Use<ContentService>();
                _.For<IUserServiceContract>().Singleton().Use<UserService>();

                _.For<ISiteRepo>().Singleton().Use<SiteRepository>();
                _.For<IContentRepository>().Singleton().Use<ContentRepository>();
                _.For<ICategoryRepo>().Singleton().Use<CategoryRepository>();
                _.For<IExtendFieldRepository>().Singleton().Use<ExtendFieldRepository>();
                _.For<IArchiveRepository>().Singleton().Use<ArchiveRepository>();
                _.For<ITemplateRepo>().Singleton().Use<TemplateRepoImpl>();

                _.For<IUserRepository>().Singleton().Use<UserRepository>();

                // Dao
                _.For<IDbProvider>().Singleton().Use<DbProviderImpl>();
                _.For<ISiteTagDao>().Singleton().Use<SiteTagDaoImpl>();
                
                //x.For<IArchiveModel>().Singleton().Use<ArchiveBLL>();
                // x.For<ICategoryModel>().Singleton().Use<CategoryBLL>();
                _.For<IComment>().Singleton().Use<CommentBll>();
                // x.For<ILink>().Singleton().Use<LinkBLL>();
                _.For<Imember>().Singleton().Use<MemberBll>();
                _.For<Imessage>().Singleton().Use<MessageBll>();
                _.For<Imodule>().Singleton().Use<ModuleBLL>();
                //x.For<ISite>().Singleton().Use<SiteBLL>();
                // x.For<TemplateBind>().Singleton().Use<TemplateBindBLL>();
                _.For<IUserBll>().Singleton().Use<UserBllBll>();
                _.For<ITable>().Singleton().Use<TableBll>();
            });
            //todo: export
            // ExportManager.Initialize(EnvUtil.GetBaseDirectory() + "public/query/", CmsDataBase.Instance);
        }
    }
}