using JR.Cms.DataTransfer;
using System;
using System.Text.RegularExpressions;
using JR.Cms.BLL;
using JR.Cms.Cache.CacheCompoment;
using JR.Cms.CacheService;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Core.Interface;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Models;
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
        	string cacheId=String.Concat("cms_s",siteId.ToString(),"_index_page");
        	
        	ICmsPageGenerator cmsPage=new PageGeneratorObject(context);
        	
            if (context.Request["cache"] == "0")
            {
                Cms.Cache.Rebuilt();
            }


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
                //DateTime dt = DateTime.Now;
                html = cmsPage.GetIndex(null);
                //context.Render("<br />"+(DateTime.Now - dt).TotalMilliseconds.ToString() + "<br />");
            }

            //response.AddHeader("Cache-Control", "max-age=" + maxAge.ToString());
            context.Render(html);
        }

        /// <summary>
        /// 访问文档
        /// </summary>
        /// <param name="context"></param>
        /// <param name="allhtml"></param>
        public static void RenderArchive(CmsContext context, string allhtml)
        {
            string id = null;
            string html;
            ArchiveDto archive = default(ArchiveDto);

            //检测网站状态
            if (!context.CheckSiteState()) return;

            //检查缓存
            if (!context.CheckAndSetClientCache()) return;


            var siteId = context.CurrentSite.SiteId;

            ICmsPageGenerator cmsPage = new PageGeneratorObject(context);


            Regex paramRegex = new Regex("/*([^/]+).html$", RegexOptions.IgnoreCase);
            if (paramRegex.IsMatch(allhtml))
            {
                id = paramRegex.Match(allhtml).Groups[1].Value;
                archive = ServiceCall.Instance.ArchiveService.GetArchiveByIdOrAlias(siteId, id);
            }

            if (archive.Id <= 0)
            {
                RenderNotFound(context);
                return;
            }
            else
            {
                //跳转
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

                    //context.Response.StatusCode = 301;
                    //context.Render(@"<html><head><meta name=""robots"" content=""noindex""><script>location.href='" +
                    //                   url + "';</script></head><body></body></html>");

                    return;
                }

                BuiltInArchiveFlags flag = ArchiveFlag.GetBuiltInFlags(archive.Flags);

                if ((flag & BuiltInArchiveFlags.Visible) != BuiltInArchiveFlags.Visible)
                    //|| (flag & BuiltInArchiveFlags.IsSystem)== BuiltInArchiveFlags.IsSystem)   //系统文档可以单独显示
                {
                    RenderNotFound(context);
                    return;
                }

                CategoryDto category = archive.Category;

                if (!(category.Id > 0))
                {
                    RenderNotFound(context);
                    return;
                }
                else
                {
                    string appPath = Cms.Context.SiteAppPath;
                    if (appPath != "/") appPath += "/";

                    if ((flag & BuiltInArchiveFlags.AsPage) == BuiltInArchiveFlags.AsPage)
                    {
                        string pattern = "^" + appPath + "[0-9a-zA-Z]+/[\\.0-9A-Za-z_-]+\\.html$";
                        string pagePattern = "^" + appPath + "[\\.0-9A-Za-z_-]+\\.html$";

                        if (!Regex.IsMatch(context.Request.Path, pagePattern))
                        {
                            context.Response.StatusCode = 301;
                            context.Response.RedirectLocation = String.Format("{0}{1}.html",
                                appPath,
                                String.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias
                                );
                            context.Response.End();
                            return;
                        }
                    }
                    else
                    {
                        //校验栏目是否正确
                        string categoryPath = category.UriPath;
                        string path = appPath != "/" ? allhtml.Substring(appPath.Length - 1) : allhtml;

                        if (!path.StartsWith(categoryPath + "/"))
                        {
                            RenderNotFound(context);
                            return;
                        }

                        //设置了别名,则跳转
                        if (!String.IsNullOrEmpty(archive.Alias) && String.Compare(id, archive.Alias,
                            StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            context.Response.StatusCode = 301;
                            context.Response.RedirectLocation = String.Format("{0}{1}/{2}.html",
                                appPath,
                                categoryPath,
                                String.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias
                                );
                            context.Response.End();
                            return;
                        }
                    }

                    //增加浏览次数
                    ++archive.ViewCount;
                    ServiceCall.Instance.ArchiveService.AddCountForArchive(siteId, archive.Id, 1);
                    //显示页面
                    html = cmsPage.GetArchive(archive);

                    //再次处理模板
                    //html = PageUtility.Render(html, new { }, false); 
                }
            }

            // return html;
            context.Render(html);
        }

        /// <summary>
        /// 呈现分类页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public static void RenderCategory(CmsContext context, string tag, int page)
        {

            //检查缓存
            if (!context.CheckAndSetClientCache()) return;

            int siteId = context.CurrentSite.SiteId;
            string html = String.Empty;
            CategoryDto category;
            string allcate = context.Request.Path.Substring(1);



            ICmsPageGenerator cmsPage = new PageGeneratorObject(context);

            category = ServiceCall.Instance.SiteService.GetCategory(siteId, tag);


            if (!(category.Id>0)) { RenderNotFound(context); return; }

            //获取路径
            string categoryPath = category.UriPath;
            string appPath = Cms.Context.SiteAppPath;
            string path = appPath != "/" ? allcate.Substring(appPath.Length) : allcate;

            if (!path.StartsWith(categoryPath))
            {
                RenderNotFound(context);
                return;
            }

            /*********************************
             *  @ 单页，跳到第一个特殊文档，
             *  @ 如果未设置则最新创建的文档，
             *  @ 如未添加文档则返回404
             *********************************/
            if (String.IsNullOrEmpty(category.Location))
            {
                html = cmsPage.GetCategory(category, page);
                context.Render(html);
            }
            else
            {
                string url;

                if (Regex.IsMatch(category.Location, "^http://", RegexOptions.IgnoreCase))
                {
                    url = category.Location;
                }
                else
                {
                    if (!category.Location.StartsWith("/"))
                    {
                        url = String.Concat(appPath,appPath.Length == 1?String.Empty:"/", category.Location);
                    }
                    else
                    {
                        url = category.Location;
                    }
                }


                context.Response.Redirect(url,true);  //302

                //context.Response.StatusCode = 301;
                //context.Render(@"<html><head><meta name=""robots"" content=""noindex""><script>location.href='" +
                 //                  url + "';</script></head><body></body></html>");
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
