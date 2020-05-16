//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : Home.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 20:13:52
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
// Modify:
//  2013-01-06  11:07   newmin [+]: add setting
//

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.Utility;
using JR.Cms.Web.Portal.Template.Model;
using JR.Cms.Web.Util;
using JR.Stand.Core.Framework.Extensions;
using JR.Stand.Core.Framework.Web.UI;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Hosting;

namespace JR.Cms.Web.Manager.Handle
{
    public class SystemHandler : BasePage
    {
        private static FontFamily font = null;

        internal string GetPath()
        {
            //如请求路径为/opsite/admin
            //取出admin供IE CSS使用
            string query = Request.GetPath();
            query = query.Substring(query.LastIndexOf('/') + 1);
            return query;
        }

        public void NotFound()
        {
            Response.WriteAsync(
                "<div style=\"margin-top:50px;text-align:center\"><span style=\"color:red\">后续推出....</span></div>");
        }

        /// <summary>
        /// 管理首页
        /// </summary>
        //[StaticCache]
        public string Index()
        {
            //重定向管理页
            string path = Request.GetPath();
            if (Request.GetQueryString() == "" || path.EndsWith("/"))
            {
                if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
                return string.Format("<script>window.parent.location.replace('{0}?ver={1}')</script>", path,
                    Cms.Version);
            }

            var pageTemplate = ResourceMap.GetPageContent(ManagementPage.Index);
            var currentSite = CurrentSite;

            var localJsRef = $"/{CmsVariables.SITE_CONF_PRE_PATH}{currentSite.SiteId.ToString()}/mui_override.js";
            if (!File.Exists(Cms.PhysicPath + localJsRef)) localJsRef = "";

            ViewData["version"] = Cms.Version;
            ViewData["logo"] = Cms.BuildOEM.Get(BuildSet.SystemLogo);
            ViewData["entryFrameUrl"] = Cms.BuildOEM.Get(BuildSet.EntryFrameUrl);
            ViewData["path"] = GetPath();
            ViewData["admin_path"] = Settings.SYS_ADMIN_TAG;
            ViewData["site_id"] = currentSite.SiteId;
            ViewData["local_js"] = localJsRef;
            ViewData["ui_css_path"] = ResourceMap.GetPageUrl(ManagementPage.UI_Index_Css);
            ViewData["page_css_path"] = ResourceMap.GetPageUrl(ManagementPage.Css_Style);
            return RequireTemplate(pageTemplate);
        }

        /// <summary>
        /// 设置站点
        /// </summary>
        public void SelectSite()
        {
            //设置站点
            var user = UserState.Administrator.Current;

            var siteId = 0;
            int.TryParse(Request.Query("site_id"), out siteId);
            var site = SiteCacheManager.GetSite(siteId);
            if (site.SiteId > 0)
            {
                if (user.IsMaster)
                {
                    CurrentSite = site;
                    RenderSuccess();
                    return;
                }

                foreach (var siteId2 in user.Roles.GetSiteIds())
                    if (siteId == siteId2)
                    {
                        CurrentSite = site;
                        RenderSuccess();
                        return;
                    }
            }

            RenderError("没有权限管理站点");
        }


        /// <summary>
        /// 服务器信息页面
        /// </summary>
        public void Summary()
        {
            //var env = HttpHosting.Context.Hosting.GetService<IHostingEnvironment>();
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Server_Summary), new
            {
                path = GetPath(),
                shortServer = "//" + Server.ShortUrlServer,
                soft_ver = Cms.Version,
                sys_alias = Settings.LICENSE_NAME, // + "(KEY:" + Settings.SYS_KEY + ")",
                //server_name = env.EnvironmentName,
                server_os = Environment.OSVersion.VersionString,
                server_local = CultureInfo.InstalledUICulture.EnglishName,
                server_ip = WebCtx.Current.UserIpAddress,
                //server_host = Request.ServerVariables["SERVER_NAME"],
                //server_iis = Request.ServerVariables["Server_SoftWare"],
                server_netver =
                    Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build +
                    "." +
                    Environment.Version.Revision,
                server_https = Request.GetEncodedUrl().IndexOf("https://", StringComparison.Ordinal) != -1 ? "是" : "否",
                //server_port = Request.ServerVariables["Server_Port"],
                server_hour = string.Format("{0}小时", (Environment.TickCount / 0x3e8 / 3600).ToString()),
                server_time = DateTime.Now.ToString(),
                server_cpu = $"{Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")},{Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS")}核",
                server_meory = Environment.WorkingSet / 1024 / 1024 + "M",
                server_net_meory =
                    ((double) Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + "M",
                //person_os = this.GetOSNameByUserAgent(base.Request.Headers.TryGetValue(UserAgent)),
                //person_ip = base.Request.UserHostAddress,
                //person_soft = base.Request.Browser.Browser,
                //person_softver = base.Request.Browser.Version,
                //person_cookie = base.Request.Browser.Cookies ? "支持" : "<span style=\"color:Red\">不支持</span>",
                // person_java = base.Request.Browser.EcmaScriptVersion.Major >= 1 ? "支持" : "<span style=\"color:Red\">不支持</span>",
            });
        }

        /// <summary>
        /// 主要首页
        /// </summary>
        public void IndexMain()
        {
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.IndexMain), null);
        }

        /// <summary>
        /// 加载登陆信息
        /// </summary>
        public void LoadSession()
        {
            var url = "";
            if (Request.TryGetHeader("Referer", out var referrer)) url = new Uri(referrer).PathAndQuery;

            var usr = UserState.Administrator.Current;
            if (usr != null)
                Response.Redirect(url,false);
            else
                Response.WriteAsync("<script>location.href='/" +
                                    Settings.SYS_ADMIN_TAG +
                                    "?action=login&return_url=" +
                                    url + "'</script>");
        }

        private string GetOSNameByUserAgent(string userAgent)
        {
            if (!string.IsNullOrEmpty(userAgent))
            {
                if (userAgent.Contains("NT 6.0"))
                    return "Windows Vista/Server 2008";
                else if (userAgent.Contains("NT 5.2"))
                    return "Windows Server 2003";
                else if (userAgent.Contains("NT 5.1"))
                    return "Windows XP";
                else if (userAgent.Contains("NT 5"))
                    return "Windows 2000";
                else if (userAgent.Contains("NT 4"))
                    return "Windows NT4";
                else if (userAgent.Contains("Me"))
                    return "Windows Me";
                else if (userAgent.Contains("98"))
                    return "Windows 98";
                else if (userAgent.Contains("95"))
                    return "Windows 95";
                else if (userAgent.Contains("Mac"))
                    return "Mac";
                else if (userAgent.Contains("Unix"))
                    return "UNIX";
                else if (userAgent.Contains("Linux"))
                    return "Linux";
                else if (userAgent.Contains("SunOS"))
                    return "SunOS";
                else if (userAgent.Contains("CentOS")) return "CentOS";
            }

            return "未知设备";
        }


        #region 登录

        /// <summary>
        /// 登录验证码
        /// </summary>
        public void VerifyCode()
        {
            string word = null;
            var v = new VerifyCodeGenerator();
            Response.ContentType("Image/Jpeg");
            var f = GetCodeFontFamily();
            var font = new Font(f, 16);
            v.AllowRepeat = false;
            var code = v.GenerateVerifyWords(4, VerifyWordOptions.Letter);
            Response.WriteAsync(v.GraphicDrawImage(code, !true, font, 50));
            VerifyCodeManager.AddWord(new string(code));
        }

        private FontFamily GetCodeFontFamily()
        {
            if (font == null)
                font = Stand.Core.Utils.Util.LoadFontFamily(Cms.PhysicPath + CmsVariables.FRAMEWORK_ASSETS_PATH +
                                                            "fonts/georgia.ttf");
            return font;
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        public void Login()
        {
            RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.Login),
                new
                {
                    path = GetPath(),
                    linux = Cms.RunAtMono ? "true" : "false",
                    version = Cms.Version,
                    year = DateTime.Now.Year.ToString()
                });
        }

        /// <summary>
        /// 登录提交
        /// </summary>
        public void Login_POST()
        {
            string username = Request.Form("uid")[0].Trim(),
                password = Request.Form("pwd")[0].Trim(),
                verifyCode = Request.Form("code")[0].Trim();

            if (verifyCode != "" || !VerifyCodeManager.Compare(verifyCode))
            {
                RenderError("验证码不正确");
                return;
            }
            if (password.Length != 32) password = password.Md5();
            var tag = UserState.Administrator.Login(username, password, 3600 * 120);
            if (tag == -1)
                RenderError("账户或密码不正确!");
            else if (tag == -2)
                RenderError("账号已被停用,请联系管理员!");
            else
                RenderSuccess();
            //保存登陆信息

            //设置站点
            //                UserBll usr = CmsLogic.UserBll.GetUser(username, password);
            //
            //                if (usr != null)
            //                {
            //                    CmsLogic.UserBll.UpdateUserLastLoginDate(username);
            //
            //                    if (!usr.Enabled)
            //                    {
            //                        base.RenderError("账号已被停用,请联系管理员!");
            //                    }
            //                    else
            //                    {
            //                        //保存登陆信息
            //                        bool result = UserState.Administrator.Login(username, password, 3600 * 120);
            //
            //                        //验证站点信息
            //                        if (usr.SiteId > 0)
            //                        {
            //                            SiteDto site = SiteCacheManager.GetSite(usr.SiteId);
            //                            if (!(site.SiteId > 0))
            //                            {
            //                                base.RenderError("账号已被停用,请联系管理员!");
            //                                return;
            //                            }
            //                            base.CurrentSite = site;
            //                        }
            //                        base.RenderSuccess();
            //                    }
            //}
            //else
            // {
            //   base.RenderError("账户或密码不正确!");
            //}
        }

        #endregion


        /// <summary>
        /// 退出登录
        /// </summary>
        public void Logout()
        {
            UserState.Administrator.Exit();

            string path = Request.GetPath();
            Response.Redirect($"{path}?module=system&action=login", false);
        }


        /// <summary>
        /// 系统设置
        /// </summary>
        public void AppConfig()
        {
            object data = new
            {
                license_name = Settings.LICENSE_NAME,
                license_key = Settings.LICENSE_KEY,
                site_domain = Request.GetHost(),
                sys_autowww = Settings.SYS_AUTOWWW,
                sys_admin_tag = Settings.SYS_ADMIN_TAG,
                sys_encode_conf = Settings.SYS_ENCODE_CONF_FILE,
                db_prefix = Settings.DB_PREFIX,
                opti_index_cache_seconds = (Settings.Opti_IndexCacheSeconds / 60).ToString(),
                opti_client_cache_seconds = (Settings.Opti_ClientCacheSeconds / 60).ToString(),
                opti_gc_collect_interval = Settings.opti_gc_collect_interval / 3600000,
                opti_debug = Settings.OPTI_DEBUG_MODE,
                sys_use_upload_raw_path = Settings.SYS_USE_UPLOAD_RAW_NAME,
                server_static = Settings.SERVER_STATIC,
                server_upgrade = Settings.SERVER_UPGRADE,
                server_static_enabled = Settings.SERVER_STATIC_ENABLED,
                tpl_full_url_path = Settings.TPL_FULL_URL_PATH,
                tpl_use_compress = Settings.TPL_USE_COMPRESS
            };
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.App_Config), new
            {
                json = JsonSerializer.Serialize(data)
            });
        }

        public void AppConfig_POST()
        {
            string item = Request.Form("item");

            var req = Request;

            switch (item)
            {
                case "1":
                    Settings.LICENSE_KEY = req.Form("license_key");
                    Settings.LICENSE_NAME = req.Form("license_name");
                    Settings.SYS_AUTOWWW = req.Form("sys_autowww") == "1";
                    Settings.SYS_ENCODE_CONF_FILE = req.Form("sys_encode_conf") == "on";
                    Settings.SQL_PROFILE_TRACE = req.Form("sql_profile_trace") == "on";
                    Settings.DB_PREFIX = req.Form("db_prefix");
                    Settings.TPL_USE_COMPRESS = req.Form("tpl_use_compress") == "on";
                    Settings.TPL_FULL_URL_PATH = req.Form("tpl_full_url_path") == "on";

                    if (!string.IsNullOrEmpty(req.Form("sys_admin_tag")))
                        Settings.SYS_ADMIN_TAG = req.Form("sys_admin_tag");

                    //缓存
                    Settings.Opti_IndexCacheSeconds = int.Parse(req.Form("opti_index_cache_seconds")) * 60;
                    Settings.Opti_ClientCacheSeconds = int.Parse(req.Form("opti_client_cache_seconds")) * 60;
                    Settings.opti_gc_collect_interval = int.Parse(req.Form("opti_gc_collect_interval")) * 3600000;

                    //是否调试
                    Settings.OPTI_DEBUG_MODE = req.Form("opti_debug") == "on";
                    Settings.SYS_USE_UPLOAD_RAW_NAME = req.Form("sys_use_upload_raw_path") == "on";
                    Settings.SERVER_STATIC_ENABLED = req.Form("server_static_enabled") == "on";

                    if (!string.IsNullOrEmpty(req.Form("server_static")))
                        Settings.SERVER_STATIC = req.Form("server_static");

                    string upgradeUrl = req.Form("server_upgrade");
                    if (!string.IsNullOrEmpty(upgradeUrl))
                    {
                        if (upgradeUrl.IndexOf("http", StringComparison.Ordinal) != 0)
                        {
                            RenderSuccess("升级服务器必须以http://开头!");
                            return;
                        }

                        Settings.SERVER_UPGRADE = req.Form("server_upgrade");
                    }

                    Configuration.BeginWrite();
                    Configuration.UpdateByPrefix("tpl");
                    Configuration.UpdateByPrefix("opti");
                    Configuration.UpdateByPrefix("sys");
                    Configuration.UpdateByPrefix("db");
                    Configuration.EndWrite();
                    Cms.Reload();
                    break;
            }


            Configuration.Renew();

            RenderSuccess("修改成功!");
        }

        public string Clear()
        {
            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Clear_Page));
        }

       

        public string Clear_POST()
        {
            switch (Request.Form("for"))
            {
                case "reboot":
                    Cms.Reload();
                    return ReturnSuccess("网站正在重启...");
                case "cache":
                    PageVariable.ResetBuilt();
                    Cms.Cache.Reset(null);
                    Cms.Template.Reload();
                    return ReturnSuccess("缓存清理成功");
            }

            return ReturnSuccess();
        }

        public string Locales()
        {
            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Locales));
        }

        public string GetLocaleJson_POST()
        {
            var path = "";
            string type = Request.Form("type");
            switch (type)
            {
                case "system":
                    path = Cms.PhysicPath + CmsVariables.FRAMEWORK_PATH + "locale/locale.db";
                    break;
                case "custom":
                    path = Cms.PhysicPath + CmsVariables.SITE_LOCALE_PATH;
                    break;
                default:
                    return ReturnError("error type");
            }

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                return "[]";
            }

            var rd = new StreamReader(path, Encoding.UTF8);
            var json = rd.ReadToEnd();
            rd.Close();
            return json;
        }

        public string CreateNewLocaleKey_POST()
        {
            string key = Request.Form("key");
            try
            {
                Locale.AddKey(key);
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            return ReturnSuccess("");
        }

        public string DelLocaleKey_POST()
        {
            string key = Request.Form("key");
            try
            {
                Locale.DelKey(key);
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            return ReturnSuccess("");
        }

        public string SaveLocale_POST()
        {
            try
            {
                var json = Locale.SaveByPostForm(Request);
                if (json != null) Cms.Language.GetPackage().LoadFromJson(json);
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            return ReturnSuccess("");
        }
    }
}