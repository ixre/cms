//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: J6.Cms
// FileName : TemplateUtility.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/19 9:51:09
// Description :
//  2013-03-21  newmin [+] : 支持将URL参数作为模板内容
//
//
// Get infromation of this software,please visit our site http://k3f.net/cms
//
//

namespace J6.Cms
{
    using System;
    using System.Web;
    using System.Text.RegularExpressions;
    using J6.DevFw.Template;
    using J6.Cms.Template;


    /// <summary>
    /// 页面实用工具
    /// </summary>
    public class PageUtility
    {
        /// <summary>
        /// 页面呈现之前发生
        /// </summary>
        public static TemplateHandler<object> OnPreRender;

        const string CopyStr = "<!-- power by J6.Cms v {0} -->";

        private static string version;
        private static string copyStr2012;

        /// <summary>
        /// 编译模板
        /// </summary>
        private static readonly TemplateHandler<object> CompliedTemplate = (object classInstance, ref string html) =>
        {
            SimpleTplEngine mctpl = new SimpleTplEngine(classInstance,!true);
            html = mctpl.Execute(html);

            return;

            if (mctpl.Count != null)
            {
                foreach (string method in mctpl.Count)
                {
                    HttpContext.Current.Response.Write("<br />" + method);
                }
            }
        };


        static PageUtility()
        {
            /*
            AssemblyFileVersionAttribute ver = (AssemblyFileVersionAttribute)typeof(Cms).Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0];
            string[] verarr = ver.Version.Split('.');
            //version = String.Format("{0}.{1}.{2}", verarr[0], verarr[1], verarr[2]);
            version = String.Format("{0}.{1}.{2}", verarr[0], verarr[1],verarr[2]);
            */

            copyStr2012 = String.Format(CopyStr, Cms.Version);
        }

        private static TemplateHandler<object> PreHandler = (object obj,ref string html) =>
        {
           //throw new Exception(html);

            /*
            const string pattern = "</body>\\s*</html>";
            const string copyStr = @"
<!--
*************************************************************************************
* 本站使用《OPSite》驱动。唯一跨平台,支持多种数据库,基于ASP.net技术构建的网站管理系统。
* 欢迎访问 http://k3f.net/cms/soft/opsite.html 获取详细信息及试用版本。

* 【承接】定制网站,营销型网站,WAP手机网站开发。联系电话：18616999822  QQ:188867734
*************************************************************************************
-->";
           // html = Regex.Replace(html, pattern, String.Format("{0}</body></html>", copyStr2012), RegexOptions.IgnoreCase);
            */

            /*

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

             */
            
            html += copyStr2012;
        };


        /// <summary>
        /// 返回模板内容
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pageFunc">对模板处理前操作，可以添加数据对象</param>
        public static string Require(string templateId,TemplatePageHandler pageFunc)
        {
            TemplatePage page = new TemplatePage(templateId);
            using (CmsTemplateImpl tpl = new CmsTemplateImpl())
            {
                //if (data != null)
                //{
                //   page.AddDataObject(data);
                //}
                page.TemplateHandleObject = tpl;
                page.OnPreInit += PreHandler;
                page.OnPreRender += CompliedTemplate;

                //注册扩展的模板解析事件
                if (OnPreRender != null) page.OnPreRender += OnPreRender;

                if (pageFunc != null) pageFunc(page);

                return page.ToString();
            }
        }

        public static TemplatePage RegistEventHandlers(TemplatePage page)
        {
        	using (CmsTemplateImpl tpl = new CmsTemplateImpl()){
        	 	page.TemplateHandleObject = tpl;
                page.OnPreInit += PreHandler;
                page.OnPreRender += CompliedTemplate;
                return page;
        	}
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
        /// <param name="templateId"></param>
        /// <param name="data"></param>
        public static void Save(string path, string templateId, object data)
        {
            TemplatePage page = new TemplatePage(templateId, data);
            page.OnPreRender += PreHandler;
            page.OnPreRender += CompliedTemplate;

            //注册扩展的模板解析事件
            if (OnPreRender != null) page.OnPreRender += OnPreRender;

            page.SaveToFile(path);
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templateId"></param>
        /// <param name="data"></param>
        public static void Save(string path, string templateId, object data,out string html)
        {
            TemplatePage page = new TemplatePage(templateId, data);
            page.OnPreRender += PreHandler;
            page.OnPreRender += CompliedTemplate;

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


        public static string CompressHtml(string html)
        {
            //html = Regex.Replace(html, ">(\\s)+<", "><");
            ////html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|(^?!=http:|https:)//(.+?)\r\n|\r|\n|\t|(\\s\\s)", String.Empty);
            //html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|\r|\n|\t|(\\s\\s)", String.Empty);
            //return html;

            html = Regex.Replace(html, ">(\\s)+<", "><");

            //替换 //单行注释
            html = Regex.Replace(html, "[\\s|\\t]+\\/\\/[^\\n]*(?=\\n)", String.Empty);

            //替换多行注释
            //const string multCommentPattern = "";
            html = Regex.Replace(html, "/\\*[^\\*]+\\*/", String.Empty);

            //替换<!-- 注释 -->
            html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->", String.Empty);

            //html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|(^?!=http:|https:)//(.+?)\r\n|\r|\n|\t|(\\s\\s)", String.Empty);
            html = Regex.Replace(html, "\r|\n|\t|(\\s\\s)", String.Empty);

            return html;
        }
    }
}
