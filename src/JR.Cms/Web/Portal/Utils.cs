using System.Text;
using JR.Cms.Conf;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Web;

namespace JR.Cms.Web.Portal
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 获取自动定向的地址
        /// </summary>
        /// <param name="request"></param>
        /// <param name="siteDto"></param>
        /// <returns></returns>
        public static string GetSiteRedirectUrl(ICompatibleRequest request, SiteDto siteDto)
        {
            if (siteDto.SeoForceHttps == 0 && siteDto.SeoForceRedirect == 0) return null;
            var target = request.GetEncodedUrl();
            if (target == null) return null;
            var forceHttps = false;
            if (siteDto.SeoForceHttps == 1 && request.GetProto()!="https")
            {
                target = target.Replace("http://", "https://");
                forceHttps = true;
            }

            string host = request.GetHost();
            if (host == "localhost") return null;
            var hostParts = host.Split('.').Length;
            // 跳转到带www的二级域名
            if (siteDto.SeoForceRedirect == 1 && hostParts == 2)
            {
                return target.Replace(host, "www." + host);
            }

            // 跳转到不带www的顶级域名
            if (siteDto.SeoForceRedirect == 2 && host.StartsWith("www."))
            {
                return target.Replace("www.", "");
            }

            return forceHttps ? target : null;
        }
    }
}