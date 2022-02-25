using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace JR.Stand.Abstracts.Web
{
    public interface ICompatibleRequest
    {
        string Method();
        string GetHeader(string key);
        string GetHost();

        /// <summary>
        /// 获取Http协议, http或https, 等同于Scheme
        /// </summary>
        /// <returns></returns>
        string GetProto();

        string GetApplicationPath();
        string GetPath();
        string GetQueryString();
        bool TryGetHeader(string key, out StringValues value);
        string UrlEncode(string url);
        string UrlDecode(string url);

        /// <summary>
        /// 获取查询参数
        /// </summary>
        /// <returns></returns>
        StringValues Query(string key);

        /// <summary>
        ///  获取表单参数
        /// </summary>
        /// <returns></returns>
        StringValues Form(string key);

        bool TryGetCookie(string member, out string o);
        IEnumerable<string> CookiesKeys();
        string GetEncodedUrl();
        IEnumerable<string> FormKeys();
        T ParseFormToEntity<T>();
        string GetParameter(string key);

        /// <summary>
        /// 获取上传的文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ICompatiblePostedFile File(string key);

        /// <summary>
        /// 按序号获取上传的文件
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        ICompatiblePostedFile FileIndex(int i);

        IDictionary<String, StringValues> Headers();
        
        /// <summary>
        /// 将请求内容绑定到类型T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Bind<T>();
    }
}