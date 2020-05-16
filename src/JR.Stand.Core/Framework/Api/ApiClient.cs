using System;
using System.Collections.Generic;
using System.Text;
using JR.Stand.Core.Framework.Net;
using JR.Stand.Core.Framework.Security;

namespace JR.Stand.Core.Framework.Api
{
    /// <summary>
    ///  接口响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        // 接口执行状态码
        public int Code;
        // 接口返回消息
        public String Message;
        // 接口返回数据
        public T Data;

    }

    public class ApiClient
    {
        private string _key = "";
        private string _secret = "";
        private string _signType = "sha1";
        private string _apiUrl = "";


        /// <summary>
        /// 配置接口
        /// </summary>
        /// <param name="server"></param>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="signType"></param>
        public ApiClient(string server, string key, string secret, string signType)
        {
            _apiUrl = server;
            _key = key;
            _secret = secret;
            _signType = signType;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="signType">签名类型</param>
        /// <param name="dataMap">数据字典</param>
        /// <param name="secret">秘钥</param>
        /// <returns></returns>
        private String Sign(String signType,IDictionary<string, string> dataMap, String secret)
        {
            // 参数排序
            SortedDictionary<string, string> sortDict = new SortedDictionary<string, string>();
            foreach (var d in dataMap)
            {
                if (d.Key != "sign" && d.Key != "sign_type")
                {
                    sortDict.Add(d.Key, d.Value);
                }
            }
            // 拼接字符
            var sb = new StringBuilder();
            int i = 0;
            foreach (var d in sortDict)
            {
                if (i > 0)
                {
                    sb.Append("&");
                }
                sb.Append(d.Key);
                sb.Append("=");
                sb.Append(d.Value);
                i++;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString() + secret);
            return CryptoUtils.HashString(signType,bytes).ToLower();
        }



        private String postRaw(String api, IDictionary<string, string> dict)
        {
            if (_apiUrl == "" || _key == "" || _signType == "" || _secret == "")
            {
                throw new ArgumentException("请先调用Configure()方法初始化接口");
            }
            // 附加参数
            dict.Add("api", api);
            dict.Add("key", this._key);
            dict.Add("sign_type", this._signType);
            String sign = this.Sign(this._signType,dict, this._secret);
            dict.Add("sign", sign);
            byte[] data = Encoding.UTF8.GetBytes(HttpClient.ParseQuery(dict));
            return HttpClient.Request(this._apiUrl, "POST", data, null, 0);
        }


        public String Post(String api, IDictionary<string, string> dict)
        {
            return this.postRaw(api, dict);
        }

        public Response<T> TPost<T>(String api, IDictionary<string, string> dict)
        {
            String rspText = this.postRaw(api, dict);
            Response<T> rsp = JsonSerializer.DeserializeObject<Response<T>>(rspText);
            return rsp;
        }

        public Response<T>[] MultiTPost<T>(String api, IDictionary<string, string> dict)
        {
            String rspText = this.postRaw(api, dict);
            Response<T>[] rsp = JsonSerializer.DeserializeObject<Response<T>[]>(rspText);
            return rsp;
        }
    }

}
