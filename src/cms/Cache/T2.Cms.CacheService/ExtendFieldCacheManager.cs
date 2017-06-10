using System.Collections.Generic;
using T2.Cms.DataTransfer;

namespace T2.Cms.CacheService
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
