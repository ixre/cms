using System;

namespace JR.Cms.Extend.SSO
{
    /// <summary>
    /// 会话管理器
    /// </summary>
    public class SessionManager
    {

        public static ISessionSet defaultSessionSet;

        private readonly ISessionSet _sessionSet;
        private readonly PersonFetchHandler _personFetchHandler;
        private SessionGenerator _generator;



        internal SessionManager(ISessionSet sessionSet,
            PersonFetchHandler personFetchHandler,string seed)
        {
            this._sessionSet = sessionSet;
            this._personFetchHandler = personFetchHandler;
            this._generator = new SessionGenerator(seed,5);
        }


        /// <summary>
        /// 获取默认的Session管理器
        /// </summary>
        /// <returns></returns>
        public static ISessionSet GetDefaultSessionSet()
        {
            if (defaultSessionSet == null)
            {
                LevelDbSessionProvider levelDb = new LevelDbSessionProvider();
                levelDb.Initilize();
                defaultSessionSet = levelDb;
            }
            return defaultSessionSet;
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="key"></param>
        /// <returns>返回Person的id,如果没有则返回-1</returns>
        public int GetSession(string key)
        {
            string s =  this._sessionSet.Get(key);
            if (s == null)
            {
                return -1;
            }
            return int.Parse(s);
        }

 
        /// <summary>
        /// 获取人员和用户信息
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public Person GetPerson(string sessionKey)
        {
            int personId = this.GetSession(sessionKey);

            if (personId == -1)
            {
                return null;
            }

            return this._personFetchHandler(personId);
        }

        /// <summary>
        /// 保存会话,并返回Session Token
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SaveSession(int personId)
        {
            string sessionToken = this._generator.CreateKey();

            string sessionRowKey = "sk_" + personId.ToString();

            //获取上次的Session记录并删除
            string oldToken = this._sessionSet.Get(sessionRowKey);
            if (!String.IsNullOrEmpty(oldToken))
            {
                this._sessionSet.Delete(oldToken);
            }

            //保存新的Session记录
            this._sessionSet.Put(sessionRowKey, sessionToken);
            this._sessionSet.Put(sessionToken, personId.ToString());

            return sessionToken;

//            HttpCookie cookie = new HttpCookie(Variables.SessionCookieName, sessionKey);
//            cookie.Expires = DateTime.Now.AddYears(2);
//
//            HttpContext.Current.Response.Cookies.Add(cookie);
//
//            return sessionToken;
        }

        /// <summary>
        /// 移除会话
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSession(string key)
        {
            //HttpContext context = HttpContext.Current;
            //HttpCookie cookie = context.Request.Cookies.Get(Variables.SessionCookieName);

            //if (cookie != null)
            //{
            //    cookie.Expires = DateTime.Now.AddYears(-2);
            //    context.Response.Cookies.Add(cookie);
            //}
            this._sessionSet.Delete(key);
        }

        //public bool VerifySessionSecret(string sessionKey, string sessionSecret)
        //{
        //    string sessionStr = this.GetSession(sessionKey);
        //    if (!String.IsNullOrEmpty(sessionStr))
        //    {
        //        string[] sessionData = sessionStr.Split(';');
        //        return String.Compare(sessionData[1], sessionSecret, true) == 0;
        //    }
        //    return false;
        //}
    }
}
