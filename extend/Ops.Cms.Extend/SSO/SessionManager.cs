using System;
using System.Web;

namespace Ops.Cms.Extend.SSO
{
    /// <summary>
    /// 会话管理器
    /// </summary>
    public class SessionManager
    {
        private ISessionSet _sessionSet;
        private PersonFetchHandler _personFetchHandler;

        internal SessionManager(ISessionSet sessionSet,
            PersonFetchHandler personFetchHandler)
        {
            this._sessionSet = sessionSet;
            this._personFetchHandler = personFetchHandler;
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetSession(string key)
        {
            //personId,sessionToken
            return this._sessionSet.Get(key);
        }

        /// <summary>
        /// 获取当前用户的SessionKey
        /// </summary>
        /// <returns></returns>
        public string GetCurrentSessionKey()
        {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies.Get(Variables.SessionCookieName);
            if (cookie == null) return null;
            return cookie.Value;
        }

        /// <summary>
        /// 获取人员和用户信息
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public Person GetPerson(string sessionKey)
        {
            HttpContext context = HttpContext.Current;
            Person person = null;

            string sessionStr = this.GetSession(sessionKey);
            if (!String.IsNullOrEmpty(sessionStr))
            {
                string[] sessionData = sessionStr.Split(';');
                int personId = int.Parse(sessionData[0]);

                //如果不传入secret或secret和session中对比一致
                //if (sessionSecret == null || sessionSecret == sessionData[1])
                person = this._personFetchHandler(personId);

            }

            return person;
        }

        /// <summary>
        /// 保存会话,并返回Session Value
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SaveSession(int personId)
        {
            string sessionKey = SessionGenerator.CreateKey();
            string sessionToken = String.Format("{0:yyyyMMHHssfff}", DateTime.Now);

            string usrKey = "sk_" + personId.ToString();

            //获取上次的Session记录并删除
            string usrKeyValue = this._sessionSet.Get(usrKey);
            if (!String.IsNullOrEmpty(usrKeyValue))
            {
                this._sessionSet.Delete(usrKeyValue);
            }

            //保存新的Session记录
            this._sessionSet.Put(usrKey, sessionKey);
            this._sessionSet.Put(sessionKey, personId.ToString() + ";" + sessionToken);


            HttpCookie cookie = new HttpCookie(Variables.SessionCookieName, sessionKey);
            cookie.Expires = DateTime.Now.AddYears(2);

            HttpContext.Current.Response.Cookies.Add(cookie);

            return sessionToken;
        }

        /// <summary>
        /// 移除会话
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSession(string key)
        {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies.Get(Variables.SessionCookieName);

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-2);
                context.Response.Cookies.Add(cookie);
            }
            this._sessionSet.Delete(key);
        }

        public bool VerifySessionSecret(string sessionKey, string sessionSecret)
        {
            string sessionStr = this.GetSession(sessionKey);
            if (!String.IsNullOrEmpty(sessionStr))
            {
                string[] sessionData = sessionStr.Split(';');
                return String.Compare(sessionData[1], sessionSecret, true) == 0;
            }
            return false;
        }
    }
}
