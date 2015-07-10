/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.j6.cc
 * 
 * name : RequestHandle.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using J6.DevFw.PluginKernel;
using System.Web;
using J6.Cms.Extend.SSO;
using J6.Cms.Extend.SSO.Server;


//  Test :
//  http://localhost:8000/com.plugin.sso.sh.aspx?action=test&token=123456
//
//  Login:
//  http://localhost:8000/com.plugin.sso.sh.aspx?action=login&token=123456&usr=user&pwd=123456
//
//  Get Session:
//  http://localhost:8000/com.plugin.sso.sh.aspx?action=getSession&token=123456&session.key=5DF4F6B09ECF3F0B
//
//  Logout:
//  
//

namespace com.plugin.sso
{
    /// <summary>
    /// Description of MobileActions.
    /// </summary>
    internal partial class RequestHandle
    {
        private IPlugin _plugin;
        private readonly SessionServer _serve;

        internal RequestHandle(IPlugin plugin)
        {
            this._plugin = plugin;


            PersonFetchHandler ph = a =>
            {
                return new Person
                {
                    Id = a,
                    Name = "用户A"
                };
            };

            SSOLoginHandler lh = (usr, pwd) =>
            {
                return 1;
            };

            ISessionSet set = SessionManager.GetDefaultSessionSet();
            this._serve = new SessionServer(set, ph, lh, "123456", "tmpsso");
            this._serve.RegisterClient("http://localhost:8001/sso_api");
        }



        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="context"></param>
        public string Default(HttpContext context)
        {
            return this._serve.Process(context);
        }


    }

}
