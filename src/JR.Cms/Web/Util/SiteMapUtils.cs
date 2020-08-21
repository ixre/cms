using System;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Web.SiteMap;
using JR.Stand.Abstracts.Safety;

namespace JR.Cms.Web.Util
{
    /// <summary>
    /// 站点地图工具类
    /// </summary>
    public static class SiteMapUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task Generate()
        {
            // 如果没有设置站点地图URL, 手动更新站点地图时,会自动获取并存储到cms.config
            if (String.IsNullOrEmpty(Settings.SYS_SITE_MAP_PATH))
            {
                return SafetyTask.CompletedTask;
            }
            return new SiteMapper(Settings.SYS_SITE_MAP_PATH, "/")
                .GenerateSiteMap(Cms.PhysicPath + "sitemap.xml");
        }
    }
}