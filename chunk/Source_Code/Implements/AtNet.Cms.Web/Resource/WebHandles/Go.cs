//
// Go.cs    跳转
// Copyright 2011 @ OPS Inc,All rights reseved !
// newmin @ 2011/03/18
//
namespace OPSite.WebHandler
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Ops.Web;

    [WebExecuteable]
    public class Go
    {
        [Get]
        public void Login()
        {
            HttpContext context=HttpContext.Current;
            string uri =context.Request.Headers["referer"];
            context.Response.Redirect("/passport/login?returnUri=" + uri);
        }

        /*
        [Get]
        public void Login(string uri)
        {
            HttpContext.Current.Response.Redirect("/passport/login?returnUri=" + (uri??"/"),true);
        }*/
    }
}