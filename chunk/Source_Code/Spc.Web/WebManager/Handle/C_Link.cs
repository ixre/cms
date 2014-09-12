//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: OPS.OPSite.Manager
// FileName : Link.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/18 11:51:46
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//
using Ops.Cms.Cache.CacheCompoment;
using Ops.Cms.Conf;

namespace Ops.Cms.WebManager
{
    using Ops.Cms.Cache;
    using Ops.Cms.CacheService;
    using Ops.Cms.DataTransfer;
    using Ops.Cms.Domain.Interface.Site.Link;
    using Ops.Cms.Web;
    using Ops.Framework.Automation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// 链接管理
    /// </summary>
    public class C_Link : BasePage
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
                ServiceCall.Instance.SiteService.GetLinksByType(this.SiteId, type,true));


            #region 链接拼凑

            LinkBehavior bh = (link) =>
            {
                sb.Append("<tr visible=\"").Append(link.Visible ? "1" : "0").Append("\" indent=\"").Append(link.ID.ToString())
                       .Append("\"><td class=\"hidden\">").Append(link.ID.ToString()).Append("</td>")
                       .Append("<td align=\"center\">").Append((++i).ToString()).Append("</td><td><b>")
                       .Append(link.Pid != 0 ? "&nbsp;&nbsp;" : "")
                       .Append(link.Visible ? link.Text : "<span style=\"color:#d0d0d0\">" + link.Text + "</span>")
                       .Append("</b>&nbsp;<span class=\"micro\">(");

                if (String.IsNullOrEmpty(link.Bind))
                {
                    sb.Append(link.Uri);
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

                sb.Append(")</span></td><td class=\"center\">").Append(link.OrderIndex.ToString()).Append("</td>")
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
                    links2 = new List<SiteLinkDto>(links.Where(a => a.Pid == links[j].ID));
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
        public void List_GET()
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


            //显示页面
            object data = new
            {
                linktype = base.Request["type"],
                linkTypeName = linkTypeName

            };
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Link_SiteLinkList), data);

        }

        /// <summary>
        /// 创建链接
        /// </summary>
        public void Create_GET()
        {
            object data;
            string plinks = "";


            SiteLinkType type = (SiteLinkType)Enum.Parse(typeof(SiteLinkType), base.Request["type"], true);
            string linkTypeName,
                   resouce;

            switch (type)
            {
                case SiteLinkType.FriendLink: linkTypeName = "友情链接"; resouce = ResourceMap.GetPageContent(ManagementPage.Link_SiteLinkEdit); break;
                default:
                case SiteLinkType.CustomLink: linkTypeName = "自定义链接"; resouce = ResourceMap.GetPageContent(ManagementPage.Link_SiteLinkEdit); break;
                case SiteLinkType.Navigation: linkTypeName = "网站导航"; resouce = ResourceMap.GetPageContent(ManagementPage.Link_SiteLinkEdit_Navigator); break;
            }


            //plinks
            StringBuilder sb = new StringBuilder();
            int siteID = base.CurrentSite.SiteId;

            IEnumerable<SiteLinkDto> parentLinks = ServiceCall.Instance.SiteService
                           .GetLinksByType(this.SiteId, type,true);

            foreach (SiteLinkDto _link in parentLinks)
            {
                if (_link.Pid == 0)
                {
                    sb.Append("<option value=\"").Append(_link.ID.ToString())
                        .Append("\">").Append(_link.Text).Append("</option>");
                }
            }
            plinks = sb.ToString();

            string json = JsonSerializer.Serialize(
            new
            {
                ID = 0,
                Text = String.Empty,
                Uri = String.Empty,
                OrderIndex = "0",
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

            base.RenderTemplate(resouce, new
            {
                entity = json,
                LinkType = Request["type"],
                linkTypeName = linkTypeName,
                categoryNodes = this.GetCategorySelector(siteID, -1),
                plinks = plinks
            });
        }




        /// <summary>
        /// 更新链接
        /// </summary>
        public void Edit_GET()
        {
            object data;
            int linkId = int.Parse(base.Request.QueryString["linkId"]);
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
                if (binds[0] == "category")
                {
                    CategoryDto cate = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, int.Parse(binds[1]));

                    bindTitle = cate.ID > 0 ?
                        String.Format("栏目：{0}", cate.Name) :
                        null;

                }
                else if (binds[0] == "archive")
                {

                    int archiveId;
                    int.TryParse(binds[1], out archiveId);

                    ArchiveDto archive = ServiceCall.Instance.ArchiveService
                        .GetArchiveById(this.SiteId, archiveId);

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
                    resouce = ResourceMap.GetPageContent(ManagementPage.Link_SiteLinkEdit);
                    break;
                default:
                case SiteLinkType.CustomLink: linkTypeName = "自定义链接";
                    resouce = ResourceMap.GetPageContent(ManagementPage.Link_SiteLinkEdit);
                    break;
                case SiteLinkType.Navigation: linkTypeName = "网站导航";
                    resouce = ResourceMap.GetPageContent(ManagementPage.Link_SiteLinkEdit_Navigator);
                    break;
            }

            //plinks
            StringBuilder sb = new StringBuilder();
            IEnumerable<SiteLinkDto> parentLinks = ServiceCall.Instance.SiteService
                .GetLinksByType(this.SiteId, link.Type,true);

            foreach (SiteLinkDto _link in parentLinks)
            {
                if (_link.Pid == 0)
                {
                    sb.Append("<option value=\"").Append(_link.ID.ToString())
                        .Append("\">").Append(_link.Text).Append("</option>");
                }
            }
            plinks = sb.ToString();


            int categoryId = -1;
            if (link.Bind != null && link.Bind.StartsWith("category:"))
            {
                categoryId = int.Parse(binds[1]);
            }

            string json = JsonSerializer.Serialize(
           new
           {
               ID = link.ID,
               Text = link.Text,
               Uri = link.Uri,
               OrderIndex = link.OrderIndex,
               Btn = "保存",
               BindId = binds == null ? "" : binds[1],
               BindType = binds == null ? "" : binds[0],
               BindTitle = bindTitle == String.Empty ? "未绑定" : bindTitle,
               Target = link.Target,
               Type = (int)link.Type,
               ImgUrl = link.ImgUrl,
               Visible = link.Visible.ToString(),
               pid = '0'
           });

            base.RenderTemplate(resouce, new
            {
                entity = json,
                LinkType = (int)link.Type,
                linkTypeName = linkTypeName,
                categoryNodes = this.GetCategorySelector(this.SiteId, -1),
                plinks = plinks
            });

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

            //link = base.Request.Form.BindToEntity(link);

            link.ImgUrl = base.Request.Form["imgurl"];
            link.OrderIndex = int.Parse(base.Request.Form["orderindex"]);
            link.Pid = int.Parse(base.Request.Form["pid"]);
            link.Target = base.Request.Form["target"];
            link.Text = base.Request.Form["text"];
            link.Type = (SiteLinkType)int.Parse(base.Request.Form["type"]);
            link.Uri = base.Request.Form["uri"];
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
        /// 更新提交
        /// </summary>
        //[MCacheUpdate(CacheSign.Link)]
        //public void Edit_POST()
        //{
        //    HttpRequest request = HttpContext.Current.Request;
        //    Link link = CmsLogic.Link.Get(a => a.ID == linkID);

        //    if (base.CompareSite(CmsLogic.Link.Get(a => a.ID == linkID).SiteId)) return;

        //    string text = request.Form["text"],
        //           uri = request.Form["uri"],
        //           target = request.Form["target"],
        //           imgurl=request.Form["imgurl"],
        //           bindtype = request.Form["bindtype"],
        //           bindID = request.Form["bindid"];

        //    bool isRightBind=true;


        //    if (String.IsNullOrEmpty(bindtype))
        //    {
        //        link.Bind = String.Empty;
        //        isRightBind = false;
        //    }
        //    else if (bindtype == "category")
        //    {
        //        if(!Regex.IsMatch(bindID, "^\\d+$"))
        //        {
        //            isRightBind = false;
        //        }
        //    }
        //    else if (bindtype != "archive")
        //    {
        //        isRightBind = false;
        //    }
        //    if(isRightBind)
        //    {
        //        link.Bind = String.Format("{0}:{1}", bindtype, bindID);
        //    }


        //    LinkType type = (LinkType)int.Parse(request.Form["linktype"]);

        //    int index = 0;
        //    int.TryParse(request.Form["index"], out index);

        //    //判断pid修改
        //    int pid =int.Parse(request["pid"]);
        //    if (link.Pid != pid)
        //    {
        //        if (link.Pid == 0 && pid != 0)
        //        {
        //            if (new List<Link>(CmsLogic.Link.GetLinks(a => a.Pid == link.ID)).Count != 0)
        //            {
        //                base.RenderError("已经被继承,无法修改继承!");
        //                return;
        //            }
        //        }

        //        link.Pid = pid;
        //    }


        //    link.Index = index;
        //    link.Uri = uri;
        //    link.Text = text;
        //    link.Type = (int)type;
        //    link.Target = target;
        //    link.ImgUrl = imgurl;

        //    CmsLogic.Link.UpdateLink(link);

        //    Cms.Cache.Clear(CacheSign.Link.ToString());

        //    base.RenderSuccess("保存成功!");
        //}

        /// <summary>
        /// 设置链接可见
        /// </summary>
        [MCacheUpdate(CacheSign.Link)]
        public string SetVisible_POST()
        {
            int linkId = int.Parse(base.Request.Form["linkId"]);
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
            int linkId = int.Parse(base.Request.Form["linkId"]);

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
            int linkId = int.Parse(base.Request.Form["linkId"]);
            ServiceCall.Instance.SiteService.DeleteLink(this.SiteId, linkId);
            base.RenderSuccess();
        }

        private string GetCategorySelector(int siteID, int categoryID)
        {
            StringBuilder sb = new StringBuilder();

            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (_category, level) =>
            {
                if (_category.Site.ID != siteID) return;

                sb.Append("<option value=\"").Append(_category.ID.ToString()).Append("\"");


                sb.Append(_category.ID == categoryID ? " selected=\"selected\"" : "").Append(">");


                for (int i = 0; i < level; i++)
                {
                    sb.Append(CmsCharMap.Dot);
                }

                sb.Append(_category.Name).Append("</option>");

            });
            return sb.ToString();
        }


        public void RelatedLink_GET()
        {
           string form = EntityForm.Build<LinkDto>(new LinkDto(), false, "btn");
           base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Link_RelatedLink), new
           {
               form=form
           });
        }

        public void RelatedLink_POST()
        {
            IEnumerable<LinkDto> links = ServiceCall.Instance.ContentService
                .GetRelatedLinks(
                this.SiteId,
                Request["typeIndent"],
                int.Parse(Request["contentId"]));

            // string pagerHtml = Helper.BuildJsonPagerInfo("javascript:window.toPage({0});", pageIndex, recordCount, pages);

            base.PagerJson(links, "共" + links.Count().ToString() + "条");
        }

        public string SaveReleatedLink_POST()
        {
            try
            {
                LinkDto link;
                link = EntityForm.GetEntity<LinkDto>();

                ServiceCall.Instance.ContentService.SaveRelatedLink(
                    this.SiteId,
                    Request["typeIndent"],
                    int.Parse(Request["contentId"]),
                    link
                    );
                return base.ReturnSuccess(link.LinkId == 0 ? "添加成功" : "保存成功");

            }catch(Exception exc)
            {
                return base.ReturnError(exc.Message);
            }

        }
        public string DeleteRelatedLink_POST()
        {
            try
            {
                ServiceCall.Instance.ContentService.RemoveRelatedLink(
                    this.SiteId,
                    Request["typeIndent"],
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
