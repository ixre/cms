using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace J6.Cms.Extend.SSO.Server
{
    public class SessionServer
    {
        private readonly SessionManager _sessionManager;
        private readonly SSOLoginHandler _ssoHandler;

        /// <summary>
        /// 存储客户端URL
        /// </summary>
        private readonly IList<String> _clients =new List<String>();
        private readonly string _commToken;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <param name="personFetchHandler"></param>
        /// <param name="handler"></param>
        /// <param name="communicateToken"></param>
        /// <param name="seed">随机种子</param>
        public SessionServer(ISessionSet set,
             PersonFetchHandler personFetchHandler,
             SSOLoginHandler handler, String communicateToken, string seed)
        {
            if (set == null)
            {
                throw new ArgumentNullException("set is null");
            }
            this._commToken = communicateToken;
            this._ssoHandler = handler;
            this._sessionManager = new SessionManager(set,
                personFetchHandler, seed ?? "tmpssokas");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personFetchHandler"></param>
        /// <param name="handler"></param>
        /// <param name="communicateToken"></param>
        /// <param name="seed"></param>
        public SessionServer(PersonFetchHandler personFetchHandler,
             SSOLoginHandler handler, String communicateToken, string seed)
        {
            this._commToken = communicateToken;
            this._ssoHandler = handler;
            ISessionSet set = SessionManager.GetDefaultSessionSet();
            this._sessionManager = new SessionManager(set,
                personFetchHandler, seed ?? "tmpssokas");
            this._ssoHandler = handler;
        }


        /// <summary>
        /// 注册客户端
        /// </summary>
        /// <param name="url"></param>
        public void RegisterClient(String url)
        {
            if (this._clients.Contains(url))
            {
                throw new ArgumentException("it's registed! url :"+url);
            }
            this._clients.Add(url);
        }

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
        public String Process(HttpContext context)
        {

            string actionStr = context.Request["action"];
            string token = context.Request["token"];
            string sessionKey = context.Request["session.key"];

            //除登出外均需要验证
            if (token != this._commToken)
            {
                return JsonConvert.SerializeObject(new SessionResult {Message = "error token"});
            }

            return ProcessRequest(context, actionStr, sessionKey);
        }

        private string ProcessRequest(HttpContext context, string actionStr, string sessionKey)
        {
            if (actionStr != null)
            {
                ServerAction action = (ServerAction) Enum.Parse(
                    typeof (ServerAction), actionStr, true);

                //登出
                if (action == ServerAction.Logout)
                {
                    SsoResult result = this.LoginOut(sessionKey);
                    //return JsonConvert.SerializeObject(result);
                    if (result.Result)
                    {
                        return "{\"Result\":true,\"Message\":\"" + result.Message.Replace("\"", "'") + "\"}";
                    }
                    else
                    {
                        return "{\"Result\":false,\"Message\":\"" + result.Message + "\"}";
                    }
                }

                if (action == ServerAction.Login)
                {
                    SsoResult result = this.Login(context.Request["usr"],
                        context.Request["pwd"]);
                    return JsonConvert.SerializeObject(result);
                }

                if (action == ServerAction.GetSession)
                {
                    Person person = this.SessionManager.GetPerson(sessionKey);
                    if (person == null)
                    {
                        return JsonConvert.SerializeObject(new SessionResult {Message = "no session"});
                    }

                    return JsonConvert.SerializeObject(new SessionResult {Result = true, Person = person});
                }

                if (action == ServerAction.Test)
                {
                    return "test ok!";
                }
            }


            return JsonConvert.SerializeObject(new SessionResult {Message = "invalid request"});
        }


        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private SsoResult Login(string usr, string pwd)
        {
            SsoResult result = new SsoResult();
          
            int personId = 0;
            if ((personId = this._ssoHandler(usr, pwd)) > 0)
            {
                result.Result = true;
                result.SessionKey = this._sessionManager.SaveSession(personId);
                result.Message = this.GetSsoLoginHtml(result.SessionKey);
            }
            else
            {
                result.Message = "";
            }
            return result;
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private SsoResult LoginOut(string sessionKey)
        {
            int personId = this._sessionManager.GetSession(sessionKey);
            if (personId != -1)
            {
                // clear session
                this._sessionManager.RemoveSession(sessionKey);
                return new SsoResult
                {
                    Result = true,
                    Message = this.GetSsoLogoutHtml()
                };
            }

            return new SsoResult
            {
                Result = false,
                Message = "no session"
            };

        }

        private String GetSsoLoginHtml(String sessionKey)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String url in this._clients)
            {
                sb.Append("<script src='").Append(url)
                    .Append(url.Contains("?") ? "&" : "?")
                    .Append("action=require&session.key=")
                    .Append(sessionKey).Append("'></script>");
            }
            return SsoUtil.EncodeBase64(sb.ToString());
        }

        private string GetSsoLogoutHtml()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String url in this._clients)
            {
                sb.Append("<script src='").Append(url)
                    .Append(url.Contains("?") ? "&" : "?")
                    .Append("action=logout").Append("'></script>");
            }
            return SsoUtil.EncodeBase64(sb.ToString());
        }

        
    }
}
