using System.Net;
using System.Web;
using JR.Stand.Core.Web;

namespace JR.Stand.Core.Framework.Web.Utils
{
    public static class HttpUtil
    {
        public static string UrlEncode(string url)
        {
#if NETSTANDARD
            return WebUtility.UrlEncode(url);
#endif
            return HttpHosting.Context.Request.UrlEncode(url);
        }

        public static string UrlDecode(string url)
        {
#if NETSTANDARD
            return WebUtility.UrlDecode(url);
#endif
            return HttpHosting.Context.Request.UrlDecode(url);
        }
    }
}