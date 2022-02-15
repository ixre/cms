using System;
using System.Collections.Generic;
using System.Net;

namespace JR.Stand.Core.Framework.Net
{
    /// <summary>
    /// HttpRequestParam
    /// </summary>
    public class HttpRequestParam
    {
        public CookieCollection Cookies;
        public int Timeout;
        public IDictionary<string, string> Header;
        public IDictionary<string, string> Form; // 发送form数据
        public Object Data;// 发body数据
    }
}