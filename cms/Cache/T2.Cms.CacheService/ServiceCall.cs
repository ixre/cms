using T2.Cms.ServiceUtil;

namespace T2.Cms.CacheService
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCall
    {
        private static ICmsServiceProvider _cmsServiceProvider;

        public static ICmsServiceProvider Instance
        {
            get
            {
                return _cmsServiceProvider ?? (_cmsServiceProvider = ServiceFactory.GetService(ServiceCallMethod.Redirect));
            }
        }
    }
}
