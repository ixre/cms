using System;
using System.Collections.Generic;
using System.Text;
using JR.Stand.Core.Framework.Security;

namespace JR.Stand.Core.Framework.Api
{
    /// <summary>
    /// API工具类
    /// </summary>
    public static class ApiUtils
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="signType">签名类型</param>
        /// <param name="dataMap">数据字典</param>
        /// <param name="secret">秘钥</param>
        /// <returns></returns>
        public static String Sign(String signType, IDictionary<string, string> dataMap, String secret)
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
            return CryptoUtils.HashString(signType, bytes).ToLower();
        }
    }
}
