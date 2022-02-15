using System;
using System.Collections.Generic;
using JR.Stand.Core.Framework.Net;

namespace JR.Stand.Core.Framework.Api
{
    public class RestfulClient
    {
        private readonly string _server; // 	= "http://localhost:1419/openapi"
        private Func<String> _accessTokenFunc;
        private string _accessToken;
        private long _lastTokenUnix;
        private int _expires;

        /// <summary>
        /// 配置接口
        /// </summary>
        /// <param name="server"></param>
        public RestfulClient(string server)
        {
            this._server = server;
        }

        /// <summary>
        /// 设置令牌更新
        /// </summary>
        /// <param name="accessTokenFunc"></param>
        /// <param name="tokenExpires"></param>
        public void UseToken(Func<String> accessTokenFunc, int tokenExpires)
        {
            this._accessTokenFunc = accessTokenFunc;
            this._expires = tokenExpires;
        }

        public String Request(String path,String method, Object data)
        {
            int now = TimeUtils.Unix();
            if (now - this._lastTokenUnix > this._expires)
            {
                this._accessToken = this._accessTokenFunc();
                this._lastTokenUnix = now;
            }
            return HttpClient.Request(this._server + path, method, new HttpRequestParam
            {
                Header = new Dictionary<string, string>
                {
                    {"Authorization", this._accessToken},
                },
                Data = data,
            });
        }


        public Response<T> Request<T>(String path,String method, Object data)
        {
            String rspText = this.Request(path,method, data);
            Response<T> rsp = JsonSerializer.DeserializeObject<Response<T>>(rspText);
            return rsp;
        }
    }
}