using Ops.Cms.DataTransfer;
using System.Collections.Generic;

namespace Ops.Cms.CacheService
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
