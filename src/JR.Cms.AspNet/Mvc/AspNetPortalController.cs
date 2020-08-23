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

using System;
using System.Web.Mvc;
using JR.Cms.Conf;
using JR.Cms.Web.Portal.Comm;
using JR.Cms.Web.Portal.Template.Rule;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Xml.AutoObject;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Web;

namespace JR.Cms.AspNet.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetRequestFilter]
    public class AspNetPortalController : Controller
    {
        private static readonly PortalControllerHandler portal = new PortalControllerHandler();
        /*
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

        */

        static AspNetPortalController()
        {
            /*
            CmsController.OnIndexRequest += Cms.Plugins.Portal.PortalRequest;
            CmsController.OnCategoryRequest += Cms.Plugins.Category.Request;
            CmsController.OnArchiveRequest += Cms.Plugins.Archive.Request;
            CmsController.OnArchivePost += Cms.Plugins.Archive.PostRequest;
            CmsController.OnSearchRequest += Cms.Plugins.Portal.SearchRequest;
            CmsController.OnTagRequest += Cms.Plugins.Portal.TagRequest;
            */
        }

        /// <summary>
        /// 首页
        /// </summary>
        public void Index()
        {
            portal.Index(HttpHosting.Context);
        }

     


        #region 系统相关

        public void Template()
        {
            string action = HttpHosting.Context.Request.GetParameter("action");
            if (action == "help")
            {
                string fileName = String.Concat(Cms.PhysicPath,
                    CmsVariables.TEMP_PATH,
                    "tpl_lang_guid.xml");
                AutoObjectXml obj = new AutoObjectXml(fileName);
                obj.InsertFromType(typeof(CmsTemplateImpl), false);
                obj.Flush();

                var rsp = HttpHosting.Context.Response;
                rsp.WriteAsync(XmlObjectDoc.DocStyleSheet);

                int tmpInt = 0;
                foreach (XmlObject _obj in obj.GetObjects())
                {
                    rsp.WriteAsync(XmlObjectDoc.GetGrid(_obj, ++tmpInt));
                }
            }
            else
            {
               HttpHosting.Context.Response.WriteAsync(TemplateUtility.GetTemplatePagesHTML());
            }
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="length"></param>
        /// <param name="opt"></param>
        /// <param name="useFor"></param>
        // [HttpGet]
        // public void VerifyImg(string length, string opt, string useFor)
        // {
        //     int _length;                        //验证码长度
        //     VerifyWordOptions _opt;  //验证码选项
        //     string verifycode;              //验证码
        //
        //     if (String.IsNullOrEmpty(length) || !Regex.IsMatch(length, "^\\d+$"))
        //     {
        //         _length = 4;
        //     }
        //     else
        //     {
        //         _length = int.Parse(length);
        //     }
        //
        //     if (String.IsNullOrEmpty(opt) || !Regex.IsMatch(opt, "^\\d$"))
        //     {
        //         _opt = VerifyWordOptions.Number;
        //     }
        //     else
        //     {
        //         _opt = (VerifyWordOptions)int.Parse(opt);
        //     }
        //
        //     VerifyCodeGenerator v = new VerifyCodeGenerator();
        //     v.AllowRepeat = false;
        //
        //     //显示验证码
        //     v.RenderGraphicImage(_length, _opt, true, out verifycode, "Image/Jpeg");
        //
        //     //保存验证码
        //     Session[String.Format("$jr.site_{0}_verifycode", this.OutputContext.CurrentSite.SiteId.ToString())] = verifycode;
        // }

        #endregion


        /// <summary>
        /// 栏目页面
        /// </summary>
        /// <param name="allCate"></param>
        /// <returns></returns>
        public void Category(string allCate)
        {
            portal.Category(HttpHosting.Context);
        }

        /// <summary>
        /// 文档页
        /// </summary>
        /// <returns></returns>
        public void Archive(string allHtml)
        {
            portal.Archive(HttpHosting.Context);
        }

        public void Error(String path)
        {
            ICompatibleHttpContext ctx = HttpHosting.Context;
            int statusCode = 500;
            if (path == "404")
            {
                statusCode = 404;
            }
            portal.Error(ctx,statusCode);
        }

        //
        // /// <summary>
        // /// 搜索列表
        // /// </summary>
        // /// <param name="c"></param>
        // /// <param name="w"></param>
        // /// <returns></returns>
        // public void Search(string c, string w)
        // {
        //     bool eventResult = false;
        //     if (OnSearchRequest != null)
        //     {
        //         OnSearchRequest(base.OutputContext, c, w, ref eventResult);
        //     }
        //
        //     //如果返回false,则执行默认输出
        //     if (!eventResult)
        //     {
        //         if (c != null) c = c.Trim();
        //         if (w != null) w = w.Trim();
        //         DefaultWebOutput.RenderSearch(base.OutputContext, c,w);
        //     }
        // }
        //
        // /// <summary>
        // /// 搜索列表
        // /// </summary>
        // /// <param name="t"></param>
        // /// <returns></returns>
        // public void Tag(string t)
        // {
        //     bool eventResult = false;
        //     if (OnTagRequest != null)
        //     {
        //         OnTagRequest(base.OutputContext, t, ref eventResult);
        //     }
        //
        //     //如果返回false,则执行默认输出
        //     if (!eventResult)
        //     {
        //         DefaultWebOutput.RenderTag(base.OutputContext, t);
        //     }
        // }
        //
        // [HttpPost]
        // public void Archive(string allhtml, FormCollection form)
        // {
        //     bool eventResult = false;
        //     if (OnArchivePost != null)
        //     {
        //         OnArchivePost(base.OutputContext, allhtml, ref eventResult);
        //     }
        //
        //     //如果返回false,则执行默认输出
        //     if (!eventResult)
        //     {
        //         DefaultWebOutput.PostArchive(base.OutputContext, allhtml);
        //     }
        // }
        //
        // #endregion
        //
        // #region 数据表格
        //
        // [AcceptVerbs("POST")]
        // public string SubmitForm(FormCollection form)
        // {
        //     //检测网站状态
        //     if (!base.OutputContext.CheckSiteState()) return String.Empty;
        //
        //     int tableID;
        //
        //     if (String.IsNullOrEmpty(Request["tableid"]))
        //     {
        //         return "{\"tag\":-1,\"message\":\"表单出错！\"}";
        //     }
        //     else
        //     {
        //         tableID = int.Parse(base.Request["tableid"]);
        //         string sessName = String.Format("cms_form_{0}_token", tableID.ToString());
        //
        //         //校验TOKEN
        //         string token = global::System.Web.HttpContext.Current.Session[sessName] as string;
        //         if (String.Compare(token, base.Request["token"], true) != 0)
        //         {
        //             return "{\"tag\":-1,\"message\":\"请勿重复提交！\"}";
        //         }
        //
        //         //提交
        //         int result = (int)CmsLogic.Table.SubmitRow(tableID, global::System.Web.HttpContext.Current.Request.Form);
        //
        //         //提交成功后，清除TOKEN
        //         if (token != null)
        //         {
        //             global::System.Web.HttpContext.Current.Session.Remove(sessName);
        //         }
        //
        //         return "{\"tag\":" + result.ToString() + ",\"message\":\"提交成功！\"}";
        //     }
        // }
        //
        // #endregion
        //
        // [HttpGet]
        // public void Combine()
        // {
        //     HttpContext context = Cms.Context.HttpContext;
        //
        //     Cms.Cache.ETagOutput(context.Response, () =>
        //         {
        //             string path = context.Request["loc"] ?? "";
        //             string type = context.Request["type"] ?? "js";
        //
        //             string[] files = path.Split(',');
        //             string appDir = Cms.PyhicPath;
        //
        //             bool compress = context.Request["comparess"] != "false";
        //
        //             foreach (string file in files)
        //             {
        //                 if (System.IO.File.Exists(appDir + file))
        //                 {
        //                     if (compress && (type == "js" || type == "css"))
        //                     {
        //                         context.Response.Write(ResourceUtility.CompressHtml(System.IO.File.ReadAllText(appDir + file)));
        //                     }
        //                     else
        //                     {
        //                         context.Response.BinaryWrite(System.IO.File.ReadAllBytes(appDir + file));
        //                     }
        //                 }
        //             }
        //
        //             switch (type.ToLower())
        //             {
        //                 case "js": context.Response.ContentType = "text/javascript";
        //                     break;
        //                 case "css": context.Response.ContentType = "text/css";
        //                     break;
        //                 case "img": context.Response.ContentType = "image/jpeg";
        //                     break;
        //                 default:
        //                     context.Response.ContentType = "application/oct-stream";
        //                     break;
        //             }
        //
        //             return "";
        //         });
        // }
        //
        // [HttpGet]
        // public void Change()
        // {
        //     HttpContext ctx = System.Web.HttpContext.Current;
        //     String langOpt = Request["lang"];
        //     String deviceOpt = Request["device"];
        //     bool isChange = false;
        //     int i;
        //     if (!String.IsNullOrEmpty(langOpt))
        //     {
        //         int.TryParse(langOpt, out i);
        //         if (Cms.Context.SetUserLanguage(ctx,i))
        //         {
        //             isChange = true;
        //         }
        //     }
        //     if (!String.IsNullOrEmpty(deviceOpt))
        //     {
        //         int.TryParse(deviceOpt, out i);
        //         if (Cms.Context.SetUserDevice(ctx, i))
        //         {
        //             isChange = true;
        //         }
        //     }
        //
        //     if (isChange)
        //     {
        //         String returnUrl = Request["return_url"];
        //         if (String.IsNullOrEmpty(returnUrl))
        //         {
        //             Uri refer = Request.UrlReferrer;
        //             returnUrl = refer == null ? "/" : refer.ToString();
        //         }
        //         Response<>.Redirect(returnUrl, true);
        //     }
        //     else
        //     {
        //         Response.Write("error params ! should be  /"+CmsVariables.DEFAULT_CONTROLLER_NAME+"/change?lang=[1-8]&device=[1-2]");
        //     }
        // }
        //
        // [HttpGet]
        // public void MLogin()
        // {
        //     Response.Redirect("/"+Settings.SYS_ADMIN_TAG,true);
        // }
    }
}