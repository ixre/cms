//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: OPS.OPSite
// FileName : TemplateUtility.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/19 9:51:09
// Description :
//  2013-03-21  newmin [+] : 支持将URL参数作为模板内容
//
//
// Get infromation of this software,please visit our site http://www.j6.cc
//
//

namespace Spc.Web
{
    using System;
    using System.IO;
    using System.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using J6.Template;
    using System.Reflection;


    /// <summary>
    /// 页面实用工具
    /// </summary>
    public class MultLangPageUtility
    {
        /// <summary>
        /// 页面呈现之前发生
        /// </summary>
        public static TemplateHandler<object> OnPreRender;

        const string copyStr = "\r\n<!-- This site driver by j6.cms {0}, help link : www.j6.cc/cms/ -->";

        private static string version;
        private static string copyStr2012;

        static MultLangPageUtility()
        {
            copyStr2012 = String.Format(copyStr,Cms.Version);
        }

        private static TemplateHandler<object> PreHandler = (object obj,ref string html) =>
        {


            /*
            const string pattern = "</body>\\s*</html>";
            const string copyStr = @"
<!--
*************************************************************************************
* 本站使用《OPSite》驱动。唯一跨平台,支持多种数据库,基于ASP.net技术构建的网站管理系统。
* 欢迎访问 http://www.j6.cc/soft/opsite.html 获取详细信息及试用版本。

* 【承接】定制网站,营销型网站,WAP手机网站开发。联系电话：18616999822  QQ:188867734
*************************************************************************************
-->";
           // html = Regex.Replace(html, pattern, String.Format("{0}</body></html>", copyStr2012), RegexOptions.IgnoreCase);
            */

            //替换标签为页面URL参数
            html = TemplateRegexUtility.Replace(html, m =>
            {
                string paramName = m.Groups[1].Value;

                if (!String.IsNullOrEmpty(HttpContext.Current.Request[paramName]))
                {
                    return HttpContext.Current.Request[paramName];
                }
                return m.Value;
            });

            html += copyStr2012;
        };
          

        /// <summary>
        /// 返回模板内容
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="data"></param>
        public static string Require(string templateID, object data)
        {
            TemplatePage page=new TemplatePage(templateID, data);
            page.TemplateHandleObject = new Spc.Template.MultLangCmsTemplates();
            page.OnPreInit += PreHandler;
            page.OnPreRender += Spc.Template.MultLangCmsTemplates.CompliedTemplate;

            //注册扩展的模板解析事件
            if (OnPreRender != null) page.OnPreRender += OnPreRender;

            return page.ToString() ;
        }

        [Obsolete("instead of Render(string templateID, object data) use Require(string templateID, object data)")]
        public static string Render(string templateID, object data)
        {
            return Require(templateID, data);
        }

        /// <summary>
        /// 呈现内容，默认不压缩
        /// </summary>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <param name="compress"></param>
        /// <returns></returns>
        public static string Render(string content, object data,bool compress)
        {
            TemplatePage page = new TemplatePage(null,data);
            page.TemplateContent = content;
            return page.ToString();
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templateID"></param>
        /// <param name="data"></param>
        public static void Save(string path, string templateID, object data)
        {
            TemplatePage page = new TemplatePage(templateID, data);
            page.OnPreRender += PreHandler;
            page.OnPreRender += Spc.Template.MultLangCmsTemplates.CompliedTemplate;

            //注册扩展的模板解析事件
            if (OnPreRender != null) page.OnPreRender += OnPreRender;

            page.SaveToFile(path);
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templateID"></param>
        /// <param name="data"></param>
        public static void Save(string path, string templateID, object data,out string html)
        {
            TemplatePage page = new TemplatePage(templateID, data);
            page.OnPreRender += PreHandler;
            page.OnPreRender += Spc.Template.MultLangCmsTemplates.CompliedTemplate;

            //注册扩展的模板解析事件
            if (OnPreRender != null) page.OnPreRender += OnPreRender;

            page.SaveToFile(path,out html);
        }

        public static void RenderException(Exception e,bool recordLog)
        {
            const string tpl = @"<!DOCTYPE html>
                                <html>
                                <head>
                                    <title>Error</title>
                                    <style type=""text/css"">
                                        body{color:#666;font:14px Arial;background:#fffff}
                                        h1{font-style:italic;font-size:30px;color:red;margin:20px 15px 10px 15px;font-weight:normal;}
                                        .shadow{padding:4px;background:#f5f5f5;width:600px;margin:50px auto 0 auto;}
                                        .border{border:solid 2px #e0e0e0;padding:10px;background:white;margin:-8px 0 0 -8px;}
p.errormsg{padding:0 20px;font-weight:bold;color:#f25a07;font-size:14px;font-family:'微软雅黑';margin:20px 0;letter-spacing:0.5px;}
p.errormsg .red{color:red;}
div.tip{font-size:12px;padding:0 20px;color:#d0d0d0;text-align:right;}
#details{font-size:12px;border-top:solid 1px #f0f0f0;/*border-bottom:solid 1px #f5f5f5;*/margin:10px;padding:10px;line-height:16px;}
                                    </style>
                                </head>
                                <body>
                                    <div class=""shadow"">
                                        <div class=""border"">
                                            <h1>ERROR</h1>
                                            <p class=""errormsg"">$MSG</p>
                                            <div id=""details"">$DETAILS</div>
                                        <div>
                                    </div>
                                </body>
                                </html>";

            if (e.GetBaseException() != null)
            {
                e = e.GetBaseException();
            }

            HttpResponse r = HttpContext.Current.Response;
            //r.StatusCode =500;
            r.Clear();
            r.Write(
                    tpl.Replace("$MSG",e.Message.Replace("\n","<br />"))
                    .Replace("$DETAILS",e.StackTrace.Replace("\n","<br />"))
                );

            r.End();
        }
    }
}
