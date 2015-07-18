//
// 文件名称: CmsController.cs
// 作者：  newmin
// 创建时间：2013-04-14
//
namespace J6.Cms.Web
{
    using System;
    using System.Web.Routing;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public class ForbiddenHandler : IRouteHandler
    {
        public class ForbiddenHttpHandler : IHttpHandler
        {
            public bool IsReusable
            {
                get { throw new NotImplementedException(); }
            }

            public void ProcessRequest(HttpContext context)
            {
                context.Response.Write("<div style=\"margin:50px auto;width:400px;font-size:14px;color:red;line-height:50px;\"><b style=\"font-size:25px\">403&nbsp;Forbidden!</b> <br />问题出现的原因，请见：http://z3q.net/cms/cms/</div>");
                context.Response.End();
            }
        }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ForbiddenHttpHandler();
        }
    }

    /// <summary>
    /// 静态文件
    /// </summary>
    public class StaticFileHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new DefaultHttpHandler();
        }
    }
}