using AtNet.Cms.ServiceContract;

namespace AtNet.Cms.ServiceUtil
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICmsServiceProvider
    {
        /// <summary>
        /// 
        /// </summary>
        ISiteServiceContract SiteService { get; }

        /// <summary>
        /// 
        /// </summary>
        IContentServiceContract ContentService { get; }

        /// <summary>
        /// 
        /// </summary>
        IArchiveServiceContract ArchiveService { get; }
    }
}
