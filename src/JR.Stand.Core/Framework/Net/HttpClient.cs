/*
 * Copyright 2010 OPS.CC,All right reserved .
 * name     : ftpclient
 * author   : newmin
 * date     : 2010/12/13
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using JR.Stand.Core.Web;
using Newtonsoft.Json;

namespace JR.Stand.Core.Framework.Net
{
    /// <summary>
    /// HTTP客户端
    /// </summary>
    public static class HttpClient
    {
        /// <summary>
        /// 发起请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">方法</param>
        /// <param name="o">数据</param>
        /// <returns></returns>
        public static string Request(string url,string method, HttpRequestParam o)
        {
            if (o == null) o = new HttpRequestParam();
            HttpWebRequest req ;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(CheckValidationResult);
                req = WebRequest.Create(url) as HttpWebRequest;
                if (req == null) throw new Exception("创建请求失败");
                req.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                req = WebRequest.Create(url) as HttpWebRequest;
            }
            if (req == null) throw new Exception("建立与" + url + "的请求连接失败");
            //request.Accept = "*/*";
            req.Method = method;
            req.KeepAlive = false;
            req.UserAgent = " Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:61.0) Gecko/20100101 Firefox/61.0";
            if (o.Header != null)
            {
                foreach (var p in o.Header)
                {
                    req.Headers[p.Key] = p.Value;
                }
            }
            //HttpWebRequest控件有一个透明过程，先向服务方查询url是否存在而不发送POST的内容，
            //服务器如果证实url是可访问的，才发送POST，早期的Apache就认为这是一种错误，而IIS却可以正确应答
            //加上下面这一句将查询服务后马上post数据
            //System.Net.ServicePointManager.Expect100Continue = false;
            //Expect:100-continue
            ServicePointManager.Expect100Continue = false;
           
            if (o.Timeout > 0)
            {
                req.Timeout = o.Timeout;
            }

            //添加cookie
            if (o.Cookies != null)
            {
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(o.Cookies);
            }

            Byte[] data = null;
            if (req.Method != "HEAD" && req.Method != "GET")
            {
                if (o.Form != null)
                {
                    req.ContentType = "application/x-www-form-urlencoded";
                    data = Encoding.UTF8.GetBytes(ParseQuery(o.Form));
                }else if (o.Data != null)
                {
                    req.ContentType = "application/json";
                    data = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject(o.Data));
                }
            }

            //发送请求
            if (data != null)
            {
                req.ContentLength = data.Length;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Dispose();
            }

            //获取响应
            HttpWebResponse rsp = req.GetResponse() as HttpWebResponse;
            if (rsp.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"远程服务器：HTTP {rsp.StatusCode} {rsp.StatusDescription}");
            }
            StreamReader reader = new StreamReader(rsp.GetResponseStream());
            String body = reader.ReadToEnd();
            reader.Close();
            return body;
        }


        /// <summary>
        /// 将字典转为URL查询参数
        /// </summary>
        /// <param name="paramMap">参数字典</param>
        /// <returns></returns>
        public static string ParseQuery(IDictionary<string, string> paramMap)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<String, String> p in paramMap)
            {
                if (i++ > 0) sb.Append("&");
                sb.Append(p.Key).Append("=").Append(HttpUtils.UrlEncode(p.Value));
            }
            return sb.ToString();
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //总是接受  
            return true;
        }


        /// <summary>
        /// 下载文件（支持断点续传）并返回字节数组
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileBytes">上次接收的文件</param>
        /// <returns></returns>
        public static byte[] DownloadFile(string url, byte[] fileBytes)
        {
            const int buffer = 32768; //32k
            byte[] data = new byte[buffer];

            MemoryStream ms = fileBytes == null || fileBytes.Length == 0
                ? new MemoryStream()
                : new MemoryStream(fileBytes);

            string remoteAddr = url;

            int fileLength = (int) ms.Length;

            HttpWebRequest wr = WebRequest.Create(remoteAddr) as HttpWebRequest;
            if (wr == null)
            {
                throw new Exception("create request failed");
            }
            if (fileLength != 0)
            {
                wr.AddRange(fileLength);
            }

            try
            {
                WebResponse rsp = wr.GetResponse();
                Stream st = rsp.GetResponseStream();
                int cRead;
                while ((cRead = st.Read(data, 0, buffer)) != 0)
                {
                    ms.Write(data, 0, cRead);
                }

                byte[] streamArray = ms.ToArray();
                ms.Dispose();

                return streamArray;
            }
            catch
            {
                // ignored
            }

            return null;
        }

        //
        //TODO:做一个支持断点下载的方法
        //
    }
}