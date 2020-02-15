//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
// 
// Project: jr.Cms.Manager
// FileName : Archive.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 10:56:06
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Infrastructure;
using JR.Cms.Library.DataAccess.BLL;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;
using JR.DevFw.Framework;
using JR.DevFw.Toolkit.Tags;
using JR.DevFw.Web;
using JsonSerializer = JR.Cms.WebImpl.Json.JsonSerializer;
using Result = JR.Cms.ServiceDto.Result;

namespace JR.Cms.WebImpl.WebManager.Handle
{
    /// <summary>
    /// 文档
    /// </summary>
    public class ArchiveC : BasePage
    {

        private static IDictionary<int, HttpTags> tags = new Dictionary<int, HttpTags>();

        /// <summary>
        /// 创建文档
        /// </summary>
        public void Create_GET()
        {
            object data;
            string tpls,
                nodesHtml,
                //栏目JSON
                extendFieldsHtml = "";                                                                 //属性Html

            Module module;

            int categoryId = int.Parse(base.Request["category.id"] ?? "1");                                    //分类ID
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(base.SiteId, categoryId);


            //获取模板视图下拉项
            StringBuilder sb2 = new StringBuilder();

            //模板目录
            DirectoryInfo dir = new DirectoryInfo(
                String.Format("{0}templates/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                base.CurrentSite.Tpl + "/"
                ));

            IDictionary<String, String> names = Cms.TemplateManager.Get(base.CurrentSite.Tpl).GetNameDictionary();
            EachClass.EachTemplatePage(
                dir,
                dir,
                sb2,
                names,
                TemplatePageType.Custom,
                TemplatePageType.Archive
            );

            tpls = sb2.ToString();
            sb2.Remove(0, sb2.Length);

            //获取该模块栏目JSON数据
            //categoryJSON = CmsLogic.Category.GetJson(m.ID);

            nodesHtml = Helper.GetCategoryIdSelector(this.SiteId, categoryId);


            //=============  拼接模块的属性值 ==============//

            /*
            //========= 统计tab =============//
            sb.Append("<ul class=\"dataExtend_tabs\">");
            foreach (DataExtend e in extends)
            {
                sb.Append("<li><span>").Append(e.Name).Append("</span></li>");
            }
            sb.Append("</ul>");

            extendItemsHtml = sb.ToString();

            sb.Capacity = 1000;
            sb.Remove(0, sb.Length);
            */



            //======== 生成值 =============//

            StringBuilder sb = new StringBuilder(500);

            sb.Append("<div class=\"dataextend_item\">");
            PropertyUI uiType;
            foreach (IExtendField p in category.ExtendFields)
            {
                uiType = (PropertyUI)int.Parse(p.Type);
                this.AppendExtendFormHtml(sb, p, p.DefaultValue);
            }


            sb.Append("</div>");
            extendFieldsHtml = sb.ToString();


            // extendFieldsHtml = "<div style=\"color:red;padding:20px;\">所选栏目模块不包含任何自定义属性，如需添加请到模块管理-》属性设置</div>";


            object json = new
            {
                IsVisible=true,
                Thumbnail = CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto,
                Location = String.Empty
            };

            data = new
            {
                extendFieldsHtml = extendFieldsHtml,
                extend_cls = category.ExtendFields.Count == 0 ? "hidden" : "",
                thumbPrefix = CmsVariables.Archive_ThumbPrefix,
                nodes = nodesHtml,
                url = Request.Url.PathAndQuery,
                tpls,
                json = JsonSerializer.Serialize(json)
            };

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_Create), data);
        }

        public string View_frame_GET()
        {
            int id = int.Parse(base.Request["archive_id"]);
            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(this.SiteId, id);
            string fullDomain = this.CurrentSite.FullDomain;
            if (fullDomain.IndexOf("#", StringComparison.Ordinal) != -1)
            {
                fullDomain = Request.Url.Scheme + ":" + fullDomain.Replace("#", WebCtx.Current.Host);
            }
            string url = fullDomain + archive.Url+".html";
            return "<html><head><title>" + archive.Title + "</title></head><body style='margin:0'><iframe src='" + url +
                   "' frameBorder='0' width='100%' height='100%'></iframe></body></html>";
        }

        /// <summary>
        /// 提交创建
        /// </summary>
        public void Create_POST()
        {
            ArchiveDto archive = default(ArchiveDto);

            var form = HttpContext.Current.Request.Form;

            string alias = form["Alias"];
            if (alias != "")
            {
                if (!ServiceCall.Instance.ArchiveService.CheckArchiveAliasAvailable(this.SiteId, -1, alias))
                {
                    base.RenderError("别名已经存在!");
                    return;
                }
            }


            archive.ViewCount = 1;
            archive.PublisherId = UserState.Administrator.Current.Id;

            archive = GetFormCopyedArchive(this.SiteId, form, archive, alias);
            Result r = ServiceCall.Instance.ArchiveService.SaveArchive(
                this.SiteId,archive.Category.ID, archive);
            if (r.ErrCode > 0)
            {
                base.RenderError(r.ErrMsg);
            }
            else
            {
                //返回文章ID
                base.RenderSuccess(r.Data["ArchiveId"].ToString());
            }
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        public void Update_GET()
        {

            object data;
            int archiveId = int.Parse(base.Request["archive.id"]);
            string tpls, nodesHtml,
                //栏目JSON
                extendFieldsHtml = "";                                        //属性Html

            Module module;

            int siteId = this.CurrentSite.SiteId;

            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(siteId, archiveId);

            int categoryId = archive.Category.ID;

            //=============  拼接模块的属性值 ==============//

            StringBuilder sb = new StringBuilder(50);

            string attrValue;
            IExtendField field;

            sb.Append("<div class=\"dataextend_item\">");

            foreach (IExtendValue extValue in archive.ExtendValues)
            {
                field = extValue.Field;
                attrValue = (extValue.Value ?? field.DefaultValue).Replace("<br />", "\n");
                this.AppendExtendFormHtml(sb, field, attrValue);
            }


            sb.Append("</div>");

            extendFieldsHtml = sb.ToString();



            //获取模板视图下拉项
            StringBuilder sb2 = new StringBuilder();

            //模板目录
            DirectoryInfo dir = new DirectoryInfo(
                String.Format("{0}templates/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                base.CurrentSite.Tpl + "/"
                ));

            IDictionary<String, String> names = Cms.TemplateManager.Get(base.CurrentSite.Tpl).GetNameDictionary();

            EachClass.EachTemplatePage(dir, dir,
                sb2,
                names,
                TemplatePageType.Custom,
                TemplatePageType.Archive
                );

            tpls = sb2.ToString();

            nodesHtml = Helper.GetCategoryIdSelector(this.SiteId, categoryId);
            

            string thumbnail = !String.IsNullOrEmpty(archive.Thumbnail)
                    ? archive.Thumbnail
                    : "/" + CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto;

            object json = new
            {

                //IsSpecial = flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial))
                //                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial)],

                //IsSystem = flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem))
                //                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem)],

                //IsNotVisible = !(flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible))
                //                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible)]),

                //AsPage = flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage))
                //                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage)],
                IsSpecial =  this.FlagAnd(archive.Flag,BuiltInArchiveFlags.IsSpecial),
                IsSystem = this.FlagAnd(archive.Flag, BuiltInArchiveFlags.IsSystem),
                IsVisible = this.FlagAnd(archive.Flag, BuiltInArchiveFlags.Visible),
                AsPage = this.FlagAnd(archive.Flag, BuiltInArchiveFlags.AsPage),
                Id = archive.Id,
                Title = archive.Title,
                SmallTitle = archive.SmallTitle,
                Alias = archive.Alias ?? String.Empty,
                Tags = archive.Tags,
                Source = archive.Source,
                Outline = archive.Outline,
                Content = archive.Content,
                TemplatePath = archive.IsSelfTemplate
                    && !String.IsNullOrEmpty(archive.TemplatePath) ?
                    archive.TemplatePath :
                    String.Empty,
                Thumbnail = thumbnail,
                Location = archive.Location
            };


            data = new
            {
                extendFieldsHtml = extendFieldsHtml,
                extend_cls = archive.ExtendValues.Count == 0 ? "hidden" : "",
                nodes = nodesHtml,
                url = Request.Url.PathAndQuery,
                tpls = tpls,
                json = JsonSerializer.Serialize(json)
            };

            base.RenderTemplate(
              CompressHtml(ResourceMap.GetPageContent(ManagementPage.Archive_Update)),
                data);
        }

        private bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            int v = (int)b;
            return (flag & v) == v;
        }

        /// <summary>
        /// 生成扩展表单HTML
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="field"></param>
        /// <param name="attrValue"></param>
        private void AppendExtendFormHtml(StringBuilder sb, IExtendField field, string attrValue)
        {
            PropertyUI uiType = (PropertyUI)int.Parse(field.Type);
            sb.Append("<dl><dt>").Append(field.Name).Append("：</dt><dd>");


            switch (uiType)
            {
                case PropertyUI.Text:
                    sb.Append("<input type=\"text\" class=\"w300 ui-box\" field=\"extend_").Append(field.GetDomainId().ToString())
                        .Append("\" value=\"").Append(attrValue).Append("\"/>");
                    break;

                case PropertyUI.MultLine:
                    sb.Append("<textarea class=\"w300 ui-box\" field=\"extend_").Append(field.GetDomainId().ToString())
                        .Append("\">").Append(attrValue).Append("</textarea>");
                    break;

                case PropertyUI.Integer:
                    sb.Append("<input type=\"text\" class=\"w300 ui-box ui-validate\" isnum=\"true\" field=\"extend_")
                        .Append(field.GetDomainId().ToString()).Append("\" value=\"").Append(attrValue).Append("\"/>");
                    break;

                case PropertyUI.Upload:
                    // sb.Append("<input type=\"text\" disabled=\"disabled\" class=\"tb_normal\" id=\"extend_").Append(field.ID.ToString())
                    sb.Append("<input type=\"text\" class=\"w300 ui-box upload_value\" id=\"extend_")
                        .Append(field.GetDomainId().ToString())
                        .Append("\" field=\"extend_").Append(field.GetDomainId().ToString())
                        .Append("\" value=\"").Append(attrValue).Append("\"/>&nbsp;&nbsp;<span class=\"ui-button w80 middle-button\" id=\"upload_")
                        .Append(field.GetDomainId().ToString()).Append("\"><span class=\" button-inner\"><span class=\"button-txt\">选择文件</span>")
                        .Append("<a href=\"javascript:;\"></a></span></span>")
                        .Append("<script type=\"text/javascript\">jr.propertyUpload(")
                        .Append("'upload_").Append(field.GetDomainId().ToString()).Append("','extend_")
                        .Append(field.GetDomainId().ToString())
                        .Append("');</script>");
                    break;
            }
            sb.Append("</dd></dl>");
        }

        public void Update_POST()
        {
            var form = HttpContext.Current.Request.Form;
            ArchiveDto archive = ServiceCall.Instance.ArchiveService
                .GetArchiveById(this.SiteId, int.Parse(form["Id"]));

            //判断是否有权修改
            if (!ArchiveUtility.CanModifyArchive(this.SiteId, archive.PublisherId))
            {
                base.RenderError("您无权修改此文档!");
                return;
            }

            string alias = String.IsNullOrEmpty(form["Alias"]) ?
                String.Empty :
                HttpContext.Current.Server.UrlEncode(form["Alias"]);

            if (alias != String.Empty && archive.Alias != alias)
            {
                if (!ServiceCall.Instance.ArchiveService
                    .CheckArchiveAliasAvailable(this.SiteId, archive.Id, alias))
                {
                    base.RenderError("别名已经存在!");
                    return;
                }
            }

            archive = GetFormCopyedArchive(this.SiteId, form, archive, alias);       
            Result r = ServiceCall.Instance.ArchiveService.SaveArchive(
                this.SiteId, archive.Category.ID, archive);
            if (r.ErrCode > 0)
            {
                base.RenderError(r.ErrMsg);
            }
            else
            {
                //返回文章ID
                base.RenderSuccess("保存成功");
            }
        }

        private ArchiveDto GetFormCopyedArchive(int siteId, NameValueCollection form, ArchiveDto archive, string alias)
        {

            string content = form["Content"];

            //自动替换Tags
            if (form["autotag"] == "on")
            {
                //todo:顺序调换了下
                HttpTags _tags = this.GetTags(siteId);
                content = _tags.Tags.RemoveAutoTags(content);
                content = _tags.Tags.ReplaceSingleTag(content);
            }
            archive.Flag = 0;
            if(form["IsVisible"] == "on")
            {
                archive.Flag |= (int)BuiltInArchiveFlags.Visible;
            }
            if(form["AsPage"] == "on")
            {
                archive.Flag |= (int)BuiltInArchiveFlags.AsPage;
            }
            if(form["IsSpecial"] == "on")
            {
                archive.Flag |= (int)BuiltInArchiveFlags.IsSpecial;
            }
            if (form["IsSystem"] == "on")
            {
                archive.Flag |= (int)BuiltInArchiveFlags.IsSystem;
            }
            archive.UpdateTime = DateTime.Now;
            archive.Title = form["Title"].Trim();
            archive.SmallTitle = form["SmallTitle"].Trim();
            archive.Location = form["location"].Trim();
            archive.Source = form["Source"];
            archive.Outline = form["Outline"];
            archive.Alias = alias;
            archive.Tags = form["Tags"].Replace("，", ",");
            archive.Content = content;
            archive.Thumbnail = form["Thumbnail"];

            //分类
            int categoryId = int.Parse(form["categoryid"]);
            archive.Category = new CategoryDto { ID = categoryId };

            //检测图片是否为默认图片
            if (archive.Thumbnail == CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto)
                archive.Thumbnail = String.Empty;

            archive.ExtendValues = new List<IExtendValue>();

            //=============== 更新扩展字段 ===================

            IExtendField field;
            string extendValue;
            foreach (string key in form.Keys)
            {
                if (key.StartsWith("extend_"))
                {
                    extendValue = form[key];
                    field = new ExtendField(int.Parse(key.Substring(7)), null);
                    archive.ExtendValues.Add(new ExtendValue(-1, field, extendValue));
                }
            }

            //更新模板设置
            archive.TemplatePath = form["TemplatePath"];
            if (archive.TemplatePath != String.Empty)
            {
                archive.IsSelfTemplate = true;
            }
            return archive;
        }


        /// <summary>
        /// 文档列表
        /// </summary>
        public string List_GET()
        {
            //const int __pageSize = 10;

            string categoryId = Request["category.id"];
            int publisherId = int.Parse(Request["author_id"] ?? "0");

            ViewData["category_id"] = categoryId ?? String.Empty;
            ViewData["author_id"] = publisherId;
            ViewData["site_id"] = this.CurrentSite.SiteId;

            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_List));
        }

        /// <summary>
        /// 文档列表Json数据
        /// </summary>
        public void PagerJsonData_POST()
        {
            HttpRequest request = HttpContext.Current.Request;


            int pageSize,
                pageIndex,
                recordCount,
                pages;

            int? categoryId = null,
                 moduleId = null;

            string _categoryId = request["category_id"],
                   _pageIndex = request["page_index"] ?? "1",
                    _pageSize = request["page_size"] ?? "10",
                   _visible = request["lb_visible"] ?? "-1",
                   _special = request["lb_special"] ?? "-1",
                   _system = request["lb_system"] ?? "-1",
                   _aspage = request["lb_page"] ?? "-1";

            int publisherId;
            int.TryParse(request["author_id"], out publisherId);

            bool includeChild = request["include_child"] == "true";
            String keyword = Request.Form["keyword"];

            if (!String.IsNullOrEmpty(keyword) && DataChecker.SqlIsInject(keyword))
            {
                throw new ArgumentException("Sql inject?", keyword);
            }

            if (_categoryId != null)
            {
                int __categoryId;
                int.TryParse(_categoryId, out __categoryId);
                if (__categoryId > 0)
                {
                    categoryId = __categoryId;
                }
            }


            //处理页码大小并保存

            if (!Regex.IsMatch(_pageIndex, "^(?!0)\\d+$"))
            {
                pageIndex = 1;   //If pageindex start with zero or lower
            }
            else
            {
                pageIndex = int.Parse(_pageIndex);
            }

            //分页尺寸
            int.TryParse(_pageSize, out pageSize);

            


            //文档数据表,并生成Html
            DataTable dt = ServiceCall.Instance.ArchiveService.GetPagedArchives(this.SiteId, categoryId,
                publisherId, includeChild, 0, keyword, null, false, pageSize, pageIndex, out recordCount, out pages);

            foreach (DataRow dr in dt.Rows)
            {
                dr["content"] = "";
            }

            //  CmsLogic.Archive.GetPagedArchives(this.SiteId, moduleID, categoryID, _author_id, flags, null, false, pageSize, pageIndex, out recordCount, out pages);
            //moduleID == null ? CmsLogic.Archive.GetPagedArchives(categoryID, _author_id,flags, null, false, pageSize, pageIndex, out recordCount, out pages)
            //: CmsLogic.Archive.GetPagedArchives((SysModuleType)(moduleID ?? 1), _author_id, flags,null, false, pageSize, pageIndex, out recordCount, out pages);


            string pagerHtml = Helper.BuildJsonPagerInfo("javascript:window.toPage(1);", "javascript:window.toPage({0});", pageIndex, recordCount, pages);

            base.PagerJson(dt, pagerHtml);

        }


        /// <summary>
        /// 预览文档
        /// </summary>
        public void View_GET()
        {
            int id = int.Parse(base.Request["archive.id"]);
            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(this.SiteId, id);
            //UserBll author = CmsLogic.UserBll.GetUser(archive.Author);
            UserDto user = ServiceCall.Instance.UserService.GetUser(archive.PublisherId);

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_View), new
            {
                title = archive.Title,
                publisherName = user == null ? "" : user.Name,
                publishdate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateTime),
                content = archive.Content,
                thumbnail = archive.Thumbnail,
                count = archive.ViewCount
            });
        }


        /// <summary>
        /// search文档列表
        /// </summary>
        public void Search_GET()
        {

            HttpRequest request = HttpContext.Current.Request;

            const int __pageSize = 10;

            object data;            //返回的数据

            int pageSize,
                pageIndex,
                recordCount,
                pages;


            string _keyword = request["keyword"],
                   _pageIndex = request["page"] ?? "1",
                   _pageSize = request["size"];


            if (String.IsNullOrEmpty(_keyword))
            {

                data = new
                {
                    keyword = _keyword ?? "",
                    headerText = "",
                    archiveListHtml = "",
                    pagerHtml = ""
                };
            }
            else
            {


                StringBuilder sb = new StringBuilder();
                string pagerHtml,                   //分页链接
                       tableHeaderText,             //表头字段
                       archiveListHtml;             //文档列表HTML

                //获取表头
                bool isMaster = UserState.Administrator.Current.IsMaster;
                tableHeaderText = isMaster ? "<th>发布人</th>" : String.Empty;


                //处理页码大小并保存

                if (!Regex.IsMatch(_pageIndex, "^(?!0)\\d+$")) pageIndex = 1;   //If pageindex start with zero or lower
                else pageIndex = int.Parse(_pageIndex);

                if (String.IsNullOrEmpty(_pageSize))
                {
                    int.TryParse(HttpContext.Current.Session["archivelist_pagesize"] as string, out pageSize);
                }
                else
                {
                    int.TryParse(_pageSize, out pageSize);
                    HttpContext.Current.Session["archivelist_pagesize"] = pageSize;
                }
                if (pageSize == 0) pageSize = __pageSize;



                //文档数据表,并生成Html
                DataTable dt = null;// CmsLogic.Archive.Search(this.CurrentSite.SiteId, _keyword, pageSize, pageIndex, out recordCount, out pages, null);

                

                bool isSpecial,
                    isSystem,
                    isVisible,
                    isPage;

                foreach (DataRow dr in dt.Rows)
                {
                    int flag = Convert.ToInt32(dr["flag"]);
                    isSpecial = this.FlagAnd(flag, BuiltInArchiveFlags.IsSpecial);
                    isSystem = this.FlagAnd(flag,BuiltInArchiveFlags.IsSystem);
                    isVisible =this.FlagAnd(flag,BuiltInArchiveFlags.Visible);
                    isPage = this.FlagAnd(flag,BuiltInArchiveFlags.AsPage);

                    //编号
                    sb.Append("<tr><td align=\"center\">").Append(dr["id"].ToString()).Append("</td>");
                    //标题
                    sb.Append("<td").Append(isSpecial ? " class=\"special\">" : ">").Append(dr["title"].ToString()).Append("</td>");

                    //管理员可以查看发布人
                    if (isMaster)
                    {
                        sb.Append("<td><a href=\"?module=archive&amp;action=list&amp;moduleID=&amp;author_id=")
                            .Append(dr["author"].ToString()).Append("\" title=\"查看该用户发布的文档\">").Append(dr["Author"].ToString()).Append("</a></td>");
                    }

                    sb.Append("<td>").Append(String.Format("{0:yyyy/MM/dd HH:mm}",TimeUtils.UnixTime(Convert.ToInt32(dr["create_time"]),TimeZone.CurrentTimeZone)))
                        .Append("</td><td align=\"center\">")
                        .Append(dr["view_count"].ToString()).Append("</td><td><button class=\"refresh\" /></td><td><button class=\"file\" /></td><td><button class=\"edit\" /></td><td><button class=\"delete\" /></td></tr>");

                }

                archiveListHtml = sb.ToString();



                string format = String.Format("?module=archive&action=search&keyword={1}&page={0}&size={2}", "{0}", HttpUtility.UrlEncode(_keyword), pageSize);

                //pagerHtml = Helper.BuildPagerInfo(format, pageIndex, recordCount, pages);

                data = new
                {
                    keyword = _keyword ?? "",
                    headerText = tableHeaderText,
                    archiveListHtml = archiveListHtml,
                    // pagerHtml = pagerHtml
                };

            }
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_Search), data);

        }

        /// <summary>
        /// 文档评论
        /// </summary>
        public void Comment_GET()
        {
            string commentListHtml;

            int id = int.Parse(base.Request["archive.id"]);
            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(this.SiteId, id);

            bool desc = base.Request["desc"] == "true";

            CategoryDto category = archive.Category;

            StringBuilder sb = new StringBuilder();
            DataTable table = CmsLogic.Comment.GetArchiveComments(archive.StrId, desc);


            int i = 1;
            bool isGuest = false;
            string nickName, content, avatar;
            Match match;
            foreach (DataRow dr in table.Rows)
            {
                //筛选出游客的昵称和内容
                if (dr["memberid"].ToString() != "0")
                {
                    nickName = dr["nickname"].ToString();
                    content = dr["content"].ToString();
                    avatar = dr["avatar"].ToString();
                    isGuest = false;
                }
                else
                {
                    content = dr["content"].ToString();
                    match = Regex.Match(content, "\\(u:'(?<user>.+?)'\\)");

                    if (match != null)
                    {
                        nickName = match.Groups["user"].Value;
                        content = Regex.Replace(content, "\\(u:'(.+?)'\\)", String.Empty);
                    }
                    else
                    {
                        nickName = "游客";
                    }

                    avatar = "?module=file&action=guestavatar";
                    isGuest = true;
                }


                sb.Append("<p class=\"comment\"><img src=\"").Append(avatar).Append("\" /><span class=\"time\">")
                .Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}", dr["create_time"])).Append("</span><span class=\"details\"><a href=\"#\">")
                .Append(nickName).Append("</a><br />").Append(content).Append("</span><span class=\"floor\">")
                .Append(i.ToString()).Append("楼</span><span class=\"control\"><a href=\"javascript:;\" onclick=\"remove(this,'").Append(dr["id"].ToString()).Append("')\">删除</a></span></p>");
                ++i;
            }

            commentListHtml = sb.Length == 0 ? "暂无评论!" : sb.ToString();

            object data = new
            {
                archive_id = archive.StrId,
                archive_title = archive.Title,
                publisher_name = archive.PublisherId,
                publish_date = String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateTime),
                commentListHtml = commentListHtml
            };

            //base.RenderTemplate(ResourceMap.CommentList, data);
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Comment_List), data);
        }

        public void Forword_GET()
        {
            int id = int.Parse(base.Request["archive.id"]);
            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(this.SiteId, id);
            string fullDomain = this.CurrentSite.FullDomain;

            if (fullDomain.IndexOf("#", StringComparison.Ordinal) != -1)
            {
                fullDomain =  fullDomain.Replace("#", WebCtx.Current.Host);
            }
            string url = Request.Url.Scheme + ":" + fullDomain + archive.Url+".html";


            if (String.IsNullOrEmpty(archive.Outline))
            {
                archive.Outline = ArchiveUtility.GetOutline(archive.Content, 100);
            }

            object data = new
            {
                title = archive.Title,
                outline = archive.Outline,
                url = url,
                forword_content = String.Format("<a href=\"{0}\" target=\"_blank\"><strong>{1}</strong><br />{2}</a>",
                    url,
                    archive.Title,
                    archive.Outline),
                link_content = ""
            };

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_Forword), data);
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        public string Delete_POST()
        {
            int id = int.Parse(base.Request["archive.id"]);
            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(this.SiteId, id);

            if (!ArchiveUtility.CanModifyArchive(this.SiteId, archive.PublisherId))
            {
                return base.ReturnError("您无权删除此文档!");
            }

            try
            {
                ServiceCall.Instance.ArchiveService.DeleteArchive(this.SiteId, archive.Id);
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }

            //删除

            //触发删除事件
            //WatchService.PrevDeleteArchive(archive);
            //WatchService.UpdateArchive(archive);

            return base.ReturnSuccess();
        }

        public string Batch_Delete_POST()
        {
            int siteId = this.CurrentSite.SiteId;
            int[] idArray = Array.ConvertAll(Request["id_array"].Split(','), a => int.Parse(a));
            try
            {
                ServiceCall.Instance.ArchiveService.BatchDelete(siteId, idArray);
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }
            return base.ReturnSuccess();
        }

        /// <summary>
        /// 重新发布文档(刷新文档)
        /// </summary>
        public void Republish_POST()
        {
            int id = int.Parse(base.Request["archive.id"]);
            ServiceCall.Instance.ArchiveService.RepublishArchive(this.SiteId, id);
            base.RenderSuccess();
        }


        public void MoveSortNumber_post()
        {
            int id = int.Parse(base.Request["archive.id"]);
            int di = int.Parse(base.Request["direction"]);

            try
            {
                ServiceCall.Instance.ArchiveService.MoveSortNumber(this.SiteId, id, di);
                base.RenderSuccess();
            }
            catch (Exception exc)
            {
                base.RenderError(exc.Message);
            }
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        public void DeleteComment_POST()
        {
            string commentId = HttpContext.Current.Request.Form["commentId"];

            //
            //TODO:根据文档获取评论
            //

            // var category = CmsLogic.Category.Get(a => a.ID == archive.Cid);
            // if (base.CompareSite(category.SiteId)) return;

            CmsLogic.Comment.DeleteComment(int.Parse(commentId));
        }




        #region Tags系统

        /// <summary>
        /// 获取当前站点的Tags对象
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        private HttpTags GetTags(int siteId)
        {
            HttpTags _tags = null;
            if (!tags.Keys.Contains(siteId))
            {
                string dirPath = String.Concat(Cms.PyhicPath, CmsVariables.SITE_CONF_PRE_PATH, siteId.ToString(), "/");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath).Create();
                }
                _tags = new HttpTags(String.Concat(dirPath, "tags.conf"));
                tags.Add(siteId, _tags);
            }
            else
            {
                _tags = tags[siteId];
            }
            return _tags;
        }

        public void TagsIndex_GET()
        {
            string js = ManagerTemplate.GetScriptTag();
            string css = ManagerTemplate.GetCssTag();

            int siteId = this.CurrentSite.SiteId;
            String content = ResourceMap.GetPageContent(ManagementPage.Archive_Tags);
            GetTags(siteId).ProcessRequest(HttpContext.Current, content.Replace("$js()", js).Replace("$css()", css));
        }

        public void TagsIndex_POST()
        {
            int siteId = this.CurrentSite.SiteId;

            String content = ResourceMap.GetPageContent(ManagementPage.Archive_Tags);
            GetTags(siteId).ProcessRequest(HttpContext.Current, content);
        }

        #endregion

    }
}

