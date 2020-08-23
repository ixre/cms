using System;
using System.Text.RegularExpressions;
using JR.Cms.Core;
using JR.Cms.Core.Interface;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.DataAccess.BLL;
using JR.Cms.Library.Utility;
using JR.Cms.Web.Portal.Template.Model;
using JR.Stand.Core.Template;
using JR.Stand.Core.Web;

namespace JR.Cms.Web.Portal
{
    /// <summary>
    ///     默认网站输出
    /// </summary>
    public static class DefaultWebOutput
    {
        /// <summary>
        ///     显示错误页面
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exc"></param>
        private static void RenderError(CmsContext context, Exception exc, bool stack)
        {
            context.HttpContext.Response.ContentType("text/html;charset=utf-8");
            context.HttpContext.Response.StatusCode(404);
            var content = "<span style='font-size:14px;color:#333;font-weight:300'>" + exc.Message;
            if (stack) content += "<br />堆栈信息:" + exc.StackTrace;

            content += "</span>";

            context.HttpContext.Response.WriteAsync(content);
        }

        /// <summary>
        ///     校验验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static bool CheckVerifyCode(string code)
        {
            var ctx = HttpHosting.Context;
            var key = $"$jr.site_{Cms.Context.CurrentSite.SiteId.ToString()}_verify_code";
            var strSiteId = ctx.Session.GetString(key);
            if (!string.IsNullOrEmpty(strSiteId))
                return string.Compare(code, strSiteId, StringComparison.OrdinalIgnoreCase) == 0;
            return true;
        }


        /// <summary>
        ///     呈现首页
        /// </summary>
        /// <param name="context"></param>
        public static void RenderIndex(CmsContext context)
        {
            try
            {
                ICmsPageGenerator cmsPage = new PageGeneratorObject(context);
                /*
                var site = context.CurrentSite;
                var siteId = site.SiteId;
                var cacheId = $"cms_s{siteId}_{(int) Cms.Context.DeviceType}_index_page";
                if (context.HttpContext.Request.Query("cache") == "0") Cms.Cache.Rebuilt();
                string html;
                if (Settings.Opti_IndexCacheSeconds > 0)
                {
                    ICmsCache cache = Cms.Cache;
                    var obj = cache.Get(cacheId);
                    if (obj == null)
                    {
                        html = cmsPage.GetIndex();
                        cache.Insert(cacheId, html, DateTime.Now.AddSeconds(Settings.Opti_IndexCacheSeconds));
                    }
                    else
                    {
                        html = obj as string;
                    }
                }
                else
                {
                    html = cmsPage.GetIndex();
                }*/

                var html = cmsPage.GetIndex();

                context.HttpContext.Response.WriteAsync(html);
            }
            catch (TemplateException ex)
            {
                RenderError(context, ex, false);
            }
        }


        /// <summary>
        ///     访问文档
        /// </summary>
        /// <param name="context"></param>
        /// <param name="archivePath">文档路径</param>
        public static void RenderArchive(CmsContext context, string archivePath)
        {
            if (archivePath.StartsWith("/")) archivePath = archivePath.Substring(1);
            var siteId = context.CurrentSite.SiteId;
            var archive = ServiceCall.Instance.ArchiveService.GetArchiveByIdOrAlias(siteId, archivePath);
            if (archive.Id <= 0)
            {
                Cms.Context.ErrorPage(404,"");
                return;
            }

            // 如果设置了302跳转
            if (!string.IsNullOrEmpty(archive.Location))
            {
                string url;
                if (Regex.IsMatch(archive.Location, "^http://", RegexOptions.IgnoreCase))
                {
                    url = archive.Location;
                }
                else
                {
                    if (archive.Location.StartsWith("/")) throw new Exception("URL不能以\"/\"开头!");
                    url = string.Concat(context.SiteDomain, "/", archive.Location);
                }

                context.HttpContext.Response.Redirect(url, false); //302
                return;
            }

            ICmsPageGenerator cmsPage = new PageGeneratorObject(context);

            if (!FlagAnd(archive.Flag, BuiltInArchiveFlags.Visible))
            {
                Cms.Context.ErrorPage(404,"");
                return;
            }

            var category = archive.Category;

            if (!(category.ID > 0))
            {
                Cms.Context.ErrorPage(404,"");
                return;
            }

            if (FlagAnd(archive.Flag, BuiltInArchiveFlags.AsPage))
            {
                var pagePattern = "^[\\.0-9A-Za-z_-]+\\.html$";

                if (!Regex.IsMatch(context.HttpContext.Request.GetPath(), pagePattern))
                {
                    context.HttpContext.Response.StatusCode(301);
                    context.HttpContext.Response.AppendHeader("Location",
                        $"{(string.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias)}.html");
                    return;
                }
            }
            else
            {
                //校验栏目是否正确
                if (!archivePath.StartsWith(category.Path + "/"))
                {
                    // 如果栏目和文档路径不匹配
                    ServiceCall.Instance.ArchiveService.UpdateArchivePath(siteId, archive.Id);
                    RenderError(context, new Exception("文档路径不匹配, 已自动纠正为正确地址, 请重新打开访问"), false);
                    return;
                }
            }

            //增加浏览次数
            ++archive.ViewCount;
            ServiceCall.Instance.ArchiveService.AddCountForArchive(siteId, archive.Id, 1);
            try
            {
                //显示页面
                var html = cmsPage.GetArchive(archive);
                context.HttpContext.Response.WriteAsync(html);
            }
            catch (TemplateException ex)
            {
                RenderError(context, ex, false);
            }
        }

        private static bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            var x = (int) b;
            return (flag & x) == x;
        }

        /// <summary>
        ///     呈现分类页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="catPath"></param>
        /// <param name="page"></param>
        public static void RenderCategory(CmsContext context, string catPath, int page)
        {
            if (catPath.StartsWith("/")) catPath = catPath.Substring(1);
            // 去掉末尾的"/"
            if (catPath.EndsWith("/")) catPath = catPath.Substring(0, catPath.Length - 1);
            var siteId = context.CurrentSite.SiteId;
            var category = ServiceCall.Instance.SiteService.GetCategory(siteId, catPath);
            if (!(category.ID > 0))
            {
                Cms.Context.ErrorPage(404,"");
                return;
            }

            if (!catPath.StartsWith(category.Path))
            {
                Cms.Context.ErrorPage(404,"");
                return;
            }

            ICmsPageGenerator cmsPage = new PageGeneratorObject(context);

            /*********************************
             *  @ 单页，跳到第一个特殊文档，
             *  @ 如果未设置则最新创建的文档，
             *  @ 如未添加文档则返回404
             *********************************/
            if (string.IsNullOrEmpty(category.Location))
            {
                try
                {
                    var html = cmsPage.GetCategory(category, page);
                    context.HttpContext.Response.WriteAsync(html);
                }
                catch (TemplateException ex)
                {
                    RenderError(context, ex, false);
                }
            }
            else
            {
                var url = category.Location;
                if (category.Location.IndexOf("://") != -1)
                {
                    url = category.Location;
                }
                else
                {
                    if (!category.Location.StartsWith("/"))
                        url = string.Concat(context.SiteAppPath, context.SiteAppPath == "/" ? "" : "/",
                            category.Location);
                }

                context.HttpContext.Response.Redirect(url, false); //302
            }
        }

        /// <summary>
        ///     呈现搜索页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="c"></param>
        /// <param name="w"></param>
        public static void RenderSearch(CmsContext context, string c, string w)
        {
            //检测网站状态
            if (!context.CheckSiteState()) return;

            ICmsPageGenerator cmsPage = new PageGeneratorObject(context);

            try
            {
                context.HttpContext.Response.WriteAsync(
                    cmsPage.GetSearch(
                        c ?? string.Empty
                        , w ?? string.Empty
                    )
                );
            }
            catch (TemplateException ex)
            {
                RenderError(context, ex, false);
            }
        }

        /// <summary>
        ///     呈现标签页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="t"></param>
        public static void RenderTag(CmsContext context, string t)
        {
            //检测网站状态
            if (!context.CheckSiteState()) return;
            try
            {
                ICmsPageGenerator cmsPage = new PageGeneratorObject(context);
                var html = cmsPage.GetTagArchive(t ?? string.Empty);
                context.HttpContext.Response.WriteAsync(html);
            }
            catch (TemplateException ex)
            {
                RenderError(context, ex, false);
            }
        }

        /// <summary>
        ///     文档页提交
        /// </summary>
        /// <param name="context"></param>
        /// <param name="allhtml"></param>
        public static void PostArchive(CmsContext context, string allhtml)
        {
            var req = context.HttpContext.Request;
            var rsp = context.HttpContext.Response;

            //检测网站状态
            if (!context.CheckSiteState()) return;


            string id = req.Form("id"); //文档编号
            Member member; //会员

            //提交留言
            if (req.Form("action") == "comment")
            {
                id = req.Form("ce_id");

                string view_name = req.Form("ce_nickname");
                string content = req.Form("ce_content");
                int memberID;
                member = UserState.Member.Current;

                //校验验证码
                if (!CheckVerifyCode(req.Form("ce_verify_code")))
                {
                    rsp.WriteAsync(
                        ScriptUtility.ParentClientScriptCall(
                            "ce_tip(false,'验证码不正确!');jr.$('ce_verify_code').nextSibling.onclick();"));
                    return;
                }

                if (string.Compare(content, "请在这里输入评论内容", StringComparison.OrdinalIgnoreCase) == 0 ||
                    content.Length == 0)
                {
                    rsp.WriteAsync(ScriptUtility.ParentClientScriptCall("ce_tip(false,'请输入内容!'); "));
                    return;
                }

                if (content.Length > 200)
                {
                    rsp.WriteAsync(ScriptUtility.ParentClientScriptCall("ce_tip(false,'评论内容长度不能大于200字!'); "));
                    return;
                }

                if (member == null)
                {
                    if (string.IsNullOrEmpty(view_name))
                    {
                        //会员未登录时，需指定名称
                        rsp.WriteAsync(ScriptUtility.ParentClientScriptCall("ce_tip(false,'不允许匿名评论!'); "));
                        return;
                    }

                    //补充用户
                    content = string.Format("(u:'{0}'){1}", view_name, content);
                    memberID = 0;
                }
                else
                {
                    memberID = UserState.Member.Current.ID;
                }

                CmsLogic.Comment.InsertComment(id, memberID, WebCtx.Current.UserIpAddress, content);
                rsp.WriteAsync(
                    ScriptUtility.ParentClientScriptCall(
                        "ce_tip(false,'提交成功!'); setTimeout(function(){location.reload();},500);"));
            }
        }
    }
}