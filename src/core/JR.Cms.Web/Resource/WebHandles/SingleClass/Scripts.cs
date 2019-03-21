/*
 * Get  用于获取资料,如javascript,image等
 * Copyright 2010 OPS,INC.All rights reseved!
 * author   : newmin
 * date     : 2010/12/11 23:56
 */
namespace OPSite.WebHandler
{
    using System;
    using System.IO;
    using System.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Resources;
    using J6.Cms;
    using J6.Web;
    using J6.Net;

    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptAttribute : Attribute, IWebExecute
    {
        //保存加载公共脚本的代码,由缓存控制
        private static string JS_String;

        public void PreExecuting()
        {
            HttpResponse response = HttpContext.Current.Response;

            //获取内容
            object obj = HttpRuntime.Cache[CacheNames.Js_global];
            if (obj == null)
            {
                //默认调用本地的JS脚本
                string jsUri = "/scripts/global.js";
                //读取远端脚本
                if (Server.StaticServer !=null) jsUri = Server.StaticServer + "/scripts/opsJS_v1.1.js";

                StringBuilder sb = new StringBuilder();
                sb.Append("document.write(\"<script type='text/javascript' charset='utf-8' src='").Append(jsUri).Append("'></script>\");")
                  .Append("document.write(\"<script type='text/javascript' src='/scripts/script.js'></script>\");");
                JS_String = sb.ToString();
                HttpRuntime.Cache.Insert(CacheNames.Js_global, 1, null, DateTime.Now.AddDays(1), TimeSpan.Zero);
            }
            else
            {
                //如果不更新脚本链接，则将修改时间更新到1个月之后
                response.AddHeader("If-Modified-Since", DateTime.Now.AddMonths(1).ToString()); 
            }
            response.ContentType = "text/javascript";
            response.AddHeader("content-disposition", "attachment;filename=global.js");
            response.Write(JS_String.ToString());
            response.End();
        }
    }

    [ScriptAttribute]
    public class Scripts
    {
    }
}