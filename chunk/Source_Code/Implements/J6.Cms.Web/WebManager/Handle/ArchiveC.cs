//
// Copyright (C) 2007-2008 Z3Q.NET,All rights reseved.
// 
// Project: J6.Cms.Manager
// FileName : Archive.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2011/10/17 10:56:06
// Description :
//
// Get infromation of this software,please visit our site http://h3f.net/cms
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
using J6.Cms.BLL;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Core;
using J6.Cms.DataTransfer;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Enum;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.Cms.Utility;
using J6.Cms.WebManager;
using J6.DevFw.Toolkit.Tags;

namespace J6.Cms.Web.WebManager.Handle
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
                Settings.TPL_MultMode ? "" : base.CurrentSite.Tpl + "/"
                ));

            EachClass.EachTemplatePage(
                dir,
                dir,
                sb2,
                TemplatePageType.Custom,
                TemplatePageType.Archive
            );

            tpls = sb2.ToString();
            sb2.Remove(0, sb2.Length);

            //获取该模块栏目JSON数据
            //categoryJSON = CmsLogic.Category.GetJson(m.ID);

            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (_category, level) =>
            {
                sb2.Append("<option value=\"").Append(_category.Id.ToString()).Append("\"");

                if (_category.Id == category.Id)
                {
                    sb2.Append(" selected=\"selected\"");
                }

                sb2.Append(">");


                for (int i = 0; i < level; i++)
                {
                    sb2.Append(CmsCharMap.Dot);
                }

                sb2.Append(_category.Name).Append("</option>");


            });
            nodesHtml = sb2.ToString();


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

                sb.Append("<dl><dt>").Append(p.Name).Append("：</dt><dd>");

                switch (uiType)
                {
                    case PropertyUI.Text:
                        sb.Append("<input type=\"text\" class=\"tb_normal box\" field=\"extend_").Append(p.Id.ToString())
                        .Append("\" value=\"").Append(p.DefaultValue).Append("\"/>");
                        break;

                    case PropertyUI.MultLine:
                        sb.Append("<textarea class=\"tb_normal box\" field=\"extend_").Append(p.Id.ToString())
                        .Append("\">").Append(p.DefaultValue).Append("</textarea>");
                        break;

                    case PropertyUI.Integer:
                        sb.Append("<input type=\"text\" class=\"tb_normal box ui-validate\" isnum=\"true\" field=\"extend_").Append(p.Id.ToString())
                       .Append("\" value=\"").Append(p.DefaultValue).Append("\"/>");
                        break;

                    case PropertyUI.Upload:
                        sb.Append("<input type=\"text  upload_value\" class=\"tb_normal box\" id=\"extend_").Append(p.Id.ToString())
                       .Append("\" field=\"extend_").Append(p.Id.ToString())
                       .Append("\" value=\"").Append(p.DefaultValue).Append("\"/>&nbsp;&nbsp;<span id=\"upload_")
                       .Append(p.Id.ToString()).Append("\">选择文件</span>")
                       .Append("<script type=\"text/javascript\">j6.propertyUpload(")
                       .Append("'upload_").Append(p.Id.ToString()).Append("','extend_").Append(p.Id.ToString()).Append("');</script>");
                        break;
                }

                sb.Append("</dd></dl>");

                /*<p><span class="txt">标签：</span>
                    <span class="input"><input type="text" name="tags" size="30"/></span>
                    <span class="msg"></span></p>
                */
            }


            sb.Append("</div>");
            extendFieldsHtml = sb.ToString();


            // extendFieldsHtml = "<div style=\"color:red;padding:20px;\">所选栏目模块不包含任何自定义属性，如需添加请到模块管理-》属性设置</div>";


            object json = new
            {
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
                tpls = tpls,
                json = JsonSerializer.Serialize(json)
            };

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_Create), data);
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

            int resultId = ServiceCall.Instance.ArchiveService.SaveArchive(this.SiteId, archive);


            //调用监视操作
            // WatchService.PublishArchive(archive);

            //返回文章ID
            base.RenderSuccess(resultId.ToString());
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        public void Update_GET()
        {

            object data;
            int archiveId = int.Parse(base.Request["archive.id"]);
            string tpls,nodesHtml,
                //栏目JSON
                extendFieldsHtml = "";                                        //属性Html

            Module module;

            int siteId = this.CurrentSite.SiteId;

            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(siteId, archiveId);

            int categoryId = archive.Category.Id;




            #region 旧的

            //获取上次发布的栏目
            //Category category = CmsLogic.Category.Get(a => a.ID == archive.Category.ID);

            //if (base.CompareSite(category.SiteId)) return;

            //bool categoryIsNull = category == null;
            /*
            if (!categoryIsNull)
            {

                module = CmsLogic.Module.GetModule(category.ModuleID);

                //=============  拼接模块的属性值 ==============//

                StringBuilder sb = new StringBuilder(50);
                IList<DataExtend> extends = null;// new List<DataExtend>(CmsLogic.DataExtend.GetExtendsByModule(module));

                if (extends.Count > 0)
                {

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


                    //======== 生成值 =============//

                    IEnumerable<DataExtendAttr> attrs;
                    IDictionary<string, string> extendFields = null;// CmsLogic.DataExtend.GetExtendFiledDictionary(archive.ID);
                    string attrValue;

                    foreach (DataExtend e in extends)
                    {
                        sb.Append("<div class=\"dataextend_item\">");

                        attrs = CmsLogic.DataExtend.GetExtendAttrs(e.ID);

                        foreach (DataExtendAttr p in attrs)
                        {


                            attrValue = extendFields.ContainsKey(p.AttrName) ? extendFields[p.AttrName] : p.AttrVal;

                            sb.Append("<dl><dt>").Append(p.AttrName).Append("：</dt><dd>");

                            if (p.AttrType == ((int)PropertyUI.Text).ToString())
                            {
                                sb.Append("<input type=\"text\" class=\"tb_normal\" name=\"extend_").Append(p.ID.ToString())
                                    .Append("\" value=\"").Append(attrValue).Append("\"/>");
                            }
                            else if (p.AttrType == ((int)PropertyUI.Upload).ToString())
                            {
                                sb.Append("<input type=\"text\" disabled=\"disabled\" class=\"tb_normal\" id=\"extend_").Append(p.ID.ToString()).Append("\" name=\"extend_").Append(p.ID.ToString())
                                    .Append("\" value=\"").Append(attrValue).Append("\"/><span id=\"upload_")
                                    .Append(p.ID.ToString()).Append("\">选择文件</span>")
                                    .Append("<script type=\"text/javascript\">j6.propertyUpload(")
                                    .Append("'upload_").Append(p.ID.ToString()).Append("','extend_").Append(p.ID.ToString()).Append("');</script>");
                            }


                            sb.Append("</dd></dl>");

                            //<p><span class="txt">标签：</span>
                            //    <span class="input"><input type="text" name="tags" size="30"/></span>
                             //   <span class="msg"></span></p>
                            
                        }


                        sb.Append("</div>");
                    }

                    extendFieldsHtml = sb.ToString();
                }

                // extendFieldsHtml = "<div style=\"color:red;padding:20px;\">所选栏目模块不包含任何自定义属性，如需添加请到模块管理-》属性设置</div>";

            }
*/

            #endregion

            //=============  拼接模块的属性值 ==============//

            StringBuilder sb = new StringBuilder(50);

            string attrValue;
            IExtendField field;

            sb.Append("<div class=\"dataextend_item\">");

            PropertyUI uiType;
            foreach (IExtendValue extValue in archive.ExtendValues)
            {
                field = extValue.Field;
                uiType = (PropertyUI)int.Parse(field.Type);
                attrValue = (extValue.Value ?? field.DefaultValue).Replace("<br />", "\n");

                sb.Append("<dl><dt>").Append(field.Name).Append("：</dt><dd>");


                switch (uiType)
                {
                    case PropertyUI.Text:
                        sb.Append("<input type=\"text\" class=\"tb_normal box\" field=\"extend_").Append(field.Id.ToString())
                            .Append("\" value=\"").Append(attrValue).Append("\"/>");
                        break;

                    case PropertyUI.MultLine:
                        sb.Append("<textarea class=\"tb_normal box\" field=\"extend_").Append(field.Id.ToString())
                        .Append("\">").Append(attrValue).Append("</textarea>");
                        break;

                    case PropertyUI.Integer:
                        sb.Append("<input type=\"text\" class=\"tb_normal box ui-validate\" isnum=\"true\" field=\"extend_")
                            .Append(field.Id.ToString()).Append("\" value=\"").Append(attrValue).Append("\"/>");
                        break;

                    case PropertyUI.Upload:
                        // sb.Append("<input type=\"text\" disabled=\"disabled\" class=\"tb_normal\" id=\"extend_").Append(field.ID.ToString())
                        sb.Append("<input type=\"text\" class=\"tb_normal box upload_value\" id=\"extend_").Append(field.Id.ToString())
                      .Append("\" field=\"extend_").Append(field.Id.ToString())
                      .Append("\" value=\"").Append(attrValue).Append("\"/>&nbsp;&nbsp;<span id=\"upload_")
                      .Append(field.Id.ToString()).Append("\">选择文件</span>")
                      .Append("<script type=\"text/javascript\">j6.propertyUpload(")
                      .Append("'upload_").Append(field.Id.ToString()).Append("','extend_").Append(field.Id.ToString())
                      .Append("');</script>");
                        break;
                }


                sb.Append("</dd></dl>");
            }


            sb.Append("</div>");

            extendFieldsHtml = sb.ToString();



            //获取模板视图下拉项
            StringBuilder sb2 = new StringBuilder();

            //模板目录
            DirectoryInfo dir = new DirectoryInfo(
                String.Format("{0}templates/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                Settings.TPL_MultMode ? "" : base.CurrentSite.Tpl + "/"
                ));

            EachClass.EachTemplatePage(dir,dir,
                sb2,
                TemplatePageType.Custom,
                TemplatePageType.Archive
                );

            tpls = sb2.ToString();
            sb2.Remove(0, sb2.Length);

            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (_category, level) =>
            {
                sb2.Append("<option value=\"").Append(_category.Id.ToString()).Append("\"");

                //if (_category.ModuleID != category.ModuleID)
                //{
                //    sb2.Append(" disabled=\"disabled\" class=\"disabled\"");
                //}


                sb2.Append(_category.Id == categoryId ? " selected=\"selected\"" : "").Append(">");


                for (int i = 0; i < level; i++)
                {
                    sb2.Append(CmsCharMap.Dot);
                }

                sb2.Append(_category.Name).Append("</option>");



            });
            nodesHtml = sb2.ToString();

            //标签
            Dictionary<string, bool> flags = ArchiveFlag.GetFlagsDict(archive.Flags);

            string thumbnail = !String.IsNullOrEmpty(archive.Thumbnail)
                    ? archive.Thumbnail
                    : "/" + CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto;

            object json = new
            {

                IsSpecial = flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial))
                                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial)],

                IsSystem = flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem))
                                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem)],

                IsNotVisible = !(flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible))
                                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible)]),

                AsPage = flags.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage))
                                            && flags[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage)],

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
                thumbPrefix = CmsVariables.Archive_ThumbPrefix,
                nodes = nodesHtml,
                url = Request.Url.PathAndQuery,
                tpls = tpls,
                json = JsonSerializer.Serialize(json)
            };

            base.RenderTemplate(
              BasePage.CompressHtml(ResourceMap.GetPageContent(ManagementPage.Archive_Update)),
                data);
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

            int resultId = ServiceCall.Instance.ArchiveService.SaveArchive(this.SiteId, archive);


            /*
            try
            {
                //调用监视操作
                //WatchService.UpdateArchive(CmsLogic.Archive.GetArchiveByID(archiveID));
            }
            catch
            {
            }
             */

            base.RenderSuccess("保存成功!");

        }

        private ArchiveDto GetFormCopyedArchive(int siteId, NameValueCollection form, ArchiveDto archive, string alias)
        {
            string flag = ArchiveFlag.GetFlagString(
                form["IsSystem"] == "on",   //系统
                form["IsSpecial"] == "on",  //特殊
                form["IsNotVisible"] != "on",    //隐藏
                form["AsPage"] == "on",     //作为单页
                null);

            string content = form["Content"];

            //自动替换Tags
            if (form["autotag"] == "on")
            {
                //todo:顺序调换了下
                HttpTags _tags = this.GetTags(siteId);
                content = _tags.Tags.RemoveAutoTags(content);
                content = _tags.Tags.ReplaceSingleTag(content);
            }

            archive.LastModifyDate = DateTime.Now;
            archive.Title = form["Title"].Trim();
            archive.SmallTitle = form["SmallTitle"].Trim();
            archive.Location = form["location"].Trim();
            archive.Source = form["Source"];
            archive.Outline = form["Outline"];
            archive.Alias = alias;
            archive.Flags = flag;
            archive.Tags = form["Tags"].Replace("，", ",");
            archive.Content = content;
            archive.Thumbnail = form["Thumbnail"];

            //分类
            int categoryId = int.Parse(form["categoryid"]);
            archive.Category = new CategoryDto { Id = categoryId };

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
        public void List_GET()
        {

            HttpRequest request = HttpContext.Current.Request;

            const int __pageSize = 10;

            object data;            //返回的数据

            int pageSize,
                pageIndex,
                recordCount,
                pages;

            bool? visible,
                 special,
                 system,
                 aspage;

            int? categoryId = null,
                 moduleId = null;

            string _categoryId = request["category.id"],    //只有这个参数为必传
                   _moduleId = request["moduleId"],
                   _pageIndex = request["page"] ?? "1",
                    _pageSize = request["size"],
                   _visible = request["visible"],
                   _special = request["special"],
                   _system = request["system"],
            _aspage = request["aspage"];

            int _publisherId = int.Parse(request["publisher_id"]??"0");

            StringBuilder sb = new StringBuilder();
            string categoryOpts,                 //栏目Options
                   pagerHtml,                   //分页链接
                   tableHeaderText,             //表头字段
                   archiveListHtml;             //文档列表HTML


            goto result;


            //处理页码大小并保存

            if (!Regex.IsMatch(_pageIndex, "^(?!0)\\d+$")) pageIndex = 1;   //If pageindex start with zero or lower
            else pageIndex = int.Parse(_pageIndex);

            if (String.IsNullOrEmpty(_pageSize))
            {
                object o = HttpContext.Current.Session["archivelist_pagesize"];
                int.TryParse(HttpContext.Current.Session["archivelist_pagesize"] as string, out pageSize);
            }
            else
            {
                int.TryParse(_pageSize, out pageSize);
                HttpContext.Current.Session["archivelist_pagesize"] = pageSize;
            }
            if (pageSize == 0) pageSize = __pageSize;



            //文档显示选项
            #region
            if (String.IsNullOrEmpty(_visible))
            {
                visible = null;
            }
            else
            {
                visible = String.Compare(_visible, String.Intern("true"), true) == 0;
            }

            if (String.IsNullOrEmpty(_special))
            {
                special = null;
            }
            else
            {
                special = String.Compare(_special, String.Intern("true"), true) == 0;
            }

            if (String.IsNullOrEmpty(_system))
            {
                system = null;
            }
            else
            {
                system = String.Compare(_system, String.Intern("true"), true) == 0;
            }


            if (String.IsNullOrEmpty(_aspage))
            {
                aspage = null;
            }
            else
            {
                aspage = String.Compare(_aspage, String.Intern("true"), true) == 0;
            }

            #endregion


            //获取表头
            bool isMaster = UserState.Administrator.Current.IsMaster;
            tableHeaderText = isMaster ? "<th style=\"width:60px\" class=\"center\">发布人</th>" : String.Empty;

            //加载栏目
            ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (category, level) =>
            {
                sb.Append("<option value=\"").Append(category.Id.ToString()).Append("\">");
                for (var i = 0; i < level; i++)
                {
                    sb.Append("-");
                }
                sb.Append(category.Name).Append("</option>");

            });

            categoryOpts = sb.ToString();
            sb.Remove(0, sb.Length);


            string[,] flags = new string[,]{
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem),system==null?"":(system.Value?"1":"0")},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial),special==null?"":(special.Value?"1":"0")},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible),visible==null?"":(visible.Value?"1":"0")},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage),aspage==null?"":(aspage.Value?"1":"0")}
            };

            //文档数据表,并生成Html
            DataTable dt = ServiceCall.Instance.ArchiveService.GetPagedArchives(this.SiteId, categoryId, _publisherId, flags, null, false, pageSize, pageIndex, out recordCount, out pages);


            //moduleID == null ? CmsLogic.Archive.GetPagedArchives(categoryID, _publisher_id,flags, null, false, pageSize, pageIndex, out recordCount, out pages)
            //: CmsLogic.Archive.GetPagedArchives((SysModuleType)(moduleID ?? 1), _publisher_id, flags,null, false, pageSize, pageIndex, out recordCount, out pages);


            IDictionary<string, bool> flagsDict;

            bool isSpecial,
                isSystem,
                isVisible,
                isPage;

            foreach (DataRow dr in dt.Rows)
            {
                flagsDict = ArchiveFlag.GetFlagsDict(dr["flags"].ToString());
                isSpecial = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial))
                    && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial)];

                isSystem = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem))
                    && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem)];

                isVisible = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible))
                                    && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible)];

                isPage = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage))
                                    && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage)];


                //编号
                sb.Append("<tr indent=\"").Append(dr["id"].ToString()).Append("\"><td align=\"center\">").Append(dr["id"].ToString()).Append("</td>");
                //标题
                sb.Append("<td").Append(isSpecial ? " class=\"special\">" : ">").Append(dr["title"].ToString());

                if (!String.IsNullOrEmpty(dr["alias"] as string))
                {
                    sb.Append("&nbsp;<span style=\"color:#777\">( 别名：").Append(dr["alias"].ToString()).Append(" )</span>");
                }
                sb.Append("</td>");

                //管理员可以查看发布人
                if (isMaster)
                {
                    sb.Append("<td class=\"center\"><a href=\"?module=archive&amp;action=list&amp;moduleID=").Append(_moduleId).Append("&amp;publisher_id=")
                        .Append(dr["author"].ToString()).Append("\" title=\"查看该用户发布的文档\">").Append(dr["Author"].ToString()).Append("</a></td>");
                }

                sb.Append("<td>").Append(String.Format("{0:yyyy/MM/dd HH:mm}", Convert.ToDateTime(dr["CreateDate"]))).Append("</td><td align=\"center\">")
                    .Append(dr["view_count"].ToString()).Append("</td><td><button class=\"refresh\" /></td><td><button class=\"file\" /></td><td><button class=\"edit\" /></td><td><button class=\"delete\" /></td></tr>");

            }

            archiveListHtml = sb.ToString();



            string format = String.Format("?module=archive&action=list&categoryID={0}&moduleID={1}&page={2}&size={3}&publisher_id={4}&visible={5}&special={6}&system={7}",
                _categoryId, _moduleId, "{0}", pageSize, _publisherId, _visible, _special, _system);

            pagerHtml = Helper.BuildPagerInfo(format, pageIndex, recordCount, pages);


        result:

            data = new
            {
                moduleID = _moduleId ?? String.Empty,
                categoryID = _categoryId ?? String.Empty,
                username = _publisherId
            };

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_List), data);
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

            int _publisherId ;
            int.TryParse(request["publisher_id"],out _publisherId);

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


            string[,] flags = new string[,]{
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem),_system},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial),_special},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible),_visible},
                {ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage),_aspage}
            };



            //文档数据表,并生成Html
            DataTable dt = ServiceCall.Instance.ArchiveService.GetPagedArchives(this.SiteId, categoryId,
                _publisherId, flags, null, false, pageSize, pageIndex, out recordCount, out pages);

            foreach (DataRow dr in dt.Rows)
            {
                dr["content"] = "";
            }

            //  CmsLogic.Archive.GetPagedArchives(this.SiteId, moduleID, categoryID, _publisher_id, flags, null, false, pageSize, pageIndex, out recordCount, out pages);
            //moduleID == null ? CmsLogic.Archive.GetPagedArchives(categoryID, _publisher_id,flags, null, false, pageSize, pageIndex, out recordCount, out pages)
            //: CmsLogic.Archive.GetPagedArchives((SysModuleType)(moduleID ?? 1), _publisher_id, flags,null, false, pageSize, pageIndex, out recordCount, out pages);


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
            //User author = CmsLogic.User.GetUser(archive.Author);
            UserDto user = ServiceCall.Instance.UserService.GetUser(archive.PublisherId);

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Archive_View), new
            {
                title = archive.Title,
                publisherName = user == null ? "" : user.Name,
                publishdate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateDate),
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


                IDictionary<string, bool> flagsDict;

                bool isSpecial,
                    isSystem,
                    isVisible,
                    isPage;

                foreach (DataRow dr in dt.Rows)
                {
                    flagsDict = ArchiveFlag.GetFlagsDict(dr["flags"].ToString());
                    isSpecial = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial))
                        && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSpecial)];

                    isSystem = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem))
                        && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.IsSystem)];

                    isVisible = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible))
                                        && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.Visible)];

                    isPage = flagsDict.ContainsKey(ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage))
                                        && flagsDict[ArchiveFlag.GetInternalFlagKey(BuiltInArchiveFlags.AsPage)];

                    //编号
                    sb.Append("<tr><td align=\"center\">").Append(dr["id"].ToString()).Append("</td>");
                    //标题
                    sb.Append("<td").Append(isSpecial ? " class=\"special\">" : ">").Append(dr["title"].ToString()).Append("</td>");

                    //管理员可以查看发布人
                    if (isMaster)
                    {
                        sb.Append("<td><a href=\"?module=archive&amp;action=list&amp;moduleID=&amp;publisher_id=")
                            .Append(dr["author"].ToString()).Append("\" title=\"查看该用户发布的文档\">").Append(dr["Author"].ToString()).Append("</a></td>");
                    }

                    sb.Append("<td>").Append(String.Format("{0:yyyy/MM/dd HH:mm}", Convert.ToDateTime(dr["CreateDate"]))).Append("</td><td align=\"center\">")
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
                .Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}", dr["createdate"])).Append("</span><span class=\"details\"><a href=\"#\">")
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
                publish_date = String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateDate),
                commentListHtml = commentListHtml
            };

            //base.RenderTemplate(ResourceMap.CommentList, data);
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Comment_List),data);
        }

        public void Forword_GET()
        {
            int id = int.Parse(base.Request["archive.id"]);
            ArchiveDto archive = ServiceCall.Instance.ArchiveService.GetArchiveById(this.SiteId, id);
            string fullDomain = this.CurrentSite.FullDomain;
            if (fullDomain.IndexOf("#") != -1)
            {
                fullDomain = fullDomain.Replace("#", Cms.Context.Host);
            }
            string url = fullDomain + archive.Url;


            if (String.IsNullOrEmpty(archive.Outline))
            {
                archive.Outline = ArchiveUtility.GetOutline(archive.Content, 100);
            }

            //Cms.Context.SiteDomain

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


        /// <summary>
        /// 重新发布文档(刷新文档)
        /// </summary>
        public void Republish_POST()
        {
            int id = int.Parse(base.Request["archive.id"]);
            ServiceCall.Instance.ArchiveService.RefreshArchive(this.SiteId, id);
            base.RenderSuccess();
        }


        public void MoveToSort_post()
        {
            int id = int.Parse(base.Request["archive.id"]);
            int di = int.Parse(base.Request["direction"]);

            try
            {
                ServiceCall.Instance.ArchiveService.MoveToSort(this.SiteId, id, di);
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
            string commentID = HttpContext.Current.Request.Form["commentId"];

            //
            //TODO:根据文档获取评论
            //

            // var category = CmsLogic.Category.Get(a => a.ID == archive.Cid);
            // if (base.CompareSite(category.SiteId)) return;

            CmsLogic.Comment.DeleteComment(int.Parse(commentID));
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
                string dirPath = String.Concat(Cms.PyhicPath, "config/s", siteId.ToString(), "/");
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

