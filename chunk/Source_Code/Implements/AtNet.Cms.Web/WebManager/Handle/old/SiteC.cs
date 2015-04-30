//
// Copyright (C) 2007-2008 S1N1.COM,All rights reseved.
// 
// Project: AtNet.Cms.Manager
// FileName : Ajax.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/15 21:16:56
// Description :
//
// Get infromation of this software,please visit our site http://cms.ops.cc
// Modify:
//  2013-06-08 22:30 newmin [!] : 添加模块更新菜单数据
//
//

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AtNet.Cms.Cache.CacheCompoment;
using AtNet.Cms.CacheService;
using AtNet.DevFw.Framework.Automation;
using AtNet.Cms.DataTransfer;

namespace AtNet.Cms.WebManager
{
    public class SiteC: BasePage
    {

        /// <summary>
        /// 检查域名是否已经绑定
        /// </summary>
        /// <param name="siteId">当前站点编号，-1表示不限制</param>
        /// <param name="domain"></param>
        /// <param name="bindDomain"></param>
        /// <returns></returns>
        private bool CheckDomainBind(int siteId, string domain, out string bindDomain)
        {
            bindDomain = null;
            if (!String.IsNullOrEmpty(domain))
            {
                char[] split = new char[] { ' ', ',' };
                foreach (string _domain in domain.Split(split))
                {
                    foreach (SiteDto s in SiteCacheManager.GetAllSites())
                    {
                        if (String.IsNullOrEmpty(s.Domain) || (siteId!=-1&&siteId==s.SiteId) ) continue;
                        if (Array.Exists(s.Domain.Split(split), a => String.Compare(a, _domain, true) == 0))
                        {
                            bindDomain += "\n" + _domain;
                            break;
                        }
                    }
                }
            }
            return bindDomain != null;
        }

        public void Index_GET()
        {
            IList<SiteDto> sites=SiteCacheManager.GetAllSites();
            TreeItem[] items=new TreeItem[sites.Count];
            for (int i = 0; i < sites.Count; i++)
            {
                items[i] = new TreeItem
                {
                    ID=sites[i].SiteId,
                    Name=sites[i].Name + (sites[i].Note.Trim()!=""?"["+sites[i].Note+"]":"")
                };
            }


            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Index), new
            {
                siteTree=Helper.SingleTree("所有站点",items)
            });
        }

        public void Create_GET()
        {
            var site = new SiteDto();
            var sites=SiteCacheManager.GetAllSites();
            if (sites.Count > 0)
            {
                site.SiteId = sites[sites.Count - 1].SiteId + 1;
                //site.Name = "未命名站点"+site.SiteId.ToString();
            }
            string html=EntityForm.Build<SiteDto>(site,true,"创建");
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Edit), new
            {
                form=html,
                tpls=Helper.GetTemplateOptions("")
            });
        }

        /// <summary>
        /// 创建站点
        /// </summary>
        [MCacheUpdate(CacheSign.Site | CacheSign.Link)]
        public void Create_POST()
        {
            string bindedDomains;

            var entity = EntityForm.GetEntity<SiteDto>();
            entity.Domain = Regex.Replace(entity.Domain, "\\s+|,+", " ");      //将多个空白替换成一个

            if (this.CheckDomainBind(-1, entity.Domain, out bindedDomains))
            {
                base.RenderError("以下域名已被绑定：<br />" + bindedDomains.Replace("\n", "<br />"));
            }
            else
            {
                ServiceCall.Instance.SiteService.SaveSite(entity);
                base.RenderSuccess("站点创建成功!");
            }
        }

        public void Edit_GET()
        {
            int siteID=int.Parse(base.Request["siteId"]??"1");

            SiteDto site =SiteCacheManager.GetSite(siteID);

            string html = EntityForm.Build<SiteDto>(site, true, "提交");
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Edit), new
            {
                form = html,
                tpls =Helper.GetTemplateOptions(site.Tpl)
            });

        }

        [MCacheUpdate(CacheSign.Site | CacheSign.Link)]
        public void Edit_POST()
        {
            string bindedDomains;

            var entity = EntityForm.GetEntity<SiteDto>();
            entity.Domain = Regex.Replace(entity.Domain, "\\s+|,+", " ");      //将多个空白替换成一个

            if (this.CheckDomainBind(entity.SiteId, entity.Domain, out bindedDomains))
            {
                base.RenderError("以下域名已被绑定：<br />" + bindedDomains.Replace("\n", "<br />"));
            }
            else
            {
                ServiceCall.Instance.SiteService.SaveSite(entity);
                base.RenderSuccess("站点保存成功!");
            }
        }
    }
}
