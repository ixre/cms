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
using System.Text.RegularExpressions;
using JR.Cms.Domain.Site;
using JR.Cms.Library.CacheProvider.CacheComponent;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Framework.Automation;
using Newtonsoft.Json;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteHandler : BasePage
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
            if (!string.IsNullOrEmpty(domainStr))
            {
                var split = new char[] {' ', ','};
                foreach (var domain in domainStr.Split(split))
                foreach (var s in SiteCacheManager.GetAllSites())
                {
                    if (string.IsNullOrEmpty(s.Domain) || siteId != -1 && siteId == s.SiteId) continue;
                    if (Array.Exists(s.Domain.Split(split),
                        a => string.Compare(a, domain, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        bindDomain += "\n" + domain;
                        break;
                    }
                }
            }

            return bindDomain != null;
        }

        public void Index()
        {
            var sites = SiteCacheManager.GetAllSites();
            var items = new TreeItem[sites.Count];
            for (var i = 0; i < sites.Count; i++)
                items[i] = new TreeItem
                {
                    ID = sites[i].SiteId,
                    Name = sites[i].Name + (sites[i].Note.Trim() != "" ? "[" + sites[i].Note + "]" : "")
                };


            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Index), new
            {
                siteTree = Helper.SingleTree("所有站点", items)
            });
        }

        public void Create()
        {
            var site = new SiteDto();
            var sites = SiteCacheManager.GetAllSites();
            if (sites.Count > 0)
                site.SiteId = sites[sites.Count - 1].SiteId + 1;
            //site.Name = "未命名站点"+site.SiteId.ToString();
            var html = EntityForm.Build<SiteDto>(site);
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Edit), new
            {
                form = html,
                post_url = "?module=site&action=create",
                tpls = Helper.GetTemplateOptions("")
            });
        }


        private SiteDto CheckSiteEntity(bool siteIsExist, SiteDto entity)
        {
            string bindedDomains;
            if (entity.AppPath.IndexOf("/", StringComparison.Ordinal) != -1)
                throw new ArgumentException("目录名不能包含\"/\"");

            if (!string.IsNullOrEmpty(entity.Domain))
            {
                entity.Domain = Regex.Replace(entity.Domain, "\\s+|,+", " "); //将多个空白替换成一个
                entity.Domain = Regex.Replace(entity.Domain, "https*://", "", RegexOptions.IgnoreCase);

                if (CheckDomainBind(siteIsExist ? entity.SiteId : -1, entity.Domain, out bindedDomains))
                    throw new ArgumentException("以下域名已被绑定：" + bindedDomains.Replace("\n", "<br />"));
            }

            return entity;
        }


        public void Edit()
        {
            var siteId = int.Parse(Request.Query("site_id").ToString() ?? "1");

            var site = SiteCacheManager.GetSite(siteId);

            var html = EntityForm.Build<SiteDto>(site);
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Edit), new
            {
                form = html,
                post_url = "?module=site&action=edit",
                tpls = Helper.GetTemplateOptions(site.Tpl)
            });
        }

        [MCache(CacheSign.Site | CacheSign.Link)]
        public string Save_POST()
        {
            var entity =Request.ParseFormToEntity<SiteDto>();
            if (entity.SiteId <= 0) return ReturnError("不允许新增站点");
            var siteIsExist = ServiceCall.Instance.SiteService.CheckSiteExists(entity.SiteId);
            //  try
            //  {
            entity = CheckSiteEntity(siteIsExist, entity);
            if (!siteIsExist) entity.SiteId = 0;
            ServiceCall.Instance.SiteService.SaveSite(entity);
            // }
            // catch (Exception exc)
            // {
            //   return base.ReturnError(exc.Message);
            //  }
            return ReturnSuccess(siteIsExist ? "站点保存成功" : "站点创建成功");
        }
        
        
        /// <summary>
        /// 站点变量
        /// </summary>
        /// <returns></returns>
        public string Variables()
        {
            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.SiteVariables));
        }

        
        public string GetVariablesJson_POST()
        {
            var list = ServiceCall.Instance.SiteService.GetSiteVariables(base.SiteId);
            return JsonConvert.SerializeObject(list);
        }

        public string CreateVariable_POST()
        {
            string key = Request.Form("key");
            try
            {
                ServiceCall.Instance.SiteService.SaveSiteVariable(SiteId,
                    new SiteVariableDto
                    {
                        Name = key,
                        Value = "",
                        Remark = ""
                    });
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                return ReturnError(exc.Message);
            }

            return ReturnSuccess("");
        }

        public string DeleteVariable_POST()
        {
            int varId = int.Parse(Request.Form("id"));
            try
            {
                ServiceCall.Instance.SiteService.DeleteSiteVariable(SiteId,varId);
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            return ReturnSuccess("");
        }

        public string SaveVariable_POST()
        {
            try
            {
                SiteVariableDto dto = new SiteVariableDto
                {
                    Id = int.Parse(Request.Form("id")),
                    Name = Request.Form("name"),
                    Value = Request.Form("value"),
                    Remark = Request.Form("remark")
                };
                ServiceCall.Instance.SiteService.SaveSiteVariable(SiteId,dto);
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            return ReturnSuccess("");
        }
    }
}