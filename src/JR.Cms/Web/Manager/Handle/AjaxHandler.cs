//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
// Modify:
//  2013-06-08 22:30 newmin [!] : 添加模块更新菜单数据
//
//

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using JR.Cms.Conf;
using JR.Cms.Infrastructure.Tree;
using JR.Cms.Library.CacheProvider;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Util;
using JR.Stand.Core;
using JR.Stand.Core.Framework.Text;
using JR.Stand.Core.Web;
using static JR.Cms.Updater;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 
    /// </summary>
    public class AjaxHandler : BasePage
    {
        private string GetAppInit()
        {
            const string tpl = @"
								   username='{0}';
                                   md={1};
                                   sites=[{2}];
                                   ip='{3}';
                                   address='{4}';
                                ";

            var usr = UserState.Administrator.Current;
            //int siteId = base.SiteId;
            //string groupName =Role.GetRoleName(usr.Roles.GetRole(siteId).GetFlag()),
            string ip = WebCtx.Current.UserIpAddress,
                address = "未知",
                sites = "",
                menuData = "[]";


            #region 获取公告

            /*
            string notice = HttpRuntime.Cache["mgr_scrollnotice"] as string;
            if (notice == null)
            {
                try
                {
                    WebRequest request = WebRequest.Create("http://ct.cms.to2.net/view/opsite_notice/");
                    StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
                    notice = sr.ReadToEnd();
                    sr.Dispose();

                    if (!String.IsNullOrEmpty(notice))
                    {
                        HttpRuntime.Cache.Insert("mgr_scrollnotice", notice, null, DateTime.Now.AddMinutes(30), TimeSpan.Zero);
                    }
                }
                catch
                {
                    notice = "获取公告失败";
                }
            }
           HttpContextAdapter.Current.Raw.Response.Write(notice);
			 */

            #endregion

            #region 获取IP信息

            /*


            //获取IP及IP信息返回
            try
            {
                WebClient wc = new WebClient();
                string html = wc.DownloadString(String.Format("http://ip138.com/ips138.asp?ip={0}&action=2", ip));
                Regex reg = new Regex("<li>本站主数据：([^\\<]+)</li>");
                Match match = reg.Match(html);
                address = match.Groups[1].Value;
                if (address == "保留地址") address = "本地局域网";
            }
            catch
            {
            }
			 * 
			 */

            #endregion


            #region 获取站点

            var currSiteId = SiteId;
            var sb = new StringBuilder(100);

            SiteDto[] siteArr = null;
            if (usr.IsMaster)
                siteArr = ServiceCall.Instance.SiteService.GetSites().ToArray();
            else
                siteArr = ServiceCall.Instance.UserService.GetUserRelationSites(usr.Id);

            var i = 0;
            foreach (var s in siteArr)
            {
                if (i++ != 0) sb.Append(",");
                sb.Append("{\"id\":\"").Append(s.SiteId.ToString())
                    .Append("\",\"name\":\"").Append(s.Name).Append("\"}");
            }

            sites = sb.ToString();

            #endregion

            #region 从缓存中获取菜单数据

            if (string.IsNullOrEmpty(Request.Query("onlysite")))
            {
                //菜单siteid
                var siteId = CurrentSite.SiteId;
                menuData = GetMenuJsonFromFile(usr.IsMaster, siteId);
            }

            #endregion

            //输出
            return string.Format(tpl, usr.Name, menuData, sites, ip, address);
        }

        /// <summary>
        /// 获取滚动公告
        /// </summary>
        public void AppInit()
        {
            Response.WriteAsync(GetAppInit());
        }

        /// <summary>
        /// 从文件中获取菜单JSON数据
        /// </summary>
        /// <returns></returns>
        private static string GetMenuJsonFromFile(bool isMaster, int siteId)
        {
            var sb = new StringBuilder();

            //获取文件
            var xd = new XmlDocument();

            //读取配置
            var setFile = $"{EnvUtil.GetBaseDirectory()}/config/sys_set.conf";
            if (File.Exists(setFile))
            {
                xd.Load(setFile);
            }
            else
            {
                xd.LoadXml(ResourceMap.GetBoardMenu()); //系统默认的配置
            }

            //读取菜单

            //sb.Remove(0, sb.Length);
            sb.Append("[");

            //XmlDocument xd = getSetFile();

            //
            //文件必须包含一个id为content的节点
            //
            XmlNodeList pl = xd.SelectNodes("/config/menu/bar"), //父列表
                sl = null, //二级列表
                tl = null; //三级列表

            int i = 0,
                j = 0,
                k = 0;

            foreach (XmlNode pn in pl)
            {
                sb.Append("{\"id\":\"").Append(pn.Attributes["id"].Value).Append("\",\"text\":\"")
                    .Append(pn.Attributes["name"].Value).Append("\",\"uri\":\"")
                    .Append("#").Append(pn.Attributes["id"].Value).Append("\",\"childs\":[");

                sl = xd.SelectNodes("/config/menu/bar[@id='" + pn.Attributes["id"].Value + "']/links");
                foreach (XmlNode sn in sl)
                {
                    sb.Append("{\"text\":\"").Append(sn.Attributes["title"].Value).Append("\",\"uri\":\"").Append("\",")
                        .Append("\"toggle\":").Append(sn.Attributes["toggle"].Value == "true" ? "true" : "false")
                        .Append(",")
                        .Append("\"iconPos\":\"").Append(sn.Attributes["iconPos"].Value).Append("\",")
                        .Append("\"childs\":[");

                    tl = sn.ChildNodes;

                    //添加二级link
                    foreach (XmlNode tn in tl)
                        if (tn.Name == "link")
                        {
                            if (tn.Attributes["siteid"].Value == "0" && !isMaster) continue;
                            ++k;
                            if (k != 1) sb.Append(",");

                            sb.Append("{\"text\":\"").Append(tn.Attributes["text"].Value)
                                .Append("\",\"uri\":\"").Append(tn.Attributes["href"].Value
                                    .Replace("$prefix", Settings.SYS_ADMIN_TAG))
                                .Append("\"}");
                        }

                    k = 0;

                    sb.Append("]}");
                    if (++j < sl.Count) sb.Append(",");
                }


                goto netx;
                //添加模块

                #region

                /*
                if (String.Compare(pn.Attributes["id"].Value, "content") == 0)
                {
                    if (j != 0)
                    {
                        sb.Append(",");
                        j = 0;
                    }
                    IList<Module> modules = new List<Module>(CmsLogic.Module.GetAvailableModules());

                    foreach (Module m in modules)
                    {
                        if (!m.IsDelete)
                        {
                            sb.Append("{\"text\":'").Append(m.Name).Append("',uri:'").Append("',toggle:").Append(modules.Count > 4 && j < modules.Count - 1 ? "true" : "false")
                                .Append(",childs:[");

                            sb.Append("{text:'").Append("发布信息").Append("',uri:'")
                                .Append("?module=archive&action=create&moduleID=")
                                .Append(m.ID.ToString()).Append("'},");

                            sb.Append("{text:'").Append("信息列表").Append("',uri:'")
                                .Append("?module=archive&action=list&module_id=")
                                .Append(m.ID.ToString()).Append("'}");


                            sb.Append("]}");
                            if (++j < modules.Count)
                            {
                                sb.Append(",");
                            }
                        }
                    }
                }
                */

                #endregion


                netx:
                j = 0;

                sb.Append("]}");

                if (++i < pl.Count) sb.Append(",");
            }

            sb.Append("]");
            return sb.ToString();
        }

        #region Upgrade

        /// <summary>
        /// 升级操作
        /// </summary>
        public void CheckUpgrade()
        {
            const string upgradeTpl =
                "{\"result\":%result%,\"message\":\"%msg%\",\"version\":\"%ver%\",\"build\":\"%build%\",\"change_log\":\"%log%\"}";

            var message = "未知异常";

            var result = Updater.CheckUpgrade();

            switch (result.FetchCode)
            {
                case -1:
                    message = "已经是最新版本";
                    break;
                // case -1: message = "bin目录无法写入更新文件,请修改权限!"; break;
                case -2:
                    message = "未发现更新版本";
                    break;
                case -3:
                    message = "无法连接到更新服务器";
                    break;
                case -4:
                    message = "更新服务器发生内部错误";
                    break;
                case -5:
                    message = "非正式环境无法升级";
                    break;
                case 1:
                    message = "有新版本可以更新!";
                    break;
            }

            Response.WriteAsync(
                upgradeTpl.Replace("%result%", result.FetchCode.ToString())
                    .Replace("%msg%", message.Replace("\"", "\\\""))
                    .Replace("%ver%", result.Version ?? "")
                    .Replace("%build%", result.Build ?? "")
                    .Replace("%log%", (result.ChangeLog ?? "").TrimStart()
                        .Replace("\"", "\\\"")
                        .Replace("\n", "<br />").Replace("\r", ""))
            );
        }

        /// <summary>
        /// 获取更新
        /// </summary>
        public void GetUpgrade_POST()
        {
            if (Cms.OfficialEnvironment) StartUpgrade();
        }

        /// <summary>
        /// 升级进度
        /// </summary>
        public void GetUpgradeStatus()
        {
            var f = UpgradePercent;
            Response.WriteAsync(f.ToString(CultureInfo.InvariantCulture));

            if (f == 1F)
            {
                ApplyBinFolder();
            }
            else if (f < 1F)
            {
                UpgradePercent += (float) new Random().Next(1, 3) / 100;
                if (UpgradePercent > 0.96F) UpgradePercent = 0.96F;
            }
        }

        #endregion


        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache_POST()
        {
            Thread.Sleep(1000);


            CmsCacheFactory.Singleton.Reset(() =>
            {
                //清除系统缓存
                Cms.Cache.Reset(null);
                //重新注册模板
                Cms.Template.Reload();
            });

            // Cms.Cache.Clear("");

            //对缓存执行操作
            //WatchService.ClearCache();
        }


        #region 文档

        /// <summary>
        /// 搜索文档
        /// </summary>
        public void GetSearchArchivesJsonResult_POST()
        {
            var key = Request.GetParameter("key").Trim();
            var size = Request.GetParameter("size") ?? "20";
            var onlyTitle = Request.GetParameter("only_title") == "true";
            var catId = 0;
            int.TryParse(Request.GetParameter("category_id"), out catId);
            int count, pages;
            var intSiteId = 0;
            int.TryParse(Request.GetParameter("site_id"), out intSiteId);
            string catPath = null;
            if (catId > 0)
            {
                var c = ServiceCall.Instance.SiteService.GetCategory(intSiteId, catId);
                catPath = c.Path;
            }

            var strBuilder = new StringBuilder(500);
            var list = ServiceCall.Instance.ArchiveService
                .SearchArchives(intSiteId, catPath, onlyTitle, key,
                    int.Parse(size),
                    1,
                    out count,
                    out pages,
                    null
                );

            var i = 0;
            strBuilder.Append("[");
            var site = default(SiteDto);
            foreach (var a in list)
            {
                if (++i != 1) strBuilder.Append(",");
                if (site.SiteId == 0 || site.SiteId != a.Category.SiteId)
                    site = ServiceCall.Instance.SiteService.GetSiteById(a.Category.SiteId);

                strBuilder.Append("{\"id\":").Append(a.Id)
                    .Append(",\"path\":\"").Append(a.Path)
                    .Append("\",\"alias\":\"").Append(string.IsNullOrEmpty(a.Alias) ? a.StrId : a.Alias)
                    .Append("\",\"title\":\"").Append(a.Title)
                    .Append("\",\"siteId\":").Append(site.SiteId)
                    .Append(",\"siteName\":\"").Append(site.Name)
                    .Append("\",\"category\":\"").Append(a.Category.Name).Append("(").Append(a.Category.Tag).Append(")")
                    .Append("\"}");
            }

            strBuilder.Append("]");

            Response.WriteAsync(strBuilder.ToString());
        }

        public void GetSearchArchivesJsonResult()
        {
            GetSearchArchivesJsonResult_POST();
        }

        #endregion

        /// <summary>
        /// 获取拼写别名
        /// </summary>
        public void GetSpellWord_POST()
        {
            var title = Request.Form("word").ToString().Trim();
            var alias = ChineseSpell.GetSpellWord(title,
                SpellOptions.TranslateUnknowWordToInterrogation |
                SpellOptions.TranslateSpecialWordToConnect);
            Response.WriteAsync(alias);
        }

        /// <summary>
        /// 分类栏目树
        /// </summary>
        /// <returns></returns>
        public string CategoryNodes()
        {
            int.TryParse(Request.Query("site_id"), out var siteId);
            if (siteId <= 0)
            {
                siteId = SiteId;
            }

            var key = Consts.NODE_TREE_JSON_KEY + ":" + siteId.ToString();
            var nodeJson = Kvdb.Gca.Get(key);
            if (string.IsNullOrEmpty(nodeJson))
            {
                TreeNode node;
                if (Request.Query("root") == "true")
                    node = ServiceCall.Instance.SiteService.GetCategoryTreeWithRootNode(siteId);
                else
                    node = ServiceCall.Instance.SiteService.GetCategoryTreeWithRootNode(siteId);
                try
                {
                    nodeJson = JsonSerializer.Serialize(node);
                    Kvdb.Gca.Put(key, nodeJson);
                }
                catch (Exception exc)
                {
                    return "{\"result\":false,\"error\":\"" + exc.Message + "\"}";
                }
            }

            return nodeJson;
        }
    }
}