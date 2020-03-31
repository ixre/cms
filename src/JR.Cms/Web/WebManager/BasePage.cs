//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
// 
// Project: jr.Cms.Manager
// FileName : BasePage.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 9:33:57
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
// Modify:
// 2013-07-15 14:00 newmin [!]: 修改模板呈现方式
//
//

using System;
using System.Collections;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using JR.Cms.Conf;
using JR.Cms.ServiceDto;
using JR.Cms.WebImpl.Json;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core;
using JR.Stand.Core.Framework.Extensions;
using JR.Stand.Core.Framework.Web;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.WebImpl.WebManager
{
    /// <summary>
    /// 
    /// </summary>
    public class BasePage
    {
        /// <summary>
        /// 
        /// </summary>
        protected ICompatibleResponse Response => HttpHosting.Context.Response;

        /// <summary>
        /// 
        /// </summary>
        protected ICompatibleRequest Request => HttpHosting.Context.Request;

        private static readonly string[] ignoreURI;
        protected SiteDto _site;
        private Hashtable _viewData;

        /// <summary>
        /// 
        /// </summary>
        public Hashtable ViewData => _viewData ?? (_viewData = new Hashtable());


        /// <summary>
        /// 管理后台模板标签
        /// </summary>
        private static readonly ManagerTemplate tpl;

        static BasePage()
        {
            ignoreURI = new string[]
            {
                "module=archive&action=update",
                "module=template&action=editfile",
                "module=file&action=editfile"
            };


            tpl = new ManagerTemplate();
        }

        internal static string ReplaceHtml(string html, string tagKey, string tagValue)
        {
            return html.Replace("${" + tagKey + "}", tagValue);
        }

        internal static string CompressHtml(string html)
        {
            //return html;
            html = Regex.Replace(html, ">(\\s)+<", "><");

            //替换 //单行注释
            html = Regex.Replace(html, "[\\s|\\t]+\\/\\/[^\\n]*(?=\\n)", string.Empty);

            //替换多行注释
            //const string multCommentPattern = "";
            html = Regex.Replace(html, "/\\*[^\\*]+\\*/", string.Empty);

            //替换<!-- 注释 -->
            html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->", string.Empty);

            //html = Regex.Replace(html, "<!--[^\\[][\\s\\S]*?-->|(^?!=http:|https:)//(.+?)\r\n|\r|\n|\t|(\\s\\s)", String.Empty);
            html = Regex.Replace(html, "\r|\n|\t|(\\s\\s)", string.Empty);

            return html;
        }

        protected void RenderTemplateUseCache(string content, object dataObj)
        {
            if (!Cms.Cache.CheckClientCacheExpiresByEtag()) return;

            string html = null;
            var response = Response;

            var _tpl = new MicroTemplateEngine(tpl);
            html = _tpl.Execute(content);

            if (dataObj != null)
            {
                //替换传入的标签参数
                var properties = dataObj.GetType().GetProperties();
                object dataValue;
                foreach (var p in properties)
                {
                    dataValue = p.GetValue(dataObj, null);
                    if (dataValue != null) html = ReplaceHtml(html, p.Name, dataValue.ToString());
                }
            }

            var query = HttpHosting.Context.Request.GetQueryString();
            if (!Array.Exists(ignoreURI, a => query.IndexOf(a) != -1)) html = CompressHtml(html);


            setHeader:

            //设置缓存
            Cms.Cache.SetClientCacheByEtag(Response);
            response.ContentType("text/html;charset=utf-8");

            //输出内容
            Response.WriteAsync(html);

            //todo: gzip
            // if (Settings.Opti_SupportGZip)
            // {
            //     response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            //     response.AddHeader("Content-Encoding", "gzip");
            // }
            //
            // response.AddHeader("X-AspNet-Version", String.Format("jr.Cms v{0}", Cms.Version));
            // response.AddHeader("Support-URL", "cms.to2.net/cms/");
        }


        protected void RenderTemplate(string content)
        {
            var response = Response;
            response.ContentType("text/html;charset=utf-8");
            response.WriteAsync(RequireTemplate(content));
        }

        protected string RequireTemplate(string content)
        {
            string html = null;
            var response = Response;

            var _tpl = new MicroTemplateEngine(tpl);
            html = _tpl.Execute(content);

            object value;
            foreach (var p in ViewData.Keys)
                if ((value = ViewData[p]) != null)
                    html = ReplaceHtml(html, p.ToString(), value.ToString());

            var query = Request.GetQueryString();

            if (!Array.Exists(ignoreURI, a => query.IndexOf(a, StringComparison.Ordinal) != -1))
                html = CompressHtml(html);

            // if (Settings.Opti_SupportGZip)
            // {
            //     response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            //     response.AddHeader("Content-Encoding", "gzip");
            // }

            HttpHosting.Context.TryGetItem<string>("ajax", out var isAjax);

            if (Request.Query("ajax") == "1" || isAjax == "1")
            {
                const string ajaxPattern = "<body([^>]*)>([\\s\\S]+)</body>";
                if (Regex.IsMatch(html, ajaxPattern))
                {
                    var match = Regex.Match(html, ajaxPattern);

                    return match.Groups[2].Value;
                }
            }

            return html;
        }

        protected void RenderTemplate(string content, object dataObj)
        {
            string html = null;
            var response = Response;

            var _tpl = new MicroTemplateEngine(tpl);
            html = _tpl.Execute(content);

            if (dataObj != null)
            {
                //替换传入的标签参数
                var properties = dataObj.GetType().GetProperties();
                object dataValue;
                foreach (var p in properties)
                {
                    dataValue = p.GetValue(dataObj, null);
                    if (dataValue != null) html = ReplaceHtml(html, p.Name, dataValue.ToString());
                }
            }

            var query = Request.GetQueryString();

            if (!Array.Exists(ignoreURI, a => query.IndexOf(a, StringComparison.Ordinal) != -1))
                html = CompressHtml(html);
            HttpHosting.Context.TryGetItem<string>("ajax", out var isAjax);

            
            if (Request.Query("ajax") == "1" ||isAjax == "1")
            {
                const string ajaxPartern = "<body([^>]*)>([\\s\\S]+)</body>";
                if (Regex.IsMatch(html, ajaxPartern))
                {
                    var match = Regex.Match(html, ajaxPartern);

                    response.WriteAsync(match.Groups[2].Value);
                    return;
                    //goto setHeader;
                }
            }

            //输出内容
            response.WriteAsync(html);

            // setHeader:
            //
            //     if (Settings.Opti_SupportGZip)
            //     {
            //         response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            //         response.AddHeader("Content-Encoding", "gzip");
            //     }
        }


        #region 页面返回呈现

        internal string ReturnSuccess(string message)
        {
            return ReturnSuccess(message, string.Empty);
        }

        internal string ReturnSuccess(string message, string data)
        {
            var sb = new StringBuilder();
            if (Request.Query("json") == "1" || Request.Query("xhr") != "")
            {
                sb.Append("{\"result\":true,\"message\":\"");
                if (message != null) sb.Append(message.Replace("\"", "\\\""));
                sb.Append("\",\"data\":\"").Append(data).Append("\"}");
                Response.ContentType ("application/json;charset=utf-8");
            }
            else
            {
                if (message != null) sb.Append(message);
            }

            return sb.ToString();
        }

        internal string ReturnSuccess()
        {
            return ReturnSuccess("操作执行成功！");
        }

        internal void RenderSuccess()
        {
            RenderSuccess("操作执行成功！");
        }

        internal void RenderSuccess(string message)
        {
            Response.WriteAsync(ReturnSuccess(message));
        }

        internal string ReturnError(string message)
        {
            var sb = new StringBuilder();
            if (Request.Query("json") == "1" || Request.Query("xhr") != "")
            {
                sb.Append("{\"result\":false,\"message\":\"");
                if (message != null) sb.Append(message.Replace("\"", "\\\"").Replace("\\n", ""));
                sb.Append("\"}");
                Response.ContentType("application/json");
            }
            else
            {
                sb.Append("<span style=\"color:red\">");
                if (message != null) sb.Append(message);
                sb.Append("</span>");
            }

            return sb.ToString();
        }

        internal string ReturnError()
        {
            return ReturnError("对不起，操作失败！");
        }

        internal void RenderError(string message)
        {
            Response.WriteAsync(ReturnError(message));
        }

        internal void Render(string html)
        {
            Response.WriteAsync(html);
        }

        #endregion

        /// <summary>
        /// 输出分页数据
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pager"></param>
        internal void PagerJson2(string html, string pager)
        {
            const string fmt = "{\"html\":\"%html%\",\"pager\":\"%pager%\"}";
            Response.ContentType("application/json;charset=utf-8");
            Response.WriteAsync(fmt.Template(html.Replace("\"", "\\\""), pager.Replace("\"", "\\\"")));
        }

        /// <summary>
        /// 输出分页数据
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pager"></param>
        internal void PagerJson(object rows, string pager)
        {
            const string fmt = "{\"pager\":\"%pager%\",\"rows\":%html%}";
            Response.ContentType ( "application/json;charset=utf-8");
            Response.WriteAsync(fmt.Template(
                pager.Replace("\"", "\\\""),
                JsonSerializer.Serialize(rows)
            ));
        }


        /// <summary>
        /// 当前管理站点
        /// </summary>
        internal SiteDto CurrentSite
        {
            get
            {
                if (_site.SiteId <= 0) _site = CmsWebMaster.CurrentManageSite;
                return _site;
            }
            set => CmsWebMaster.SetCurrentManageSite(HttpHosting.Context, value);
        }

        protected int SiteId => CurrentSite.SiteId;

        /// <summary>
        /// 比较是否为当前站点的分类
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        internal bool CompareSite(int siteId)
        {
            //当前站点
            var site = CurrentSite;
            if (siteId != site.SiteId)
            {
                RenderError("分类不存在！");
                return true;
            }

            return false;
        }
    }
}