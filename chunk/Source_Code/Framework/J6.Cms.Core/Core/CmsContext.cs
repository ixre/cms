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
using System.Text.RegularExpressions;
using System.Web;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface.Site;
using J6.Cms.Infrastructure;
using J6.DevFw.Framework.Web.Cache;
using J6.DevFw.Template;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Common.Language;
using J6.Cms.Domain.Interface.Enum;
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

        private const String UserLanguageCookieName = "cms_lang";
        private const String UserDeviceCookieName = "cms_device";

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

        /// <summary>
        /// 用户请求使用的设备类型
        /// </summary>
        public DeviceType DeviceType
        {
            get { return this._userDevice; }
        }

        /// <summary>
        /// 用户语言
        /// </summary>
        public Languages UserLanguage { get { return this._userLanguage; } }

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

            if (!Cms.IsInitFinish) return;
            //设置当前站点
            this.CurrentSite = SiteCacheManager.GetSingleOrDefaultSite(this._context.Request.Url); ;
            //是否为虚拟目录运行
            if ((SiteRunType)this.CurrentSite.RunType == SiteRunType.VirtualDirectory)
                this.IsVirtualDirectoryRunning = true;
            this._userLanguage = this.CurrentSite.Language;
            LoadByCookie();
        }

        private void LoadByCookie()
        {
            int lang = this.GetUserLangSet(this._context); //从COOKIE中加载语言
            if (lang != -1)
            {
                this._userLanguage = (Languages)lang;
            }

            this._userDevice = this.GetUserDeviceSet(this._context);
        }

        /// <summary>
        /// 从cookie中获取语言
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private int GetUserLangSet(HttpContext ctx)
        {
            Object s = ctx.Session["user.lang.set"];
            if (s != null) return Convert.ToInt32(s);
            HttpCookie ck = ctx.Request.Cookies.Get(UserLanguageCookieName);
            if (ck != null)
            {
                int i;
                if (int.TryParse(ck.Value, out i) && Enum.IsDefined(typeof(Languages), i))
                {
                    this.SetSessionLangSet(ctx, i);
                    return i;
                }
            }
            this.SetSessionLangSet(ctx, -1);
            return -1;
        }

        /// <summary>
        /// 设置用户的语言
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public bool SetUserLanguage(HttpContext ctx, int lang)
        {
            if (Enum.IsDefined(typeof(Languages), lang))
            {
                this._userLanguage = (Languages)lang; //保存
                if (ctx == null) ctx = this._context;
                HttpCookie ck = ctx.Request.Cookies.Get(UserLanguageCookieName);
                if (ck == null)
                {
                    ck = new HttpCookie(UserLanguageCookieName, lang.ToString());
                }
                else
                {
                    ck.Value = lang.ToString();
                }
                ck.Expires = DateTime.Now.AddHours(24);
                ck.Path = this.SiteAppPath;
                ck.HttpOnly = true;
                ctx.Response.Cookies.Add(ck);
                this.SetSessionLangSet(ctx, lang);
                return true;
            }
            return false;
        }


        public bool SetUserDevice(HttpContext ctx, int device)
        {
            if (Enum.IsDefined(typeof(DeviceType), device))
            {
                this._userDevice = (DeviceType)device; //保存
                if (ctx == null) ctx = this._context;
                HttpCookie ck = ctx.Request.Cookies.Get(UserDeviceCookieName);
                if (ck == null)
                {
                    ck = new HttpCookie(UserDeviceCookieName, device.ToString());
                }
                else
                {
                    ck.Value = device.ToString();
                }
                ck.Expires = DateTime.Now.AddHours(24);
                ck.Path = this.SiteAppPath;
                ck.HttpOnly = true;
                ctx.Response.Cookies.Add(ck);
                this.SetSessionUserDeviceSet(ctx, device);
                return true;
            }
            return false;
        }

        private void SetSessionLangSet(HttpContext ctx, int lang)
        {
            ctx.Session["user.lang.set"] = lang;
        }

        /// <summary>
        /// 当前站点
        /// </summary>
        public SiteDto CurrentSite { get; private set; }

        public static bool IsMobileAgent(HttpRequest req)
        {
            var u = req.ServerVariables["HTTP_USER_AGENT"];
            var b =
                new Regex(
                    @"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od|ad)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var v =
                new Regex(
                    @"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return b.IsMatch(u) || v.IsMatch(u.Substring(0, 4));
        }

        private DeviceType GetUserDeviceSet(HttpContext ctx)
        {
            Object s = ctx.Session["user.device.set"];
            if (s != null) return (DeviceType)Convert.ToInt32(s);

            HttpCookie ck = ctx.Request.Cookies.Get(UserDeviceCookieName);
            if (ck != null)
            {
                int i;
                int.TryParse(ck.Value, out i);
                if (Enum.IsDefined(typeof(DeviceType), i))
                {
                    this.SetSessionUserDeviceSet(ctx, i);
                    return (DeviceType)i;
                }
            }
            if (IsMobileAgent(ctx.Request))
            {
                this.SetSessionUserDeviceSet(ctx, (int)DeviceType.Mobi);
                return DeviceType.Mobi;
            }
            this.SetSessionUserDeviceSet(ctx, (int)DeviceType.Mobi);
            return DeviceType.Standard;
        }

        private void SetSessionUserDeviceSet(HttpContext ctx, int deviceType)
        {
            ctx.Session["user.device.set"] = deviceType;
        }

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
        private Languages _userLanguage;
        private DeviceType _userDevice;


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