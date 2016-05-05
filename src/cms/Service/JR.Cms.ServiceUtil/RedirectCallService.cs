using JR.Cms.Service;
using JR.Cms.ServiceContract;
using StructureMap;

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
                return ObjectFactory.GetInstance<ISiteServiceContract>();
            }
        }

        public IArchiveServiceContract ArchiveService
        {
            get
            {
                return ObjectFactory.GetInstance<IArchiveServiceContract>();
            }
        }

        public IContentServiceContract ContentService
        {
            get {
                return ObjectFactory.GetInstance<IContentServiceContract>();
            }
        }


        public IUserServiceContract UserService
        {
            get
            {
                return ObjectFactory.GetInstance<IUserServiceContract>();
            }
        }
    }
}
