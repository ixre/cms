using JR.Cms.Conf;
using JR.Stand.Abstracts.Web;

namespace JR.Cms.Web.Portal
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// 获取自动定向的地址,如果nginx设置了https跳转, 参考如下:
        /// if ($server_port !~ 443){
        ///     rewrite ^(/.*)$ https://fze.net$1 permanent;
        /// }
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRdUrl(ICompatibleRequest request)
        {
            string host = request.GetHost();
            if (host == "localhost") return null;
            var target = request.GetEncodedUrl();
            //if (target == null) return null;
            var forceHttps = false;
            if (Settings.SYS_FORCE_HTTPS && request.GetProto() !="https")
            {
                target = target.Replace("http://", "https://");
                forceHttps = true;
            }

            var hostParts = host.Split('.').Length;
            // 跳转到带www的二级域名
            if (Settings.SYS_WWW_RD == 1 && hostParts == 2)
            {
                return target.Replace(host, "www." + host);
            }

            // 跳转到不带www的顶级域名
            if (Settings.SYS_WWW_RD == 2 && host.StartsWith("www."))
            {
                return target.Replace("www.", "");
            }

            return forceHttps ? target : null;
        }
    }
}