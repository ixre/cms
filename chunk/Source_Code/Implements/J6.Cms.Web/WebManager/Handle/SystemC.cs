//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : Home.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 20:13:52
// Description :
//
// Get infromation of this software,please visit our site http://h3f.net/cms
// Modify:
//  2013-01-06  11:07   newmin [+]: add setting
//

using System;
using System.IO;
using System.Text;
using System.Web;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Utility;
using J6.Cms.WebManager;
using J6.DevFw.Framework.Web.UI;
using SharpCompress.Archive;
using SharpCompress.Common;

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
            HttpContext.Current.Response.Write("<div style=\"margin-top:50px;text-align:center\"><span style=\"color:red\">后续推出....</span></div>");
        }

        /// <summary>
        /// 管理首页
        /// </summary>
        //[StaticCache]
        public void Index_GET()
        {
            //重定向管理页
            string path = base.Request.Path;
            if (base.Request.Url.Query == "" || path.EndsWith("/"))
            {
                if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
                base.Response.Write(String.Format("<script>window.parent.location.replace('{0}?ver={1}')</script>", path, Cms.Version));
                return;
            }

            string pageTemplate;
            UserDto usr = UserState.Administrator.Current;

            if (!usr.IsMaster)
            {
                //子站用户跳转标识
                //if (base.Request["f"] != "s")
                //{
                //    base.Response.Write(String.Format("<script>window.parent.location.replace('{0}?ver={1}&f=s')</script>", base.Request.Path, Cms.Version));
                //    return;
                //}
                //else
                //{
                //    page_Template = ResourceMap.GetPageContent(ManagementPage.SUB_Index);
                //}
                pageTemplate = ResourceMap.GetPageContent(ManagementPage.SUB_Index);
            }
            else
            {
                pageTemplate = ResourceMap.GetPageContent(ManagementPage.Index);
            }

            base.RenderTemplateUseCache(pageTemplate,
                                        new
                                        {
                                            version = Cms.Version,
                                            path = GetPath(),
                                            admin_path = Settings.SYS_ADMIN_TAG,
                                            //ui_component_path = ResourceMap.GetPageUrl(ManagementPage.UI_Component),
                                            ui_css_path = ResourceMap.GetPageUrl(ManagementPage.UI_Index_Css),
                                            ui_custom_js_path = ResourceMap.GetPageUrl(ManagementPage.UI_Index_Custom_Js)
                                            //initData=new Ajax().GetAppInit()
                                        });
        }

        /// <summary>
        /// 设置站点
        /// </summary>
        public void SelectSite_GET()
        {
            //设置站点
            UserDto user = UserState.Administrator.Current;
            if (user.IsMaster)
            {
                int siteId = 0;
                int.TryParse(base.Request["siteid"], out siteId);
                var site = SiteCacheManager.GetSite(siteId);
                if (site.SiteId > 0)
                {
                    base.CurrentSite = site;
                    base.RenderSuccess();
                    return;
                }
            }

            base.RenderError("没有权限管理其他站点");
        }

        /// <summary>
        /// 欢迎首页
        /// </summary>
        public void Welcome_GET()
        {
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Welcome), new
            {
                path = GetPath(),
                dashurl = Server.DashbordRssServer,
                soft_ver = Cms.Version,
                sys_alias = Settings.License_NAME,
            });
        }

        /// <summary>
        /// 服务器信息页面
        /// </summary>
        public void ServerSummary_GET()
        {
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Server_Summary), new
                                {
                                    path = GetPath(),
                                    dashurl = Server.DashbordRssServer,
                                    soft_ver = Cms.Version,
                                    sys_alias = Settings.License_NAME,// + "(KEY:" + Settings.SYS_KEY + ")",
                                    server_name = HttpContext.Current.Server.MachineName,
                                    server_os = Environment.OSVersion.VersionString,
                                    server_local = System.Globalization.CultureInfo.InstalledUICulture.EnglishName,
                                    server_ip = Request.ServerVariables["LOCAl_ADDR"],
                                    server_host = Request.ServerVariables["SERVER_NAME"],
                                    server_iis = Request.ServerVariables["Server_SoftWare"],
                                    server_netver = Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." + Environment.Version.Revision,
                                    server_https = Request.Url.ToString().IndexOf("https://") != -1 ? "是" : "否",
                                    server_port = Request.ServerVariables["Server_Port"],
                                    server_hour = String.Format("{0}小时", ((Environment.TickCount / 0x3e8) / 3600).ToString()),
                                    server_time = DateTime.Now.ToString(),
                                    server_cpu = String.Format("{0},{1}核", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"), Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS")),
                                    server_meory = (Environment.WorkingSet / 1024 / 1024) + "M",
                                    server_net_meory = ((Double)System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + "M",
                                    person_os = this.GetOSNameByUserAgent(base.Request.UserAgent),
                                    person_ip = base.Request.UserHostAddress,
                                    person_soft = base.Request.Browser.Browser,
                                    person_softver = base.Request.Browser.Version,
                                    person_cookie = base.Request.Browser.Cookies ? "支持" : "<span style=\"color:Red\">不支持</span>",
                                    person_java = base.Request.Browser.EcmaScriptVersion.Major >= 1 ? "支持" : "<span style=\"color:Red\">不支持</span>",
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
        /// 加载页面
        /// </summary>
        public void Load_GET()
        {
            base.RenderTemplate(ResourceMap.PageLoading, null);
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
                font = new System.Drawing.Font(font.FontFamily, 16);
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
                db_prefix = Settings.DB_PREFIX,
                opti_IndexCacheSeconds = (Settings.Opti_IndexCacheSeconds / 60).ToString(),
                opti_ClientCacheSeconds = (Settings.Opti_ClientCacheSeconds / 60).ToString(),
                Opti_GC_Collect_Interval = (Settings.Opti_GC_Collect_Interval / 3600000),
                opti_debug = Settings.Opti_Debug,
                server_static = Settings.SERVER_STATIC,
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

                    /// <summary>
                    /// 静态分发内容
                    /// </summary>
                    Settings.SERVER_STATIC_ENABLED = req["server_static_enabled"] == "on";

                    if (!String.IsNullOrEmpty(req["server_static"]))
                    {
                        Settings.SERVER_STATIC = req["server_static"];
                    }

                    Configuration.Update("tpl");
                    Configuration.Update("opti");
                    Configuration.Update("sys");
                    Configuration.Update("db");

                    break;
            }


            Configuration.Renew();

            base.RenderSuccess("修改成功!");
        }


        #region 系统补丁

        /// <summary>
        /// 系统补丁页面
        /// </summary>
        public void Patch_GET()
        {
            string patchlistHtml;

            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "patch");
            if (!dir.Exists)
            {
                dir.Create();
                patchlistHtml = "<div style=\"color:red\">没有补丁可供安装！</div>";
            }
            else
            {
                FileInfo[] files = dir.GetFiles("*.zip");
                if (files.Length == 0)
                {
                    patchlistHtml = "<div style=\"color:red\">没有补丁可供安装！</div>";
                }
                else
                {
                    //对文件排序
                    //Array.Sort(files, new Comparison<FileInfo>((a, b) => { return a.CreationTime > b.CreationTime ? 1 : -1; }));

                    StringBuilder sb = new StringBuilder();
                    int i = 0;
                    foreach (FileInfo file in files)
                    {
                        sb.Append("<div><input type=\"radio\" name=\"applypatchfile\"")
                            .Append(++i == 1 ? " checked=\"checked\"" : String.Empty)
                            .Append(" value=\"")
                            .Append(file.Name).Append("\"/><label>")
                            .Append(file.Name).Append("</label></div>");
                    }

                    patchlistHtml = sb.ToString();
                }
            }

            base.RenderTemplate(ResourceMap.Patch, new
                                {
                                    patchlist = patchlistHtml
                                });
        }

        /// <summary>
        /// 上传补丁文件
        /// </summary>
        public void PatchUpload_POST()
        {
            if (base.Request.Files.Count != 0)
            {
                HttpPostedFile file = base.Request.Files[0];

                try
                {
                    file.SaveAs(String.Format("{0}/patch/{1}", AppDomain.CurrentDomain.BaseDirectory, file.FileName));
                    base.Response.Write("<script>window.parent.location.reload();</script>");
                }
                catch
                {
                    base.Response.Write("<script>alert('无权限保存补丁文件!');</script>");
                }
            }
        }

        /// <summary>
        /// 安装补丁
        /// </summary>
        public void Patch_POST()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = String.Format(@"{0}\patch\{1}", appDir, base.Request["file"]);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                //bool result=ZipUtility.UncompressFile(@"C:\", filePath, true);
                try
                {
                    System.Threading.Thread.Sleep(1000);

                    #region dotnetzip
                    /*

                    // var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                    using (ZipFile zip = ZipFile.Read(filePath))//, options))
                    {
                        // This call to ExtractAll() assumes:
                        //   - none of the entries are password-protected.
                        //   - want to extract all entries to current working directory
                        //   - none of the files in the zip already exist in the directory;
                        //     if they do, the method will throw.
                        zip.ExtractAll(appDir,ExtractExistingFileAction.OverwriteSilently);
                    }

					 */
                    #endregion

                    #region sharpcompress

                    IArchive archive = ArchiveFactory.Open(filePath);
                    foreach (IArchiveEntry entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            //Console.WriteLine(entry.FilePath);
                            entry.WriteToDirectory(appDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                    }
                    archive.Dispose();

                    #endregion


                }
                catch (System.Exception ex1)
                {
                    base.Response.Write(ex1.Message);
                }
            }
        }

        #endregion


    }
}
