
namespace JR.Cms.CacheService
{
    /// <summary>
    /// 
    /// </summary>
    public static class CategoryCacheManager
    {
        /// <summary>
        /// 获取站点面包屑
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <param name="split"></param>
        /// <param name="linkFormat"></param>
        /// <returns></returns>
        public static string GetSitemapHtml(int siteId, string catPath, string split,string linkFormat)
        {
            return ServiceCall.Instance.SiteService.GetCategorySitemapHtml(siteId, catPath, split, linkFormat);
        }
    }
}
