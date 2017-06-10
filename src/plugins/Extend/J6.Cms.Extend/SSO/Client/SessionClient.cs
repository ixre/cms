using System;
using System.Web;

namespace T2.Cms.Extend.SSO.Client
{
    public class SessionClient:ISessionClient
    {  
        private readonly string _serverUrl;
        private readonly string _token;
        private static readonly string sessionCookieName = "_ssokey";

        public SessionClient(string serverUrl, string token)
        {
            this._serverUrl = serverUrl;
            this._token = token;
        }

        #region  处理请求

        /// <summary>
        /// 获取执行动作
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public String getAction(HttpContext context)
        {
            return (context.Request["action"] ?? "").ToLower();
        }

        /// <summary>
        /// 获取会话Key
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public String getSessionKey(HttpContext context)
        {
            return context.Request["session.key"];
        }

        public void HandleSsoRequest(HttpContext context)
        {
            String action = this.getAction(context);

            //接受会话
            if (action == "require")
            {
                String sessionKey = this.getSessionKey(context);
                HttpCookie cookie = new HttpCookie(sessionCookieName, sessionKey);
                cookie.Expires = DateTime.Now.AddYears(2);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else if (action == "logout")
            {
                HttpCookie cookie = context.Request.Cookies.Get(sessionCookieName);
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddYears(-2);
                    context.Response.Cookies.Add(cookie);
                }
            }
        }

        #endregion

        public SessionResult GetSession(string sessionKey)
        {
            return ClientUtil.RequestSession(this._serverUrl, this._token, sessionKey);
        }

        public SsoResult Login(string user, string pwd)
        {
            SsoResult result =  ClientUtil.LoginRequest(this._serverUrl, this._token, user,pwd);
            if (result.Result && !string.IsNullOrEmpty(result.Message))
            {
                string oldMsg = result.Message;
                try
                {
                    result.Message = SsoUtil.DecodeBase64(result.Message);
                }
                catch
                {
                    result.Message = oldMsg;
                }
            }
            return result;
        }

        public SsoResult Logout(string sessionKey)
        {
            SsoResult result = ClientUtil.LogoutRequest(this._serverUrl, this._token,sessionKey);
            if (result.Result && !string.IsNullOrEmpty(result.Message))
            {
                string oldMsg = result.Message;
                try
                {
                    result.Message = SsoUtil.DecodeBase64(result.Message);
                }
                catch
                {
                    result.Message = oldMsg;
                }
            }
            return result;
        }


        public string GetSessionKey()
        {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies.Get(sessionCookieName);
            if (cookie == null) return null;
            return cookie.Value;
        }
    }
}
