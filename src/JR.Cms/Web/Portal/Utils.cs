using System.Text;
using JR.Cms.Conf;
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
        /// <returns></returns>
        public static string GetRdUrl(ICompatibleRequest request)
        {
            var target = request.GetEncodedUrl();
            if (target == null) return null;
            var forceHttps = false;
            //StringBuilder sb = new StringBuilder();
            // var headers = request.Headers();
            // foreach (string key in headers.Keys)
            // {
            //     sb.Append(key).Append("=").Append(headers[key]).Append("|");
            // }
            //
            // string proto = request.GetHeader("X-Forwarded-Proto") ?? "+";
            //
            // throw new Exception(sb.ToString() + "/" + Settings.SYS_FORCE_HTTPS + "/" + target);
            if (Settings.SYS_FORCE_HTTPS && request.GetProto()!="https")
            {
                target = target.Replace("http://", "https://");
                forceHttps = true;
            }

            string host = request.GetHost();
            if (host == "localhost") return null;
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