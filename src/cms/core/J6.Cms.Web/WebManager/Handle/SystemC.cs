//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : Home.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 20:13:52
// Description :
//
// Get infromation of this software,please visit our site http://k3f.net/cms
// Modify:
//  2013-01-06  11:07   newmin [+]: add setting
//

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Core;
using J6.Cms.DataTransfer;
using J6.Cms.Utility;
using J6.Cms.WebManager;
using J6.DevFw.Framework.Web.UI;

namespace J6.Cms.Web.WebManager.Handle
{
    public class SystemC : BasePage
    {

        internal string GetPath()
        {
            //如请求路径为/opsite/admin
            //取出admin供IE CSS使用
            string query = HttpContext.Current.Request.Path;
            query = query.Substring(query.LastIndexOf('/') + 1);
            return query;
        }

        public void NotFound_GET()
        {
            HttpContext.Current.Response.Write(
                "<div style=\"margin-top:50px;text-align:center\"><span style=\"color:red\">后续推出....</span></div>");
        }

        /// <summary>
        /// 管理首页
        /// </summary>
        //[StaticCache]
        public string Index_GET()
        {
            //重定向管理页
            string path = base.Request.Path;
            if (base.Request.Url.Query == "" || path.EndsWith("/"))
            {
                if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
                return String.Format("<script>window.parent.location.replace('{0}?ver={1}')</script>", path, Cms.Version);
            }

            string pageTemplate = ResourceMap.GetPageContent(ManagementPage.Index);
            SiteDto currentSite = this.CurrentSite;

            String localJsRef = String.Format("/{0}{1}/mui_override.js", CmsVariables.SITE_CONF_PRE_PATH,
                currentSite.SiteId.ToString());
            if (!File.Exists(Cms.PyhicPath + localJsRef))
            {
                localJsRef = "";

            }

            ViewData["version"] = Cms.Version;
            ViewData["logo"] = Cms.BuildSet.Get(BuildSet.SystemLogo);
            ViewData["entryFrameUrl"] = Cms.BuildSet.Get(BuildSet.EntryFrameUrl);
            ViewData["path"] = GetPath();
            ViewData["admin_path"] = Settings.SYS_ADMIN_TAG;
            ViewData["site_id"] = currentSite.SiteId;
            ViewData["local_js"] = localJsRef;
            ViewData["ui_css_path"] = ResourceMap.GetPageUrl(ManagementPage.UI_Index_Css);

            return base.RequireTemplate(pageTemplate);
        }

        /// <summary>
        /// 设置站点
        /// </summary>
        public void SelectSite_GET()
        {
            //设置站点
            UserDto user = UserState.Administrator.Current;

            int siteId = 0;
            int.TryParse(base.Request["site_id"], out siteId);
            var site = SiteCacheManager.GetSite(siteId);
            if (site.SiteId > 0)
            {
                if (user.IsMaster)
                {
                    base.CurrentSite = site;
                    base.RenderSuccess();
                    return;
                }

                foreach (int siteId2 in user.Roles.GetSiteIds())
                {
                    if (siteId == siteId2)
                    {
                        base.CurrentSite = site;
                        base.RenderSuccess();
                        return;
                    }
                }
            }

            base.RenderError("没有权限管理站点");
        }


        /// <summary>
        /// 服务器信息页面
        /// </summary>
        public void Summary_GET()
        {
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Server_Summary), new
            {
                path = GetPath(),
                shortServer = "//" + Server.ShortUrlServer,
                soft_ver = Cms.Version,
                sys_alias = Settings.License_NAME, // + "(KEY:" + Settings.SYS_KEY + ")",
                server_name = HttpContext.Current.Server.MachineName,
                server_os = Environment.OSVersion.VersionString,
                server_local = CultureInfo.InstalledUICulture.EnglishName,
                server_ip = Request.ServerVariables["LOCAl_ADDR"],
                server_host = Request.ServerVariables["SERVER_NAME"],
                server_iis = Request.ServerVariables["Server_SoftWare"],
                server_netver =
                    Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." +
                    Environment.Version.Revision,
                server_https = Request.Url.ToString().IndexOf("https://") != -1 ? "是" : "否",
                server_port = Request.ServerVariables["Server_Port"],
                server_hour = String.Format("{0}小时", ((Environment.TickCount / 0x3e8) / 3600).ToString()),
                server_time = DateTime.Now.ToString(),
                server_cpu =
                    String.Format("{0},{1}核", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
                        Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS")),
                server_meory = (Environment.WorkingSet / 1024 / 1024) + "M",
                server_net_meory =
                    ((Double)Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + "M",
                person_os = this.GetOSNameByUserAgent(base.Request.UserAgent),
                person_ip = base.Request.UserHostAddress,
                person_soft = base.Request.Browser.Browser,
                person_softver = base.Request.Browser.Version,
                person_cookie = base.Request.Browser.Cookies ? "支持" : "<span style=\"color:Red\">不支持</span>",
                person_java =
                    base.Request.Browser.EcmaScriptVersion.Major >= 1 ? "支持" : "<span style=\"color:Red\">不支持</span>",
            });

        }

        /// <summary>
        /// 主要首页
        /// </summary>
        public void IndexMain_GET()
        {
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.IndexMain), null);
        }

        /// <summary>
        /// 加载登陆信息
        /// </summary>
        public void LoadSession_GET()
        {
            string url;
            if (Request.UrlReferrer != null)
            {
                url = Request.UrlReferrer.PathAndQuery;
            }
            else
            {
                url = "";
            }

            var usr = UserState.Administrator.Current;
            if (usr != null)
            {
                Response.Redirect(url);
            }
            else
            {
                Response.Write("<script>location.href='/" +
                    Settings.SYS_ADMIN_TAG +
                    "?action=login&return_url=" +
                    url + "'</script>");
            }

            Response.End();
        }

        private string GetOSNameByUserAgent(string userAgent)
        {

            if (!String.IsNullOrEmpty(userAgent))
            {

                if (userAgent.Contains("NT 6.0"))
                {
                    return "Windows Vista/Server 2008";
                }
                else if (userAgent.Contains("NT 5.2"))
                {
                    return "Windows Server 2003";
                }
                else if (userAgent.Contains("NT 5.1"))
                {
                    return "Windows XP";
                }
                else if (userAgent.Contains("NT 5"))
                {
                    return "Windows 2000";
                }
                else if (userAgent.Contains("NT 4"))
                {
                    return "Windows NT4";
                }
                else if (userAgent.Contains("Me"))
                {
                    return "Windows Me";
                }
                else if (userAgent.Contains("98"))
                {
                    return "Windows 98";
                }
                else if (userAgent.Contains("95"))
                {
                    return "Windows 95";
                }
                else if (userAgent.Contains("Mac"))
                {
                    return "Mac";
                }
                else if (userAgent.Contains("Unix"))
                {
                    return "UNIX";
                }
                else if (userAgent.Contains("Linux"))
                {
                    return "Linux";
                }
                else if (userAgent.Contains("SunOS"))
                {
                    return "SunOS";
                }
                else if (userAgent.Contains("CentOS"))
                {
                    return "CentOS";
                }
            }
            return "未知设备";
        }


        #region 登录

        /// <summary>
        /// 登录验证码
        /// </summary>
        public void VerifyCode_GET()
        {
            string word = null;
            VerifyCode v = new VerifyCode();
            var font = v.GetDefaultFont();
            try
            {
                font = new Font(font.FontFamily, 16);
                v.AllowRepeat = false;
                Response.BinaryWrite(v.GraphicDrawImage(4,
                    VerifyWordOptions.Letter,
                    !true,
                    font,
                    50,
                    out word));
                VerifyCodeManager.AddWord(word);
                Response.ContentType = "Image/Jpeg";
            }
            catch (Exception exc)
            {
                if (font != null)
                {
                    font.Dispose();
                }
                throw exc;
            }
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        public void Login_GET()
        {
            base.RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.Login),
                new
                {
                    path = GetPath(),
                    version = Cms.Version,
                    year = DateTime.Now.Year.ToString()
                });
        }

        /// <summary>
        /// 登录提交
        /// </summary>
        public void Login_POST()
        {
            string username = base.Request.Form["uid"],
            password = base.Request.Form["pwd"],
            verifycode = base.Request.Form["code"];

            if (!VerifyCodeManager.Compare(verifycode))
            {
                base.RenderError("验证码不正确");
            }
            else
            {
                int tag = UserState.Administrator.Login(username, password, 3600 * 120);

                if (tag == -1)
                {
                    base.RenderError("账户或密码不正确!");
                }
                else if (tag == -2)
                {
                    base.RenderError("账号已被停用,请联系管理员!");
                }
                else
                {
                    base.RenderSuccess();
                    //保存登陆信息
                }

                //设置站点
                //                User usr = CmsLogic.User.GetUser(username, password);
                //
                //                if (usr != null)
                //                {
                //                    CmsLogic.User.UpdateUserLastLoginDate(username);
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
        }

        #endregion

        /// <summary>
        /// 快捷工具
        /// </summary>
        ///
        /*
        public void Quick_GET()
        {
            string moduleOpts,
            categoryOpts,
            authorOpts;

            StringBuilder sb = new StringBuilder();

            foreach (var m in CmsLogic.Module.GetSiteModules(base.CurrentSite.SiteId))
            {
                if (!m.IsDelete)
                {
                    sb.Append("<option value=\"").Append(m.ID.ToString()).Append("\">")
                        .Append(m.Name).Append("</option>");
                }
            }
            moduleOpts = sb.ToString();
            sb.Remove(0, sb.Length);


            //计算栏目
            CmsLogic.Category.HandleCategoryTree(CmsLogic.Category.Get(a => a.Lft == 1).Name, (category, level) =>
                                                 {
                                                    sb.Append("<option value=\"").Append(category.Lft.ToString()).Append("\">");
                                                    for (var i = 0; i < level; i++)
                                                    {
                                                        sb.Append("一");
                                                    }
                                                    sb.Append(category.Name).Append("</option>");

                                                 });


            categoryOpts = sb.ToString();
            sb.Remove(0, sb.Length);


            //作者
            foreach (User u in  CmsLogic.User.GetAllUser())
            {
                sb.Append("<option value=\"").Append(u.UserName).Append("\">")
                    .Append(u.Name).Append("(").Append(u.UserName).Append(")</option>");
            }
            authorOpts = sb.ToString();


            base.RenderTemplate(WebManagerResource.quick, new
                                {
                                    module_opts = moduleOpts,
                                    category_opts = categoryOpts,
                                    author_opts = authorOpts
                                });
        }
         */

        /// <summary>
        /// 退出登录
        /// </summary>
        public void Logout_GET()
        {
            UserState.Administrator.Exit();

            string path = HttpContext.Current.Request.Path;
            HttpContext.Current.Response.Redirect(String.Format("{0}?module=system&action=login", path), true);
        }


        /// <summary>
        /// 系统设置
        /// </summary>
        public void AppConfig_GET()
        {
            object data = new
            {
                license_name = Settings.License_NAME,
                license_key = Settings.License_KEY,
                site_domain = Request.Url.Host,
                sys_autowww = Settings.SYS_AUTOWWW,
                sys_admin_tag = Settings.SYS_ADMIN_TAG,
                sys_encode_conf = Settings.SYS_ENCODE_CONF_FILE,
                db_prefix = Settings.DB_PREFIX,
                opti_IndexCacheSeconds = (Settings.Opti_IndexCacheSeconds / 60).ToString(),
                opti_ClientCacheSeconds = (Settings.Opti_ClientCacheSeconds / 60).ToString(),
                Opti_GC_Collect_Interval = (Settings.Opti_GC_Collect_Interval / 3600000),
                opti_debug = Settings.Opti_Debug,
                server_static = Settings.SERVER_STATIC,
                server_upgrade = Settings.SERVER_UPGRADE,
                server_static_enabled = Settings.SERVER_STATIC_ENABLED,
                tpl_usefullpath = Settings.TPL_UseFullPath,
                tpl_usecompress = Settings.TPL_UseCompress

            };
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.App_Config), new
            {
                json = JsonSerializer.Serialize(data)
            });
        }
        public void AppConfig_POST()
        {
            string item = HttpContext.Current.Request["item"];

            HttpRequest req = base.Request;

            switch (item)
            {
                case "1":
                    Settings.License_KEY = req["license_key"];
                    Settings.License_NAME = req["license_name"];
                    Settings.SYS_AUTOWWW = req["sys_autowww"] == "on";
                    Settings.SYS_ENCODE_CONF_FILE = req["sys_encode_conf"] == "on";
                    Settings.DB_PREFIX = req["db_prefix"];
                    Settings.TPL_UseCompress = req["tpl_usecompress"] == "on";
                    Settings.TPL_UseFullPath = req["tpl_usefullpath"] == "on";

                    if (!String.IsNullOrEmpty(req["sys_admin_tag"]))
                    {
                        Settings.SYS_ADMIN_TAG = req["sys_admin_tag"];
                    }

                    //缓存
                    Settings.Opti_IndexCacheSeconds = int.Parse(req["opti_IndexCacheSeconds"]) * 60;
                    Settings.Opti_ClientCacheSeconds = int.Parse(req["opti_ClientCacheSeconds"]) * 60;
                    Settings.Opti_GC_Collect_Interval = int.Parse(req["Opti_GC_Collect_Interval"]) * 3600000;

                    //是否调试
                    Settings.Opti_Debug = req["opti_debug"] == "on";

                    Settings.SERVER_STATIC_ENABLED = req["server_static_enabled"] == "on";

                    if (!String.IsNullOrEmpty(req["server_static"]))
                    {
                        Settings.SERVER_STATIC = req["server_static"];
                    }
                    if (!String.IsNullOrEmpty(req["server_upgrade"]))
                    {
                        Settings.SERVER_UPGRADE = req["server_upgrade"];
                    }

                    Configuration.BeginWrite();
                    Configuration.UpdateByPrefix("tpl");
                    Configuration.UpdateByPrefix("opti");
                    Configuration.UpdateByPrefix("sys");
                    Configuration.UpdateByPrefix("db");
                    Configuration.EndWrite();

                    Cms.Template.Register();
                    break;
            }


            Configuration.Renew();

            base.RenderSuccess("修改成功!");
        }

        public string Clear_GET()
        {
            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Clear_Page));
        }

        public string Clear_POST()
        {
            switch (base.Request["for"])
            {
                case "reboot":
                    HttpRuntime.UnloadAppDomain();
                    return base.ReturnSuccess("网站正在重启...");
                case "cache":
                    PageVariable.ResetBuilt();
                    return base.ReturnSuccess("缓存清理成功");
            }
            return base.ReturnSuccess();
        }

        public string Locale_GET() {
            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Locale));
        }

        public string GetLocaleJson_POST()
        {
            String path= "";
            String type = this.Request.Form["type"];
            switch (type)
            {
                case "system":
                    path = Cms.PyhicPath + CmsVariables.FRAMEWORK_PATH + "locale/locale.db";
                    break;
                case "custom":
                    path = Cms.PyhicPath + CmsVariables.SITE_LOCALE_PATH;
                    break;
                default:
                    return base.ReturnError("error type");
            }
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                return "[]";
            }
            
            StreamReader rd = new StreamReader(path,Encoding.UTF8);
            String json = rd.ReadToEnd();
            rd.Close();
            return json;
        }

        public string CreateNewLocaleKey_POST()
        {
            String key = this.Request.Form["key"];
            try
            {
                Locale.AddKey(key);
            }
            catch (Exception exc)
            {
                return this.ReturnError(exc.Message);
            }
            return this.ReturnSuccess("");
        }

        public string DelLocaleKey_POST()
        {
            String key = this.Request.Form["key"];
            try
            {
                Locale.DelKey(key);
            }
            catch (Exception exc)
            {
                return this.ReturnError(exc.Message);
            }
            return this.ReturnSuccess("");
        }

        public string SaveLocale_POST()
        {
            try
            {
                String json = Locale.SaveByPostForm(this.Request.Form);
                if (json != null)
                {
                    Cms.Language.GetPackage().LoadFromJson(json);
                }
            }
            catch (Exception exc)
            {
                return this.ReturnError(exc.Message);
            }
            return this.ReturnSuccess("");
        }
    }
}
