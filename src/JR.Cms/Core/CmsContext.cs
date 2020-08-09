/**
 * Copyright (C) 2007-2015 TO2.NET,All rights reserved.
 * Get more infromation of this software,please visit site http://to2.net/jr-cms
 * 
 * name : CmsContext.cs
 * author : newmin (new.min@msn.com)
 * date : 2014/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core;
using JR.Stand.Core.Framework.Web.Cache;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Web;

namespace JR.Cms.Core
{
    /// <summary>
    /// 应用程序配置
    /// </summary>
    public class CmsContext
    {
        private static readonly Regex MobileDevRegexp = new Regex("android|iPhone|blackberry|nokia|MicroMessager|WindowsPhone",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// 错误日志文件
        /// </summary>
        private static readonly string ErrorFilePath;

        private const string UserLanguageCookieName = "_cms_lang";
        private const string UserDeviceCookieName = "_cms_device";

        /// <summary>
        /// 是否作为虚拟目录运行
        /// </summary>
        private readonly bool _isVirtualDirectoryRunning;

        /// <summary>
        /// 请求上下文
        /// </summary>
        private readonly ICompatibleHttpContext _context;

        static CmsContext()
        {
            ErrorFilePath = EnvUtil.GetBaseDirectory()+ "/tmp/logs/error.log";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpCtx"></param>
        public CmsContext(ICompatibleHttpContext httpCtx)
        {
            _context = httpCtx;
            if (!Cms.IsInitFinish) return;
            //设置当前站点
            var request = _context.Request;
            var path = request.GetPath();

            string appPath = "";
            if (path != "/")
            {
                appPath = path.Substring(1);
                if (appPath.EndsWith("/")) appPath = appPath.Substring(0, appPath.Length - 1);
            }
            CurrentSite = SiteCacheManager.GetSingleOrDefaultSite(request.GetHost(), appPath);
            //是否为虚拟目录运行
            if ((SiteRunType) CurrentSite.RunType == SiteRunType.VirtualDirectory)
                _isVirtualDirectoryRunning = true;
            this._containCookie = _context.Request.GetHeader("Cookie") != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public ICompatibleHttpContext HttpContext => _context;


        /// <summary>
        /// 上下文来源
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 用户请求使用的设备类型
        /// </summary>
        public DeviceType DeviceType
        {
            get
            {
                if (this._userDevice == null)
                {
                    this._userDevice = GetUserDeviceSet();
                }
                return this._userDevice?? DeviceType.Standard;
            }
        }

        /// <summary>
        /// 用户语言
        /// </summary>
        public Languages UserLanguage
        {
            get
            {
                if (this._userLanguage == Languages.Unknown)
                {
                    if (this._containCookie)
                    {
                        this._userLanguage = this.GetUserLangSetFromCookie();
                        if (this._userLanguage == Languages.Unknown)
                        {
                            this._userLanguage = CurrentSite.Language;
                            //SetSessionLangSet((int)this._userLanguage);
                        }
                    }
                    this._userLanguage = CurrentSite.Language;
                }
                return this._userLanguage;
            }
        }

        /// <summary>
        /// 从cookie中获取语言
        /// </summary>
        /// <returns></returns>
        private Languages GetUserLangSetFromCookie()
        {
            var s = this._context.Request.GetHeader("Cookie");
                
            // var s = _context.Session.GetInt32("user.lang.set");
            // if (s > 0) return (Languages)s;
            
            // Cookie中存在key, 且是语言枚举的成员.　反之lang = -1
            _context.Request.TryGetCookie(UserLanguageCookieName,out var ck);
            if (!String.IsNullOrEmpty(ck))
            {
                if (Enum.TryParse<Languages>(ck, out var lang))return lang;
            }
            //SetSessionLangSet(lang);
            return Languages.Unknown;
        }

        /// <summary>
        /// 设置用户的语言
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public bool SetUserLanguage(int lang)
        {
            if (Enum.IsDefined(typeof(Languages), lang))
            {
                var sameLang = lang == (int) this.CurrentSite.Language;
                _userLanguage = (Languages) lang; //保存
                var opt = new HttpCookieOptions
                {
                    Expires = DateTime.Now.AddHours((sameLang ? -24 : 24) * 365),
                    Path = SiteAppPath,
                    HttpOnly = true
                };
                _context.Response.AppendCookie(UserLanguageCookieName, lang.ToString(), opt);

                //SetSessionLangSet(lang);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool SetUserDevice(int device)
        {
            if (Enum.IsDefined(typeof(DeviceType), device))
            {
                var same = device == (int) this._userDevice;
                // _userDevice = (DeviceType) device; //保存
                var opt = new HttpCookieOptions
                {
                    Expires = DateTime.Now.AddHours((same?-24:24)*365),
                    Path = SiteAppPath,
                    HttpOnly = true
                };
                _context.Response.AppendCookie(UserDeviceCookieName, device.ToString(), opt);
                //SetSessionUserDeviceSet(device);
                return true;
            }

            return false;
        }

        // private void SetSessionLangSet(int lang)
        // {
        //     _context.Session.SetInt32("user.lang.set", lang);
        // }

        /// <summary>
        /// 当前站点
        /// </summary>
        public SiteDto CurrentSite { get; }

        private bool IsMobileAgent()
        {
            if (_context.Request.TryGetHeader("HTTP_USER_AGENT", out var u))
            {
                return MobileDevRegexp.IsMatch(u);
            }
            return false;
        }

        private DeviceType GetUserDeviceSet()
        {
            
            // var s = ctx.Session.GetString("user.device.set");
            // if (!String.IsNullOrEmpty(s))
            // {
            //     return (DeviceType) Convert.ToInt32(s);
            // }
            if (this._containCookie)
            {
                this._context.Request.TryGetCookie(UserDeviceCookieName, out var ck);
                if (!String.IsNullOrEmpty(ck))
                {
                    if (Enum.TryParse<DeviceType>(ck, out var device)) return device;
                }
            }
            //如果包含手机的域名或agent
            var host = this._context.Request.GetHost();
            if (host.StartsWith("m.") || host.StartsWith("wap.") || IsMobileAgent()) return DeviceType.Mobile;
            return DeviceType.Standard;
        }

        private void SetSessionUserDeviceSet(int deviceType)
        {
            _context.Session.SetInt32("user.device.set", deviceType);
        }
        
        /// <summary>
        /// 站点域名
        /// </summary>
        private string _siteDomain;

        private PageDataItems _dataItems;
        private string _staticDomain;
        private string _resourceDomain;
        private string _siteAppPath;
        private Languages _userLanguage;
        private DeviceType? _userDevice;
        private string _proto;
        private readonly bool _containCookie;

        /// <summary>
        /// 系统应用程序目录
        /// </summary>
        public string ApplicationPath => HttpApp.GetApplicationPath();

        /// <summary>
        /// 站点应用程序目录
        /// </summary>
        public string SiteAppPath
        {
            get
            {
                if (this._siteAppPath == null)
                {
                    this._siteAppPath = _isVirtualDirectoryRunning ? "/" + CurrentSite.AppPath : "/";
                }

                return _siteAppPath;
            }
        }


        /// <summary>
        /// 域名
        /// </summary>
        public string SiteDomain
        {
            get
            {
                if (_siteDomain == null)
                {
                    if (SiteAppPath != "/")
                    {
                        _siteDomain = WebCtx.Current.Domain + SiteAppPath;
                    }
                    else
                    {
                        _siteDomain = WebCtx.Current.Domain;
                    }
                }

                return _siteDomain;
            }
        }

        /// <summary>
        /// 资源域
        /// </summary>
        public string ResourceDomain => _resourceDomain ?? (_resourceDomain = WebCtx.Current.Domain);

        /// <summary>
        /// 静态资源域
        /// </summary>
        public string StaticDomain
        {
            get
            {
                if (_staticDomain == null)
                {
                    if (Settings.SERVER_STATIC_ENABLED && Settings.SERVER_STATIC.Length != 0)
                    {
                        var proto = this.GetProto();
                        // this._staticDomain = String.Concat("http://", Settings.SERVER_STATIC);
                        _staticDomain = string.Concat(proto,"://", Settings.SERVER_STATIC);
                    }
                    else
                    {
                        _staticDomain = this.ResourceDomain;
                    }
                }

                return _staticDomain;
            }
        }

        private object GetProto()
        {
            if (this._proto == null)
            {
                this._proto = this._context.Request.GetProto();
            }

            return this._proto;
        }

        /// <summary>
        /// 数据项
        /// </summary>
        public PageDataItems Items
        {
            get
            {
                if (_dataItems == null) _dataItems = new PageDataItems(this._context);
                return _dataItems;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public static void SaveErrorLog(Exception exception)
        {
            lock (ErrorFilePath)
            {
                var req = HttpHosting.Context.Request;
                var path = req.GetPath();
                var query = req.GetQueryString();
                var pathAndQuery = path + query;
                 req.TryGetHeader("Referer",out var referer);
                 if (!File.Exists(ErrorFilePath))
                {
                    var dir = EnvUtil.GetBaseDirectory() + "/tmp/logs";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.Create(ErrorFilePath).Close();
                }

                HttpHosting.Context.Response.WriteAsync(File.Exists(ErrorFilePath).ToString());
                using (var fs = new FileStream(ErrorFilePath, FileMode.Append, FileAccess.Write))
                {
                    var sw = new StreamWriter(fs);
                    var sb = new StringBuilder();

                    sb.Append("---------------------------------------------------------------------\r\n")
                        .Append("[错误]：IP:")
                        .Append(HttpHosting.Context.RemoteAddress())
                        .Append("\t时间：").Append(DateTime.Now.ToString())
                        .Append("\r\n[信息]：").Append(exception.Message)
                        .Append("\r\n[路径]：").Append(pathAndQuery)
                        .Append("  -> 来源：").Append(referer)
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
            if (CurrentSite.State == SiteState.Normal) return true;
            if (CurrentSite.State == SiteState.Closed)
            {
                RenderNotfound();
                return false;
            }
            if (CurrentSite.State == SiteState.Paused)
            {
                this._context.Response.WriteAsync(
                    $"<body style=\"background:#FBFBFB\"><center><br /><br /><h2 style=\"font-weight:300\">" +
                    $"站点维护中,如需访问请联系站点管理员</h2>" +
                    $"<h6 style=\"color:#666;font-weight:300\">JRCms v{Cms.Version}</h6></center></body>");
                return false;
                //this.RenderNotfound("<h1 style=\"color:red\">网站维护中,暂停访问！</h1>");
            }
            this._context.Response.WriteAsync(
                $"<body style=\"background:#FBFBFB\"><center><br /><br /><h2 style=\"font-weight:300\">" +
                $"未找到站点</h2>" +
                $"<h6 style=\"color:#666;font-weight:300\">JRCms v{Cms.Version}</h6></center></body>");
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
                    CacheUtil.SetClientCache(this._context.Response, Settings.Opti_ClientCacheSeconds);
                else
                    return false;
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
                    CacheUtil.SetClientCache(this._context.Response, maxAge);
                else
                    return false;
            }

            return true;
        }




        /// <summary>
        /// 显示400页面
        /// </summary>
        /// <returns></returns>
        public void RenderNotfound()
        {
            RenderNotfound("File not found!", null);
        }

        /// <summary>
        /// 显示400页面
        /// </summary>
        /// <returns></returns>
        public void RenderNotfound(string message, TemplatePageHandler handler)
        {
            string html = null;
            try
            {
                var pageName = $"/{CurrentSite.Tpl}/not_found";
                var tpl = Cms.Template.GetTemplate(pageName);
                handler?.Invoke(tpl);
                html = tpl.Compile();
            }
            catch
            {
                html = "File not found!";
            }
            this._context.Response.StatusCode(404);
            this._context.Response.WriteAsync(html);
        }

        public string ComposeUrl(string url)
        {
            if (url.StartsWith("/")) throw new ArgumentException("URL不能以\"/\"开头!");
            return string.Concat(SiteDomain, url);
        }
    }


    /// <summary>
    /// 页面数据项
    /// </summary>
    public class PageDataItems
    {
        private ICompatibleHttpContext _context;

        internal PageDataItems(ICompatibleHttpContext context)
        {
            this._context = context;
        }
        public object this[string key]
        {
            get => this._context.TryGetItem<object>(key, out var v) ? v : null;
            set => this._context.SaveItem(key,value);
        }
    }
}