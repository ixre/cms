using JR.Cms.DataTransfer;
using System;
using System.Text.RegularExpressions;
using JR.Cms.Cache.CacheCompoment;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Core.Interface;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.DataAccess.BLL;
using JR.Cms.Utility;
using JR.Cms.Web.PageModels;

namespace JR.Cms.Web
{
    /// <summary>
    /// 默认网站输出
    /// </summary>
    public class DefaultWebOuput
    {

    	internal static void RenderNotFound(CmsContext context)
    	{
    		context.RenderNotfound("No such file",tpl=>{
                                    tpl.AddVariable("site", new PageSite(context.CurrentSite));
    		                       	tpl.AddVariable("page",new PageVariable());
    		     					PageUtility.RegistEventHandlers(tpl);
    		 });
    	}
    	///<summary>
        /// 校验验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckVerifyCode(string code)
        {
            var sess =  System.Web.HttpContext.Current.Session[String.Format("$jr.site_{0}_verifycode", Cms.Context.CurrentSite.SiteId.ToString())];
            if (sess != null)
            {
                return String.Compare(code, sess.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
            }
            return true;
        }


        /// <summary>
        /// 呈现首页
        /// </summary>
        /// <param name="context"></param>
        public static void RenderIndex(CmsContext context)
        {
            //检测网站状态
            if (!context.CheckSiteState()) return;
            //检查缓存
            if (!context.CheckAndSetClientCache()) return;

            SiteDto site = context.CurrentSite;
        	int siteId=site.SiteId;
        	string cacheId=String.Format("cms_s{0}_{1}_index_page",siteId,(int)Cms.Context.DeviceType);
        	ICmsPageGenerator cmsPage=new PageGeneratorObject(context);
            if (context.Request["cache"] == "0") Cms.Cache.Rebuilt();
            string html;
            if (Settings.Opti_IndexCacheSeconds > 0)
            {
                ICmsCache cache = Cms.Cache;
                object obj = cache.Get(cacheId);
                if (obj == null)
                {
                    html = cmsPage.GetIndex(null);
                    cache.Insert(cacheId, html, DateTime.Now.AddSeconds(Settings.Opti_IndexCacheSeconds));
                }
                else
                {
                    html = obj as string;
                }
            }
            else
            {
                html = cmsPage.GetIndex(null);
            }
            context.Render(html);
        }

        /// <summary>
        /// 访问文档
        /// </summary>
        /// <param name="context"></param>
        /// <param name="allhtml"></param>
        public static void RenderArchive(CmsContext context, string allhtml)
        {
            string html;
            ArchiveDto archive = default(ArchiveDto);

            var siteId = context.CurrentSite.SiteId;
            String archivePath = allhtml.Substring(0, allhtml.Length - ".html".Length);
            archive = ServiceCall.Instance.ArchiveService.GetArchiveByIdOrAlias(siteId, archivePath);
            if (archive.Id <= 0)
            {
                RenderNotFound(context);
                return;
            }

            // 如果设置了302跳转
            if (!String.IsNullOrEmpty(archive.Location))
            {
                string url;
                if (Regex.IsMatch(archive.Location, "^http://", RegexOptions.IgnoreCase))
                {
                    url = archive.Location;

                }
                else
                {
                    if (archive.Location.StartsWith("/")) throw new Exception("URL不能以\"/\"开头!");
                    url = String.Concat(context.SiteDomain, "/", archive.Location);
                }
                context.Response.Redirect(url, true); //302
                return;
            }

            ICmsPageGenerator cmsPage = new PageGeneratorObject(context);

            if (!FlagAnd(archive.Flag, BuiltInArchiveFlags.Visible))
            {
                RenderNotFound(context);
                return;
            }

            CategoryDto category = archive.Category;

            if (!(category.ID > 0))
            {
                RenderNotFound(context);
                return;
            }
            if (FlagAnd(archive.Flag, BuiltInArchiveFlags.AsPage))
            {
                string pagePattern = "^[\\.0-9A-Za-z_-]+\\.html$";

                if (!Regex.IsMatch(context.Request.Path, pagePattern))
                {
                    context.Response.StatusCode = 301;
                    context.Response.RedirectLocation = String.Format("{0}.html",
                        String.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias
                        );
                    context.Response.End();
                    return;
                }
            }
            else
            {
                //校验栏目是否正确
                string categoryPath = category.Path;
                if (!archivePath.StartsWith(categoryPath + "/"))
                {
                    RenderNotFound(context);
                    return;
                }
            }

            //增加浏览次数
            ++archive.ViewCount;
            ServiceCall.Instance.ArchiveService.AddCountForArchive(siteId, archive.Id, 1);
            //显示页面
            html = cmsPage.GetArchive(archive);
            
            // return html;
            context.Render(html);
        }

        private static bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            int x = (int)b;
            return (flag & x) == x;
        }

        /// <summary>
        /// 呈现分类页
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public static void RenderCategory(CmsContext ctx, string catPath, int page)
        {

            int siteId = ctx.CurrentSite.SiteId;
            string html = String.Empty;
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(siteId, catPath);
            if (!(category.ID>0)) { RenderNotFound(ctx); return; }

            if (!catPath.StartsWith(category.Path))
            {
                RenderNotFound(ctx);
                return;
            }

            ICmsPageGenerator cmsPage = new PageGeneratorObject(ctx);

            /*********************************
             *  @ 单页，跳到第一个特殊文档，
             *  @ 如果未设置则最新创建的文档，
             *  @ 如未添加文档则返回404
             *********************************/
            if (String.IsNullOrEmpty(category.Location))
            {
                html = cmsPage.GetCategory(category, page);
                ctx.Render(html);
            }
            else
            {
                string url = category.Location;
                if (category.Location.IndexOf("://") != -1)
                {
                    url = category.Location;

                }
                else
                {
                    if (!category.Location.StartsWith("/"))
                    {
                        url = String.Concat(ctx.SiteAppPath,ctx.SiteAppPath =="/"?"":"/", category.Location);
                    }
                }
                ctx.Response.Redirect(url,true);  //302
            }
        }

        /// <summary>
        /// 呈现搜索页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="c"></param>
        /// <param name="w"></param>
        public static void RenderSearch(CmsContext context, string c, string w)
        { 
            //检测网站状态
            if (!context.CheckSiteState()) return;

        	ICmsPageGenerator cmsPage=new PageGeneratorObject(context);
        	
            context.Render(
            cmsPage.GetSearch(
                 c ?? String.Empty
                , w ?? String.Empty
                )
                );
        }

        /// <summary>
        /// 呈现标签页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="t"></param>
        public static void RenderTag(CmsContext context, string t)
        {
            //检测网站状态
            if (!context.CheckSiteState()) return;

        	ICmsPageGenerator cmsPage=new PageGeneratorObject(context);
        	
            context.Render(cmsPage.GetTagArchive(t ?? String.Empty));
        }

        /// <summary>
        /// 文档页提交
        /// </summary>
        /// <param name="context"></param>
        /// <param name="allhtml"></param>
        public static void PostArchive(CmsContext context, string allhtml)
        {
            var form = context.Request.Form;
            var rsp = context.Response;

            //检测网站状态
            if (!context.CheckSiteState()) return;


            string id = form["id"];           //文档编号
            Member member;              //会员

            //提交留言
            if (form["action"] == "comment")
            {
                id = form["ce_id"];

                string view_name = form["ce_nickname"];
                string content = form["ce_content"];
                int memberID;
                member = UserState.Member.Current;

                //校验验证码
                if (!CheckVerifyCode(form["ce_verifycode"]))
                {
                    rsp.Write(ScriptUtility.ParentClientScriptCall("cetip(false,'验证码不正确!');jr.$('ce_verifycode').nextSibling.onclick();"));
                    return;
                }
                else if (String.Compare(content, "请在这里输入评论内容", true) == 0 || content.Length == 0)
                {
                    rsp.Write(ScriptUtility.ParentClientScriptCall("cetip(false,'请输入内容!'); "));
                    return;
                }
                else if (content.Length > 200)
                {
                    rsp.Write(ScriptUtility.ParentClientScriptCall("cetip(false,'评论内容长度不能大于200字!'); "));
                    return;
                }

                if (member == null)
                {
                    if (String.IsNullOrEmpty(view_name))
                    {
                        //会员未登录时，需指定名称
                        rsp.Write(ScriptUtility.ParentClientScriptCall("cetip(false,'不允许匿名评论!'); "));
                        return;
                    }
                    else
                    {
                        //补充用户
                        content = String.Format("(u:'{0}'){1}", view_name, content);
                        memberID = 0;
                    }
                }
                else
                {
                    memberID = UserState.Member.Current.ID;
                }
                CmsLogic.Comment.InsertComment(id, memberID, context.Request.UserHostAddress, content);
                rsp.Write(ScriptUtility.ParentClientScriptCall("cetip(false,'提交成功!'); setTimeout(function(){location.reload();},500);"));
                return;
            }
        }
    }
}
