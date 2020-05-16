using JR.Cms.Conf;
using System;
using System.Web;

namespace JR.Cms.AspNet
{
    public class HttpModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_StartRequest);
        }

        private void context_StartRequest(object sender, EventArgs e)
        {
            // var context = (HttpApplication)sender;
            // // 如果没有安装,则跳转到安装地址
            // if(!CmsInstallChecker.Check(context))context.Response.End();
            // NewMethod(context);
        }

    }
}
