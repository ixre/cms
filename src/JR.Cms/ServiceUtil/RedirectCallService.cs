using JR.Cms.Infrastructure.Ioc;
using JR.Cms.ServiceContract;
using JR.Cms.ServiceImpl;

namespace JR.Cms.ServiceUtil
{
    internal class RedirectCallService : ICmsServiceProvider
    {
        public RedirectCallService()
        {
            ServiceInit.Initialize();
        }

        public ISiteServiceContract SiteService => Ioc.GetInstance<ISiteServiceContract>();

        public IArchiveServiceContract ArchiveService => Ioc.GetInstance<IArchiveServiceContract>();

        public IContentServiceContract ContentService => Ioc.GetInstance<IContentServiceContract>();


        public IUserServiceContract UserService => Ioc.GetInstance<IUserServiceContract>();
    }
}