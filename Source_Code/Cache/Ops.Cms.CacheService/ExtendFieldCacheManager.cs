using System.Collections.Generic;
using AtNet.Cms.DataTransfer;

namespace AtNet.Cms.CacheService
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtendFieldCacheManager
    {
        public static IList<ExtendFieldDto> GetExtendFields(int siteId)
        {
            IList<ExtendFieldDto> extends = new List<ExtendFieldDto>(ServiceCall.Instance.SiteService.GetExtendFields(siteId));
            return extends;
        }
    }
}
