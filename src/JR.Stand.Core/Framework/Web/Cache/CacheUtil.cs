/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/24
 * 时间: 17:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */

using System;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Core.Framework.Web.Cache
{
    /// <summary>
    /// Description of CacheUtil.
    /// </summary>
    public sealed class CacheUtil
    {
        /// <summary>
        /// 检查客户端缓存是否过期,如果未过期，则直接输出http 304
        /// </summary>
        /// <param name="maxAge"></param>
        /// <returns></returns>
        public static bool CheckClientCacheExpires(int maxAge)
        {
            /*
                   Public  指示响应可被任何缓存区缓存。
                   Private  指示对于单个用户的整个或部分响应消息，不能被共享缓存处理。这允许服务器仅仅描述当用户的
                   部分响应消息，此响应消息对于其他用户的请求无效。
                   no-cache  指示请求或响应消息不能缓存（HTTP/1.0用Pragma的no-cache替换）
                   根据什么能被缓存
                   no-store  用于防止重要的信息被无意的发布。在请求消息中发送将使得请求和响应消息都不使用缓存。
                   根据缓存超时
                   max-age  指示客户机可以接收生存期不大于指定时间（以秒为单位）的响应。
                   min-fresh  指示客户机可以接收响应时间小于当前时间加上指定时间的响应。
                   max-stale  指示客户机可以接收超出超时期间的响应消息。如果指定max-stale消息的值，那么客户机可以
                   接收超出超时期指定值之内的响应消息。
                   Expires 表示存在时间，允许客户端在这个时间之前不去检查（发请求），等同max-age的
                   效果。但是如果同时存在，则被Cache-Control的max-age覆盖。
                   格式：
			 */


            if (maxAge > 0)
            {
                ICompatibleResponse response = HttpHosting.Context.Response;
                ICompatibleRequest request = HttpHosting.Context.Request;
                var v = request.TryGetHeader("If-Modified-Since",out var e);
                if (v)
                {
                    string sinceModified = e;
                    //现在时间
                    DateTime nowTime = DateTime.Now.ToUniversalTime();

                    //最后修改时间
                    DateTime sinceTime;
                    DateTime.TryParse(sinceModified, out sinceTime);
                    sinceTime = sinceTime.ToUniversalTime();

                    if ((nowTime - sinceTime).TotalSeconds < maxAge)
                    {
                        response.StatusCode(304);
                        //response.Status = "304 Not Modified";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 检查客户端缓存是否过期,如果未过期，则直接输出http 304
        /// </summary>
        /// <param name="etag"></param>
        /// <returns></returns>
        public static bool CheckClientCacheExpires(string etag)
        {
            ICompatibleRequest request = HttpHosting.Context.Request;
            var v = request.TryGetHeader("If-None-Match",out var e);
            if (v)
            {
                string clientEtag = e;
                if (String.CompareOrdinal(clientEtag, String.Concat("\"", etag, "\"")) == 0)
                {
                    
                    ICompatibleResponse response =HttpHosting.Context.Response;
                    response.StatusCode(304);
                    //response.Status = "304 Not Modified";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 设置客户端保存缓存
        /// </summary>
        /// <param name="response"></param>
        /// <param name="maxAge"></param>
        public static void SetClientCache(ICompatibleResponse response, int maxAge)
        {
            DateTime nowTime = DateTime.Now.ToUniversalTime();
            response.AddHeader("Cache-Control", "max-age=" + maxAge);
            response.AddHeader("Last-Modified", nowTime.ToString("r"));
            //response.AddHeader("Cache-Control", "max-age=" + maxAge.ToString());
            //response.AddHeader("Last-Modified", nowTime.ToString("r"));
        }

        /// <summary>
        /// 设置客户端保存缓存
        /// </summary>
        /// <param name="response"></param>
        /// <param name="etag"></param>
        public static void SetClientCache(ICompatibleResponse response, string etag)
        {
            response.AddHeader("ETag", "\"" + etag + "\"");
        }

        /// <summary>
        /// 输出缓存内容
        /// </summary>
        /// <param name="response"></param>
        /// <param name="maxage"></param>
        /// <param name="handler"></param>
        /// <returns>是否缓存在客户端</returns>
        public static bool Output(ICompatibleResponse response, int maxage, StringCreatorHandler handler)
        {
            /*
                   Public  指示响应可被任何缓存区缓存。
                   Private  指示对于单个用户的整个或部分响应消息，不能被共享缓存处理。这允许服务器仅仅描述当用户的
                   部分响应消息，此响应消息对于其他用户的请求无效。
                   no-cache  指示请求或响应消息不能缓存（HTTP/1.0用Pragma的no-cache替换）
                   根据什么能被缓存
                   no-store  用于防止重要的信息被无意的发布。在请求消息中发送将使得请求和响应消息都不使用缓存。
                   根据缓存超时
                   max-age  指示客户机可以接收生存期不大于指定时间（以秒为单位）的响应。
                   min-fresh  指示客户机可以接收响应时间小于当前时间加上指定时间的响应。
                   max-stale  指示客户机可以接收超出超时期间的响应消息。如果指定max-stale消息的值，那么客户机可以
                   接收超出超时期指定值之内的响应消息。
                   Expires 表示存在时间，允许客户端在这个时间之前不去检查（发请求），等同max-age的
                   效果。但是如果同时存在，则被Cache-Control的max-age覆盖。
                   格式：
			 */

            #region 获取缓存状态

            ICompatibleRequest request = HttpHosting.Context.Request;

            //现在时间
            DateTime nowTime = DateTime.Now.ToUniversalTime();
            bool b=request.TryGetHeader("If-Modified-Since",out var sinceModified);
           if (b)
            {
                //最后修改时间
                DateTime sinceTime;
                DateTime.TryParse(sinceModified, out sinceTime);
                sinceTime = sinceTime.ToUniversalTime();

                if ((nowTime - sinceTime).TotalSeconds < maxage)
                {
                    response.StatusCode(304);
                   // response.Status = "304 Not Modified";
                    return true;
                }
            }

            #endregion

            #region 输出内容并缓存

            response.WriteAsync(handler());

            response.AddHeader("Cache-Control", "max-age=" + maxage.ToString());
            response.AddHeader("Last-Modified", nowTime.ToString("r"));

            #endregion

            return false;
        }

        /// <summary>
        /// 输出缓存内容
        /// </summary>
        /// <param name="response"></param>
        /// <param name="etag"></param>
        /// <param name="handler"></param>
        /// <returns>是否缓存在客户端</returns>
        public static bool Output(HttpResponse response, string etag, StringCreatorHandler handler)
        {
            HttpHosting.Context.Request.TryGetHeader("If-None-Match",out var clientEtag);
            if (String.Compare(clientEtag, String.Concat("\"", etag, "\""), false) == 0)
            {
                response.StatusCode = 304;
                //response.Status = "304 Not Modified";
                return true;
            }
            response.Headers.Add("ETag", "\"" + etag + "\"");
            response.WriteAsync(handler());
            return false;
        }
    }
}