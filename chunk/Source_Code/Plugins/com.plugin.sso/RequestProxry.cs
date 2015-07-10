/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.j6.cc
 * 
 * name : RequestProxry.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System.Web;
using J6.DevFw.PluginKernel;
using J6.Cms.Utility;
using J6.DevFw.Web.Plugin;

namespace com.plugin.sso
{
    /// <summary>
    /// Description of HandleRequest.
    /// </summary>
    public class RequestProxry
    {
        private readonly IPluginApp _app;
        private readonly RequestHandle _handler;


        public RequestProxry(IPluginApp app, IPlugin plugin)
        {
            this._app = app;
            this._handler = new RequestHandle(plugin);
        }

        public static bool VerifyLogin(HttpContext context)
        {
            bool result = UserState.Administrator.HasLogin;
            if (!result)
            {
                context.Response.Write("<script>window.parent.location.replace('/admin?return_url=')</script>");
            }
            return result;
        }

        public void HandleGet(HttpContext context, ref bool handled)
        {
            if (this._app.HandleRequestUse(this._handler, context, false))
            {
                handled = true;
            }
        }

        public void HandlePost(HttpContext context, ref bool handled)
        {
            if (this._app.HandleRequestUse(this._handler, context, true))
            {
                handled = true;
            }
        }

    }
}
