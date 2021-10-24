using JR.Cms.ServiceUtil;

namespace JR.Cms.Library.CacheService
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCall
    {
        private static ICmsServiceProvider _cmsServiceProvider;

        public static ICmsServiceProvider Instance => _cmsServiceProvider ??
                                                      (_cmsServiceProvider =
                                                          ServiceFactory.GetService(ServiceCallMethod.Local));
    }
}