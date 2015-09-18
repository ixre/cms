/**
 * Copyright (C) 2007-2015 Z3Q.NET,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.z3q.net
 * 
 * name : CmsContext.cs
 * author : newmin (new.min@msn.com)
 * date : 2014/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface.Site;
using J6.Cms.Infrastructure;
using J6.DevFw.Framework.Web.Cache;
using J6.DevFw.Template;
using J6.Cms.DataTransfer;
using J6.DevFw.Web;

namespace J6.Cms.Core
{
    /// <summary>
    /// 应用程序配置
    /// </summary>
    public class CmsContext
    {
        /// <summary>
        /// 运行时发生
        /// </summary>
        public static event CmsHandler OnBeginRequest;

        /// <summary>
        /// 错误日志文件
        /// </summary>
        public static string ErrorFilePath;

        /// <summary>
        /// 是否作为虚拟目录运行
        /// </summary>
        public bool IsVirtualDirectoryRunning = false;

        /// <summary>
        /// 请求上下文
        /// </summary>
        private readonly HttpContext _context;

        public HttpContext HttpContext { get { return _context; } }


        /// <summary>
        /// 上下文来源
        /// </summary>
        public object Source { get; set; }

        public HttpResponse Response { get { return _context.Response; } }
        public HttpRequest Request { get { return _context.Request; } }


        static CmsContext()
        {
            ErrorFilePath = AppDomain.CurrentDomain.BaseDirectory + "tmp/logs/error.log";
        }

        public CmsContext()
        {
            if (OnBeginRequest != null)
            {
                OnBeginRequest();
            }

            this._context = HttpContext.Current;

            //设置当前站点
            this.CurrentSite = SiteCacheManager.GetSingleOrDefaultSite(this._context.Request.Url); ;

            //是否为虚拟目录运行
            if ((SiteRunType)this.CurrentSite.RunType == SiteRunType.VirtualDirectory)
                this.IsVirtualDirectoryRunning = true;
        }



        /// <summary>
        /// 当前站点
        /// </summary>
        public SiteDto CurrentSite { get; private set; }

        /// <summary>
        /// 路径和查询
        /// </summary>
        //public string PathAndQuery
        //{
        //    get
        //    {
        //        string str = context.Request.Url.PathAndQuery;
        //        return !IsVirtualDirectoryRunning ? str : str.Replace(this.CurrentSite.DirName + "/", String.Empty);
        //    }
        //}



        /// <summary>
        /// 站点域名
        /// </summary>
        private string _siteDomain;
        private PageDataItems _dataItems;
        private string _staticDomain;
        private string _resouceDomain;
        private string _host;
        private string _siteAppPath;


        /// <summary>
        /// 当前的Host,包含端口，如：z3q.net:8080
        /// </summary>
        public string Host
        {
            get
            {
                if (this._host == null)
                {
                    this._host = String.Format("{0}{1}", _context.Request.Url.Host,
                                                _context.Request.Url.Port != 80 ? ":" + _context.Request.Url.Port.ToString() : "");
                }
                return this._host;
            }
        }

        /// <summary>
        /// 系统应用程序目录
        /// </summary>
        public string ApplicationPath
        {
            get { return HttpApp.GetApplicationPath(); }
        }

        /// <summary>
        /// 站点应用程序目录
        /// </summary>
        public string SiteAppPath
        {
            get
            {
                return this._siteAppPath ??
                    (this._siteAppPath = this.IsVirtualDirectoryRunning ? "/" + this.CurrentSite.DirName : "/");
            }
        }


        /// <summary>
        /// 域名
        /// </summary>
        public string SiteDomain
        {
            get
            {
                if (this._siteDomain == null)
                {
                    if (this.SiteAppPath != "/")
                    {
                        this._siteDomain = WebCtx.Current.Domain + this.SiteAppPath;
                    }
                    else
                    {
                        this._siteDomain = WebCtx.Current.Domain;
                    }
                }
                return _siteDomain;
            }
        }

        /// <summary>
        /// 资源域
        /// </summary>
        public string ResourceDomain
        {
            get
            {
                return this._resouceDomain ?? (this._resouceDomain = WebCtx.Current.Domain);
            }
        }

        /// <summary>
        /// 静态资源域
        /// </summary>
        public string StaticDomain
        {
            get
            {
                if (this._staticDomain == null)
                {
                    if (Settings.SERVER_STATIC_ENABLED && Settings.SERVER_STATIC.Length != 0)
                    {
                        // this._staticDomain = String.Concat("http://", Settings.SERVER_STATIC);
                        this._staticDomain = String.Concat("//", Settings.SERVER_STATIC);
                    }
                    else
                    {
                        this._staticDomain = this.ResourceDomain;
                    }
                }

                return this._staticDomain;
            }
        }
        /// <summary>
        /// 数据项
        /// </summary>
        public PageDataItems Items
        {
            get
            {
                if (_dataItems == null)
                {
                    _dataItems = new PageDataItems();
                }
                return _dataItems;
            }
        }



        public static void SaveErrorLog(Exception exception)
        {
            lock (ErrorFilePath)
            {
                HttpRequest req = HttpContext.Current.Request;

                if (!File.Exists(ErrorFilePath))
                {
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "tmp/logs";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.Create(ErrorFilePath).Close();
                }
                HttpContext.Current.Response.Write((File.Exists(ErrorFilePath).ToString()));
                using (FileStream fs = new FileStream(ErrorFilePath, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    StringBuilder sb = new StringBuilder();

                    sb.Append("---------------------------------------------------------------------\r\n")
                        .Append("[错误]：IP:").Append(req.UserHostAddress)
                        .Append("\t时间：").Append(DateTime.Now.ToString())
                        .Append("\r\n[信息]：").Append(exception.Message)
                        .Append("\r\n[路径]：").Append(req.Url.PathAndQuery)
                        .Append("  -> 来源：").Append(req.Headers["referer"] ?? "无")
                        .Append("\r\n[堆栈]：").Append(exception.StackTrace)
                        .Append("\r\n\r\n");

                    sw.Write(sb.ToString());

                    sw.Flush();
                    sw.Dispose();
                    fs.Dispose();
                }
            }
        }


        public bool CheckSiteState()
        {
            if (this.CurrentSite.State == SiteState.Normal)
            {
                return true;
            }
            else if (this.CurrentSite.State == SiteState.Closed)
            {
                this.RenderNotfound();
            }
            else if (this.CurrentSite.State == SiteState.Paused)
            {
                this.Render("<h1 style=\"color:red;text-align:center;font-size:16px;padding:20px\">网站维护中,暂停访问！</h1>");
                //this.RenderNotfound("<h1 style=\"color:red\">网站维护中,暂停访问！</h1>");
            }
            return false;
        }

        /// <summary>
        /// 检查或设置客户端缓存(后台启用缓存并设时间>0)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndSetClientCache()
        {
            if (Settings.Opti_ClientCache && Settings.Opti_ClientCacheSeconds > 0)
            {
                if (CacheUtil.CheckClientCacheExpires(Settings.Opti_ClientCacheSeconds))
                {
                    CacheUtil.SetClientCache(this.Response, Settings.Opti_ClientCacheSeconds);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 检查或设置客户端缓存(自定义时间,单位：秒)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndSetClientCache(int maxAge)
        {
            if (maxAge > 0)
            {
                if (CacheUtil.CheckClientCacheExpires(maxAge))
                {
                    CacheUtil.SetClientCache(this.Response, maxAge);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 呈现
        /// </summary>
        /// <param name="html"></param>
        public void Render(string html)
        {
            HttpResponse response = this.Response;

            response.Write(html);

            //GZip压缩
            if (Settings.Opti_SupportGZip)
            {
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                response.AddHeader("Content-Encoding", "gzip");
            }
            /*
            else
            {
            	response.Filter=new DeflateStream(response.Filter,CompressionMode.Compress);
                response.AddHeader("Content-Encoding", "deflate");
            }*/

            response.AddHeader("X-AspNet-Version", String.Format("Cms.Cms.net v{0}", Cms.Version));
            response.AddHeader("Support-URL", "s.z3q.net/cms");
        }


        /// <summary>
        /// 显示400页面
        /// </summary>
        /// <returns></returns>
        public void RenderNotfound()
        {
            this.RenderNotfound("File not found!", null);
        }

        /// <summary>
        /// 显示400页面
        /// </summary>
        /// <returns></returns>
        public void RenderNotfound(string message, TemplatePageHandler handler)
        {
            Response.StatusCode = 404;

            string html = null;
            try
            {
                TemplatePage tpl = new TemplatePage(String.Format("/{0}/not_found", this.CurrentSite.Tpl));
                if (handler != null)
                {
                    handler(tpl);
                }
                tpl.Render();
                return;
            }
            catch
            {
                html = "File not found!";
            }

            Response.Write(html);
        }

        public string ComposeUrl(string url)
        {
            if (url.StartsWith("/"))
                throw new ArgumentException("URL不能以\"/\"开头!");

            return String.Concat(this.SiteDomain, url);
        }
    }


    /// <summary>
    /// 页面数据项
    /// </summary>
    public class PageDataItems
    {
        public object this[object key]
        {
            get
            {
                return HttpContext.Current.Items[key];
            }
            set
            {
                HttpContext.Current.Items[key] = value;
            }
        }
    }

}