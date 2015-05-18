//
// 文件名称: CmsController.cs
// 作者：  newmin
// 创建时间：2012-10-01
// 修改说明：
//  2012-11-12  06:55   newmin  [!]:修改自定义页面的链接
//  2013-01-05  09:02   newmin  [!]:修正特殊文档不能查找到第一个，不立即返回404的错误
//  2013-02-01  10:00   newmin  [+]: 表格提交
//  2013-03-06  11:17   newmin  [+]: 评论模块
//
namespace Spc.Web
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Spc;
    using Spc.BLL;
    using Spc.Models;
    using Ops.Data;
    using Ops.Graphic;
    using Ops.Template;
    using Spc.Web.Mvc;
    using Spc.Cache;

    /// <summary>
    /// 
    /// </summary>
    public class MultLangCmsController : Spc.Web.Mvc.ControllerBase
    {
        private static ArchiveBLL bll = new ArchiveBLL();
        private static CategoryBLL cbll = new CategoryBLL();
        private static CommentBLL cmbll=new CommentBLL();
        ICmsPageGenerator cmsPage = new MultLangPageGeneratorObject();


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 设置语言

            string lang = filterContext.RouteData.Values["lang"] as string;
            if (lang != null)
            {
                lang = Array.Find(Lang.Langs, a => String.Compare(a, lang, true) == 0);
                if (lang == null)
                {
                    base.RenderNotfound();
                    filterContext.HttpContext.Response.End();
                    return;
                }
            }
            else
            {
                lang = Lang.defaultLang;
            }
            HttpContext.Items["lang"] = lang;

            #endregion
        }

        #region 首页

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public void ChangeLang(string lang)
        {
            if (String.IsNullOrEmpty(lang))
            {
                lang = Lang.defaultLang;
            }

            if (Array.IndexOf(Lang.Langs, lang) != -1)
            {
                Response.Redirect("/" + lang + "/",true);
                return;
            }


             base.RenderNotfound();
        }

        /// <summary>
        /// 首页
        /// </summary>
        public void Index(string lang)
        {
            //if (!String.IsNullOrEmpty(Settings.SYS_VIRTHPATH))
            //{
            //    throw new NotSupportedException("不支持虚拟路径运行，请在后台管理中取消！");
            //}

            string html = String.Empty;
            if (Settings.Opti_IndexCacheSeconds > 0)
            {
                ICmsCache cache= Cms.Cache;
                object obj = cache.Get("cms_index_page_"+lang);
                if (obj == null)
                {
                    html = cmsPage.GetIndex(lang);
                    cache.Insert("cms_index_page_" + lang, html, DateTime.Now.AddSeconds(Settings.Opti_IndexCacheSeconds));
                }
                else
                {
                    html = obj as string;
                }

            }
            else
            {
                html = cmsPage.GetIndex(lang);
            }

            //response.AddHeader("Cache-Control", "max-age=" + maxAge.ToString());
            base.Render(html);

        }

        #endregion

        #region 系统相关

        public void Template()
        {
            TemplateUtility.PrintTemplatesInfo();
        }


        /// <summary>
        /// 管理网站
        /// </summary>
        [ValidateInput(false)]
        public void Admin()
        {
            global::Spc.WebManager.Logic.Request(System.Web.HttpContext.Current);
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="length"></param>
        /// <param name="opt"></param>
        [HttpGet]
        public void VerifyImg(string length, string opt,string usefor)
        {
            int _length;                        //验证码长度
            VerifyWordOptions _opt;  //验证码选项
            string verifycode;              //验证码

            if (String.IsNullOrEmpty(length) || !Regex.IsMatch(length, "^\\d+$"))
            {
                _length = 4;
            }
            else
            {
                _length = int.Parse(length);
            }

            if (String.IsNullOrEmpty(opt) || !Regex.IsMatch(opt, "^\\d$"))
            {
                _opt = VerifyWordOptions.Number;
            }
            else
            {
                _opt = (VerifyWordOptions)int.Parse(opt);
            }

            VerifyCode v = new VerifyCode();
            v.AllowRepeat = false;

            //显示验证码
            v.RenderGraphicImage(_length, _opt, true, out verifycode, "Image/Jpeg");

            //保存验证码
             Session["$cms.verifycode"]=verifycode;
        }

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
       private bool CheckVerifyCode(string code)
       {
        
                             var sess = Session["$cms.verifycode"];
                            if (sess != null)
                            {
                                return String.Compare(code, sess.ToString(), true) == 0;
                            }
                            return true;
       }

        #endregion

        #region 文档

        /// <summary>
        /// 栏目页面
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        /// <returns></returns>
       public void Category(string lang, string allcate)
       {
           string tag = null;
           int page = 1;

           Regex paramRegex = new Regex("/*([^/]+)/(p(\\d+).html)*$", RegexOptions.IgnoreCase);
           if (paramRegex.IsMatch(allcate))
           {
               Match mc = paramRegex.Match(allcate);

               tag = mc.Groups[1].Value;

               //计算页吗:页码如:p3
               if (mc.Groups[3].Value != "")
               {
                   page = int.Parse(mc.Groups[3].Value);
               }
           }
           else
           {
               base.RenderNotfound();
               return;
           }

           string html = String.Empty;
           Category category = cbll.Get(a => String.Compare(tag, a.Tag, true) == 0);

           if (category == null)
           {
               base.RenderNotfound();
               return;
           }
           else
           {
               //获取路径
               string categoryPath = ArchiveUtility.GetCategoryUrlPath(category);

               if (!allcate.StartsWith(categoryPath + "/"))
               {
                   base.RenderNotfound();
                   return;
               }
               /*********************************
                *  @ 单页，跳到第一个特殊文档，
                *  @ 如果未设置则最新创建的文档，
                *  @ 如未添加文档则返回404
                *********************************/
               if (category.ModuleID == (int)SysModuleType.CustomPage)
               {
                   Archive a = bll.GetFirstSpecialArchive(category.ID);
                   if (a == null)
                   {
                       global::System.Data.DataTable dt = bll.GetArchives(category.ID, 1);
                       if (dt.Rows.Count == 1)
                       {
                           a = dt.Rows[0].ToEntity<Archive>();
                       }
                       else
                       {
                           base.RenderNotfound("栏目下应确保有推荐的文档存在!");
                           return;
                       }
                   }
                   Response.StatusCode = 302;

                   Response.RedirectLocation=String.Format("/{0}/{1}/{2}.html",
                       lang,
                       categoryPath,
                       String.IsNullOrEmpty(a.Alias) ? a.ID : a.Alias
                       );
                   Response.End();
                   return;
               }

               html = cmsPage.GetCategory(category, page, lang);
           }

           base.Render(html);
       }

        /// <summary>
        /// 文档页
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="id"></param>
        /// <returns></returns>
       public void Archive(string lang, string allhtml)
       {
           string id = null;
           string html = String.Empty;
           Archive archive = null;
           Category category = null;


           Regex paramRegex = new Regex("/*([^/]+).html$", RegexOptions.IgnoreCase);
           if (paramRegex.IsMatch(allhtml))
           {
               id = paramRegex.Match(allhtml).Groups[1].Value;
               archive = bll.Get(id);
           }



           if (archive == null)
           {
               base.RenderNotfound();
               return;
           }
           else
           {
               BuiltInArchiveFlags flag = ArchiveFlag.GetBuiltInFlags(archive.Flags);

               if ((flag & BuiltInArchiveFlags.Visible) != BuiltInArchiveFlags.Visible)
                    //|| (flag & BuiltInArchiveFlags.IsSystem) == BuiltInArchiveFlags.IsSystem)
               {
                   base.RenderNotfound();
                   return;
               }


               category = cbll.Get(a => a.ID == archive.Cid);
               if (category == null)
               {
                   base.RenderNotfound();
                   return;
               }
               else
               {

                   string vpath = "/" + lang;

                   if ((flag & BuiltInArchiveFlags.AsPage) == BuiltInArchiveFlags.AsPage)
                   {
                       string pattern = "^" + vpath + "/[0-9a-zA-Z]+/[\\.0-9A-Za-z_-]+\\.html$";
                       string pagePattern = "^" + vpath + "/[\\.0-9A-Za-z_-]+\\.html$";

                       if (!Regex.IsMatch(Request.Path, pagePattern))
                       {
                           Response.StatusCode = 301;
                           Response.RedirectLocation = String.Format("/{0}/{1}.html",
                               lang,
                               String.IsNullOrEmpty(archive.Alias) ? archive.ID : archive.Alias
                               );
                           Response.End();
                           return;
                       }
                   }
                   else
                   {
                       //校验栏目是否正确
                       string categoryPath = ArchiveUtility.GetCategoryUrlPath(category);
                       if (!allhtml.StartsWith(categoryPath + "/"))
                       {
                           base.RenderNotfound();
                           return;
                       }

                       //设置了别名,则跳转
                       if (!String.IsNullOrEmpty(archive.Alias) && String.Compare(id, archive.Alias, true) != 0) //设置了别名        
                       {
                           Response.StatusCode = 301;
                           Response.RedirectLocation = String.Format("/{0}/{1}/{2}.html",
                               lang,
                               category.Tag,
                               String.IsNullOrEmpty(archive.Alias) ? archive.ID : archive.Alias
                               );
                           Response.End();
                           return;
                       }
                   }


                   //增加浏览次数
                   ++archive.ViewCount;
                   new System.Threading.Thread(() =>
                   {
                       try
                       {
                           bll.AddViewCount(archive.ID, 1);
                       }
                       catch
                       {
                       }
                   }).Start();

                   //显示页面
                   html = cmsPage.GetArchive(category, archive, lang);

                   //再次处理模板
                   //html = PageUtility.Render(html, new { }, false); 

               }
           }

           base.Render(html);
       }


        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public void Search(string c, string w, string lang)
        {
            base.Render(cmsPage.GetSearch(
                 c ?? String.Empty,
                 w ?? String.Empty,
                 lang
                )
                );
        }

        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public void Tag(string lang,string t)
        {
            base.Render(
                cmsPage.GetTagArchive(t ?? String.Empty, lang)
                );
        }

        [HttpPost]
        public string Archive(FormCollection form)
        {
            string id=form["id"];           //文档编号
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
                    return ScriptUtility.ParentClientScriptCall("cetip(false,'验证码不正确!');cms.$('ce_verifycode').nextSibling.onclick();");
                }
                else if (String.Compare(content, "请在这里输入评论内容", true) == 0 || content.Length == 0)
                {
                    return ScriptUtility.ParentClientScriptCall("cetip(false,'请输入内容!'); ");
                }
                else if (content.Length > 200)
                {
                    return ScriptUtility.ParentClientScriptCall("cetip(false,'评论内容长度不能大于200字!'); ");
                }

                if (member == null)
                {
                    if (String.IsNullOrEmpty(view_name))
                    {
                        //会员未登录时，需指定名称
                        return ScriptUtility.ParentClientScriptCall("cetip(false,'不允许匿名评论!'); ");
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
                cmbll.InsertComment(id, memberID, Request.UserHostAddress, content);
                return ScriptUtility.ParentClientScriptCall("cetip(false,'提交成功!'); setTimeout(function(){location.reload();},500);");
            }

            //其他操作
            return String.Empty;
        }

        #endregion

        #region 数据表格

        [AcceptVerbs("POST")]
        public string SubmitForm(FormCollection form)
        {
            int tableID;

            if (String.IsNullOrEmpty(Request["tableid"]))
            {
                return "{tag:-1,message:'表单出错！'}";
            }
            else
            {
                tableID = int.Parse(base.Request["tableid"]);
                string sessName=String.Format("[custom]${0}_token", tableID.ToString());

                //校验TOKEN
                string token = global::System.Web.HttpContext.Current.Session[sessName] as string;
                if (String.Compare(token, base.Request["token"], true) != 0)
                {
                    return "{tag:-1,message:'请勿重复提交！'}";
                }

                //提交
                int result = (int)new TableBLL().SubmitRow(tableID, global::System.Web.HttpContext.Current.Request.Form);

                //提交成功后，清除TOKEN
                if (token != null)
                {
                    global::System.Web.HttpContext.Current.Session.Remove(sessName);
                }

                return "{tag:" + result.ToString() + ",message:'提交成功！'}";
            }
        }

        #endregion

    }
}
