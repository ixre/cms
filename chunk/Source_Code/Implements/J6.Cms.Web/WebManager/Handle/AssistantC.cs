
using System;
using System.Collections;
using System.Collections.Generic;
using J6.Cms.CacheService;
using J6.Cms.DataTransfer;
using J6.Cms.WebManager;

namespace J6.Cms.Web.WebManager.Handle
{
    internal  class AssistantC:BasePage
    {
        public string Category_Clone_GET()
        {
            int fromSiteId;
            int.TryParse(Request["target_site"], out fromSiteId);

            String siteOpt;
            SiteDto targetSite = default(SiteDto);
            
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var e in SiteCacheManager.GetAllSites())
            {
                if (e.SiteId != fromSiteId)
                {
                    dict.Add(e.SiteId.ToString(), e.Name);
                }
                else
                {
                    targetSite = e;
                }
            }
            siteOpt = Helper.GetOptions(dict, null);

            ViewData["site_opt"] = siteOpt;
            ViewData["target_name"] = targetSite.Name;
            ViewData["target_site"] = targetSite.SiteId;

            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Assistant_Category_Clone));
        }
    }
}
