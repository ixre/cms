using T2.Cms.Infrastructure.Ioc;
using T2.Cms.Service;
using T2.Cms.ServiceContract;

namespace T2.Cms.ServiceUtil
{
    internal class RedirectCallService:ICmsServiceProvider
    {
        public RedirectCallService()
        {
            ServiceInit.Initialize();
        }

        public ISiteServiceContract SiteService
        {
            get {
                return Ioc.GetInstance<ISiteServiceContract>();
            }
        }

        public IArchiveServiceContract ArchiveService
        {
            get
            {
                return Ioc.GetInstance<IArchiveServiceContract>();
            }
        }

        public IContentServiceContract ContentService
        {
            get {
                return Ioc.GetInstance<IContentServiceContract>();
            }
        }


        public IUserServiceContract UserService
        {
            get
            {
                return Ioc.GetInstance<IUserServiceContract>();
            }
        }
    }
}
