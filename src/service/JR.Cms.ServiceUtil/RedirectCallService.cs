using JR.Cms.Infrastructure.Ioc;
using JR.Cms.Service;
using JR.Cms.ServiceContract;

namespace JR.Cms.ServiceUtil
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
