//
// Copyright (C) 2007-2008 TO2.NET,All rights reserved.
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
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.DataAccess.BLL;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Utils;
using JR.Stand.Core.Web;
using Org.BouncyCastle.Utilities;
using JsonSerializer = JR.Cms.Web.Util.JsonSerializer;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 文档
    /// </summary>
    public class ArchiveHandler : BasePage
    {
        // private static IDictionary<int, HttpTags> tags = new Dictionary<int, HttpTags>();

        /// <summary>
        /// 创建文档
        /// </summary>
        public void Create()
        {
            //栏目JSON
            string extendFieldsHtml = ""; //属性Html

            Module module;

            var categoryId = int.Parse(Request.Query("category.id").ToString() ?? "1"); //分类ID
            var category = LocalService.Instance.SiteService.GetCategory(SiteId, categoryId);


            //获取模板视图下拉项
            var sb2 = new StringBuilder();

            //模板目录
            var dir = new DirectoryInfo($"{EnvUtil.GetBaseDirectory()}/templates/{CurrentSite.Tpl + "/"}");

            var names = Cms.TemplateManager.Get(CurrentSite.Tpl).GetNameDictionary();
            EachClass.EachTemplatePage(
                dir,
                dir,
                sb2,
                names,
                new[]{
                TemplatePageType.Custom,
                TemplatePageType.Archive
                }
            );

            var tplList = sb2.ToString();
            sb2.Remove(0, sb2.Length);

            //获取该模块栏目JSON数据
            //categoryJSON = CmsLogic.Category.GetJson(m.ID);

            var nodesHtml = Helper.GetCategoryIdSelector(SiteId, categoryId);


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

            var sb = new StringBuilder(500);

            sb.Append("<div class=\"data-extend-item\">");
            foreach (var p in category.ExtendFields)
            {
                // PropertyUI uiType = (PropertyUI) int.Parse(p.Type);
                AppendExtendFormHtml(sb, p, p.DefaultValue);
            }

            sb.Append("</div>");
            extendFieldsHtml = sb.ToString();

            // extendFieldsHtml = "<div style=\"color:red;padding:20px;\">所选栏目模块不包含任何自定义属性，如需添加请到模块管理-》属性设置</div>";
            object json = new
            {
                IsVisible = true,
                TemplatePath = "",
                Thumbnail = CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto,
                Location = string.Empty
            };
            var path = Request.GetPath();
            var query = Request.GetQueryString();
            object data = new
            {
                extendFieldsHtml = extendFieldsHtml,
                extend_cls = category.ExtendFields.Count == 0 ? "hidden" : "",
                thumbPrefix = CmsVariables.Archive_ThumbPrefix,
                nodes = nodesHtml,
                url = path + query,
                tpls = tplList,
                json = JsonSerializer.Serialize(json)
            };

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_Create), data);
        }

        public string View_frame()
        {
            var id = int.Parse(Request.Query("archive_id"));
            var archive = LocalService.Instance.ArchiveService.GetArchiveById(SiteId, id);
            var fullDomain = CurrentSite.FullDomain;
            if (fullDomain.IndexOf("#", StringComparison.Ordinal) != -1)
            {
                fullDomain = Request.GetProto() + ":" + fullDomain.Replace("#", WebCtx.Current.Host);
            }
            var url = fullDomain + archive.Path + ".html";
            return "<html><head><title>" + archive.Title + "</title></head><body style='margin:0'><iframe src='" + url +
                   "' frameBorder='0' width='100%' height='100%'></iframe></body></html>";
        }

        /// <summary>
        /// 提交创建
        /// </summary>
        public void Create_POST()
        {
            var archive = default(ArchiveDto);


            string alias = Request.Form("Alias");
            archive.ViewCount = 1;
            archive.PublisherId = UserState.Administrator.Current.Id;

            archive = GetFormCopiedArchive(SiteId, Request, archive, alias);
            var r = LocalService.Instance.ArchiveService.SaveArchive(
                SiteId, archive.Category.ID, archive);
            RenderJson(r);
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        public void Update()
        {
            var archiveId = int.Parse(Request.Query("archive.id"));
            string extendFieldsHtml = ""; //属性Html

            var siteId = CurrentSite.SiteId;

            var archive = LocalService.Instance.ArchiveService.GetArchiveById(siteId, archiveId);

            var categoryId = archive.Category.ID;

            //=============  拼接模块的属性值 ==============//

            var sb = new StringBuilder(50);

            sb.Append("<div class=\"data-extend-item\">");

            foreach (var extValue in archive.ExtendValues)
            {
                var field = extValue.Field;
                var attrValue = (extValue.Value ?? field.DefaultValue).Replace("<br />", "\n");
                AppendExtendFormHtml(sb, field, attrValue);
            }


            sb.Append("</div>");

            extendFieldsHtml = sb.ToString();


            //获取模板视图下拉项
            var sb2 = new StringBuilder();

            //模板目录
            var dir = new DirectoryInfo(
                $"{EnvUtil.GetBaseDirectory()}/templates/{CurrentSite.Tpl + "/"}");
            var names = Cms.TemplateManager.Get(CurrentSite.Tpl).GetNameDictionary();

            EachClass.EachTemplatePage(dir, dir,
                sb2,
                names,
                new[]{
                TemplatePageType.Custom,
                TemplatePageType.Archive
                }
            );

            var tplList = sb2.ToString();
            var nodesHtml = Helper.GetCategoryIdSelector(SiteId, categoryId);
            var thumbnail = !string.IsNullOrEmpty(archive.Thumbnail)
                ? archive.Thumbnail
                : "/" + CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto;

            long now = TimeUtils.Unix();
            long createUnix = TimeUtils.Unix(archive.CreateTime);
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
                IsSpecial = FlagAnd(archive.Flag, BuiltInArchiveFlags.IsSpecial),
                IsSystem = FlagAnd(archive.Flag, BuiltInArchiveFlags.IsSystem),
                IsVisible = FlagAnd(archive.Flag, BuiltInArchiveFlags.Visible),
                AsPage = FlagAnd(archive.Flag, BuiltInArchiveFlags.AsPage),
                Id = archive.Id,
                Title = archive.Title,
                SmallTitle = archive.SmallTitle,
                Alias = archive.Alias ?? string.Empty,
                Tags = archive.Tags,
                Source = archive.Source,
                Outline = archive.Outline,
                Content = archive.Content,
                TemplatePath = archive.IsSelfTemplate
                               && !string.IsNullOrEmpty(archive.TemplatePath)
                    ? archive.TemplatePath
                    : string.Empty,
                Thumbnail = thumbnail,
                Location = archive.Location,
                ScheduleTime = archive.ScheduleTime > 0 ? String.Format("{0:yyyy-MM-dd}", TimeUtils.UnixTime(archive.ScheduleTime)) : "",
                // 如果创建时间大于0，并且当前时间大于创建时间+24小时，则不能再修改定时发送
                ScheduleClassName = createUnix > 0 && (now - createUnix) > 3600 * 24 ? "hidden" : "",
            };

            var path = Request.GetPath();
            var query = Request.GetQueryString();

            object data = new
            {
                extendFieldsHtml = extendFieldsHtml,
                extend_cls = archive.ExtendValues.Count == 0 ? "hidden" : "",
                nodes = nodesHtml,
                url = path + query,
                tpls = tplList,
                json = JsonSerializer.Serialize(json)
            };

            RenderTemplate(
                ResourceMap.GetPageContent(ManagementPage.Archive_Update),
                data);
        }

        private bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            var v = (int)b;
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
            var uiType = (PropertyUI)int.Parse(field.Type);
            sb.Append("<dl><dt>").Append(field.Name).Append("：</dt><dd>");
            switch (uiType)
            {
                case PropertyUI.Text:
                    sb.Append("<input type=\"text\" class=\"w300 ui-box\" field=\"extend_")
                        .Append(field.GetDomainId().ToString())
                        .Append("\" value=\"").Append(attrValue).Append("\"/>");
                    break;

                case PropertyUI.MultiLine:
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
                        .Append("\" value=\"").Append(attrValue)
                        .Append("\"/>&nbsp;&nbsp;<span class=\"ui-button w80 middle-button\" id=\"upload_")
                        .Append(field.GetDomainId().ToString())
                        .Append("\"><span class=\" button-inner\"><span class=\"button-txt\">选择文件</span>")
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
            var archive = LocalService.Instance.ArchiveService
                .GetArchiveById(SiteId, int.Parse(Request.Form("Id")));

            //判断是否有权修改
            if (!ArchiveUtility.CanModifyArchive(SiteId, archive.PublisherId))
            {
                RenderError("您无权修改此文档!");
                return;
            }

            var alias = string.IsNullOrEmpty(Request.Form("Alias"))
                ? string.Empty
                : HttpUtils.UrlEncode(Request.Form("Alias"));

            archive = GetFormCopiedArchive(SiteId, Request, archive, alias);
            var r = LocalService.Instance.ArchiveService.SaveArchive(
                SiteId, archive.Category.ID, archive);
            RenderJson(r);
        }

        private ArchiveDto GetFormCopiedArchive(int siteId, ICompatibleRequest form, ArchiveDto archive, string alias)
        {
            string content = form.Form("Content");

            //自动替换Tags
            if (form.Form("auto_tag") == "on")
            {
                var cs = LocalService.Instance.ContentService;
                content = cs.RemoveWord(content);
                content = cs.Replace(content, false, true);
                //todo: tags 顺序调换了下
                /*
                HttpTags _tags = this.GetTags(siteId);
                content = _tags.Tags.RemoveAutoTags(content);
                content = _tags.Tags.ReplaceSingleTag(content);
                */
            }

            archive.Flag = 0;
            if (form.Form("IsVisible") == "on") archive.Flag |= (int)BuiltInArchiveFlags.Visible;
            if (form.Form("AsPage") == "on") archive.Flag |= (int)BuiltInArchiveFlags.AsPage;
            if (form.Form("IsSpecial") == "on") archive.Flag |= (int)BuiltInArchiveFlags.IsSpecial;
            if (form.Form("IsSystem") == "on") archive.Flag |= (int)BuiltInArchiveFlags.IsSystem;
            archive.UpdateTime = DateTime.Now;
            archive.Title = form.Form("Title").ToString().Trim();
            archive.SmallTitle = form.Form("SmallTitle").ToString().Trim();
            archive.Location = form.Form("location").ToString().Trim();
            archive.Source = form.Form("Source");
            archive.Outline = form.Form("Outline");
            archive.Alias = alias;
            archive.Tags = form.Form("Tags").ToString().Replace("，", ",");
            archive.Content = content;
            archive.Thumbnail = form.Form("Thumbnail");
            archive.ScheduleTime = 0;
            String date = form.Form("ScheduleTime");
            if (!String.IsNullOrEmpty(date))
            {
                if (!form.Form("IsSchedule").Equals("0"))
                {
                    long unix = TimeUtils.DateUnix(DateTime.Parse(date + " 00:00:00"));
                    archive.ScheduleTime = unix + TimeUtils.Unix() % (3600 * 24);
                }
            }
            //分类
            var categoryId = int.Parse(form.Form("categoryid"));
            archive.Category = new CategoryDto { ID = categoryId };

            //检测图片是否为默认图片
            if (archive.Thumbnail == CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto)
                archive.Thumbnail = string.Empty;

            archive.ExtendValues = new List<IExtendValue>();

            //=============== 更新扩展字段 ===================

            IExtendField field;
            string extendValue;
            foreach (var key in form.FormKeys())
                if (key.StartsWith("extend_"))
                {
                    extendValue = form.Form(key);
                    field = new ExtendField(int.Parse(key.Substring(7)), null);
                    archive.ExtendValues.Add(new ExtendValue(-1, field, extendValue));
                }

            //更新模板设置
            archive.TemplatePath = form.Form("TemplatePath");
            if (archive.TemplatePath != string.Empty) archive.IsSelfTemplate = true;
            return archive;
        }


        /// <summary>
        /// 文档列表
        /// </summary>
        public string List()
        {
            //const int __pageSize = 10;

            string categoryId = Request.Query("category.id");
            int.TryParse(Request.Query("author_id"), out var publisherId);

            ViewData["category_id"] = categoryId ?? string.Empty;
            ViewData["author_id"] = publisherId;
            ViewData["site_id"] = CurrentSite.SiteId;

            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_List));
        }

        /// <summary>
        /// 文档列表Json数据
        /// </summary>
        public void PagerJsonData_POST()
        {


            int
                pageIndex,
                recordCount,
                pages;

            int? categoryId = null,
                moduleId = null;

            int.TryParse(Request.Form("page_size"), out var pageSize);
            if (pageSize == 0) pageSize = 10;


            string _categoryId = Request.Form("category_id"),
                _pageIndex = Request.Form("page_index");
            //   _pageSize = Request.Form("page_size")[0] ?? "10",
            //  _visible = Request.Form("lb_visible")[0] ?? "-1",
            //  _special = Request.Form("lb_special")[0] ?? "-1",
            //  _system = Request.Form("lb_system")[0] ?? "-1",
            //  _aspage = Request.Form("lb_page")[0] ?? "-1";

            int.TryParse(Request.Query("author_id"), out var publisherId);

            var includeChild = Request.Query("include_child") == "true";
            string keyword = Request.Form("keyword");

            if (!string.IsNullOrEmpty(keyword) && DataChecker.SqlIsInject(keyword))
                throw new ArgumentException("sql inject?", keyword);

            if (_categoryId != null)
            {
                int.TryParse(_categoryId, out var __categoryId);
                if (__categoryId > 0) categoryId = __categoryId;
            }


            //处理页码大小并保存

            if (!Regex.IsMatch(_pageIndex, "^(?!0)\\d+$"))
                pageIndex = 1; //If pageindex start with zero or lower
            else
                pageIndex = int.Parse(_pageIndex);


            //文档数据表,并生成Html
            var dt = LocalService.Instance.ArchiveService.GetPagedArchives(SiteId, categoryId,
                publisherId, includeChild, 0, keyword, null, false, pageSize, pageIndex, out recordCount, out pages);

            foreach (DataRow dr in dt.Rows) dr["content"] = "";

            //  CmsLogic.Archive.GetPagedArchives(this.SiteId, moduleID, categoryID, _author_id, flags, null, false, pageSize, pageIndex, out recordCount, out pages);
            //moduleID == null ? CmsLogic.Archive.GetPagedArchives(categoryID, _author_id,flags, null, false, pageSize, pageIndex, out recordCount, out pages)
            //: CmsLogic.Archive.GetPagedArchives((SysModuleType)(moduleID ?? 1), _author_id, flags,null, false, pageSize, pageIndex, out recordCount, out pages);


            var pagerHtml = Helper.BuildJsonPagerInfo("javascript:window.toPage(1);", "javascript:window.toPage({0});",
                pageIndex, recordCount, pages);

            PagerJson(dt, pagerHtml);
        }


        /// <summary>
        /// 预览文档
        /// </summary>
        public void View()
        {
            var id = int.Parse(Request.Query("archive.id"));
            var archive = LocalService.Instance.ArchiveService.GetArchiveById(SiteId, id);
            //UserBll author = CmsLogic.UserBll.GetUser(archive.Author);
            var user = LocalService.Instance.UserService.GetUser(archive.PublisherId);

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_View), new
            {
                title = archive.Title,
                publisherName = user == null ? "" : user.Name,
                publishdate = $"{archive.CreateTime:yyyy-MM-dd HH:mm:ss}",
                content = archive.Content,
                thumbnail = archive.Thumbnail,
                count = archive.ViewCount
            });
        }


        /// <summary>
        /// search文档列表
        /// </summary>
        public void Search()
        {

            const int __pageSize = 10;

            object data; //返回的数据

            int pageSize,
                pageIndex,
                recordCount,
                pages;

            string _keyword = Request.Query("keyword"),
                _pageIndex = Request.Query("page")[0] ?? "1",
                _pageSize = Request.Query("size");

            if (string.IsNullOrEmpty(_keyword))
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
                var sb = new StringBuilder();
                string pagerHtml, //分页链接
                    tableHeaderText, //表头字段
                    archiveListHtml; //文档列表HTML

                //获取表头
                var isMaster = UserState.Administrator.Current.IsMaster;
                tableHeaderText = isMaster ? "<th>发布人</th>" : string.Empty;


                //处理页码大小并保存

                if (!Regex.IsMatch(_pageIndex, "^(?!0)\\d+$")) pageIndex = 1; //If pageindex start with zero or lower
                else pageIndex = int.Parse(_pageIndex);

                if (string.IsNullOrEmpty(_pageSize))
                {
                    pageSize = HttpHosting.Context.Session.GetInt32("archivelist_pagesize");
                }
                else
                {
                    int.TryParse(_pageSize, out pageSize);
                    HttpHosting.Context.Session.SetInt32("archivelist_pagesize", pageSize);
                }

                if (pageSize == 0) pageSize = __pageSize;


                //文档数据表,并生成Html
                DataTable
                    dt = null; // CmsLogic.Archive.Search(this.CurrentSite.SiteId, _keyword, pageSize, pageIndex, out recordCount, out pages, null);


                bool isSpecial,
                    isSystem,
                    isVisible,
                    isPage;

                foreach (DataRow dr in dt.Rows)
                {
                    var flag = Convert.ToInt32(dr["flag"]);
                    isSpecial = FlagAnd(flag, BuiltInArchiveFlags.IsSpecial);
                    isSystem = FlagAnd(flag, BuiltInArchiveFlags.IsSystem);
                    isVisible = FlagAnd(flag, BuiltInArchiveFlags.Visible);
                    isPage = FlagAnd(flag, BuiltInArchiveFlags.AsPage);

                    //编号
                    sb.Append("<tr><td align=\"center\">").Append(dr["id"].ToString()).Append("</td>");
                    //标题
                    sb.Append("<td").Append(isSpecial ? " class=\"special\">" : ">").Append(dr["title"].ToString())
                        .Append("</td>");

                    //管理员可以查看发布人
                    if (isMaster)
                        sb.Append("<td><a href=\"?module=archive&amp;action=list&amp;moduleID=&amp;author_id=")
                            .Append(dr["author"].ToString()).Append("\" title=\"查看该用户发布的文档\">")
                            .Append(dr["Author"].ToString()).Append("</a></td>");

                    sb.Append("<td>").Append(string.Format("{0:yyyy/MM/dd HH:mm}",
                            TimeUtils.UnixTime(Convert.ToInt32(dr["create_time"]), TimeZone.CurrentTimeZone)))
                        .Append("</td><td align=\"center\">")
                        .Append(dr["view_count"].ToString())
                        .Append(
                            "</td><td><button class=\"refresh\" /></td><td><button class=\"file\" /></td><td><button class=\"edit\" /></td><td><button class=\"delete\" /></td></tr>");
                }

                archiveListHtml = sb.ToString();


                var format = string.Format("?module=archive&action=search&keyword={1}&page={0}&size={2}", "{0}",
                    HttpUtils.UrlEncode(_keyword), pageSize);

                //pagerHtml = Helper.BuildPagerInfo(format, pageIndex, recordCount, pages);

                data = new
                {
                    keyword = _keyword ?? "",
                    headerText = tableHeaderText,
                    archiveListHtml = archiveListHtml,
                    // pagerHtml = pagerHtml
                };
            }

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_Search), data);
        }

        /// <summary>
        /// 文档评论
        /// </summary>
        public void Comment()
        {
            string commentListHtml;

            var id = int.Parse(Request.Query("archive.id"));
            var archive = LocalService.Instance.ArchiveService.GetArchiveById(SiteId, id);

            var desc = Request.Query("desc") == "true";

            var category = archive.Category;

            var sb = new StringBuilder();
            var table = CmsLogic.Comment.GetArchiveComments(archive.StrId, desc);


            var i = 1;
            var isGuest = false;
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
                        content = Regex.Replace(content, "\\(u:'(.+?)'\\)", string.Empty);
                    }
                    else
                    {
                        nickName = "游客";
                    }

                    avatar = "?module=file&action=guestavatar";
                    isGuest = true;
                }


                sb.Append("<p class=\"comment\"><img src=\"").Append(avatar).Append("\" /><span class=\"time\">")
                    .Append(string.Format("{0:yyyy-MM-dd HH:mm:ss}", dr["create_time"]))
                    .Append("</span><span class=\"details\"><a href=\"#\">")
                    .Append(nickName).Append("</a><br />").Append(content).Append("</span><span class=\"floor\">")
                    .Append(i.ToString())
                    .Append("楼</span><span class=\"control\"><a href=\"javascript:;\" onclick=\"remove(this,'")
                    .Append(dr["id"].ToString()).Append("')\">删除</a></span></p>");
                ++i;
            }

            commentListHtml = sb.Length == 0 ? "暂无评论!" : sb.ToString();

            object data = new
            {
                archive_id = archive.StrId,
                archive_title = archive.Title,
                publisher_name = archive.PublisherId,
                publish_date = string.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateTime),
                commentListHtml = commentListHtml
            };

            //base.RenderTemplate(ResourceMap.CommentList, data);
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Comment_List), data);
        }

        public void Forword()
        {
            var id = int.Parse(Request.Query("archive.id"));
            var archive = LocalService.Instance.ArchiveService.GetArchiveById(SiteId, id);
            var fullDomain = CurrentSite.FullDomain;

            if (fullDomain.IndexOf("#", StringComparison.Ordinal) != -1)
                fullDomain = fullDomain.Replace("#", WebCtx.Current.Host);
            var url = Request.GetProto() + ":" + fullDomain + archive.Path + ".html";


            if (string.IsNullOrEmpty(archive.Outline))
                archive.Outline = ArchiveUtility.GetOutline(archive.Content, 100);

            object data = new
            {
                title = archive.Title,
                outline = archive.Outline,
                url = url,
                forword_content = string.Format("<a href=\"{0}\" target=\"_blank\"><strong>{1}</strong><br />{2}</a>",
                    url,
                    archive.Title,
                    archive.Outline),
                link_content = ""
            };

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_Forward), data);
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        public string Delete_POST()
        {
            var id = int.Parse(Request.Form("archive.id"));
            var archive = LocalService.Instance.ArchiveService.GetArchiveById(SiteId, id);

            if (!ArchiveUtility.CanModifyArchive(SiteId, archive.PublisherId)) return ReturnError("您无权删除此文档!");

            try
            {
                LocalService.Instance.ArchiveService.DeleteArchive(SiteId, archive.Id);
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            //删除

            //触发删除事件
            //WatchService.PrevDeleteArchive(archive);
            //WatchService.UpdateArchive(archive);

            return ReturnSuccess();
        }

        public string Batch_Delete_POST()
        {
            var siteId = CurrentSite.SiteId;
            var idArray = Array.ConvertAll(Request.Form("id_array")[0].Split(','), a => int.Parse(a));
            try
            {
                LocalService.Instance.ArchiveService.BatchDelete(siteId, idArray);
            }
            catch (Exception exc)
            {
                return ReturnError(exc.Message);
            }

            return ReturnSuccess();
        }

        /// <summary>
        /// 重新发布文档(刷新文档)
        /// </summary>
        public void Republish_POST()
        {
            var id = int.Parse(Request.Form("archive.id"));
            LocalService.Instance.ArchiveService.RepublishArchive(SiteId, id);
            RenderSuccess();
        }


        public void MoveSortNumber_post()
        {
            var id = int.Parse(Request.Form("archive.id"));
            var di = int.Parse(Request.Form("direction"));

            try
            {
                LocalService.Instance.ArchiveService.MoveSortNumber(SiteId, id, di);
                RenderSuccess();
            }
            catch (Exception exc)
            {
                RenderError(exc.Message);
            }
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        public void DeleteComment_POST()
        {
            string commentId = Request.Form("commentId");

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

        // private HttpTags GetTags(int siteId)
        // {
        //     HttpTags _tags = null;
        //     if (!tags.Keys.Contains(siteId))
        //     {
        //         string dirPath = String.Concat(Cms.PyhicPath, CmsVariables.SITE_CONF_PRE_PATH, siteId.ToString(), "/");
        //         if (!Directory.Exists(dirPath))
        //         {
        //             Directory.CreateDirectory(dirPath).Create();
        //         }
        //         _tags = new HttpTags(String.Concat(dirPath, "tags.conf"));
        //         tags.Add(siteId, _tags);
        //     }
        //     else
        //     {
        //         _tags = tags[siteId];
        //     }
        //     return _tags;
        // }
        public void TagsIndex()
        {
            var js = ManagerTemplate.GetScriptTag();
            var css = ManagerTemplate.GetCssTag();

            var siteId = CurrentSite.SiteId;
            var content = ResourceMap.GetPageContent(ManagementPage.Archive_Tags);
            //GetTags(siteId).ProcessRequest(HttpContext.Current, content.Replace("$js()", js).Replace("$css()", css));
        }

        public void TagsIndex_POST()
        {
            var siteId = CurrentSite.SiteId;

            var content = ResourceMap.GetPageContent(ManagementPage.Archive_Tags);
            // GetTags(siteId).ProcessRequest(HttpContext.Current, content);
        }

        #endregion
    }
}