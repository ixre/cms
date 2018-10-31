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

using System.Web;
using T2.Cms.BLL;
using T2.Cms.Conf;
using T2.Cms.DataTransfer;
using JR.DevFw.Framework.Web.UI;
using JR.DevFw.Framework.Xml.AutoObject;

namespace T2.Cms.Web.Mvc
{
    using T2.Cms;
    using T2.Cms.Resource;
    using T2.Cms.Template;
    using JR.DevFw.Template;
    using System;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using T2.Cms.Core;




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
            CmsController.OnIndexRequest += Cms.Plugins.Portal.PortalRequest;
            CmsController.OnCategoryRequest += Cms.Plugins.Category.Request;
            CmsController.OnArchiveRequest += Cms.Plugins.Archive.Request;
            CmsController.OnArchivePost += Cms.Plugins.Archive.PostRequest;
            CmsController.OnSearchRequest += Cms.Plugins.Portal.SearchRequest;
            CmsController.OnTagRequest += Cms.Plugins.Portal.TagRequest;
        }

        /// <summary>
        /// 首页
        /// </summary>
        public void Index()
        {
            bool eventResult = false;
            if (OnIndexRequest != null)
            {
                OnIndexRequest(base.OutputContext, ref eventResult);
            }
            if (!this.CheckSiteUrl()) return;
            SiteDto site = Cms.Context.CurrentSite;

            //跳转
            if (!String.IsNullOrEmpty(site.Location) && Request.Url != null 
                && Request.Url.Query.Length ==0)
            {
                Response.Redirect(site.Location, true);  //302
                return;
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderIndex(base.OutputContext);
            }
        }

        private bool CheckSiteUrl()
        {
            SiteDto site = Cms.Context.CurrentSite;
            if (!String.IsNullOrEmpty(site.AppPath))
            {
                String targetUrl = "/" + site.AppPath;
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
                string fileName = String.Concat(Cms.PyhicPath,
                    CmsVariables.TEMP_PATH,
                    "tpl_lang_guid.xml");
                AutoObjectXml obj = new AutoObjectXml(fileName);
                obj.InsertFromType(typeof(CmsTemplateImpl), false);
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
        /// <param name="useFor"></param>
        [HttpGet]
        public void VerifyImg(string length, string opt, string useFor)
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

            VerifyCodeGenerator v = new VerifyCodeGenerator();
            v.AllowRepeat = false;

            //显示验证码
            v.RenderGraphicImage(_length, _opt, true, out verifycode, "Image/Jpeg");

            //保存验证码
            Session[String.Format("$jr.site_{0}_verifycode", this.OutputContext.CurrentSite.SiteId.ToString())] = verifycode;
        }

        #endregion

        #region 文档


        /// <summary>
        /// 获取真实的请求地址
        /// </summary>
        /// <param name="path"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        public string SubPath(String path,string appPath)
        {
            int len = appPath.Length;
            if (len > 2)
            {
                if (path.Length < len) return "";
                return path.Substring(len);
            }
            return path;
        }

        /// <summary>
        /// 栏目页面
        /// </summary>
        /// <param name="allCate"></param>
        /// <returns></returns>
        public void Category(string allCate)
        {
            CmsContext ctx = base.OutputContext;
            //检测网站状态
            if (!ctx.CheckSiteState()) return;
            //检查缓存
            if (!ctx.CheckAndSetClientCache()) return;
            String catPath = this.SubPath(allCate, ctx.SiteAppPath);
            //验证是否为当前站点的首页
            if (catPath.Length == 0)
            {
                this.Index();
                return;
            }
            int page = 1;
            if (catPath.IndexOf(".") != -1)
            {
                //获取页码和tag
                Regex paramRegex = new Regex("/*((.+)/(p(\\d+)\\.html)?|(.+))$", RegexOptions.IgnoreCase);
                Match mc = paramRegex.Match(catPath);
                if (mc.Groups[4].Value != "")
                {
                    page = int.Parse(mc.Groups[4].Value);
                }
                catPath = mc.Groups[mc.Groups[2].Value != "" ? 2 : 5].Value;
            }
            // 去掉末尾的"/"
            if (catPath.EndsWith("/"))
            {
                catPath = catPath.Substring(0, catPath.Length - 1);
            }

            //执行
            bool eventResult = false;
            if (OnCategoryRequest != null)
            {
                OnCategoryRequest(ctx, catPath, page, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderCategory(ctx, catPath, page);
            }
        }

        /// <summary>
        /// 文档页
        /// </summary>
        /// <returns></returns>
        public void Archive(string allHtml)
        {
            CmsContext ctx = base.OutputContext;
            //检测网站状态
            if (!ctx.CheckSiteState()) return;
            //检查缓存
            if (!ctx.CheckAndSetClientCache()) return;
            String archivePath = this.SubPath(allHtml, ctx.SiteAppPath);


            bool eventResult = false;
            if (OnArchiveRequest != null)
            {
                OnArchiveRequest(ctx, archivePath, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderArchive(ctx, archivePath);
            }
        }


        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="c"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public void Search(string c, string w)
        {
            bool eventResult = false;
            if (OnSearchRequest != null)
            {
                OnSearchRequest(base.OutputContext, c, w, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                if (c != null) c = c.Trim();
                if (w != null) w = w.Trim();
                DefaultWebOuput.RenderSearch(base.OutputContext, c,w);
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
                OnTagRequest(base.OutputContext, t, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.RenderTag(base.OutputContext, t);
            }
        }

        [HttpPost]
        public void Archive(string allhtml, FormCollection form)
        {
            bool eventResult = false;
            if (OnArchivePost != null)
            {
                OnArchivePost(base.OutputContext, allhtml, ref eventResult);
            }

            //如果返回false,则执行默认输出
            if (!eventResult)
            {
                DefaultWebOuput.PostArchive(base.OutputContext, allhtml);
            }
        }

        #endregion

        #region 数据表格

        [AcceptVerbs("POST")]
        public string SubmitForm(FormCollection form)
        {
            //检测网站状态
            if (!base.OutputContext.CheckSiteState()) return String.Empty;

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

        [HttpGet]
        public void Combine()
        {
            HttpContext context = Cms.Context.HttpContext;

            Cms.Cache.ETagOutput(context.Response, () =>
                {
                    string path = context.Request["loc"] ?? "";
                    string type = context.Request["type"] ?? "js";

                    string[] files = path.Split(',');
                    string appDir = Cms.PyhicPath;

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

        [HttpGet]
        public void Change()
        {
            HttpContext ctx = System.Web.HttpContext.Current;
            String langOpt = Request["lang"];
            String deviceOpt = Request["device"];
            bool isChange = false;
            int i;
            if (!String.IsNullOrEmpty(langOpt))
            {
                int.TryParse(langOpt, out i);
                if (Cms.Context.SetUserLanguage(ctx,i))
                {
                    isChange = true;
                }
            }
            if (!String.IsNullOrEmpty(deviceOpt))
            {
                int.TryParse(deviceOpt, out i);
                if (Cms.Context.SetUserDevice(ctx, i))
                {
                    isChange = true;
                }
            }

            if (isChange)
            {
                String returnUrl = Request["return_url"];
                if (String.IsNullOrEmpty(returnUrl))
                {
                    Uri refer = Request.UrlReferrer;
                    returnUrl = refer == null ? "/" : refer.ToString();
                }
                Response.Redirect(returnUrl, true);
            }
            else
            {
                Response.Write("error params ! should be  /"+CmsVariables.DEFAULT_CONTROLLER_NAME+"/change?lang=[1-8]&device=[1-2]");
            }
        }

        [HttpGet]
        public void MLogin()
        {
            Response.Redirect("/"+Settings.SYS_ADMIN_TAG,true);
        }
    }
}
