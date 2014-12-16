
namespace Ops.Cms.Extend.SSO.Server
{
    public class SessionServer
    {
        private SessionManager _defaultSessionManager;
        private SessionManager _sessionManager;
        private SessionServerResponse _response;

        public SessionServer(ISessionSet set,
            PersonFetchHandler personFetchHandler)
        {
            if (set == null)
            {
                LevelDbSessionProvider levelDb = new LevelDbSessionProvider();
                levelDb.Initilize();

                _defaultSessionManager = new SessionManager(levelDb, 
                    personFetchHandler);
                this._sessionManager = _defaultSessionManager;
            }
            else
            {
                this._sessionManager = new SessionManager(set,
                    personFetchHandler);
            }
        }

        public SessionServer(PersonFetchHandler personFetchHandler) :
            this(null, personFetchHandler) { }

        /// <summary>
        /// 会话管理
        /// </summary>
        public SessionManager SessionManager
        {
            get
            {
                return this._sessionManager;
            }
        }

        /// <summary>
        /// 会话服务响应
        /// </summary>
        public SessionServerResponse Response
        {
            get
            {
                return this._response ?? (this._response = new SessionServerResponse(this));
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="pwd"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool Login(string usr, string pwd, SSOLoginHandler handler)
        {
            int personId = 0;
            if ((personId = handler(usr, pwd))>0)
            {
                this._sessionManager.SaveSession(personId);
                return true;
            }
            return false;
        }

        public void LoginOut()
        {
            string key = this._sessionManager.GetCurrentSessionKey();
            if (!string.IsNullOrEmpty(key))
            {
                this._sessionManager.RemoveSession(key);
            }
        }
    }
}
