//
// Copyright 2011 (C) OPSoft INC,All rights reseved.
// Project : OPSite.Plugin
// Name : 采集插件
// Author : newmin
// Date : 2011-08-27
//

using Ops.Cms.CacheService;
using Ops.Cms.DataTransfer;
using Ops.Cms.Domain.Interface.Content.Archive;
using Ops.Cms.Utility;
using Ops.Cms.Web.WebManager;
using Ops.Plugin.NetCrawl;
using System;
using System.Text;
using System.Web;

namespace Ops.Cms.PluginExample.ashx
{
    public class Collection : WebManage
    {
        private int siteId;

        //开始之前执行
        public override void Begin_Request()
        {
            SiteDto site = CmsWebMaster.CurrentManageSite;

            if (!(site.SiteId > 0)) throw new Exception("请登陆后再进行操作!");
            this.siteId = site.SiteId;

            PermissionAttribute permission = new PermissionAttribute(HttpContext.Current.Request.Path);
            permission.Validate(UserState.Administrator.Current);
        }


        /// <summary>
        /// 生成一个select HTML元素
        /// </summary>
        /// <param name="value">初始值</param>
        /// <returns></returns>
        private string GetSelectElement(string value)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<select id=\"categoryselect\" name=\"category\">");

            ServiceCall.Instance.SiteService.HandleCategoryTree(this.siteId, 1, (c, level) =>
            {
                sb.Append("<option style=\"background:#f0f0f0\" value=\"").Append(c.ID.ToString()).Append("\">");
                for (int i = 0; i < level; i++)
                {
                    sb.Append("-");
                }
                sb.Append(c.Name);
                //.Append(" / ").Append(c.Tag)
                sb.Append("</option>");
            });


            sb.Append("</select>");

            return sb.ToString();
        }

        public override string Return_InvokePageHtml()
        {
            return String.Format(@"<h1 style=""margin:5px;padding:0;font-size:14px;color:green"">采集说明：</h1>
                                选择栏目：{0}&nbsp;&nbsp;发布选项：<input type=""checkbox"" name=""visible"" id=""visible""/><label for=""visible"">立即显示(是否可见)</label>",
                this.GetSelectElement(null));
        }

        public override string Invoke_SinglePage(Project project, string pageUri)
        {
            project.UseMultiThread = true;

            //重置计数
            project.ResetState();


            project.InvokeSingle(pageUri, data =>
            {
                this.CreateNewArchive(data);

                /*
                if (visible)
                {
                    WatchService.PublishArchive(archive);
                }
                 */
            });

            return null;
        }

        /// <summary>
        /// 创建新的文档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateNewArchive(DataPack data)
        {
            bool visible = request.Form["visible"] == "on";
            int categoryID = int.Parse(request.Form["category"]);
            string author = UserState.Administrator.Current.UserName;

            //string[,] flags;
            string flags = ArchiveFlag.GetFlagString(false, false, visible, false, null);

            ArchiveDto archive = new ArchiveDto
            {
                Agree = 0,
                Disagree = 0,
                Category = new CategoryDto {ID = categoryID},
                Flags = flags,
                Author = author,
                Tags = String.Empty,
                Outline = String.Empty,
                Source = String.Empty,
                Title = data["title"],
                Content = ReplaceContent(data["content"]),
                CreateDate = DateTime.Now
            };

            return ServiceCall.Instance.ArchiveService.SaveArchive(this.siteId, archive);
        }

        public override string Invoke_ListPage(Project project, string listPageUri)
        {
            project.UseMultiThread = true;

            //重置计数
            project.ResetState();

            project.InvokeList(listPageUri, data =>
            {
                this.CreateNewArchive(data);

                /*
                if (visible)
                {
                    WatchService.PublishArchive(archive);
                }
                 */
            });

            return String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount,
                project.State.FailCount);
        }

        public override string Invoke_ListPage(Project project, object parameter)
        {
            project.UseMultiThread = true;

            //重置计数
            project.ResetState();

            project.InvokeList(parameter, data =>
            {
                this.CreateNewArchive(data);
                /*
                if (visible)
                {
                    WatchService.PublishArchive(archive);
                }
                 */
            });

            return String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount,
                project.State.FailCount);
        }

        private static string ReplaceContent(string content)
        {
            return content;

            //
            //UNDONE:
            //
            //return new TagsManager().Replace("/tags/{0}/", content);
        }
    }
}