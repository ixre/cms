using System.Web;
using J6.Cms.Core;

namespace J6.Cms.PluginExample
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