using JR.Cms.ServiceUtil;

namespace JR.Cms.Library.CacheService
{
    /// <summary>
    /// 
    /// </summary>
    public static class LocalService
    {
        private static ICmsServiceProvider _cmsServiceProvider;

        /// <summary>
        /// 服务实例
        /// </summary>
        public static ICmsServiceProvider Instance => _cmsServiceProvider ??
                                                      (_cmsServiceProvider =
                                                          ServiceFactory.GetService(ServiceCallMethod.Local));
    }
}