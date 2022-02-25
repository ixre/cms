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
        class HttpRequestParamBuilder
        {
            private CookieCollection Cookies;
            private int Timeout;
            private IDictionary<string, string> Header = new Dictionary<string, string>();
            private  IDictionary<string, string> Form = new Dictionary<string, string>(); // 发送form数据
            public Object Data;// 发body数据
        }
        
        public CookieCollection Cookies;
        public int Timeout;
        public IDictionary<string, string> Header;
        public IDictionary<string, string> Form; // 发送form数据
        public Object Body;// 发body数据
    }
}