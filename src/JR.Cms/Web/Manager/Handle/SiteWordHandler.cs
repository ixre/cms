using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using JR.Cms.Library.CacheService;
using JR.Stand.Core.Extensions;
using JR.Stand.Toolkit.HttpTag;

namespace JR.Cms.Web.Manager.Handle
{
    /// <summary>
    /// 站内关键词
    /// </summary>
    public class SiteWordHandler : BasePage
    {
        /// <summary>
        /// 
        /// </summary>
        public void Index()
        {
            string tagsHtml;

            // int pageIndex;
            //
            // int.TryParse(request.GetParameter("page"), out pageIndex);
            // if (pageIndex < 1) pageIndex = 1;

            StringBuilder sb = new StringBuilder();

            Regex reg = new Regex("\\$\\{([a-zA-Z]+)\\}");
            var words = LocalService.Instance.ContentService.GetWords();
            if (words.Count > 0)
            {
                foreach (SiteWord tag in words)
                {
                    sb.Append("<tr><td class=\"center\"><input type=\"checkbox\" class=\"ck\" name=\"ck")
                        .Append(tag.Id.ToString()).Append("\" data-id=\"").Append(tag.Id.ToString()).Append("\"/></td>")
                        .Append("<td class=\"center1\">").Append(tag.Word).Append("</td><td>")
                        .Append(tag.Url).Append("</td><td>")
                        .Append(tag.Title)
                        .Append(
                            "</td><td class=\"center\"><a href=\"javascript:;\" onclick=\"edit(this)\">修改</a></td></tr>");
                }

                tagsHtml = sb.ToString();
            }
            else
            {
                tagsHtml = "<tr><td colspan=\"5\" class=\"center\">暂无tags!</td></tr>";
            }

            object data = new
            {
                tagsHtml = tagsHtml,
            };
            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Addon_Site_Word), data);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        public void Index_POST()
        {
            string action = Request.Form("action");

            switch (action)
            {
                case "save":
                    SaveWord();
                    break;
                case "delete":
                    DeleteWord();
                    break;
                case "create":
                    CreateWord();
                    break;
            }
        }


        private void CreateWord()
        {
            string msg;

            string word = Request.Form("word"),
                url = Request.Form("url"),
                title = Request.Form("title");

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(word))
            {
                sb.Append("名称不能为空!");
            }

            if (sb.Length != 0)
            {
                msg = sb.ToString();
                goto tip;
            }

            Error err = LocalService.Instance.ContentService.SaveWord(new SiteWord
            {
                Word = word,
                Url = url,
                Title = title,
            });

            msg = err == null ? "添加成功" : err.Message;
            tip:
            Response.WriteAsync($"<script>window.parent.tip('{msg}');</script>");
        }

        /// <summary>
        /// 删除Tags
        /// </summary>
        private void DeleteWord()
        {
            int i = 0;
            foreach (var key in Request.FormKeys())
            {
                if (key.StartsWith("ck") && Request.Form(key) == "on")
                {
                    int id = int.Parse(key.Substring(2));
                    LocalService.Instance.ContentService.DeleteWord(new SiteWord
                    {
                        Id = id,
                    });
                    i++;
                }
            }

            var msg = i == 0 ? "没选择要删除的项目!" : "删除成功";

            Response.WriteAsync($"<script>window.parent.tip('{msg}');</script>");
        }

        /// <summary>
        /// 保存Tags
        /// </summary>
        private void SaveWord()
        {
            string msg;

            Regex reg = new Regex("^word_([a-zA-Z0-9]+)$");

            int i = 0;
            foreach (var p in Request.FormKeys())
            {
                if (!reg.IsMatch(p)) continue;
                var id = reg.Match(p).Groups[1].Value;

                LocalService.Instance.ContentService.SaveWord(new SiteWord
                {
                    Id = int.Parse(id),
                    Word = Request.GetParameter("word_" + id),
                    Url = Request.GetParameter("url_" + id),
                    Title = Request.GetParameter("title_" + id)
                });
                i++;
            }

            msg = i == 0 ? "没有保存的项!" : "保存成功";

            Response.WriteAsync($"<script>window.parent.tip('{msg}');</script>");
        }
        
        // /// <summary>
        // /// 批量替换文档标签
        // /// </summary>
        // public void ReplaceTags()
        // {
        //     RenderTemplate(ResourceMap.ArchiveTagReplace, null);
        // }
        //
        // public void ReplaceTags_POST()
        // {
        //     foreach (Archive archive in ServiceCall.Instance.ArchiveService.SearchArchives().ToEntityList<Archive>())
        //     {
        //         //archive.Content = new TagsManager().Replace("/tags?t={0}", archive.Content);
        //         CmsLogic.Archive.Update(archive);
        //     }
        // }
    }
}