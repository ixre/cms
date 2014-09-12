using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Text;
using OPS.OPSite.Models;
using OPS.OPSite;
using OPS.OPSite.Cache;
using OPS.OPSite.DAL;
using OPS.OPSite.BLL;
using System.Data;
using OPS.Web.UI;
using System.Text.RegularExpressions;
using OPSite.Plugin;

namespace OPSite.Web.Controllers
{
    public class HomeController :Controller
    {
        private static readonly ArchiveBLL bll = new ArchiveBLL();
        private static readonly CategoryBLL cbll = new CategoryBLL();
        private static readonly MemberBLL mbll = new MemberBLL();
        private static readonly CommentBLL cmbll = new CommentBLL();
        private static readonly UserBLL ubll = new UserBLL();

       // public void Tpl() { TemplateUtility.PrintTemplatesInfo(); }

        //热门贴
        private string hotTopics = String.Empty,
            recommendTopics = String.Empty,
            companyInfo = String.Empty;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //return;
            if (DateTime.Now > new DateTime(2012, 11, 1)) Response.Redirect("/images@#/",true);

            hotTopics = HttpRuntime.Cache["hottopics"] as string;
            recommendTopics = HttpRuntime.Cache["recommendtopics"] as string;
            companyInfo = HttpRuntime.Cache["companyinfo"] as string;

            DataTable table;
            StringBuilder sb;

            if (hotTopics == null)
            {
                sb = new StringBuilder();
                table = bll.GetArchivesByViewCount(ModuleType.TextNews, 6);
                foreach (DataRow dr in table.Rows)
                {
                    sb.Append("<dt><a title=\"").Append(dr["title"].ToString()).Append("\" href=\"/s_").Append(dr["id"].ToString()).Append("/\">")
                        .Append(dr["title"].ToString().Length > 15 ? dr["title"].ToString().Substring(0, 13) + "..." : dr["title"].ToString()).Append("</a></dt>");
                }
                hotTopics = sb.ToString();

                HttpRuntime.Cache.Insert("hottopics", hotTopics, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
            }

            if (recommendTopics == null)
            {
                sb = new StringBuilder();
                table = bll.GetSpecialArchives(ModuleType.TextNews, 6);
                foreach (DataRow dr in table.Rows)
                {
                    sb.Append("<h4><a title=\"").Append(dr["title"].ToString()).Append("\" href=\"s-").Append(dr["id"].ToString()).Append("/\" target=\"_blank\"><img src=\"")
                       .Append(ArchiveUtility.GetFirstPicUri(dr["content"].ToString(),false) ?? "/images/nopic.gif").Append("\" width=\"70\" height=\"70\" alt=\"").Append(dr["title"].ToString()).Append("\"/></a></h4>");
                }
                recommendTopics = sb.ToString();

                HttpRuntime.Cache.Insert("recommendtopics", recommendTopics, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
            }

            if (companyInfo == null)
            {
                Archive a = bll.Get("about");
                if (a != null)
                {
                    Regex reg=new Regex("<[^>]+>");
                    companyInfo = reg.Replace(a.Content, String.Empty);
                    if (companyInfo.Length > 130)companyInfo= companyInfo.Remove(130);
                    companyInfo += "...&nbsp;【<a href=\"/home/about/\" style=\"color:green\">详细</a>】";
                }
               // HttpRuntime.Cache.Insert("companyInfo", recommendTopics, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
            }
        }


        [AcceptVerbs("GET","POST")]
        public void AjaxTest()
        {
            System.Threading.Thread.Sleep(2000);

            Response.Write(Request["p"]);
        }

        public void Tpl()
        {
            OPSoft.Template.TemplateUtility.PrintTemplatesInfo();
        }

        public void Traffic()
        {
            TrafficCounter.Record(Request.UserHostAddress);
            Response.Write(TrafficCounter.GetData(DateTime.Now));
        }

        public void Index(string p)
        {

            const int pageSize = 20;


            string archiveListHtml,
                    pagedHtml;

            int recordCount,
                pages;

            int page;
            int.TryParse(p, out page);
            if (page < 1) page = 1;

            StringBuilder sb = new StringBuilder(800);

            User m;
            int i = 0, j;
            string membername;
            foreach (DataRow dr in bll.GetPagedArchives(null,null,true,null,false,null,false,pageSize, page, out recordCount, out pages).Rows)
            {
                j = i++ % 3;
                m = ubll.GetUser(dr["author"].ToString());
                membername = m == null ? dr["author"].ToString() : m.Name;
                if (j == 1) membername = String.Format("<span style=\"color:green\">{0}</span>", membername);
                else if (j == 2) membername = String.Format("<span style=\"color:#0066cc\">{0}</span>", membername);

                sb.Append("<li><img src=\"/images/default_face.gif\" width=\"20\" height=\"20\" align=\"absmiddle\" /><span class=\"bbs1\">")
                    .Append(membername).Append("&nbsp;</span><span class=\"cDGray\">").Append(String.Format("{0:MM月dd日}", dr["createdate"]))
                    .Append("</span> 发布: <a href=\"/s_").Append(dr["id"].ToString()).Append("/\" target=\"_blank\" class=\"bbs1\">").Append(dr["title"].ToString())
                    .Append("</a></li>");

            }

            archiveListHtml = sb.ToString();

            pagedHtml = OPS.Web.UI.PagedLinkBuilder.BuildPagerInfo("/", "/{0}/", page, recordCount, pages);


            //输出到页面
            PageUtility.Render("0ffc35d4b482f1d1",
            new
            {
                pageTitle = page == 1 ? "" : "(第" + page + "页)",
                archiveListHtml = archiveListHtml,
                pagedHtml = pagedHtml,

                h_topics = hotTopics,
                r_topics = recommendTopics,
                h_companyinfo = companyInfo
            });

        }

        /// <summary>
        /// 验证码
        /// </summary>
        public void VerifyImage()
        {
            VerifyCode v = new VerifyCode();
            v.AllowRepeat = false;
            v.DisplayAsVerifyImage(4, VerifyWordOptions.Letter);
        }

        /// <summary>
        /// 管理地址
        /// </summary>
        [ValidateInput(false)]
        public void Manage()
        {
            OPS.OPSite.Manager.Logic.Request(System.Web.HttpContext.Current);
        }


        /// <summary>
        /// 关于我们页面
        /// </summary>
        public void About()
        {
            Archive a = bll.Get("about");
            if (a == null) { Render404(); return; }


            PageUtility.Render("6f076647256a4ea1",
                new
                {
                    archiveName = a.Title,
                    content = a.Content,


                    h_topics = hotTopics,
                    r_topics = recommendTopics,
                    h_companyinfo=companyInfo
                });
        }

        /// <summary>
        /// 客户服务
        /// </summary>
        public void Service()
        {

            Archive a = bll.Get("service");
            if (a == null) { Render404(); return; }


            PageUtility.Render("6f076647256a4ea1",
                new
                {
                    archiveName = a.Title,
                    content = a.Content,


                    h_topics = hotTopics,
                    r_topics = recommendTopics,
                    h_companyinfo = companyInfo
                });
        }

        /// <summary>
        /// 列表页面
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public void List(string tag, int page)
        {
            const int pageSize = 20;

            Category catalog = cbll.Get(a => a.Tag == tag);
            if (catalog == null){ Render404(); return;}

            string archiveListHtml,
                    pagedHtml,
                    keywords = catalog.Keywords,
                    descripton = catalog.Description;

            int recordCount,
                pages;

            StringBuilder sb = new StringBuilder(800);

            User m;
            int i = 0, j;
            string membername;
            foreach (DataRow dr in bll.GetPagedArchives(catalog.ID, pageSize,page, out recordCount, out pages).Rows)
            {
                j = i++ % 3;
                m=ubll.GetUser(dr["author"].ToString());
                membername = m == null ? dr["author"].ToString() : m.Name;
                if (j == 1) membername = String.Format("<span style=\"color:green\">{0}</span>", membername);
                else if (j == 2) membername = String.Format("<span style=\"color:#0066cc\">{0}</span>", membername);

                sb.Append("<li><img src=\"/images/default_face.gif\" width=\"20\" height=\"20\" align=\"absmiddle\" /><span class=\"bbs1\">")
                    .Append(membername).Append("&nbsp;</span><span class=\"cDGray\">").Append(String.Format("{0:MM月dd日}",dr["createdate"]))
                    .Append("</span> 发布: <a href=\"/s_").Append(dr["id"].ToString()).Append("/\" target=\"_blank\" class=\"bbs1\">").Append(dr["title"].ToString())
                    .Append("</a> [<span class=\"cDGray\">").Append(catalog.Name).Append("</span>]</li>");

            }

            archiveListHtml = sb.ToString();

            pagedHtml = OPS.Web.UI.PagedLinkBuilder.BuildPagerInfo("/"+catalog.Tag+"/","/" + catalog.Tag + "/{0}/",page, recordCount, pages);


            //输出到页面
            PageUtility.Render("ba546efe2c0c5095",
            new
            {
                catalogName = catalog.Name,
                catalogTag = catalog.Tag,
                pageTitle = page == 1 ? "" : "(第" + page + "页)",
                archiveListHtml = archiveListHtml,
                pagedHtml = pagedHtml,

                h_topics = hotTopics,
                r_topics = recommendTopics,
                h_companyinfo = companyInfo
            });

        }

        /// <summary>
        /// 文档显示页面
        /// </summary>
        /// <param name="id"></param>
        public void Show(string id)
        {

            Archive archive;
            Member member;
            Category catalog;
            DataTable comments;
            string user,            //发贴名称
                replayContent;      //评论列表

            
            StringBuilder sb = new StringBuilder();
            
            archive= bll.Get(id);
            if (archive == null || archive.IsSystem) {Render404(); return;}




            //增加评论

            new System.Threading.Thread(() =>
            {
                bll.AddViewCount(id, 1);
            }).Start();

            //获取分类
            catalog = cbll.Get(c => c.ID == archive.CatalogID);

            //获取作者昵称
            member = mbll.GetMember(archive.Author);
            user = member == null ? "游客" : member.Nickname;

            //获取评论
            comments = cmbll.GetArchiveComments(archive.ID);
            string nickname, content;
            Match match;
            for (int i = 0; i < comments.Rows.Count; i++)
            {
                content = comments.Rows[i]["content"].ToString();
                match = Regex.Match(content, "\\(u:'(?<user>.+?)'\\)");

                if (match != null)
                {
                    nickname = match.Groups["user"].Value;
                    content = Regex.Replace(content, "\\(u:'(.+?)'\\)", String.Empty);
                }
                else
                {
                    nickname = "游客";
                }

                sb.Append("<li><dl><!--<em><a href=\"#\">引用</a></em>--><span class=\"cDGray\">")
                 .Append((i + 1).ToString()).Append("楼</span>&nbsp;<span class=\"cGray\">").Append(nickname)
                 .Append("</span>&nbsp;于 <span class=\"cDGray\">")
                 .Append(String.Format("{0:yyyy-MM-dd HH:mm:ss}", comments.Rows[i]["createDate"])).Append("</span> 说：</dl><dt><p>")
                 .Append(content).Append("</p></dt></li>");
            }
            if (sb.Length == 0)
            {
                replayContent = "<li style=\"text-align:center\">暂无评论</li>";
            }
            else
            {
                replayContent = sb.ToString();
                sb.Remove(0, sb.Length);
            }


            string tagsLink = String.Empty;



            //如果关键词不为空
            if (!String.IsNullOrEmpty(archive.Tags))
            {
                sb.Remove(0, sb.Length);
                Array.ForEach(archive.Tags.Split(','), str =>
                {
                    sb.Append("<strong><a href=\"${domain}/tag/")
                        .Append(Server.UrlEncode(str)).Append("/\" title=\"")
                        .Append(str).Append("\">").Append(str).Append("</a></strong>");
                });

                tagsLink = sb.ToString();
            }

            
            //呈现页面
            PageUtility.Render("3d9e2da210eea3fb",
                 new
                 {
                     sitemap = @"<a href=""${domain}/archive${suffix}/list/${catalogTag}/"">${catalogName}</a>",
                     catalogName = catalog.Name,
                     catalogTag = catalog.Tag,
                     archiveTitle = archive.Title,
                     author = archive.Author,
                     content = archive.Content,
                     keywords = archive.Tags,
                     click=++archive.ViewCount,
                     user=user,
                     replays=comments.Rows.Count,
                     outline = ArchiveUtility.GetOutline(archive, 190),
                     source = String.IsNullOrEmpty(archive.Source) ? "${site.shortname}" : archive.Source,
                     publishdate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateDate),
                     tagsLink = tagsLink,

                     guestName=String.Format("游客:{0}",Request.UserHostAddress),
                     replayContent=replayContent,


                     h_topics = hotTopics,
                     r_topics = recommendTopics,
                     h_companyinfo = companyInfo

                 });

        }

        /// <summary>
        /// 增加评论
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [AcceptVerbs("POST")]
        [ValidateInput(false)]
        public string Show(string id,FormCollection form)
        {
            const string tipFMT = "<script type=\"text/javascript\">alert('{0}')</script>";

            string view_name=form["view_name"];

            if (String.IsNullOrEmpty(view_name))
            {
                return String.Format(tipFMT, "名称不能为空!");
            }
            else if (form["verifycode"] == "" || !VerifyCode.Verify(form["verifycode"], true))
            {
                return String.Format(tipFMT, "验证码不正确!");
            }

            string content = form["editor"];
            if (content.Length == 0)
            {
                return String.Format(tipFMT, "请输入内容!");
            }

            //去掉HTML

            //补充用户
            content = String.Format("(u:'{0}'){1}", view_name, content);

            cmbll.InsertComment(id, 0, content);


            return "<script type=\"text/javascript\">alert('评论成功！');window.parent.location.reload();</script>";

        }



        public void Post()
        {
            PageUtility.Render("00711f0d5916456d", new
            {
                guestName = String.Format("游客:{0}", Request.UserHostAddress),

                h_topics = hotTopics,
                r_topics = recommendTopics,
                h_companyinfo = companyInfo

            });
        }

        [AcceptVerbs("POST")]
        [ValidateInput(false)]
        public string Post(FormCollection form)
        {
            const string tipFMT = "<script type=\"text/javascript\">alert('{0}')</script>";

            string nickname = form["post_name"],
                title = form["post_title"],
                tag = form["post_type"],
                verifyCode = form["verifycode"],
                content = form["editor"];

            if (content.Length == 0) return String.Format(tipFMT,"请输入内容");
            else if (verifyCode == "" || !VerifyCode.Verify(verifyCode, true))
            {
                return String.Format(tipFMT, "验证码不正确!");
            }
            Category catalog = cbll.Get(a => a.Tag == tag);

            string id= bll.Create(new Archive
            {
                Author = nickname,
                CatalogID = catalog.ID,
                Content = content,
                Tags = "",
                Source = "",
                Title = title,
                ViewCount = 1,
                Visible = true
            });

            return String.Format("<script type=\"text/javascript\">alert('发布成功！');window.parent.location.href='/s_{0}/';</script>",id);
        }

        private void Render404()
        {
            Response.Write("404");
        }
    }
}
