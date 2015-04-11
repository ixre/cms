
namespace AtNet.Cms.Domain.Interface.Content
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContentRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        IContentContainer GetContent(int siteId);
    }
}
