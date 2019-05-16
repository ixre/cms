//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Cms.Cache.CacheCompoment;
using JR.Cms.CacheService;
using JR.Cms.DataTransfer;
using JR.Cms.WebManager;
using JR.DevFw.Framework.Automation;

namespace JR.Cms.Web.WebManager.Handle
{
    public class SiteC: BasePage
    {

        /// <summary>
        /// 检查域名是否已经绑定
        /// </summary>
        /// <param name="siteId">当前站点编号，-1表示不限制</param>
        /// <param name="domainStr"></param>
        /// <param name="bindDomain"></param>
        /// <returns></returns>
        private bool CheckDomainBind(int siteId, string domainStr, out string bindDomain)
        {
            bindDomain = null;
            if (!String.IsNullOrEmpty(domainStr))
            {
                char[] split = new char[] { ' ', ',' };
                foreach (string domain in domainStr.Split(split))
                {
                    foreach (SiteDto s in SiteCacheManager.GetAllSites())
                    {
                        if (String.IsNullOrEmpty(s.Domain) || (siteId!=-1&&siteId==s.SiteId) ) continue;
                        if (Array.Exists(s.Domain.Split(split), a => String.Compare(a, domain, StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            bindDomain += "\n" + domain;
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
            string html=EntityForm.Build<SiteDto>(site);
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Edit), new
            {
                form=html,
                post_url = "?module=site&action=create",
                tpls=Helper.GetTemplateOptions("")
            });
        }


        private SiteDto CheckSiteEntity(bool siteIsExist, SiteDto entity)
        {
            string bindedDomains;
            if (entity.AppPath.IndexOf("/", StringComparison.Ordinal) != -1)
            {
                throw new ArgumentException("目录名不能包含\"/\"");
            }

            if (!String.IsNullOrEmpty(entity.Domain))
            {
                entity.Domain = Regex.Replace(entity.Domain, "\\s+|,+", " "); //将多个空白替换成一个
                entity.Domain = Regex.Replace(entity.Domain, "https*://", "", RegexOptions.IgnoreCase);

                if (this.CheckDomainBind(siteIsExist ? entity.SiteId : -1, entity.Domain, out bindedDomains))
                {
                    throw new ArgumentException("以下域名已被绑定：" + bindedDomains.Replace("\n", "<br />"));
                }
            }
            return entity;
        }
    

        public void Edit_GET()
        {
            int siteId=int.Parse(base.Request["site_id"]??"1");

            SiteDto site =SiteCacheManager.GetSite(siteId);

            string html = EntityForm.Build<SiteDto>(site);
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Edit), new
            {
                form = html,
                post_url = "?module=site&action=edit",
                tpls =Helper.GetTemplateOptions(site.Tpl)
            });
        }

        [MCacheUpdate(CacheSign.Site | CacheSign.Link)]
        public string Save_POST()
        {
            var entity = EntityForm.GetEntity<SiteDto>();
            if(entity.SiteId <= 0)
            {
                return this.ReturnError("不允许新增站点");
            }
            bool siteIsExist = ServiceCall.Instance.SiteService.CheckSiteExists(entity.SiteId);
          //  try
          //  {
                entity = CheckSiteEntity(siteIsExist,entity);
                if (!siteIsExist)entity.SiteId = 0;
                ServiceCall.Instance.SiteService.SaveSite(entity);
           // }
           // catch (Exception exc)
           // {
            //   return base.ReturnError(exc.Message);
          //  }
            return base.ReturnSuccess(siteIsExist ? "站点保存成功" : "站点创建成功");
        }
    }
}
