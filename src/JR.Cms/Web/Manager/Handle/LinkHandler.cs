//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
// 
// Project: jr.Cms.Manager
// FileName : Link.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/18 11:51:46
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Library.CacheProvider.CacheCompoment;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Util;
using JR.Stand.Core.Web;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 链接管理
    /// </summary>
    public class LinkHandler : BasePage
    {
        /// <summary>
        /// 数据
        /// </summary>
        public void Data()
        {
            var type = (SiteLinkType) Enum.Parse(typeof(SiteLinkType),
                Request.Query("type"), true);

            //链接列表
            var sb = new StringBuilder();
            var i = 0;

            var bindTitle = string.Empty;
            var archive = default(ArchiveDto);
            var cate = default(CategoryDto);

            IList<SiteLinkDto> links = new List<SiteLinkDto>(
                ServiceCall.Instance.SiteService.GetLinksByType(SiteId, type, true));


            #region 链接拼凑

            LinkBehavior bh = (link) =>
            {
                i++;
                sb.Append("<tr visible=\"").Append(link.Visible ? "1" : "0").Append("\" indent=\"")
                    .Append(link.Id.ToString())
                    .Append("\"><td class=\"hidden\">").Append(link.Id.ToString()).Append("</td>")
                    .Append("<td class=\"")
                    .Append(link.Pid != 0 ? "child" : "parent")
                    .Append("\">")
                    .Append(link.Visible ? link.Text : "<span style=\"color:#d0d0d0\">" + link.Text + "</span>")
                    .Append("<span class=\"micro\">(");

                if (string.IsNullOrEmpty(link.Bind))
                {
                    if (link.Uri == "")
                        sb.Append("<span style=\"color:red\">未设置</span>");
                    else
                        sb.Append(link.Uri);
                }
                else
                {
                    var binds = (link.Bind ?? "").Split(':');
                    if (binds.Length != 2 || binds[1] == string.Empty)
                    {
                        binds = null;
                    }
                    else
                    {
                        if (binds[0] == "category")
                        {
                            cate = ServiceCall.Instance.SiteService.GetCategory(SiteId, int.Parse(binds[1]));
                            bindTitle = cate.ID > 0 ? string.Format("绑定栏目：{0}", cate.Name) : null;
                        }
                        else if (binds[0] == "archive")
                        {
                            int archiveId;
                            int.TryParse(binds[1], out archiveId);

                            archive = ServiceCall.Instance.ArchiveService
                                .GetArchiveById(SiteId, archiveId);

                            if (archive.Id <= 0)
                                binds = null;
                            else
                                bindTitle = string.Format("绑定文档：{0}", archive.Title);
                        }
                    }

                    sb.Append(bindTitle);
                }

                sb.Append(")</span></td>")
                    .Append("</tr>");

                return "";
            };

            #endregion

            foreach (var t1 in links)
            {
                if (t1.Pid == 0)
                {
                    bh(t1);
                    //设置子类
                    IList<SiteLinkDto> links2 = new List<SiteLinkDto>(links.Where(a => a.Pid == t1.Id));
                    foreach (var t in links2)
                    {
                        bh(t);
                        //links.Remove(links2[k]);
                    }
                }
            }

            var linkRowsHtml = sb.Length != 0
                ? string.Concat("<table cellspacing=\"0\" class=\"ui-table\">", sb.ToString(), "</table>")
                : "<div style=\"text-align:center\">暂无链接，请点击右键添加！</div>";

            //输出分页数据
            PagerJson2("<div style=\"display:none\">for ie6</div>" + linkRowsHtml,
                $"共{i.ToString()}条");
        }

        /// <summary>
        /// 链接列表
        /// </summary>
        public string List()
        {
            var type = (SiteLinkType) Enum.Parse(typeof(SiteLinkType),
                Request.Query("type"), true);
            string linkTypeName;
            switch (type)
            {
                case SiteLinkType.FriendLink:
                    linkTypeName = "友情链接";
                    break;
                default:
                case SiteLinkType.CustomLink:
                    linkTypeName = "自定义链接";
                    break;
                case SiteLinkType.Navigation:
                    linkTypeName = "网站导航";
                    break;
            }


            ViewData["link_type"] = Request.Query("type");
            ViewData["type_name"] = linkTypeName;
            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Link_List));
        }

        /// <summary>
        /// 创建链接
        /// </summary>
        public string Create()
        {
            var parentOptions = "";
            string linkType = Request.Query("type");
            var type = (SiteLinkType) Enum.Parse(typeof(SiteLinkType), linkType, true);
            string linkTypeName;
            string resource;
            switch (type)
            {
                case SiteLinkType.FriendLink:
                    linkTypeName = "友情链接";
                    resource = ResourceMap.GetPageContent(ManagementPage.Link_Edit);
                    break;
                default:
                case SiteLinkType.CustomLink:
                    linkTypeName = "自定义链接";
                    resource = ResourceMap.GetPageContent(ManagementPage.Link_Edit);
                    break;
                case SiteLinkType.Navigation:
                    linkTypeName = "网站导航";
                    resource = ResourceMap.GetPageContent(ManagementPage.Link_Edit_Navigator);
                    break;
            }
            // ParentOptions
            var sb = new StringBuilder();
            var siteId = CurrentSite.SiteId;

            var parentLinks = ServiceCall.Instance.SiteService
                .GetLinksByType(SiteId, type, true);

            foreach (var _link in parentLinks)
                if (_link.Pid == 0)
                    sb.Append("<option value=\"").Append(_link.Id.ToString())
                        .Append("\">").Append(_link.Text).Append("</option>");
            parentOptions = sb.ToString();
            var json = JsonSerializer.Serialize(
                new
                {
                    Id = 0,
                    Text = string.Empty,
                    Uri = string.Empty,
                    SortNumber = "0",
                    Btn = "添加",
                    BindId = string.Empty,
                    BindType = string.Empty,
                    BindTitle = "未绑定",
                    Target = string.Empty,
                    Type = int.Parse(linkType),
                    ImgUrl = string.Empty,
                    Pid = 0,
                    Visible = "True"
                });

            ViewData["entity"] = json;
            ViewData["link_type"] = (int) type;
            ViewData["form_title"] = "创建" + linkTypeName;
            ViewData["category_opts"] = Helper.GetCategoryIdSelector(SiteId, -1);
            ViewData["parent_opts"] = parentOptions;
            ViewData["site_id"] = siteId;

            return RequireTemplate(resource);
        }


        /// <summary>
        /// 更新链接
        /// </summary>
        public string Edit()
        {
            var linkId = int.Parse(Request.Query("link_id"));
            var bindId = 0;
            var siteId = SiteId;
            var categoryId = 0;
            var parentOptions = "";

            var link = ServiceCall.Instance.SiteService.GetLinkById(SiteId, linkId);

            var bindTitle = string.Empty;
            var binds = (link.Bind ?? "").Split(':');
            if (binds.Length != 2 || binds[1] == string.Empty)
            {
                binds = null;
            }
            else
            {
                bindId = int.Parse(binds[1]);

                if (binds[0] == "category")
                {
                    var cate = ServiceCall.Instance.SiteService.GetCategory(SiteId, bindId);

                    bindTitle = cate.ID > 0 ? $"栏目：{cate.Name}" : null;
                    categoryId = cate.ID;
                }
                else if (binds[0] == "archive")
                {
                    var archive = ServiceCall.Instance.ArchiveService
                        .GetArchiveById(SiteId, bindId);

                    if (archive.Id <= 0)
                        binds = null;
                    else
                        bindTitle = $"文档：{archive.Title}";
                }
            }

            string linkTypeName;
             string   resource;

            switch (link.Type)
            {
                case SiteLinkType.FriendLink:
                    linkTypeName = "友情链接";
                    resource = ResourceMap.GetPageContent(ManagementPage.Link_Edit);
                    break;
                default:
                case SiteLinkType.CustomLink:
                    linkTypeName = "自定义链接";
                    resource = ResourceMap.GetPageContent(ManagementPage.Link_Edit);
                    break;
                case SiteLinkType.Navigation:
                    linkTypeName = "网站导航";
                    resource = ResourceMap.GetPageContent(ManagementPage.Link_Edit_Navigator);
                    break;
            }

            // ParentOptions
            var sb = new StringBuilder();
            var parentLinks = ServiceCall.Instance.SiteService
                .GetLinksByType(SiteId, link.Type, true);

            foreach (var _link in parentLinks)
            {
                if (_link.Pid == 0)
                    sb.Append("<option value=\"").Append(_link.Id.ToString())
                        .Append("\">").Append(_link.Text).Append("</option>");
                parentOptions = sb.ToString();
            }

            var json = JsonSerializer.Serialize(
                new
                {
                    Id = link.Id,
                    Text = link.Text,
                    Uri = link.Uri,
                    SortNumber = link.SortNumber,
                    Btn = "保存",
                    BindId = bindId,
                    BindType = binds == null ? "" : binds[0],
                    BindTitle = bindTitle == string.Empty ? "未绑定" : bindTitle,
                    Target = link.Target,
                    Type = (int) link.Type,
                    ImgUrl = link.ImgUrl,
                    Visible = link.Visible.ToString(),
                    Pid = link.Pid,
                    CategoryId = categoryId
                });

            ViewData["entity"] = json;
            ViewData["link_type"] = (int) link.Type;
            ViewData["form_title"] = "修改" + linkTypeName;
            ViewData["category_opts"] = Helper.GetCategoryIdSelector(SiteId, -1);
            ViewData["parent_opts"] = parentOptions;
            ViewData["site_id"] = siteId;

            return RequireTemplate(resource);
        }

        [MCacheUpdate(CacheSign.Link)]
        public string Save_POST()
        {
            var link = default(SiteLinkDto);

            int.TryParse(Request.Form("Id").ToString() ?? "0", out var linkId);

            string bindType = Request.Form("bindType"),
                bindId = Request.Form("bindId");

            if (linkId > 0) link = ServiceCall.Instance.SiteService.GetLinkById(SiteId, linkId);

            link.ImgUrl = Request.Form("ImgUrl");
            link.SortNumber = int.Parse(Request.Form("SortNumber"));
            link.Pid = int.Parse(Request.Form("Pid"));
            link.Target = Request.Form("Target");
            link.Text = Request.Form("Text").ToString().Trim();
            link.Type = (SiteLinkType) int.Parse(Request.Form("type"));
            link.Uri = Request.Form("Uri");
            link.Visible = Request.Form("visible") == "True";


            if (!string.IsNullOrEmpty(bindType) && Regex.IsMatch(bindId, "^\\d+$"))
            {
                link.Bind = $"{bindType}:{bindId}";
            }
            else
            {
                link.Bind = string.Empty;
            }

            var id = ServiceCall.Instance.SiteService.SaveLink(SiteId, link);

            return ReturnSuccess();
        }


        /// <summary>
        /// 设置链接可见
        /// </summary>
        [MCacheUpdate(CacheSign.Link)]
        public string Set_visible_POST()
        {
            var linkId = int.Parse(Request.Form("link_id"));
            var link = ServiceCall.Instance.SiteService.GetLinkById(SiteId, linkId);
            link.Visible = !link.Visible;
            var id = ServiceCall.Instance.SiteService.SaveLink(SiteId, link);

            return ReturnSuccess();
        }

        /// <summary>
        /// 查看是否可见
        /// </summary>
        public void GetVisible_POST()
        {
            var linkId = int.Parse(Request.Form("link_id"));

            var link = ServiceCall.Instance.SiteService.GetLinkById(SiteId, linkId);

            if (link.Visible)
                RenderSuccess();
            else
                RenderError("链接不可见");
        }

        /// <summary>
        /// 删除链接
        /// </summary>
        [MCacheUpdate(CacheSign.Link)]
        public void Delete_POST()
        {
            var linkId = int.Parse(Request.Form("link_id"));
            ServiceCall.Instance.SiteService.DeleteLink(SiteId, linkId);
            RenderSuccess();
        }


        public string Related_link()
        {
            ViewData["indent_opts"] = GetContentRelatedIndentOptions();
            ViewData["content_type"] = Request.Query("content_type");
            ViewData["content_id"] = Request.Query("content_id");
            ViewData["scheme"] = Request.GetScheme() + ":";
            ViewData["host"] = WebCtx.Current.Host;
            ViewData["site_id"] = SiteId.ToString();

            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Link_RelatedLink));
        }

        private string GetContentRelatedIndentOptions()
        {
            var sb = new StringBuilder();
            var indents = ServiceCall.Instance.ContentService.GetRelatedIndents();

            string siteLimit;
            string cateLimit;
            foreach (var indent in indents)
                if (indent.Value.Enabled)
                {
                    if (indent.Value.SiteLimit == "-")
                        siteLimit = CurrentSite.SiteId.ToString();
                    else if (indent.Value.SiteLimit == "*")
                        siteLimit = "";
                    else
                        siteLimit = indent.Value.SiteLimit;

                    if (indent.Value.CategoryLimit == "*")
                        cateLimit = "";
                    else
                        cateLimit = indent.Value.CategoryLimit;

                    sb.Append("<option site-lmt=\"").Append(siteLimit).Append("\" cate-lmt=\"")
                        .Append(cateLimit).Append("\" value=\"").Append(indent.Key.ToString())
                        .Append("\"> ").Append(indent.Value.Name).Append(" - [").Append(indent.Key.ToString())
                        .Append("]</option>");
                }

            return sb.ToString();
        }

        public void Related_link_POST()
        {
            var links = ServiceCall.Instance.ContentService
                .GetRelatedLinks(
                    SiteId,
                    Request.Form("ContentType"),
                    int.Parse(Request.Form("ContentId")));
            PagerJson(links, "共" + links.Count().ToString() + "条");
        }

        public string Save_related_link_POST()
        {
            try
            {
                var dto = new RelatedLinkDto
                {
                    Id = int.Parse(Request.Form("Id")),
                    ContentId = int.Parse(Request.Form("ContentId")),
                    ContentType = Request.Form("ContentType"),
                    RelatedIndent = int.Parse(Request.Form("RelatedIndent")),
                    RelatedSiteId = int.Parse(Request.Form("RelatedSiteId")),
                    RelatedContentId = int.Parse(Request.Form("RelatedContentId")),
                    Enabled = Request.Form("Enabled") == "1",
                };

                ServiceCall.Instance.ContentService.SaveRelatedLink(SiteId, dto);

                return ReturnSuccess(dto.Id == 0 ? "添加成功" : "保存成功");
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }
        }

        public string Delete_related_link_POST()
        {
            try
            {
                ServiceCall.Instance.ContentService.RemoveRelatedLink(
                    SiteId,
                    Request.Query("contentType"),
                    int.Parse(Request.Query("contentId")),
                    int.Parse(Request.Query("id"))
                );
                return ReturnSuccess();
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }
        }
    }
}