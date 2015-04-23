

namespace OPSite.WebHandler
{
    using System;
    using System.Web;
    using Ops.Web;
    using Ops.Cms.Models;
    using Ops.Cms;

    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple=false,Inherited=false)]
    public class LogonAttribute:Attribute,IWebExecute
    {
        public void PreExecuting()
        {
            if (UserState.Member.Current == null)
            {
                HttpContext context=HttpContext.Current;

                context.Response.Write("<script>alert('请先登录!');window.parent.location.href='/passport/login?returnUri="+
                   context.Server.UrlEncode(context.Request.Url.ToString())+"';</script>");
                HttpContext.Current.Response.End();
            }
        }
    }
}