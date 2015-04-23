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

using AtNet.Cms.BLL;
using AtNet.Cms.Conf;
using AtNet.Cms.DataTransfer;
using AtNet.DevFw.Framework.Web.UI;
using AtNet.DevFw.Framework.Xml.AutoObject;

namespace AtNet.Cms.Web.Mvc
{
    using AtNet.Cms;
    using AtNet.Cms.Resource;
    using AtNet.Cms.Template;
    using AtNet.DevFw.Template;
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;




    /// <summary>
    /// 
    /// </summary>
    public abstract class CmsController : ControllerBase
    {

        /// <summary>
        /// 首页请求
        /// </summary>
        public static event CmsPageHandler OnIndexRequest;

        /// <summary>
        /// 栏目页请求
        /// </summary>
        public static event CmsPageHandler<string, int> OnCategoryRequest;

        /// <summary>
        /// 文档页请求
        /// </summary>
        public static event CmsPageHandler<string> OnArchiveRequest;

        /// <summary>
        /// 文档提交请求
        /// </summary>
        public static event CmsPageHandler<string> OnArchivePost;

        /// <summary>
        /// 搜索请求
        /// </summary>
        public static event CmsPageHandler<string, string> OnSearchRequest;

        /// <summary>
        /// 标签页请求
        /// </summary>
        public static event CmsPageHandler<string> OnTagRequest;


        static CmsController()
        {
            CmsController.OnIndexRequest += AtNet.Cms.Cms.Plugins.Portal.PortalRequest;
            CmsController.OnCategoryRequest += AtNet.Cms.Cms.Plugins.Category.Request;
            CmsController.OnArchiveRequest += AtNet.Cms.Cms.Plugins.Archive.Request;
            CmsController.OnArchivePost += AtNet.Cms.Cms.Plugins.Archive.PostRequest;
            CmsController.OnSearchRequest += AtNet.Cms.Cms.Plugins.Portal.SearchRequest;
            CmsController.OnTagRequest += AtNet.Cms.Cms.Plugins.Portal.TagRequest;
        }

        /// <summary>
        /// 首页
        /// </summary>
        public void Index()
        {
            bool eventResult = false;

            if (OnIndexRequest != null)
            {
                OnIndexRequest(base.OutputCntext, ref eventResult);
            }
             
            if (!this.CheckSiteUrl()) return;

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderIndex(base.OutputCntext);
            }
        }

        private bool CheckSiteUrl()
        {
            SiteDto site = Cms.Context.CurrentSite;
            if (!String.IsNullOrEmpty(site.DirName))
            {
                String targetUrl = "/" + site.DirName;
                if (!Request.Path.StartsWith(targetUrl))
                {
                    Response.Redirect(targetUrl);
                    return false;
                }
            }
            return true;
        }


        #region 系统相关

        public void Template()
        {
            string action = System.Web.HttpContext.Current.Request["action"];
            if (action == "help")
            {
                string fileName = String.Concat(AtNet.Cms.Cms.PyhicPath,
                    CmsVariables.TEMP_PATH,
                    "tpl_lang_guid.xml");
                AutoObjectXml obj = new AutoObjectXml(fileName);
                obj.InsertFromType(typeof(CmsTemplates), false);
                obj.Flush();

                var rsp = System.Web.HttpContext.Current.Response;
                rsp.Write(XmlObjectDoc.DocStyleSheet);

                int tmpInt = 0;
                foreach (XmlObject _obj in obj.GetObjects())
                {

                    rsp.Write(XmlObjectDoc.GetGrid(_obj, ++tmpInt));
                }
            }
            else
            {
                TemplateUtility.PrintTemplatesInfo();
            }
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="length"></param>
        /// <param name="opt"></param>
        [HttpGet]
        public void VerifyImg(string length, string opt, string usefor)
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
            Session[String.Format("$cms.site_{0}_verifycode", this.OutputCntext.CurrentSite.SiteId.ToString())] = verifycode;
        }

        #endregion

        #region 文档

        /// <summary>
        /// 栏目页面
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public void Category(string allcate)
        {
            int page = 1;
            string tag;

            //验证是否为当前站点的首页
            if (this.OutputCntext.SiteAppPath != "/" && Regex.IsMatch(allcate, "^[^/]+/{0,1}$"))
            {
                this.Index();
                return;
            }

            //获取页码和tag
            Regex paramRegex = new Regex("/*(([^/]+)/(p(\\d+)\\.html)?|([^/]+))$", RegexOptions.IgnoreCase);

            Match mc = paramRegex.Match(allcate);

            tag = mc.Groups[mc.Groups[2].Value != "" ? 2 : 5].Value;

            //计算页吗:页码如:p3
            if (mc.Groups[4].Value != "")
            {
                page = int.Parse(mc.Groups[4].Value);
            }


            //执行
            bool eventResult = false;
            if (OnCategoryRequest != null)
            {
                OnCategoryRequest(base.OutputCntext, tag, page, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderCategory(base.OutputCntext, tag, page);
            }
        }

        /// <summary>
        /// 文档页
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void Archive(string allhtml)
        {
            bool eventResult = false;
            if (OnArchiveRequest != null)
            {
                OnArchiveRequest(base.OutputCntext, allhtml, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderArchive(base.OutputCntext, allhtml);
            }
        }


        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public void Search(string c, string w)
        {
            bool eventResult = false;
            if (OnSearchRequest != null)
            {
                OnSearchRequest(base.OutputCntext, c, w, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderSearch(base.OutputCntext, c, w);
            }
        }

        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public void Tag(string t)
        {

            bool eventResult = false;
            if (OnTagRequest != null)
            {
                OnTagRequest(base.OutputCntext, t, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderTag(base.OutputCntext, t);
            }
        }

        [HttpPost]
        public void Archive(string allhtml, FormCollection form)
        {
            bool eventResult = false;
            if (OnArchivePost != null)
            {
                OnArchivePost(base.OutputCntext, allhtml, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.PostArchive(base.OutputCntext, allhtml);
            }
        }

        #endregion

        #region 数据表格

        [AcceptVerbs("POST")]
        public string SubmitForm(FormCollection form)
        {
            //检测网站状态
            if (!base.OutputCntext.CheckSiteState()) return String.Empty;

            int tableID;

            if (String.IsNullOrEmpty(Request["tableid"]))
            {
                return "{tag:-1,message:'表单出错！'}";
            }
            else
            {
                tableID = int.Parse(base.Request["tableid"]);
                string sessName = String.Format("cms_form_{0}_token", tableID.ToString());

                //校验TOKEN
                string token = global::System.Web.HttpContext.Current.Session[sessName] as string;
                if (String.Compare(token, base.Request["token"], true) != 0)
                {
                    return "{tag:-1,message:'请勿重复提交！'}";
                }

                //提交
                int result = (int)CmsLogic.Table.SubmitRow(tableID, global::System.Web.HttpContext.Current.Request.Form);

                //提交成功后，清除TOKEN
                if (token != null)
                {
                    global::System.Web.HttpContext.Current.Session.Remove(sessName);
                }

                return "{tag:" + result.ToString() + ",message:'提交成功！'}";
            }
        }

        #endregion


        public void Combine()
        {
            var context = AtNet.Cms.Cms.Context.HttpContext;

            AtNet.Cms.Cms.Cache.ETagOutput(context.Response, () =>
                {
                    string path = context.Request["loc"] ?? "";
                    string type = context.Request["type"] ?? "js";

                    string[] files = path.Split(',');
                    string appDir = AtNet.Cms.Cms.PyhicPath;

                    bool compress = context.Request["comparess"] != "false";

                    foreach (string file in files)
                    {
                        if (System.IO.File.Exists(appDir + file))
                        {
                            if (compress && (type == "js" || type == "css"))
                            {
                                context.Response.Write(ResourceUtility.CompressHtml(System.IO.File.ReadAllText(appDir + file)));
                            }
                            else
                            {
                                context.Response.BinaryWrite(System.IO.File.ReadAllBytes(appDir + file));
                            }
                        }
                    }

                    switch (type.ToLower())
                    {
                        case "js": context.Response.ContentType = "text/javascript";
                            break;
                        case "css": context.Response.ContentType = "text/css";
                            break;
                        case "img": context.Response.ContentType = "image/jpeg";
                            break;
                        default:
                            context.Response.ContentType = "application/oct-stream";
                            break;
                    }

                    return "";
                });
        }

    }
}
