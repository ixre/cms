using AtNet.Cms.ServiceContract;

namespace AtNet.Cms.ServiceUtil
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICmsServiceProvider
    {
        /// <summary>
        /// 站点服务
        /// </summary>
        ISiteServiceContract SiteService { get; }

        /// <summary>
        /// 内容服务 
        /// </summary>
        IContentServiceContract ContentService { get; }

        /// <summary>
        /// 文档服务
        /// </summary>
        IArchiveServiceContract ArchiveService { get; }

        /// <summary>
        /// 用户服务
        /// </summary>
        IUserServiceContract UserService { get; }
    }
}
