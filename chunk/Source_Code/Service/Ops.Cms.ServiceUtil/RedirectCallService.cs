using Ops.Cms.Service;
using Ops.Cms.ServiceContract;
using StructureMap;

namespace Ops.Cms.ServiceUtil
{
    internal class RedirectCallService:ICmsServiceProvider
    {
        static RedirectCallService()
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
    }
}
