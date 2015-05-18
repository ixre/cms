using System.Web;
using AtNet.Cms.Core;

namespace AtNet.Cms.PluginExample
{
    public class PluginMethods
    {
        public static void PortalRequest(CmsContext site, ref bool result)
        {
            HttpContext.Current.Response.Write("cross plugin visite site:" + site.CurrentSite.Name);
            result = true;
        }
    }
}