/*
 * Copyright 2010 OPS,All right reseved .
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
using System.Web;

namespace JR.Stand.Core.Framework.Net
{
    /// <summary>
    /// HTTP客户端
    /// </summary>
    public class HttpClient
    {
        private string uri;

        private HttpClient(string uri)
        {
            this.uri = uri;
        }

        /// <summary>
        /// 发起POST请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postData">数据</param>
        /// <param name="cookies">cookies</param>
        /// <returns></returns>
        public static string Post(string url, string postData, CookieCollection cookies)
        {
            return Request(url, "POST", Encoding.UTF8.GetBytes(postData), cookies, 0);
        }

        /// <summary>
        /// 发起请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">方法</param>
        /// <param name="data">数据</param>
        /// <param name="cookies">cookies</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static string Request(string url,string method, byte[] data, CookieCollection cookies, int timeout)
        {
            HttpWebRequest req = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(CheckValidationResult);
                req = WebRequest.Create(url) as HttpWebRequest;
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
            //httpwebrequest控件有一个透明过程，先向服务方查询url是否存在而不发送POST的内容，
            //服务器如果证实url是可访问的，才发送POST，早期的Apache就认为这是一种错误，而IIS却可以正确应答
            //加上下面这一句将查询服务后马上post数据
            //System.Net.ServicePointManager.Expect100Continue = false;
            //Expect:100-continue
            ServicePointManager.Expect100Continue = false;
            if (req.Method == "POST")
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
            if (timeout > 0)
            {
                req.Timeout = timeout;
            }

            //添加cookie
            if (cookies != null)
            {
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(cookies);
            }

            //发送请求
            req.ContentLength = data.Length;
            Stream requestStream = req.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Dispose();
            //获取响应
            HttpWebResponse rsp = req.GetResponse() as HttpWebResponse;
            if (rsp.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(String.Format("远程服务器：HTTP {0} {1}", rsp.StatusCode, rsp.StatusDescription));
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
                sb.Append(p.Key).Append("=").Append(HttpUtility.UrlEncode(p.Value));
            }
            return sb.ToString();
        }

        public string Post(string postData, CookieCollection cookies)
        {
            return Post(this.uri, postData, cookies);
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
            int cread;
            int cTotal;

            MemoryStream ms = fileBytes == null || fileBytes.Length == 0
                ? new MemoryStream()
                : new MemoryStream(fileBytes);

            string remoteAddr = url;

            int fileLength = (int) ms.Length;

            HttpWebRequest wr = WebRequest.Create(remoteAddr) as HttpWebRequest;
            if (fileLength != 0)
            {
                wr.AddRange(fileLength);
            }

            try
            {
                WebResponse rsp = wr.GetResponse();

                Stream st = rsp.GetResponseStream();

                cTotal = (int) rsp.ContentLength;

                while ((cread = st.Read(data, 0, buffer)) != 0)
                {
                    ms.Write(data, 0, cread);
                }

                byte[] streamArray = ms.ToArray();
                ms.Dispose();

                return streamArray;
            }
            catch
            {
            }
            return null;
        }

        //
        //TODO:做一个支持断点下载的方法
        //
    }
}