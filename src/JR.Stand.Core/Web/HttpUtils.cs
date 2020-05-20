using System;
using JR.Stand.Abstracts.Web;

namespace JR.Stand.Core.Web
{
    public static class HttpUtils
    {
        
        public static bool IsHttpsRequest(ICompatibleRequest request)
        {
            //throw new Exception(request.GetHeader("SSL-FLAG")??"xxx" +"|"+request.GetHeader("From-Https")??"OO");
            if (request.GetScheme() == "https") return true;
            if (request.GetHeader("X-Forwarded-Proto") == "https") return true;
            // 兼容西部数码虚拟主机
            if (request.GetHeader("SSL-FLAG") == "SSL" ||
                request.GetHeader("From-Https") == "on") return true;
            return false;
        }
    }
}