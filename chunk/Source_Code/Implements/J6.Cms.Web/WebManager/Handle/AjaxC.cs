//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://cms.s1n1.com
// Modify:
//  2013-06-08 22:30 newmin [!] : 添加模块更新菜单数据
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using J6.Cms.BLL;
using J6.Cms.Cache;
using J6.Cms.Cache.CacheCompoment;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface.User;
using J6.Cms.Infrastructure.Tree;
using J6.Cms.Utility;
using J6.Cms.WebManager;
using J6.DevFw.Framework.Text;

namespace J6.Cms.Web.WebManager.Handle
{
    public class AjaxC : BasePage
    {

        internal string GetAppInit()
        {
            const string tpl = @"
								   username='{0}';
								   groupname='{1}';
                                   md={2};
                                   sites=[{3}];
                                   ip='{4}';
                                   address='{5}';
                                ";

            HttpRequest request = HttpContext.Current.Request;
            UserDto usr = UserState.Administrator.Current;
            string groupName = Role.GetRoleName(usr.RoleFlag),
            ip = request.UserHostAddress,
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
                    WebRequest request = WebRequest.Create("http://ct.j6.cc/view/opsite_notice/");
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
            HttpContext.Current.Response.Write(notice);
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

            int currSiteId = base.SiteId;

            StringBuilder sb = new StringBuilder(100);

            int i = 0;
            foreach (SiteDto s in ServiceCall.Instance.SiteService.GetSites())
            {
                if (s.SiteId != currSiteId)
                {
                    if (i++ != 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append("{id:'").Append(s.SiteId.ToString())
                        .Append("',name:'").Append(s.Name.Replace("'", "\\'"));

                    if (s.Note.Trim() != "")
                    {
                        sb.Append("[").Append(s.Note).Append("]");

                    }
                    sb.Append("'}");
                }
            }

            sites = sb.ToString();

            #endregion

            #region 从缓存中获取菜单数据

            if (String.IsNullOrEmpty(request["onlysite"]))
            {
                //菜单siteid
                int siteId = base.CurrentSite.SiteId;
                string cacheKey = String.Format("{0}_{1}_manager_menujson", CacheSign.Site.ToString(), (siteId).ToString());
                object json =Cms.Cache.Get(cacheKey);
                if (json != null)
                {
                    menuData = json as string;
                }
                else
                {
                    menuData = GetMenuJsonFromFile(siteId);
                    Cms.Cache.Insert(cacheKey, menuData);
                }

            }

            #endregion

            //输出
            return String.Format(tpl, usr.Name, groupName, menuData, sites, ip, address);

        }
        /// <summary>
        /// 获取滚动公告
        /// </summary>
        public void AppInit_GET()
        {
            HttpContext.Current.Response.Write(this.GetAppInit());
        }

        /// <summary>
        /// 从文件中获取菜单JSON数据
        /// </summary>
        /// <returns></returns>
        private static string GetMenuJsonFromFile(int siteID)
        {
            StringBuilder sb = new StringBuilder();

            //获取文件
            XmlDocument xd = new XmlDocument();

            //读取配置
            string setfile = String.Format("{0}config/sysset.conf", AppDomain.CurrentDomain.BaseDirectory);
            if (global::System.IO.File.Exists(setfile))
            {
                xd.Load(setfile);
            }
            else
            {
                xd.LoadXml(ResourceMap.Sysset_conf);   //系统默认的配置
            }


            //读取菜单

            //sb.Remove(0, sb.Length);
            sb.Append("[");

            //XmlDocument xd = getSetFile();

            //
            //文件必须包含一个id为content的节点
            //
            XmlNodeList pl = xd.SelectNodes("/config/menu/bar"),    //父列表
            sl = null,                                                        //二级列表
            tl = null;                                                        //三级列表

            int i = 0,
            j = 0,
            k = 0;

            foreach (XmlNode pn in pl)
            {
                sb.Append("{id:'").Append(pn.Attributes["id"].Value).Append("',text:'").Append(pn.Attributes["name"].Value).Append("',uri:'")
                    .Append("#").Append(pn.Attributes["id"].Value).Append("',childs:[");

                sl = xd.SelectNodes("/config/menu/bar[@id='" + pn.Attributes["id"].Value + "']/links");
                foreach (XmlNode sn in sl)
                {
                    sb.Append("{text:'").Append(sn.Attributes["title"].Value).Append("',uri:'").Append("',")
                        .Append("toggle:").Append(sn.Attributes["toggle"].Value == "true" ? "true" : "false").Append(",")
                        .Append("iconPos:'").Append(sn.Attributes["iconPos"].Value).Append("',")
                        .Append("childs:[");

                    tl = sn.ChildNodes;

                    //添加二级link
                    foreach (XmlNode tn in tl)
                    {
                        if (tn.Name == "link")
                        {
                            if (tn.Attributes["siteid"].Value == "0" && siteID > 0)
                            {
                                continue;
                            }
                            ++k;
                            if (k != 1)
                            {
                                sb.Append(",");
                            }

                            sb.Append("{text:'").Append(tn.Attributes["text"].Value)
                                .Append("',uri:'").Append(tn.Attributes["href"].Value.Replace("$prefix", Settings.SYS_ADMIN_TAG))
                                .Append("'}");
                        }
                    }

                    k = 0;

                    sb.Append("]}");
                    if (++j < sl.Count)
                    {
                        sb.Append(",");
                    }
                }


                goto netx;
                //添加模块
                #region
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
                            sb.Append("{text:'").Append(m.Name).Append("',uri:'").Append("',toggle:").Append(modules.Count > 4 && j < modules.Count - 1 ? "true" : "false")
                                .Append(",childs:[");

                            sb.Append("{text:'").Append("发布信息").Append("',uri:'")
                                .Append("?module=archive&action=create&moduleID=")
                                .Append(m.ID.ToString()).Append("'},");

                            sb.Append("{text:'").Append("信息列表").Append("',uri:'")
                                .Append("?module=archive&action=list&moduleid=")
                                .Append(m.ID.ToString()).Append("'}");


                            sb.Append("]}");
                            if (++j < modules.Count)
                            {
                                sb.Append(",");
                            }
                        }
                    }
                }
                #endregion


            netx:
                j = 0;

                sb.Append("]}");

                if (++i < pl.Count)
                {
                    sb.Append(",");
                }

            }
            sb.Append("]");
            return sb.ToString();
        }

        #region Upgrade

        /// <summary>
        /// 升级操作
        /// </summary>
        public void CheckUpgrade_POST()
        {
            const string upgradeTpl = "{result:%result%,message:'%msg%',version:'%ver%',log:'%log%'}";

            string message = "未知异常";
            string changeLog = String.Empty;
            string[] verResult;

            int result = Updater.CheckUpgrade(out verResult);

            switch (result)
            {
                case -1: message = "已经是最新版本"; break;
                // case -1: message = "bin目录无法写入更新文件,请修改权限!"; break;
                case -2: message = "未发现更新版本"; break;
                case -3: message = "无法连接到更新服务器"; break;
                case -4: message = "更新服务器发生内部错误"; break;
                case 1: message = "有新版本可以更新!"; break;
            }

            if (result == 1)
            {
                changeLog = verResult[2];
            }

            base.Response.Write(
                upgradeTpl.Replace("%result%", result.ToString())
                .Replace("%msg%", message.Replace("'", "\\'")).Replace("%ver%", verResult[0])
                .Replace("%log%", changeLog.Replace("'", "\\'").Replace("\n", "<br />").Replace("\r", ""))
            );
        }

        /// <summary>
        /// 获取更新
        /// </summary>
        public void GetUpgrade_POST()
        {
            Updater.StartUpgrade();
        }

        /// <summary>
        /// 升级进度
        /// </summary>
        public void UpgradeStatus_POST()
        {
            float f = Updater.UpgradePercent;
            base.Response.Write(f.ToString(CultureInfo.InvariantCulture));
            if (f == 1F)
            {
                Updater.ApplyCoreLib();
            }
            else if (f < 1F)
            {
                Updater.UpgradePercent += (float)new Random().Next(1, 3) / 100;
                if (Updater.UpgradePercent > 0.96F)
                {
                    Updater.UpgradePercent = 0.96F;
                }
            }

            Thread.Sleep(1000);
        }

        #endregion


        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache_POST()
        {
            Thread.Sleep(1000);


            CacheFactory.Sington.Reset(() =>
                            {
                                //清除系统缓存
                                foreach (DictionaryEntry d in HttpRuntime.Cache)
                                {
                                    HttpRuntime.Cache.Remove(d.Key.ToString());
                                }

                                //重新注册模板
                                J6.Cms.Cms.Template.Register();
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
            string key = base.Request["key"].Trim();
            string size = base.Request["size"] ?? "20";
            int count, pages;
            StringBuilder strBuilder = new StringBuilder(500);

            IEnumerable<ArchiveDto> list = ServiceCall.Instance.ArchiveService
                .SearchArchives(this.SiteId, key,
                int.Parse(size),
                1,
                 out count,
                 out pages,
                 null
                );



            int i = 0;
            strBuilder.Append("[");
            foreach (ArchiveDto a in list)
            {
                if (++i != 1)
                {
                    strBuilder.Append(",");
                }
                strBuilder.Append("{'id':'").Append(a.Id)
                    .Append("','alias':'").Append(String.IsNullOrEmpty(a.Alias) ? a.StrId : a.Alias)
                    .Append("',title:'").Append(a.Title)
                    .Append("',category:'").Append(a.Category.Name).Append("(").Append(a.Category.Tag).Append(")")
                    .Append("'}");
            }
            strBuilder.Append("]");

            base.Response.Write(strBuilder.ToString());
        }

        public void GetSearchArchivesJsonResult_GET()
        {
            GetSearchArchivesJsonResult_POST();
        }
        #endregion

        /// <summary>
        /// 获取拼写别名
        /// </summary>
        public void GetSpellWord_POST()
        {
            string title =base.Request["word"].Trim();
            string alias = ChineseSpell.GetSpellWord(title,
                SpellOptions.TranslateUnknowWordToInterrogation |
                SpellOptions.TranslateSpecialWordToConnect);
            base.Response.Write(alias);
        }

        /// <summary>
        /// 分类栏目树
        /// </summary>
        /// <returns></returns>
        public string CategoryNodes_GET()
        {
            TreeNode node = ServiceCall.Instance.SiteService.GetCategoryTreeNode(this.SiteId, 1);
            try
            {
                return JsonSerializer.Serialize(node);
            }
            catch (Exception exc)
            {
                return "{result:false,error:'" + exc.Message + "'}";
            }
        }
    }
}
