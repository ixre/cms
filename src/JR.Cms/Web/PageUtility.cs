//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms
// FileName : TemplateUtility.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/19 9:51:09
// Description :
//  2013-03-21  newmin [+] : 支持将URL参数作为模板内容
//
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.Text.RegularExpressions;
using JR.Cms.Web.Portal.Template.Rule;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Web;

namespace JR.Cms.Web
{
    /// <summary>
    /// 页面实用工具
    /// </summary>
    public static class PageUtility
    {
        /// <summary>
        /// 页面呈现之前发生
        /// </summary>
        private static TemplateHandler<object> preRender;

        private const string CopyStr =
            "\n<!-- This website use JRCMS v{0}, learn more please visiting https://fze.net/cms  -->";

        private static readonly string CopyStr2019;

        /// <summary>
        /// 编译模板
        /// </summary>
        private static readonly TemplateHandler<object> CompliedTemplate = (object classInstance, ref string html) =>
        {
            var tpl = new SimpleTplEngine(classInstance, !true);
            html = tpl.Execute(html);
        };


        static PageUtility()
        {
            CopyStr2019 = string.Format(CopyStr, Cms.Version);
        }

        private static readonly TemplateHandler<object> PreHandler = (object obj, ref string html) =>
        {
            //throw new Exception(html);

            /*
            const string pattern = "</body>\\s*</html>";
            const string copyStr = @"
<!--
*************************************************************************************
* 本站使用《JR-CMS》驱动。唯一跨平台,支持多种数据库,基于ASP.net技术构建的网站管理系统。
* 欢迎访问 https://fze.net/cms 获取详细信息及免费试用版本。

* 【承接】定制网站,营销型网站,WAP手机网站开发。QQ/微信:188867734
*************************************************************************************
-->";*/
            html = html.Insert(Math.Max(html.LastIndexOf("</body>", StringComparison.Ordinal), 0), CopyStr2019);
        };


        /// <summary>
        /// 返回模板内容
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="pageFunc">对模板处理前操作，可以添加数据对象</param>
        public static string Require(string templateName, TemplatePageHandler pageFunc)
        {
            var page = Cms.Template.GetTemplate(templateName);
            RegisterEventHandlers(page);
            // 注册扩展的模板解析事件
            if (preRender != null) page.OnPreRender += preRender;
            pageFunc?.Invoke(page);
            return page.Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static void RegisterEventHandlers(TemplatePage page)
        {
            var context = HttpHosting.Context;
            using (var tpl = new CmsTemplateImpl(context))
            {
                page.TemplateHandleObject = tpl;
                page.OnPreInit += PreHandler;
                page.OnPreRender += CompliedTemplate;
            }
        }


        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templateId"></param>
        /// <param name="data"></param>
        public static void Save(string path, string templateId, object data)
        {
            var page = Cms.Template.GetTemplate(templateId);
            page.AddDataObject(data);
            page.OnPreRender += PreHandler;
            page.OnPreRender += CompliedTemplate;

            //注册扩展的模板解析事件
            if (preRender != null) page.OnPreRender += preRender;

            page.SaveToFile(path);
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templateId"></param>
        /// <param name="data"></param>
        public static void Save(string path, string templateId, object data, out string html)
        {
            var page = Cms.Template.GetTemplate(templateId);
            page.AddDataObject(data);
            page.OnPreRender += PreHandler;
            page.OnPreRender += CompliedTemplate;
            //注册扩展的模板解析事件
            if (preRender != null) page.OnPreRender += preRender;
            page.SaveToFile(path, out html);
        }

        public static void RenderException(Exception e, bool recordLog)
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

            if (e.GetBaseException() != null) e = e.GetBaseException();

            var r = HttpHosting.Context.Response;
            //r.StatusCode =500;
            //r.Clear();
            r.WriteAsync(
                tpl.Replace("$MSG", e.Message.Replace("\n", "<br />"))
                    .Replace("$DETAILS", e.StackTrace.Replace("\n", "<br />"))
            );
        }
    }
}