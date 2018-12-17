//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
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
using System.Web;
using JR.Cms.Cache.CacheCompoment;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.DataTransfer;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.WebManager;
using JR.DevFw.Web;

namespace JR.Cms.Web.WebManager.Handle
{
    /// <summary>
    /// 链接管理
    /// </summary>
    public class LinkC : BasePage
    {

        /// <summary>
        /// 数据
        /// </summary>
        public void Data_GET()
        {
            SiteLinkType type = (SiteLinkType)Enum.Parse(typeof(SiteLinkType), HttpContext.Current.Request["type"], true);

            string linkRowsHtml;
            //链接列表
            StringBuilder sb = new StringBuilder();
            int i = 0;

            string bindTitle = String.Empty;
            ArchiveDto archive = default(ArchiveDto);
            CategoryDto cate = default(CategoryDto);

            IList<SiteLinkDto> links = new List<SiteLinkDto>(
                ServiceCall.Instance.SiteService.GetLinksByType(this.SiteId, type, true));
            

            #region 链接拼凑

            LinkBehavior bh = (link) =>
            {
                i ++;
                sb.Append("<tr visible=\"").Append(link.Visible ? "1" : "0").Append("\" indent=\"").Append(link.Id.ToString())
                       .Append("\"><td class=\"hidden\">").Append(link.Id.ToString()).Append("</td>")
                       .Append("<td class=\"")
                       .Append(link.Pid != 0 ? "child" : "parent")
                       .Append("\">")
                       .Append(link.Visible ? link.Text : "<span style=\"color:#d0d0d0\">" + link.Text + "</span>")
                       .Append("<span class=\"micro\">(");

                if (String.IsNullOrEmpty(link.Bind))
                {
                    if (link.Uri == "")
                    {
                        sb.Append("<span style=\"color:red\">未设置</span>");
                    }
                    else
                    {
                        sb.Append(link.Uri);
                    }
                }
                else
                {
                    string[] binds = (link.Bind ?? "").Split(':');
                    if (binds.Length != 2 || binds[1] == String.Empty)
                    {
                        binds = null;
                    }
                    else
                    {
                        if (binds[0] == "category")
                        {

                            cate = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, int.Parse(binds[1]));
                            bindTitle = cate.ID > 0 ?
                                String.Format("绑定栏目：{0}", cate.Name) :
                                null;
                        }
                        else if (binds[0] == "archive")
                        {
                            int archiveId;
                            int.TryParse(binds[1], out archiveId);

                            archive = ServiceCall.Instance.ArchiveService
                                .GetArchiveById(this.SiteId, archiveId);

                            if (archive.Id <= 0)
                            {
                                binds = null;
                            }
                            else
                            {
                                bindTitle = String.Format("绑定文档：{0}", archive.Title);
                            }
                        }
                    }

                    sb.Append(bindTitle);
                }

                sb.Append(")</span></td>")
                .Append("</tr>");

                return "";
            };


            #endregion

            IList<SiteLinkDto> links2;
            for (int j = 0; j < links.Count; j++)
            {
                if (links[j].Pid == 0)
                {
                    bh(links[j]);

                    //设置子类
                    links2 = new List<SiteLinkDto>(links.Where(a => a.Pid == links[j].Id));
                    for (int k = 0; k < links2.Count; k++)
                    {
                        bh(links2[k]);
                        //links.Remove(links2[k]);
                    }
                }
            }



            linkRowsHtml = sb.Length != 0 ?
                String.Concat("<table cellspacing=\"0\" class=\"ui-table\">", sb.ToString(), "</table>") :
                "<div style=\"text-align:center\">暂无链接，请点击右键添加！</div>";

            //输出分页数据
            base.PagerJson2("<div style=\"display:none\">for ie6</div>" + linkRowsHtml, String.Format("共{0}条", i.ToString()));
        }

        /// <summary>
        /// 链接列表
        /// </summary>
        public string List_GET()
        {
            SiteLinkType type = (SiteLinkType)Enum.Parse(typeof(SiteLinkType), HttpContext.Current.Request["type"], true);
            string linkTypeName;
            switch (type)
            {
                case SiteLinkType.FriendLink: linkTypeName = "友情链接"; break;
                default:
                case SiteLinkType.CustomLink: linkTypeName = "自定义链接"; break;
                case SiteLinkType.Navigation: linkTypeName = "网站导航"; break;
            }


            ViewData["link_type"] = base.Request["type"];
            ViewData["type_name"] = linkTypeName;
           return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Link_List));

        }

        /// <summary>
        /// 创建链接
        /// </summary>
        public string Create_GET()
        {
            object data;
            string plinks = "";


            SiteLinkType type = (SiteLinkType)Enum.Parse(typeof(SiteLinkType), base.Request["type"], true);
            string linkTypeName,
                   resouce;

            switch (type)
            {
                case SiteLinkType.FriendLink: linkTypeName = "友情链接"; resouce = ResourceMap.GetPageContent(ManagementPage.Link_Edit); break;
                default:
                case SiteLinkType.CustomLink: linkTypeName = "自定义链接"; resouce = ResourceMap.GetPageContent(ManagementPage.Link_Edit); break;
                case SiteLinkType.Navigation: linkTypeName = "网站导航"; resouce = ResourceMap.GetPageContent(ManagementPage.Link_Edit_Navigator); break;
            }


            //plinks
            StringBuilder sb = new StringBuilder();
            int siteId = base.CurrentSite.SiteId;

            IEnumerable<SiteLinkDto> parentLinks = ServiceCall.Instance.SiteService
                           .GetLinksByType(this.SiteId, type, true);

            foreach (SiteLinkDto _link in parentLinks)
            {
                if (_link.Pid == 0)
                {
                    sb.Append("<option value=\"").Append(_link.Id.ToString())
                        .Append("\">").Append(_link.Text).Append("</option>");
                }
            }
            plinks = sb.ToString();

            string json = JsonSerializer.Serialize(
            new
            {
                Id = 0,
                Text = String.Empty,
                Uri = String.Empty,
                SortNumber = "0",
                Btn = "添加",
                BindId = String.Empty,
                BindType = String.Empty,
                BindTitle = "未绑定",
                Target = String.Empty,
                Type = Request["type"],
                ImgUrl = String.Empty,
                pid = '0',
                Visible = "True"
            });

            ViewData["entity"] = json;
            ViewData["link_type"] = (int)type;
            ViewData["form_title"] = "创建" + linkTypeName;
            ViewData["category_opts"] = Helper.GetCategoryIdSelector(this.SiteId, -1);
            ViewData["parent_opts"] = plinks;
            ViewData["site_id"] = siteId;

            return base.RequireTemplate(resouce);

        }




        /// <summary>
        /// 更新链接
        /// </summary>
        public string Edit_GET()
        {

            int linkId = int.Parse(base.Request.QueryString["link_id"]);
            int bindId = 0;
            int siteId = this.SiteId;
            int categoryId = 0;
            string plinks = "";

            SiteLinkDto link = ServiceCall.Instance.SiteService.GetLinkById(this.SiteId, linkId);

            string bindTitle = String.Empty;
            string[] binds = (link.Bind ?? "").Split(':');
            if (binds.Length != 2 || binds[1] == String.Empty)
            {
                binds = null;
            }
            else
            {

                bindId = int.Parse(binds[1]);

                if (binds[0] == "category")
                {
                    CategoryDto cate = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, bindId);

                    bindTitle = cate.ID > 0 ?
                        String.Format("栏目：{0}", cate.Name) :
                        null;
                    categoryId = cate.ID;
                }
                else if (binds[0] == "archive")
                {
                    ArchiveDto archive = ServiceCall.Instance.ArchiveService
                        .GetArchiveById(this.SiteId, bindId);

                    if (archive.Id <= 0)
                    {
                        binds = null;
                    }
                    else
                    {
                        bindTitle = String.Format("文档：{0}", archive.Title);
                    }
                }
            }

            string linkTypeName,
                  resouce;

            switch ((SiteLinkType)link.Type)
            {
                case SiteLinkType.FriendLink: linkTypeName = "友情链接";
                    resouce = ResourceMap.GetPageContent(ManagementPage.Link_Edit);
                    break;
                default:
                case SiteLinkType.CustomLink: linkTypeName = "自定义链接";
                    resouce = ResourceMap.GetPageContent(ManagementPage.Link_Edit);
                    break;
                case SiteLinkType.Navigation: linkTypeName = "网站导航";
                    resouce = ResourceMap.GetPageContent(ManagementPage.Link_Edit_Navigator);
                    break;
            }

            //plinks
            StringBuilder sb = new StringBuilder();
            IEnumerable<SiteLinkDto> parentLinks = ServiceCall.Instance.SiteService
                .GetLinksByType(this.SiteId, link.Type, true);

            foreach (SiteLinkDto _link in parentLinks)
            {
                if (_link.Pid == 0)
                {
                    sb.Append("<option value=\"").Append(_link.Id.ToString())
                        .Append("\">").Append(_link.Text).Append("</option>");
                }
            }
            plinks = sb.ToString();

            string json = JsonSerializer.Serialize(
           new
           {
               Id = link.Id,
               Text = link.Text,
               Uri = link.Uri,
               SortNumber = link.SortNumber,
               Btn = "保存",
               BindId = bindId,
               BindType = binds == null ? "" : binds[0],
               BindTitle = bindTitle == String.Empty ? "未绑定" : bindTitle,
               Target = link.Target,
               Type = (int)link.Type,
               ImgUrl = link.ImgUrl,
               Visible = link.Visible.ToString(),
               Pid = link.Pid,
               CategoryId = categoryId
           });

            ViewData["entity"] = json;
            ViewData["link_type"] = (int) link.Type;
            ViewData["form_title"] = "修改" + linkTypeName;
            ViewData["category_opts"] = Helper.GetCategoryIdSelector(this.SiteId, -1);
            ViewData["parent_opts"] = plinks;
            ViewData["site_id"] = siteId;

            return base.RequireTemplate(resouce);

        }
        [MCacheUpdate(CacheSign.Link)]
        public string Save_POST()
        {
            HttpRequest request = HttpContext.Current.Request;
            SiteLinkDto link = default(SiteLinkDto);

            int linkId = 0;
            int.TryParse(request["Id"] ?? "0", out linkId);

            string bindtype = request.Form["bindtype"],
                   bindId = request.Form["bindid"];

            if (linkId > 0) link = ServiceCall.Instance
                .SiteService.GetLinkById(this.SiteId, linkId);

            link.ImgUrl = base.Request.Form["ImgUrl"];
            link.SortNumber = int.Parse(base.Request.Form["SortNumber"]);
            link.Pid = int.Parse(base.Request.Form["Pid"]);
            link.Target = base.Request.Form["Target"];
            link.Text = base.Request.Form["Text"].Trim();
            link.Type = (SiteLinkType)int.Parse(base.Request.Form["type"]);
            link.Uri = base.Request.Form["Uri"];
            link.Visible = base.Request.Form["visible"] == "True";


            if (!String.IsNullOrEmpty(bindtype)
                && Regex.IsMatch(bindId, "^\\d+$"))
            {
                link.Bind = String.Format("{0}:{1}", bindtype, bindId);
            }
            else
            {
                link.Bind = String.Empty;
            }

            int id = ServiceCall.Instance.SiteService.SaveLink(this.SiteId, link);

            return base.ReturnSuccess();
        }


        /// <summary>
        /// 设置链接可见
        /// </summary>
        [MCacheUpdate(CacheSign.Link)]
        public string Set_visible_POST()
        {
            int linkId = int.Parse(base.Request.Form["link_id"]);
            SiteLinkDto link = ServiceCall.Instance.SiteService.GetLinkById(this.SiteId, linkId);
            link.Visible = !link.Visible;
            int id = ServiceCall.Instance.SiteService.SaveLink(this.SiteId, link);

            return base.ReturnSuccess();
        }

        /// <summary>
        /// 查看是否可见
        /// </summary>
        public void GetVisible_POST()
        {
            int linkId = int.Parse(base.Request.Form["link_id"]);

            SiteLinkDto link = ServiceCall.Instance.SiteService.GetLinkById(this.SiteId, linkId);

            if (link.Visible)
            {
                base.RenderSuccess();
            }
            else
            {
                base.RenderError("链接不可见");
            }
        }

        /// <summary>
        /// 删除链接
        /// </summary>
        [MCacheUpdate(CacheSign.Link)]
        public void Delete_POST()
        {
            int linkId = int.Parse(base.Request.Form["link_id"]);
            ServiceCall.Instance.SiteService.DeleteLink(this.SiteId, linkId);
            base.RenderSuccess();
        }



        public string Related_link_GET()
        {
            ViewData["indent_opts"] = this.GetContentRelatedIndentOptions();
            ViewData["content_type"] = Request.QueryString["content_type"];
            ViewData["content_id"] = Request.QueryString["content_id"];
            ViewData["scheme"] = Request.Url.Scheme + ":";
            ViewData["host"] = WebCtx.Current.Host;
            ViewData["site_id"] = this.SiteId.ToString();

            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Link_RelatedLink));
        }

        private string GetContentRelatedIndentOptions()
        {
            StringBuilder sb = new StringBuilder();
            IDictionary<int, RelateIndent> indents = ServiceCall.Instance.ContentService.GetRelatedIndents();

            string siteLimit;
            string cateLimit;
            foreach (var indent in indents)
            {
                if (indent.Value.Enabled)
                {
                    if (indent.Value.SiteLimit == "-")
                    {
                        siteLimit = this.CurrentSite.SiteId.ToString();
                    }
                    else if (indent.Value.SiteLimit == "*")
                    {
                        siteLimit = "";
                    }
                    else
                    {
                        siteLimit = indent.Value.SiteLimit;
                    }

                    if (indent.Value.CategoryLimit == "*")
                    {
                        cateLimit = "";
                    }
                    else
                    {
                        cateLimit = indent.Value.CategoryLimit;
                    }

                    sb.Append("<option site-lmt=\"").Append(siteLimit).Append("\" cate-lmt=\"")
                        .Append(cateLimit).Append("\" value=\"").Append(indent.Key.ToString())
                        .Append("\"> ").Append(indent.Value.Name).Append(" - [").Append(indent.Key.ToString()).Append("]</option>");
                }
            }
            return sb.ToString();
        }

        public void Related_link_POST()
        {
            IEnumerable<RelatedLinkDto> links = ServiceCall.Instance.ContentService
                .GetRelatedLinks(
                    this.SiteId,
                    Request.Form["ContentType"],
                    int.Parse(Request.Form["ContentId"]));
            base.PagerJson(links, "共" + links.Count().ToString() + "条");
        }

        public string Save_related_link_POST()
        {
            try
            {
                RelatedLinkDto dto = new RelatedLinkDto
                {
                    Id = int.Parse(Request.Form["Id"]),
                    ContentId = int.Parse(Request.Form["ContentId"]),
                    ContentType = Request.Form["ContentType"],
                    RelatedIndent = int.Parse(Request.Form["RelatedIndent"]),
                    RelatedSiteId = int.Parse(Request.Form["RelatedSiteId"]),
                    RelatedContentId = int.Parse(Request.Form["RelatedContentId"]),
                    Enabled = Request.Form["Enabled"] == "1",
                };

                ServiceCall.Instance.ContentService.SaveRelatedLink(this.SiteId,dto);

                return base.ReturnSuccess(dto.Id == 0 ? "添加成功" : "保存成功");

            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }

        }
        public string Delete_related_link_POST()
        {
            try
            {
                ServiceCall.Instance.ContentService.RemoveRelatedLink(
                    this.SiteId,
                    Request["contentType"],
                    int.Parse(Request["contentId"]),
                    int.Parse(Request["id"])
                    );
                return base.ReturnSuccess();
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }
        }

    }
}
