using System.Collections.Generic;
using JR.Cms.ServiceDto;

namespace JR.Cms.Library.CacheService
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtendFieldCacheManager
    {
        public static IList<ExtendFieldDto> GetExtendFields(int siteId)
        {
            IList<ExtendFieldDto> extends =
                new List<ExtendFieldDto>(ServiceCall.Instance.SiteService.GetExtendFields(siteId));
            return extends;
        }
    }
}