using System;
using System.IO;
using System.Text;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Web.Cache;
using JR.Stand.Core.PluginKernel.Web;
using JR.Stand.Core.Web.Cache;
using JR.Stand.Core.Web.Cache.Component;
using JR.Stand.Core.Web.Plugin;

namespace JR.Stand.Core.Web
{
    /// <summary>
    /// 应用程序上下文
    /// </summary>
    public class WebCtx
    {

        /// <summary>
        /// 错误日志文件
        /// </summary>
        public static string ErrorFilePath;
        
        public static ICompatibleHttpContext HttpCtx => HttpHosting.Context;


        static WebCtx()
        {
            ErrorFilePath = EnvUtil.GetBaseDirectory() + "tmp/logs/error.log";
        }

        private PageDataItems _dataItems;
        private string _host;
        private string _siteAppPath;
        private static IPluginApp<HttpContextImpl> _extends;
        private string _domain;
        private string _appPath;
        private readonly ICompatibleHttpContext _context;

        private WebCtx(ICompatibleHttpContext ctx)
        {
            this._context = ctx;
        }

        /// <summary>
        /// 运行时发生
        /// </summary>
        public static event FwHandler OnBeginRequest;


        /// <summary>
        /// 当前的Host,包含端口，如：www.ops.cc:8080
        /// </summary>
        public string Host
        {
            get
            {
                if (this._host == null)
                {
                    // 通过Header获取Host
                    this._host = this._context.Request.GetHeader("Host");
                    // 通过ASP.NET方式获取Host
                    if (String.IsNullOrEmpty(this._host))
                    {
                        this._host = this._context.Request.GetHost();
                    }
                }
                return this._host;
            }
        }

        public static WebCtx Current
        {
            get
            {
                const string key = "_$web_ctx_instance";
                var context = HttpHosting.Context;
                if (!context.TryGetItem<WebCtx>(key, out var ctx))
                {
                    ctx = new WebCtx(context);
                    context.SaveItem(key,ctx);
                    
                }
                return ctx;
            }
        }

        /// <summary>
        /// 系统应用程序目录
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                if (this._appPath == null)
                {
                    //var s = Environment.GetEnvironmentVariable("ASPNETCORE_APPL_PATH");
                   // this._appPath = this._context.Raw.Request.PathBase.Value;
                   this._appPath = this._context.Request.GetApplicationPath();
                }

                return this._appPath;
            }
        }

        /// <summary>
        /// 插件扩展
        /// </summary>
        public IPluginApp<HttpContextImpl> Plugin
        {
            get
            {
                if (_extends == null)
                {
                    var handler = new WebPluginHandleProxy<HttpContextImpl>();
                    _extends = new WebPluginApp<HttpContextImpl>(handler);
                }
                return _extends;
            }
        }


        public ICache Cache
        {
            get { return CacheFactory.Sington; }
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
                    _dataItems = new PageDataItems(this._context);
                }
                return _dataItems;
            }
        }
        public string Domain
        {
            get
            {
                if (this._domain == null)
                {
                    String appPath = this.ApplicationPath;
                    var schema = this._context.Request.GetProto();
                    var host = this._context.Request.GetHost();
                    this._domain = $"{schema}://{host}{(appPath == "/" ? "" : appPath)}";
                }

                return this._domain;
            }
            set { this._domain = value; }
        }

        public string UserIpAddress
        {
            get
            {
              //return  this._context.Raw.Connection.RemoteIpAddress.ToString();
              return this._context.RemoteAddress();
            }
        }
        public static void SaveErrorLog(Exception exception)
        {
            lock (ErrorFilePath)
            {
                ICompatibleRequest req = HttpCtx.Request;
                var path = req.GetPath();
                var query = req.GetQueryString();
                var PathAndQuery = path + query;
                var referer = req.GetHeader("Referer");;
                if (!File.Exists(ErrorFilePath))
                {
                    string dir = EnvUtil.GetBaseDirectory() + "tmp/logs";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.Create(ErrorFilePath).Close();
                }

                HttpCtx.Response.WriteAsync((File.Exists(ErrorFilePath).ToString()));
                using (FileStream fs = new FileStream(ErrorFilePath, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    StringBuilder sb = new StringBuilder();

                    sb.Append("---------------------------------------------------------------------\r\n")
                        .Append("[错误]：IP:").Append(HttpCtx.RemoteAddress())
                        .Append("\t时间：").Append(DateTime.Now.ToString())
                        .Append("\r\n[信息]：").Append(exception.Message)
                        .Append("\r\n[路径]：").Append(PathAndQuery)
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


        //        public bool CheckSiteState()
        //        {
        //            if (this.CurrentSite.State == SiteState.Normal)
        //            {
        //                return true;
        //            }
        //            else if (this.CurrentSite.State == SiteState.Closed)
        //            {
        //                this.RenderNotfound();
        //            }
        //            else if (this.CurrentSite.State == SiteState.Paused)
        //            {
        //                this.Render("<h1 style=\"color:red;text-align:center;font-size:16px;padding:20px\">网站维护中,暂停访问！</h1>");
        //                //this.RenderNotfound("<h1 style=\"color:red\">网站维护中,暂停访问！</h1>");
        //            }
        //            return false;
        //        }

        /// <summary>
        /// 检查或设置客户端缓存(后台启用缓存并设时间>0)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndSetClientCache()
        {
            if (WebConf.Opti_ClientCache && WebConf.Opti_ClientCacheSeconds > 0)
            {
                if (CacheUtil.CheckClientCacheExpires(WebConf.Opti_ClientCacheSeconds))
                {
                    CacheUtil.SetClientCache(this._context.Response, WebConf.Opti_ClientCacheSeconds);
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
                    CacheUtil.SetClientCache(this._context.Response, maxAge);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static void Restart()
        {
            HttpCtx.Hosting.Stop();
           // var app = HttpCtx.Raw.RequestServices.GetService<IApplicationLifetime>();
           // app.StopApplication();
        }
    }
}