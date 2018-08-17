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
namespace CMS.Web.Controllers
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using J6.Cms;
    using J6.Cms.BLL;
    using J6.Cms.Models;
    using J6.Data;
    using J6.Graphic;
    using J6.Template;

    /// <summary>
    /// 
    /// </summary>
    public class CmsController :CMS.ControllerBase
    {
        private static ArchiveBLL bll = new ArchiveBLL();
        private static CategoryBLL cbll = new CategoryBLL();
        private static CommentBLL cmbll=new CommentBLL();

        #region 首页

        /// <summary>
        /// 首页
        /// </summary>
        public void Index()
        {
           PageGenerator.Generate(PageGeneratorObject.Default);
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
            global::J6.Cms.WebManager.Logic.Request(System.Web.HttpContext.Current);
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
        /// 分类页面
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public string Category(string tag, int page)
        {
            string html = String.Empty;
            Category category = cbll.Get(a => String.Compare(tag, a.Tag, true) == 0);

            if (category != null)
            {
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
                            return base.Render404();
                        }
                    }

                    Response.StatusCode = 302;
                    Response.RedirectLocation = String.Format("/{0}/{1}.html",
                        category.Tag,
                        String.IsNullOrEmpty(a.Alias) ? a.ID : a.Alias
                        );
                    Response.End();
                    return null;
                }

                html = PageGenerator.ReturnGenerate(PageGeneratorObject.CategoryPage, category, page);
            }
            else
            {
                return base.Render404();
            }

            return html;
        }

        /// <summary>
        /// 文档页
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Archive(string tag, string id)
        {
            string html = String.Empty;
            Archive archive;
            Category category;

            archive = bll.Get(id);
            if (archive != null)
            {
                category = cbll.Get(a => a.ID == archive.Cid);

                if (category != null)
                {
                    const string pattern = "^/[0-9a-zA-Z]+/[\\.0-9A-Za-z_-]+\\.html$";
                    const string pagePattern = "^/[\\.0-9A-Za-z_-]+\\.html$";

                    if (ArchiveFlag.GetFlag(archive.Flags, ArchiveInternalFlags.AsPage))
                    {
                        if (!Regex.IsMatch(Request.Path, pagePattern))
                        {
                            Response.StatusCode = 301;
                            Response.RedirectLocation = String.Format("/{0}.html",
                                String.IsNullOrEmpty(archive.Alias) ? archive.ID : archive.Alias
                                );
                            Response.End();
                            return null;
                        }
                    }
                    else if (!Regex.IsMatch(Request.Path, pattern) ||
                        (String.Compare(tag, category.Tag, true) != 0 ||                                                 //如果分类tag不对，则301跳转
                        (!String.IsNullOrEmpty(archive.Alias) && String.Compare(id, archive.Alias, true) != 0)
                        ))   //设置了别名        
                    {
                        Response.StatusCode = 301;
                        Response.RedirectLocation = String.Format("/{0}/{1}.html",
                            category.Tag,
                            String.IsNullOrEmpty(archive.Alias) ? archive.ID : archive.Alias
                            );
                        Response.End();
                        return null;
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
                    html = PageGenerator.Generate(PageGeneratorObject.ArchivePage, category, archive);
                    //再次处理模板
                    //html = PageUtility.Render(html, new { }, false); 

                }
            }
            else
            {
                html= base.Render404();
            }
            return html;
        }

        [Obsolete]
        public string ArchivePage(string tag, string id)
        {
            return Archive(tag,id);
        } 

        [Obsolete]
        public string CategoryPage(string tag, int page)
        {
            return Category(tag,page);
        }

        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public string Search(string c,string w)
        {
           return PageGenerator.ReturnGenerate(PageGeneratorObject.Search,c??String.Empty, w??String.Empty);
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

        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string Tag(string t)
        {
            return PageGenerator.ReturnGenerate(PageGeneratorObject.Tag,t??String.Empty);
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
